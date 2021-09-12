using System;
using System.Collections.Generic;
using System.Text;

namespace k8s.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyIsRequiredAttribute : Attribute
    {
        protected bool isRequired { get; set; }

        public bool ValueName
        {
            get
            {
                return isRequired;
            }
            set
            {
                isRequired = value;
            }
        }

        public PropertyIsRequiredAttribute()
        {
            isRequired = false;
        }

        public PropertyIsRequiredAttribute(bool valueName)
        {
            isRequired = valueName;
        }
    }

}
