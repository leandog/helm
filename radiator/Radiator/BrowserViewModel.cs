using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Akavache;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace Radiator {

    public class BrowserViewModel : INotifyPropertyChanged {

        private KinectSensor sensor;
        private SpeechRecognitionEngine speechEngine;
        private readonly ObservableCollection<PageMapping> _pageSettings = new ObservableCollection<PageMapping>();

        public event PropertyChangedEventHandler PropertyChanged;

        private String _currentUrl = "";
        public string CurrentUrl {
            get { return _currentUrl; }
            set {
                _currentUrl = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void LoadPageSettings() {
            var pages = await BlobCache.UserAccount.GetOrCreateObject("Pages", () => new List<PageMapping>());
            pages.ForEach(mapping => _pageSettings.Add(mapping));

            InitializeSpeech();
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

        private void InitializeSpeech() {
            foreach (var potentialSensor in KinectSensor.KinectSensors) {
                if (potentialSensor.Status == KinectStatus.Connected) {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor) {
                try {
                    this.sensor.Start();
                } catch (IOException) {
                    this.sensor = null;
                }
            }

            if (null == this.sensor) {
                return;
            }

            RecognizerInfo ri = GetKinectRecognizer();

            if (null != ri) {
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                var directions = new Choices();
                foreach(var pageMapping in _pageSettings) {
                    directions.Add(new SemanticResultValue(pageMapping.VoiceCommand, pageMapping.Url));
                }
                var gb = new GrammarBuilder {Culture = ri.Culture};
                gb.Append(directions);
                var g = new Grammar(gb);
                speechEngine.LoadGrammar(g);

                speechEngine.SpeechRecognized += speechEngine_SpeechRecognized;
                speechEngine.SpeechRecognitionRejected += speechEngine_SpeechRecognitionRejected;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

                speechEngine.SetInputToAudioStream(sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        void speechEngine_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e) {
        }

        void speechEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e) {
            var target = e.Result.Semantics.Value.ToString();
            if (target.StartsWith("app:"))
                ApplicationRunningHelper.BringAppToFront(target.Replace("app:",""));
            else
                CurrentUrl = target;
        }
    }
}
