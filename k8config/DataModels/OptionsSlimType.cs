using System;

namespace k8config.DataModels
{
    public class OptionsSlimType
    {
        public int index { get; set; }
        public string name { get; set;  }
        public object value { get; set; }
        public string displayType { get; set; }
        public bool isRequired { get; set; }
        public bool isList { get; set; }
        public Type type { get; set; }
    }
}
