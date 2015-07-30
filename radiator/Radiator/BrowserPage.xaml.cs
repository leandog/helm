using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Radiator {

    public partial class BrowserPage : Page {



        public string CurrentUrl {
            get { return (string)GetValue(CurrentUrlProperty); }
            set { SetValue(CurrentUrlProperty, value); }
        }

        public static readonly DependencyProperty CurrentUrlProperty =
            DependencyProperty.Register("CurrentUrl", typeof(string), typeof(BrowserPage), new PropertyMetadata("", PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            var browser = (BrowserPage) sender;
            browser.Browser.Source = new Uri((string)args.NewValue);
        }

        public BrowserPage() {
            InitializeComponent();

            var binding = new Binding("CurrentUrl");
            this.SetBinding(CurrentUrlProperty, binding);

            Loaded += BrowserPage_Loaded;
        }

        void BrowserPage_Loaded(object sender, RoutedEventArgs e) {
            ViewModel.LoadPageSettings();
        }

        private void OnSettingsClicked(object sender, RoutedEventArgs e) {
            NavigationService.Navigate(new SettingsPage());
        }

        private BrowserViewModel ViewModel {
            get { return (BrowserViewModel) DataContext; }
        }
    }
}
