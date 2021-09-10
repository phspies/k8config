using k8config.Utilities;
using k8s.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace k8config.GUIEvents.RealtimeMode.DataModels
{
    public class PodType : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _namespace = string.Empty;
        private string _name = string.Empty;
        private string _ready = string.Empty;
        private string _status = string.Empty;
        private int _restarts = int.MinValue;
        private string _age = string.Empty;

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
        public string Namespace
        {
            get
            {
                return this._namespace;
            }

            set
            {
                if (value != this._namespace)
                {
                    this._namespace = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [DataName("Name")]
        public string Name
        {
            get
            {
                return this._name;
            }

            set
            {
                if (value != this._name)
                {
                    this._name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [DataName("Ready")]
        public string Ready
        {
            get
            {
                return this._ready;
            }

            set
            {
                if (value != this._ready)
                {
                    this._ready = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [DataName("Status")]
        public string Status
        {
            get
            {
                return this._status;
            }

            set
            {
                if (value != this._status)
                {
                    this._status = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [DataName("Restarts")]
        public int Restarts
        {
            get
            {
                return this._restarts;
            }

            set
            {
                if (value != this._restarts)
                {
                    this._restarts = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [DataName("Age")]
        public string Age
        {
            get
            {
                return this._age;
            }

            set
            {
                if (value != this._age)
                {
                    this._age = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
