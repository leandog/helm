using System;

namespace Radiator {

    public class RecognizedEventArgs : EventArgs {

        public RecognizedEventArgs(string value) {
            Value = value;
        }

        public string Value { get; private set; }
    }
}
