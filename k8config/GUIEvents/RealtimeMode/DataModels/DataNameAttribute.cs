using System;
using System.Collections.Generic;
using System.Linq;

namespace k8config.GUIEvents.RealtimeMode.DataModels
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataNameAttribute : Attribute
    {
        protected string _column { get; set; }
        protected bool _visible { get; set; }

        public string Column
        {
            get
            {
                return _column;
            }
            set
            {
                _column = value;
            }
        }
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }

        public DataNameAttribute()
        {
            _column = "";
            _visible = true;
        }

        public DataNameAttribute(string column, bool visible = true)
        {
            _column = column;
            _visible = visible;
        }
    }
}