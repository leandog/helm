using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace Radiator {

    public class SpeechRecognizer {

        private KinectSensor _kinect;
        private SpeechRecognitionEngine _speechEngine;

        public event EventHandler<RecognizedEventArgs> SpeechRecognized;

        public void StartListening(IEnumerable<SemanticResultValue> voiceCommands) {
            _kinect = GetKinectSensor();
            if (null == _kinect) {
                return;
            }

            var recognizerInfo = GetKinectRecognizer();
            if (null == recognizerInfo) {
                return;
            }

            _speechEngine = new SpeechRecognitionEngine(recognizerInfo.Id);
            _speechEngine.SpeechRecognized += OnKinectSpeechRecognized;

            AddVoiceCommandsToSpeechEngine(_speechEngine, voiceCommands, recognizerInfo);

            // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
            // This will prevent recognition accuracy from degrading over time.
            _speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

            _speechEngine.SetInputToAudioStream(_kinect.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            _speechEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void AddVoiceCommandsToSpeechEngine(SpeechRecognitionEngine speechEngine, IEnumerable<SemanticResultValue> voiceCommands, RecognizerInfo recognizerInfo) {
            var directions = new Choices();
            foreach (var voiceCommand in voiceCommands) {
                directions.Add(voiceCommand);
            }
            var grammarBuilder = new GrammarBuilder {Culture = recognizerInfo.Culture};
            grammarBuilder.Append(directions);
            var grammar = new Grammar(grammarBuilder);
            speechEngine.LoadGrammar(grammar);
        }

        private KinectSensor GetKinectSensor() {
            var kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);

            if (null != kinect) {
                try {
                    kinect.Start();
                } catch (IOException) {
                    kinect = null;
                }
            }

            return kinect;
        }

        private static RecognizerInfo GetKinectRecognizer() {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers()) {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase)) {
                    return recognizer;
                }
            }
            return null;
        }

        private void OnKinectSpeechRecognized(object sender, SpeechRecognizedEventArgs args) {
            var target = args.Result.Semantics.Value.ToString();
            FireSpeechRecognized(target);
        }

        private void FireSpeechRecognized(string value) {
            if(SpeechRecognized != null)
                SpeechRecognized(this, new RecognizedEventArgs(value)); 
        }

    }
}
