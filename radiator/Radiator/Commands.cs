using System.Collections.Generic;
using System.Windows.Input;

namespace Radiator {

    public static class Commands {

        public static RoutedCommand ShowSettingsCommand = new RoutedCommand("ShowSettings", typeof (Commands),
            new InputGestureCollection(new List<object> { new KeyGesture(Key.S, ModifierKeys.Control|ModifierKeys.Shift)}));
    }
}
