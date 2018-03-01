using System;
using System.Collections.Generic;

namespace MessageCenter.Framework.Log
{
    public interface ILogEmitter
    {
        void Init(Dictionary<string, string> param);

        void EmitLog(LogEntry log);
    }
}
