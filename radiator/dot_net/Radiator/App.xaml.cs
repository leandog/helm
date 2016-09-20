using System.Windows;
using Akavache;

namespace Radiator {

    public partial class App : Application {

        protected override void OnStartup(StartupEventArgs e) {
            BlobCache.ApplicationName = "Radiator";
        }
    }
}
