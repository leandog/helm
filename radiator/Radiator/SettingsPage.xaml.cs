using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Radiator {

    public partial class SettingsPage : Page {

        public SettingsPage() {
            InitializeComponent();
            Loaded += OnPageLoaded;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e) {
            ViewModel.LoadPageSettings();
        }

        private void OnAddPageClicked(object sender, RoutedEventArgs args) {
            ViewModel.AddPage();
        }

        private void OnSaveClicked(object sender, RoutedEventArgs args) {
            ViewModel.SavePageSettings();
        }

        private void OnExitClicked(object sender, RoutedEventArgs args) {
            NavigationService.Navigate(new BrowserPage());
        }

        private SettingsViewModel ViewModel {
            get { return (SettingsViewModel) DataContext; }
        }
    }
}
