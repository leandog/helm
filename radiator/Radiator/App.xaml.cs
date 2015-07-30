using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Navigation;
using Akavache;

namespace Radiator {

    public partial class App : Application {

        protected override async void OnStartup(StartupEventArgs e) {
            BlobCache.ApplicationName = "Radiator";

        }
    }
}
