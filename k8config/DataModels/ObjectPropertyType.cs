using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config.DataModels
{
    public class ObjectPropertyType
    {
        public int index { get; set; }
        public bool isList { get; set; }
        public string name { get; set;}
        public Type kubeType { get; set; }
        public string KubeTypeString { get; set; }
        public bool isRequired { get; set; }
        public object? kubeObject { get; set; }
    }
}
