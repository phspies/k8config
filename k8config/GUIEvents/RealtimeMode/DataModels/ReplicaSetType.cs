using k8config.Utilities;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config.GUIEvents.RealtimeMode.DataModels
{
    public class ReplicaSetType
    {
        public ReplicaSetType(V1ReplicaSet _replicaset)
        {
            Namespace = _replicaset.Namespace();
            Name = _replicaset.Name();
            Desired = _replicaset.Spec.Replicas ?? 0;
            Current = _replicaset.Status.AvailableReplicas ?? 0;
            Ready = _replicaset.Status.ReadyReplicas ?? 0;
            Age = DateTimeExtensions.GetPrettyDate(_replicaset.Metadata.CreationTimestamp ?? DateTime.Now);
        }
        [DataName("Namespace")]
        public string Namespace { get; set; }
        [DataName("Name")]
        public string Name { get; set; }
        [DataName("Desired")]
        public int Desired { get; set; }
        [DataName("Current")]
        public int Current { get; set; }
        [DataName("Ready")]
        public int Ready { get; set; }
        [DataName("Age")]
        public string Age { get; set; }
    }
}
