using System;
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
            var browser = (BrowserPage)sender;
            browser.Browser.Source = new Uri((string)args.NewValue);
        }

        public BrowserPage() {
            InitializeComponent();

            var binding = new Binding("CurrentUrl");
            SetBinding(CurrentUrlProperty, binding);

            Loaded += OnBrowserPageLoaded;
            Unloaded += OnBrowserPageUnloaded;
        }

        private void OnBrowserPageLoaded(object sender, RoutedEventArgs args) {
            ViewModel.Load();
        }

        private void OnBrowserPageUnloaded(object sender, RoutedEventArgs args) {
            ViewModel.Unload();
        }

        private BrowserViewModel ViewModel {
            get { return (BrowserViewModel)DataContext; }
        }
    }
}
