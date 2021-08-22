using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config.DataModels
{
    public class ObjectPropertyType
    {
        public string name { get; set;}
        public Type kubeType { get; set; }
        public object kubeObject { get; set; }
    }
}
