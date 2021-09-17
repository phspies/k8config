using k8config.Utilities;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace k8config.GUIEvents.RealtimeMode.DataModels
{
    public class NamespaceListType
    {
        private List<NamespaceType> List;
        public NamespaceListType()
        {
            List = new List<NamespaceType>();
        }
        public void AddChange(NamespaceType newObject)
        {
            if (List.Any(x => x.Name == newObject.Name))
            {
                lock (List)
                {
                    NamespaceType temp = List.Find(x => x.Name == newObject.Name);
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
            var properties = typeof(NamespaceType).GetProperties();
            DataTable dataTable = new DataTable();
            dataTable.TableName = typeof(NamespaceType).FullName;
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
            var properties = typeof(NamespaceType).GetProperties();
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
        public void Delete(NamespaceType removeObject)
        {
            if (List.Any(x => x.Name == removeObject.Name))
            {
                lock (List)
                {
                    List.Remove(removeObject);
                }
            }
        }
        public List<NamespaceType> Sort()
        {
            lock (List)
            {
                return new List<NamespaceType>(List.OrderByDescending(x => x.Name).ToList());
            }
        }
    }
    public class NamespaceType
    {
        public NamespaceType(V1Namespace _namespace)
        {
            Name = _namespace.Name();
            Status = _namespace.Status.Phase;
            Age = DateTimeExtensions.GetPrettyDate(_namespace.Metadata.CreationTimestamp ?? DateTime.Now);
            RawObject = _namespace;
        }
        [DataName("Name")]
        public string Name { get; set; }

        [DataName("Status")]
        public string Status { get; set; }

        [DataName("Age")]
        public string Age { get; set; }
        [DataName("RawObject")]
        public object RawObject { get; set; }
    }
}
