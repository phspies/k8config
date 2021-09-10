using k8config.GUIEvents.RealtimeMode.DataModels;
using k8config.Utilities;
using k8s.Models;
using System.Data;
using System.Linq;

namespace k8config.GUIEvents.RealtimeMode.DataTables
{
    public static class CRUDNamespaceDataTable
    {
        public static void AddOrUpdate(DataTable _datatable, V1Namespace _namespace)
        {
            DataRow _row = _datatable.AsEnumerable().FirstOrDefault(row => row.Field<string>("Name") == _namespace.GetNestedPropertyValue("Metadata.Name").ToString());
            if (_row != null)
            {
                _datatable.UpdateDataTable(_row, new NamespaceType(_namespace));
            }
            else
            {
                _datatable.AddDataTable(new NamespaceType(_namespace));
            }
        }
        public static void Delete(DataTable _datatable, V1Namespace _namespace)
        {
            DataRow _row = _datatable.AsEnumerable().FirstOrDefault(row => row.Field<string>("Name") == _namespace.GetNestedPropertyValue("Metadata.Name").ToString());
            if (_row != null)
            {
                _datatable.Rows.Remove(_row);
            }
        }
    }
}
