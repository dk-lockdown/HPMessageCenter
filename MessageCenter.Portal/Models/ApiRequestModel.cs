using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageCenter.Portal.Models
{
    public class ApiRequestModel<T>
    {
        public string RequestTime { get; set; }

        public string Sign { get; set; }

        public T Data { get; set; }
    }
}
