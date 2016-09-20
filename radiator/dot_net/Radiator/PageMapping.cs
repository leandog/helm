using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Radiator {

    public class PageMapping : INotifyPropertyChanged {

        private string _voiceCommand = "";
        public string VoiceCommand {
            get { return _voiceCommand; }
            set {
                _voiceCommand = value;
                OnPropertyChanged();
            }
        }

        private string _url = "";
        public string Url {
            get { return _url; }
            set {
                _url = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            if(PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
