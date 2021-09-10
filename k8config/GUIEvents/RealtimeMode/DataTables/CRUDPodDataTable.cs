using k8config.GUIEvents.RealtimeMode.DataModels;
using k8config.Utilities;
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
    public static class CRUDPodDataTable
    {
        public static void AddOrUpdate(DataTable _datatable, V1Pod _pod)
        {
            DataRow _row = _datatable.AsEnumerable().FirstOrDefault(row => row.Field<string>("Name") == _pod.GetNestedPropertyValue("Metadata.Name").ToString());
            if (_row != null)
            {
                _datatable.UpdateDataTable(_row, new PodType(_pod));
            }
            else
            {
                _datatable.AddDataTable(new PodType(_pod));
            }
        }
        public static void Delete(DataTable _datatable, V1Pod _pod)
        {
            DataRow _row = _datatable.AsEnumerable().FirstOrDefault(row => row.Field<string>("Name") == _pod.GetNestedPropertyValue("Metadata.Name").ToString());
            if (_row != null)
            {
                _datatable.Rows.Remove(_row);
            }
        }
    }
}
