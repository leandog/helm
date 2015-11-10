using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Akavache;
using System;

namespace Radiator {

    public partial class MainWindow : NavigationWindow {

        public MainWindow() {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs args) {
            try
            {
                var pages = await BlobCache.UserAccount.GetOrCreateObject("Pages", () => new List<PageMapping>());
                pages.ForEach(mapping => new Uri(((PageMapping)mapping).Url));

                if (pages.Count == 0)
                {
                    Navigate(new SettingsPage());
                }
                else
                {
                    Navigate(new BrowserPage());
                }
            }
            catch (Exception e)
            {
                Navigate(new SettingsPage());
            }
        }

        private void CanShowSettings(object sender, CanExecuteRoutedEventArgs args) {
            args.Handled = true;
            args.CanExecute = (Content.GetType() != typeof(SettingsPage));
        }

        private void ShowSettings(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Navigate(new SettingsPage());
        }

        private void NextPage(object sender, ExecutedRoutedEventArgs args)
        {
            args.Handled = true;
            if ((Content.GetType() == typeof(BrowserPage)))
            {
                BrowserPage bp = (BrowserPage)Content;
                bp.ViewModel.NextPage();
            }
        }

        private void PrevPage(object sender, ExecutedRoutedEventArgs args)
        {
            args.Handled = true;
            BrowserPage bp = (BrowserPage)Content;
            bp.ViewModel.PrevPage();
        }
    }
}
