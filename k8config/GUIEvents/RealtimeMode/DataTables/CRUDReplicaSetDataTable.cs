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
    public static class CRUDReplicaSetDataTable
    {
        public static void AddOrUpdate(DataTable _datatable, V1ReplicaSet _replicaset)
        {
            DataRow _row = _datatable.AsEnumerable().FirstOrDefault(row => row.Field<string>("Name") == _replicaset.GetNestedPropertyValue("Metadata.Name").ToString());
            lock (_datatable)
            {
                if (_row != null)
                {

                    _datatable.UpdateDataTable(_row, new ReplicaSetType(_replicaset));
                }
                else
                {
                    _datatable.AddDataTable(new ReplicaSetType(_replicaset));
                }
            }
        }
        public static void Delete(DataTable _datatable, V1ReplicaSet _replicaset)
        {
            DataRow _row = _datatable.AsEnumerable().FirstOrDefault(row => row.Field<string>("Name") == _replicaset.GetNestedPropertyValue("Metadata.Name").ToString());
            if (_row != null)
            {
                _datatable.Rows.Remove(_row);
            }
        }
    }
}
