using MessageTransit.Monitor;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCenter
{
    public interface IConsumerContainer
    {
        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="topic"></param>
        void Add(string exchange, string topic);

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="topic"></param>
        void Remove(string exchange, string topic);        
    }
}
