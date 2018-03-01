using MessageCenter.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageCenter.Portal.Models
{
    public class ClusterIndexModel
    {
        public List<string> Servers { get; set; }

        public List<Topic> Topics { get; set; }
    }
}
