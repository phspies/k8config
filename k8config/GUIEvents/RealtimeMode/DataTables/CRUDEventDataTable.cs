using k8config.GUIEvents.RealtimeMode.DataModels;
using k8config.Utilities;
using k8s;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace k8config.GUIEvents.RealtimeMode.DataTables
{
    public static class CRUDEventDataTable
    {
        public static void Add(DataTable _datatable, WatchEventType _eventType, object _object)
        {
            _datatable.AddDataTable(new EventType(_eventType, _object));
        }
    }
}
