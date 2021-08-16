using System.Collections.Generic;

namespace k8config.DataModels
{
    public class GroupType
    {
        public GroupType()
        {
            properties = new List<GroupType>();
        }
        public bool rootObject { get; set; }
        public bool required { get; set; }
        public List<KubeObjectType> kubedetails { get; set; }
        public string name { get; set; }
        public string reference { get; set; }
        public string fullpath { get; set; }
        public string description { get; set; }
        public string format { get; set; }
        public string type { get; set; }
        public List<GroupType> properties { get; set; }
    }
}
