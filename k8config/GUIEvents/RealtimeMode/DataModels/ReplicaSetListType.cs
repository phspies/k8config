using k8config.Utilities;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace k8config.GUIEvents.RealtimeMode.DataModels
{
    public class ReplicaSetListType
    {
        private List<ReplicaSetType> List;
        public ReplicaSetListType()
        {
            List = new List<ReplicaSetType>();
        }
        public void AddChange(ReplicaSetType newObject)
        {
            if (List.Any(x => x.Name == newObject.Name))
            {
                lock (List)
                {
                    ReplicaSetType temp = List.Find(x => x.Name == newObject.Name);
                    temp = newObject;
                }
            }
            else
            {
                lock (List)
                {
                    List.Add(newObject);
                }
            }
        }
        public DataTable CreateDataTable()
        {
            var properties = typeof(ReplicaSetType).GetProperties();
            DataTable dataTable = new DataTable();
            dataTable.TableName = typeof(ReplicaSetType).FullName;
            foreach (PropertyInfo info in properties)
            {
                DataColumn newColumn = new DataColumn((info.GetCustomAttribute(typeof(DataNameAttribute), false) as DataNameAttribute).Column, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType);
                if (newColumn.ColumnName == "RawObject")
                {
                    newColumn.ColumnMapping = MappingType.Hidden;
                }
                dataTable.Columns.Add(newColumn);
            }
            return dataTable;
        }
        public DataTable ToDataTable()
        {
            var properties = typeof(ReplicaSetType).GetProperties();
            DataTable dataTable = CreateDataTable();
            Sort().ForEach(entity =>
            {
                DataRow _row = dataTable.NewRow();
                for (int i = 0; i < properties.Length; i++)
                {
                    _row.SetField((properties[i].GetCustomAttribute(typeof(DataNameAttribute), false) as DataNameAttribute).Column, properties[i].GetValue(entity));

                }
                dataTable.Rows.Add(_row);
            });
            return dataTable;
        }
        public void Delete(ReplicaSetType removeObject)
        {
            if (List.Any(x => x.Name == removeObject.Name))
            {
                lock (List)
                {
                    List.Remove(removeObject);
                }
            }
        }
        public List<ReplicaSetType> Sort()
        {
            lock (List)
            {
                return new List<ReplicaSetType>(List.OrderByDescending(x => x.Name).ToList());
            }
        }
    }
    public class ReplicaSetType
    {
        public ReplicaSetType(V1ReplicaSet _replicaset)
        {
            Namespace = _replicaset.Namespace();
            Name = _replicaset.Name();
            Desired = _replicaset.Spec.Replicas ?? 0;
            Current = _replicaset.Status.AvailableReplicas ?? 0;
            Ready = _replicaset.Status.ReadyReplicas ?? 0;
            Age = DateTimeExtensions.GetPrettyDate(_replicaset.Metadata.CreationTimestamp ?? DateTime.Now);
            RawObject = _replicaset;
        }
        [DataName("Namespace")]
        public string Namespace { get; set; }
        [DataName("Name")]
        public string Name { get; set; }
        [DataName("Desired")]
        public int Desired { get; set; }
        [DataName("Current")]
        public int Current { get; set; }
        [DataName("Ready")]
        public int Ready { get; set; }
        [DataName("Age")]
        public string Age { get; set; }
        [DataName("RawObject")]
        public object RawObject { get; set; }
    }
}
