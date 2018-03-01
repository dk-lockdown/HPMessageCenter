using MessageTransit.Message;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageTransit
{
    public interface IProcessor
    {
        Task<bool> Process(IMessage message);
    }
   
}
