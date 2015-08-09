using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Akavache;
using Microsoft.Speech.Recognition;

namespace Radiator {

    public class BrowserViewModel : INotifyPropertyChanged {

        private readonly ObservableCollection<PageMapping> _pageSettings = new ObservableCollection<PageMapping>();
        private readonly SpeechRecognizer _speechRecognizer = new SpeechRecognizer();

        public event PropertyChangedEventHandler PropertyChanged;

        private string _currentUrl = "";
        public string CurrentUrl {
            get { return _currentUrl; }
            set {
                _currentUrl = value;
                OnPropertyChanged();
            }
        }

        public async void LoadPageSettings() {
            var pages = await BlobCache.UserAccount.GetOrCreateObject("Pages", () => new List<PageMapping>());
            pages.ForEach(mapping => _pageSettings.Add(mapping));

            _speechRecognizer.SpeechRecognized += OnSpeechRecognized;

            var voiceCommands = _pageSettings.Select(p => new SemanticResultValue(p.VoiceCommand, p.Url));
            _speechRecognizer.StartListening(voiceCommands);
        }

        private void OnSpeechRecognized(object sender, RecognizedEventArgs args) {
            CurrentUrl = args.Value;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
