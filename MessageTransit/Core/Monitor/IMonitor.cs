using MessageTransit.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransit.Monitor
{
    public interface IMonitor
    {
        void onEvent(MessageEventArgs e);
    }

    public class MessageEventArgs : EventArgs
    {
        public IMessage message { get; private set; }

        public MessageEventArgs(IMessage message)
        {
            this.message = message;
        }
    }

    public class MessageSuccessEventArgs : MessageEventArgs
    {
        public long milliseconds { get; private set; }

        public MessageSuccessEventArgs(IMessage message,long milliseconds)
            : base(message)
        {
            this.milliseconds = milliseconds;
        }
    }

    public class MessageExceptionEventArgs : MessageEventArgs
    {
        public Exception ex;
        public long milliseconds { get; private set; }

        public MessageExceptionEventArgs(IMessage message,Exception ex, long milliseconds)
            :base(message)
        {
            this.ex = ex;
            this.milliseconds = milliseconds;
        }
    }
}
