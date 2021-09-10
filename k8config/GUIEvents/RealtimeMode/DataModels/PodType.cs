using k8config.Utilities;
using k8s.Models;
using System;
using System.Linq;

namespace k8config.GUIEvents.RealtimeMode.DataModels
{
    public class PodType
    {
        public PodType(V1Pod _pod)
        {
            Namespace = _pod.Namespace();
            Name = _pod.Name();
            Ready = _pod.Status.ContainerStatuses == null ? "0/0" : $"{_pod.Status.ContainerStatuses?.Where(x => x.Ready == true).Count()}/{_pod.Status.ContainerStatuses?.Count()}";
            Status = _pod.Status?.Phase;
            Restarts = _pod.Status.ContainerStatuses?.Select(x => x.RestartCount)?.Sum() ?? 0;
            Age = DateTimeExtensions.GetPrettyDate(_pod.Metadata.CreationTimestamp ?? DateTime.Now);
        }
        [DataName("Namespace")]
        public string Namespace { get; set; }
        [DataName("Name")]
        public string Name { get; set; }
        [DataName("Ready")]
        public string Ready { get; set; }
        [DataName("Status")]
        public string Status { get; set; }
        [DataName("Restarts")]
        public int Restarts { get; set; }
        [DataName("Age")]
        public string Age { get; set; }
    }
}
