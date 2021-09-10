using k8config.Utilities;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config.GUIEvents.RealtimeMode.DataModels
{
    public class ServiceType
    {
        public ServiceType(V1Service _service)
        {
            Namespace = _service.Namespace();
            Name = _service.Name();
            Type = _service.Spec.Type;
            ClusterIP = string.Join(",", _service.Spec?.ClusterIPs);
            ExternalIP = _service.Spec.ExternalIPs == null ? "<none>" : string.Join(",", _service.Spec?.ExternalIPs);
            Ports = string.Join(",", _service.Spec.Ports.Select(x => $"{x?.Port}{(x?.NodePort == null ? "" : ":" + x?.NodePort)}/{x?.Protocol}"));
            Age = DateTimeExtensions.GetPrettyDate(_service.Metadata.CreationTimestamp ?? DateTime.Now);
        }
        [DataName("Namespace")]
        public string Namespace { get; set; }
        [DataName("Name")]
        public string Name { get; set; }
        [DataName("Type")]
        public string Type { get; set; }
        [DataName("ClusterIP")]
        public string ClusterIP { get; set; }
        [DataName("ExternalIP")]
        public string ExternalIP { get; set; }
        [DataName("Ports")]
        public string Ports { get; set; }
        [DataName("Age")]
        public string Age { get; set; }
    }
}
