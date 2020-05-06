using System;

namespace Jay.VTS
{
    public class FrameEventArgs : EventArgs 
    {
        public enum Exits { ReturnValue = -1, Return = 0, InternalException = 1, CodeException = 2 }
        public Exits ExitCode;
        public VTSException Error;
        public string InternalError;

        public FrameEventArgs() { }

        public FrameEventArgs SetExitCode(Exits Code) { 
            ExitCode = Code;
            return this;
        }

        public FrameEventArgs SetError(VTSException Error) {
            this.Error = Error;
            return this;
        }

        public FrameEventArgs SetInternal(string Internal) {
            this.InternalError = Internal;
            return this;
        }
    }
}