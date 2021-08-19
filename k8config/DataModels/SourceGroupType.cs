using System.Collections.Generic;

namespace k8config.DataModels
{
    public class SourceGroupType
    {
        public SourceGroupType()
        {
            properties = new List<SourceGroupType>();
        }
        public bool rootObject { get; set; }
        public bool required { get; set; }
        public List<KubeObjectType> kubedetails { get; set; }
        public string name { get; set; }
        public string reference { get; set; }
        public string fullpath { get; set; }
        public string description { get; set; }
        public FieldFormat format { get; set; }
        public FieldType type { get; set; }
        public List<SourceGroupType> properties { get; set; }
    }
}
