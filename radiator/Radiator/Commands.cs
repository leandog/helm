using System.Collections.Generic;
using System.Windows.Input;

namespace Radiator {

    public static class Commands {

        public static RoutedCommand NextPageCommand = new RoutedCommand("NextPage", typeof(Commands),
    new InputGestureCollection(new List<object> { new KeyGesture(Key.Right) }));

        public static RoutedCommand PrevPageCommand = new RoutedCommand("PrevPage", typeof(Commands),
    new InputGestureCollection(new List<object> { new KeyGesture(Key.Left) }));

        public static RoutedCommand ShowSettingsCommand = new RoutedCommand("ShowSettings", typeof (Commands),
            new InputGestureCollection(new List<object> { new KeyGesture(Key.S, ModifierKeys.Control|ModifierKeys.Shift)}));

    }
}
