using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace Radiator {

    public class SpeechRecognizer {

        private KinectSensor _kinect;
        private SpeechRecognitionEngine _speechEngine;

        public event EventHandler<RecognizedEventArgs> SpeechRecognized;

        private void FireSpeechRecognized(string value) {
            if(SpeechRecognized != null)
                SpeechRecognized(this, new RecognizedEventArgs(value)); 
        }

        public void StartListening(IEnumerable<SemanticResultValue> voiceCommands) {

            foreach (var potentialSensor in KinectSensor.KinectSensors) {
                if (potentialSensor.Status == KinectStatus.Connected) {
                    _kinect = potentialSensor;
                    break;
                }
            }

            if (null != _kinect) {
                try {
                    _kinect.Start();
                } catch (IOException) {
                    _kinect = null;
                }
            }

            if (null == _kinect) {
                return;
            }

            RecognizerInfo recognizerInfo = GetKinectRecognizer();
            if (null != recognizerInfo) {
                _speechEngine = new SpeechRecognitionEngine(recognizerInfo.Id);

                var directions = new Choices();
                foreach (var voiceCommand in voiceCommands) {
                    directions.Add(voiceCommand);
                }
                var grammarBuilder = new GrammarBuilder {Culture = recognizerInfo.Culture};
                grammarBuilder.Append(directions);
                var grammar = new Grammar(grammarBuilder);
                _speechEngine.LoadGrammar(grammar);

                _speechEngine.SpeechRecognized += OnKinectSpeechRecognized;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                _speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

                _speechEngine.SetInputToAudioStream(_kinect.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                _speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
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
    }
}
