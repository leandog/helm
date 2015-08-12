using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Akavache;
using System.Threading.Tasks;

namespace Radiator {

    public class SettingsViewModel : INotifyPropertyChanged {

        private readonly ObservableCollection<PageMapping> _pageSettings = new ObservableCollection<PageMapping>();
        public ObservableCollection<PageMapping> PageSettings {
            get { return _pageSettings; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            if(PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void LoadPageSettings() {
            var pages = await BlobCache.UserAccount.GetOrCreateObject("Pages", () => new List<PageMapping>());
            pages.ForEach(mapping => PageSettings.Add(mapping));
        }

        public void AddPage() {
            PageSettings.Add(new PageMapping());
        }

        public async Task SavePageSettings() {
            var pageSettings = PageSettings.Where(p => !string.IsNullOrWhiteSpace(p.Url) && !string.IsNullOrWhiteSpace(p.VoiceCommand)).ToList();
            await BlobCache.UserAccount.InsertObject("Pages", pageSettings);
        }
    }
}
