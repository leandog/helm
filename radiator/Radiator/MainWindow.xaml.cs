using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Navigation;
using Akavache;

namespace Radiator {

    public partial class MainWindow : NavigationWindow {

        public MainWindow() {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs args) {
            var pages = await BlobCache.UserAccount.GetOrCreateObject("Pages", () => new List<PageMapping>());

            if (pages.Count == 0)
                Navigate(new SettingsPage());
            else
                Navigate(new BrowserPage());
        }
    }
}
