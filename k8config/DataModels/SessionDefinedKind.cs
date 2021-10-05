using k8s;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config.DataModels
{
    public class SessionDefinedKind
    {
        public string name { get; set; }
        public int index { get; set; }
        public string kind { get; set; }
        public object KubeObject { get; set; }
        public string displayKind { get { return kind.Replace("V1", ""); } }
        public IKubernetesObject<V1ObjectMeta> metaData { get { return KubeObject as IKubernetesObject<V1ObjectMeta>; } }
    }
}
