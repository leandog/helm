using System;
using System.Reflection;
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
            HideScriptErrors(browser.Browser, true);
            browser.Browser.Source = new Uri((string)args.NewValue);
        }

        public BrowserPage() {
            InitializeComponent();

            var binding = new Binding("CurrentUrl");
            SetBinding(CurrentUrlProperty, binding);

            HideScriptErrors(Browser, true);
            Loaded += OnBrowserPageLoaded;
            Unloaded += OnBrowserPageUnloaded;
        }

        private void OnBrowserPageLoaded(object sender, RoutedEventArgs args) {
            ViewModel.Load();
        }

        private void OnBrowserPageUnloaded(object sender, RoutedEventArgs args) {
            ViewModel.Unload();
        }

        public BrowserViewModel ViewModel {
            get { return (BrowserViewModel)DataContext; }
        }

        public static void HideScriptErrors(WebBrowser wb, bool Hide)
        {
            System.Reflection.FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);

            if (fiComWebBrowser == null) return;

            object objComWebBrowser = fiComWebBrowser.GetValue(wb);

            if (objComWebBrowser == null) return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { Hide });
        }
    }
}
