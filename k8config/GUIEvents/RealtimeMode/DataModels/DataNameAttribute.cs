using System;
using System.Collections.Generic;
using System.Linq;

namespace k8config.GUIEvents.RealtimeMode.DataModels
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataNameAttribute : Attribute
    {
        protected string _valueName { get; set; }

        public string ValueName
        {
            get
            {
                return _valueName;
            }
            set
            {
                _valueName = value;
            }
        }

        public DataNameAttribute()
        {
            _valueName = "";
        }

        public DataNameAttribute(string valueName)
        {
            _valueName = valueName;
        }
    }
}