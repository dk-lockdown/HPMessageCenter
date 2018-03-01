using MessageCenter.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageCenter.Portal.Models
{
    public class ConsumerContainerIndexModel
    {
        public List<string> Consumers { get; set; }

        public List<Topic> Topics { get; set; }
    }
}
