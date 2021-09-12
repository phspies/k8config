using System;
using System.Collections.Generic;
using System.Text;

namespace k8s.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyDescriptionAttribute : Attribute
    {
        protected string description { get; set; }

        public string ValueName
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        public PropertyDescriptionAttribute()
        {
            description = "";
        }

        public PropertyDescriptionAttribute(string valueName)
        {
            description = valueName;
        }
    }

}
