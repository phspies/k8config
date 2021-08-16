using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config.DataModels
{
    public class KubeObjectType
    {
        public string group { get; set; }
        public string kind { get; set; }
        public string version { get; set; }
    }
}
