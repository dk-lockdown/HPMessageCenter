#if NETSTANDARD1_3 || NETSTANDARD2_0
using System;
using System.Collections.Generic;

namespace MessageCenter.Framework.Log
{
    internal static class EmitterFactory
    {
        private static List<ILogEmitter> s_Emitters = null;
        private static object s_SyncObj = new object();

        public static List<ILogEmitter> Create()
        {
            if (s_Emitters == null)
            {
                lock (s_SyncObj)
                {
                    if (s_Emitters == null)
                    {
                        LogSetting s = LogSection.GetSetting();
                        List<ILogEmitter> list = new List<ILogEmitter>();

                        if (s.Emitters != null)
                        {
                            s.Emitters.ForEach(p =>
                            {
                                ILogEmitter e;
                                switch (p.Type)
                                {
                                    case "text":
                                        e = new TextEmitter();
                                        break; 
                                    default:
                                        Type type = Type.GetType(p.Type, true);
                                        e = (ILogEmitter)Activator.CreateInstance(type);
                                        break;
                                }
                                e.Init(p.Parameters);
                                list.Add(e);
                            });
                        }
                      
                        s_Emitters = list;
                    }
                }
            }
            return s_Emitters;
        }
    }
}
#endif