using k8config.Utilities;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config.GUIEvents.RealtimeMode.DataModels
{
    public class DeploymentType
    {
        public DeploymentType(V1Deployment _deployment)
        {
            Namespace = _deployment.Namespace();
            Name = _deployment.Name();
            Ready = $"{_deployment.Status.AvailableReplicas ?? 0}/{_deployment.Status.Replicas ?? 0}";
            UpToDate = _deployment.Status.UpdatedReplicas ?? 0;
            Available = _deployment.Status.AvailableReplicas ?? 0;
            Age = DateTimeExtensions.GetPrettyDate(_deployment.Metadata.CreationTimestamp ?? DateTime.Now);
        }
        [DataName("Namespace")]
        public string Namespace { get; set; }
        [DataName("Name")]
        public string Name { get; set; }
        [DataName("Ready")]
        public string Ready { get; set; }
        [DataName("Up-To-Date")]
        public int UpToDate { get; set; }
        [DataName("Available")]
        public int Available { get; set; }
        [DataName("Age")]
        public string Age { get; set; }
    }
}
