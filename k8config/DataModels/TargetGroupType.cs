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
        public FieldFormat format { get; set; }
        public FieldType type { get; set; }
        public List<TargetGroupType> properties { get; set; }
    }
    public enum FieldFormat
    {
        Object = 0,
        Array = 1,
        Map = 2,
        Item = 3,
        Int32 = 4,
        Int64 = 5
    }
    public enum FieldType
    {
        Object = 0,
        String = 1,
        Integer = 2,
        Boolean = 3,
        Array = 4,

    }
}
