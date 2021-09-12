using System;
using System.Collections.Generic;
using System.Text;

namespace k8s.Models
{
    [AttributeUsage(AttributeTargets.All)]
    public class KubernetesPropertyAttribute : Attribute
    {
        private string descriptionValue { get; set; }
        private bool isrequiredValue { get; set; }
        public KubernetesPropertyAttribute(bool IsRequired, string Description)
        {
            this.descriptionValue = Description;
            this.isrequiredValue = IsRequired;
        }

        public KubernetesPropertyAttribute(string Description)
        {
            this.descriptionValue = Description;
        }

        public virtual string Description
        {
            get
            {
                return this.descriptionValue;
            }
        }

        public virtual bool IsRequired
        {
            get
            {
                return this.isrequiredValue;
            }
        }
    }
}
