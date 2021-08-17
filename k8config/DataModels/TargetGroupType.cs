using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config.DataModels
{
    public class TargetGroupType
    {
        public TargetGroupType()
        {
            properties = new List<TargetGroupType>();
        }
        public int index { get; set; }
        public bool rootObject { get; set; }
        public bool required { get; set; }
        public KubeObjectType kubedetails { get; set; }
        public string comment { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public string format { get; set; }
        public bool isItem { get; set; }
        public string type { get; set; }
        public List<TargetGroupType> properties { get; set; }
    }
}
