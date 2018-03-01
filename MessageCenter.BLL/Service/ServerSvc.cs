using MessageCenter.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCenter.BLL
{
    public class ServerSvc
    {
        public static void AddServerNode(string server)
        {
            if (ServerDA.ExistsServer(server))
            {
                throw new BusinessException("ServerHost已经存在！");
            }
            ServerDA.AddServerNode(server);
        }

        public static void RemoveServerNode(string server)
        {
            ServerDA.RemoveServerNode(server);
        }

        public static IEnumerable<string> LoadServerNodes()
        {
            return ServerDA.LoadServerNodes();
        }
    }
}
