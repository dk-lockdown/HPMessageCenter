using MessageTransit.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransit
{
    public class TextMessage:IMessage
    {
        public Dictionary<string, string> Headers { get; set; }
        public string MessageText { get; set; }

        public IMessage putHeaders(string key, string value)
        {
            if(Headers==null)
            {
                Headers = new Dictionary<string, string>();
            }
            Headers.Add(key, value);
            return this;
        }
    }
}
