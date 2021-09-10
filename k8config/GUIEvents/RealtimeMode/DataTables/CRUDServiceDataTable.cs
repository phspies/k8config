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
    public static class CRUDServiceDataTable
    {
        public static void AddOrUpdate(DataTable _datatable, V1Service _service)
        {
            DataRow _row = _datatable.AsEnumerable().FirstOrDefault(row => row.Field<string>("Name") == _service.GetNestedPropertyValue("Metadata.Name").ToString());
            if (_row != null)
            {
                _datatable.UpdateDataTable(_row, new ServiceType(_service));
            }
            else
            {
                _datatable.AddDataTable(new ServiceType(_service));
            }
        }
        public static void Delete(DataTable _datatable, V1Service _service)
        {
            DataRow _row = _datatable.AsEnumerable().FirstOrDefault(row => row.Field<string>("Name") == _service.GetNestedPropertyValue("Metadata.Name").ToString());
            if (_row != null)
            {
                _datatable.Rows.Remove(_row);
            }
        }
    }
}
