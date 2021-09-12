using k8config.Utilities;
using k8s;
using k8s.Models;
using System;
using System.Linq;

namespace k8config.GUIEvents.RealtimeMode.DataModels
{
    public class EventType
    {
        private object privateobject;
        public EventType(WatchEventType _eventType, object _object)
        {
            TimeStamp = DateTime.Now;
            Action = _eventType.ToString();
            privateobject = _object;
            Name = _object.GetNestedPropertyValue("Metadata.Name")?.ToString();
        }
        [DataName("TimeStamp")]
        public DateTime TimeStamp { get; set; }
        [DataName("Name")]
        public string Name { get; set; }
        [DataName("Kubernetes Type")]
        public string Type
        {
            get { return privateobject.GetType().Name.Split(".").Last().ToString(); }
            private set { }
        }
        [DataName("Action")]
        public string Action { get; set; }


    }
}
