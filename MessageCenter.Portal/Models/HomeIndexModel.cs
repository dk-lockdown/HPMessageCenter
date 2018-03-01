using MessageCenter.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageCenter.Portal.Models
{
    public class HomeIndexModel
    {
        public List<App> Apps { get; set; }

        public List<Exchange> Exchanges { get; set; }
    }
}
