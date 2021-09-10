using k8config.Utilities;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config.GUIEvents.RealtimeMode.DataModels
{
    public class NamespaceType
    {
        public NamespaceType(V1Namespace _namespace)
        {
            Name = _namespace.Name();
            Status = _namespace.Status.Phase;
            Age = DateTimeExtensions.GetPrettyDate(_namespace.Metadata.CreationTimestamp ?? DateTime.Now);
        }
        [DataName("Name")]
        public string Name { get; set; }

        [DataName("Status")]
        public string Status { get; set; }

        [DataName("Age")]
        public string Age { get; set; }
    }
}
