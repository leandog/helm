using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Akavache;
using Microsoft.Speech.Recognition;

namespace Radiator {

    public class BrowserViewModel : INotifyPropertyChanged {

        private const uint PAGE_CYCLE_DELAY = 15 * 1000;
        private const uint IN_USE_DELAY = 180 * 1000;

        private Timer _pageCycleTimer;
        private readonly ObservableCollection<PageMapping> _pageSettings = new ObservableCollection<PageMapping>();
        private readonly SpeechRecognizer _speechRecognizer = new SpeechRecognizer();

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly List<string> _urls = new List<string>();
        public List<string> Urls {
            get { return _urls; }
        }

        private string _currentUrl = "";
        public string CurrentUrl {
            get { return _currentUrl; }
            set {
                _currentUrl = value;
                OnPropertyChanged();
            }
        }

        public async void Load() {
            var pages = await BlobCache.UserAccount.GetOrCreateObject("Pages", () => new List<PageMapping>());
            pages.ForEach(mapping => _pageSettings.Add(mapping));

            _speechRecognizer.SpeechRecognized += OnSpeechRecognized;
            _speechRecognizer.StartListening(GetVoiceCommands());

            Urls.Clear();
            Urls.AddRange(pages.Select(p => p.Url));

            CurrentUrl = Urls.FirstOrDefault();

            _pageCycleTimer = new Timer(OnPageCycleTimer, null, PAGE_CYCLE_DELAY, PAGE_CYCLE_DELAY);
        }

        public void Unload() {
            _pageCycleTimer.Dispose();
        }

        private void OnPageCycleTimer(object state) {
            CurrentUrl = GetNextUrl(CurrentUrl);
        }

        private string GetNextUrl(string currentUrl) {
            var currentIndex = Urls.IndexOf(currentUrl);
            var nextIndex = 0;
            if (currentIndex < Urls.Count - 1) {
                nextIndex = currentIndex + 1;
            }

            return Urls[nextIndex];
        }

        private IEnumerable<SemanticResultValue> GetVoiceCommands() {
            return _pageSettings.Select(p => new SemanticResultValue(p.VoiceCommand, p.Url));
        }

        private void OnSpeechRecognized(object sender, RecognizedEventArgs args) {
            _pageCycleTimer.Change(IN_USE_DELAY, PAGE_CYCLE_DELAY);
            CurrentUrl = args.Value;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
