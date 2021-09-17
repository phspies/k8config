using k8config.Utilities;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace k8config.GUIEvents.RealtimeMode.DataModels
{
    public class ServiceListType
    {
        private List<ServiceType> List;
        public ServiceListType()
        {
            List = new List<ServiceType>();
        }
        public void AddChange(ServiceType newObject)
        {
            if (List.Any(x => x.Name == newObject.Name))
            {
                lock (List)
                {
                    ServiceType temp = List.Find(x => x.Name == newObject.Name);
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
            var properties = typeof(ServiceType).GetProperties();
            DataTable dataTable = new DataTable();
            dataTable.TableName = typeof(ServiceType).FullName;
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
            var properties = typeof(ServiceType).GetProperties();
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
        public void Delete(ServiceType removeObject)
        {
            if (List.Any(x => x.Name == removeObject.Name))
            {
                lock (List)
                {
                    List.Remove(removeObject);
                }
            }
        }
        public List<ServiceType> Sort()
        {
            lock (List)
            {
                return new List<ServiceType>(List.OrderByDescending(x => x.Name).ToList());
            }
        }
    }
    public class ServiceType
    {
        public ServiceType(V1Service _service)
        {
            Namespace = _service.Namespace();
            Name = _service.Name();
            Type = _service.Spec.Type;
            ClusterIP = string.Join(",", _service.Spec?.ClusterIPs);
            ExternalIP = _service.Spec.ExternalIPs == null ? "<none>" : string.Join(",", _service.Spec?.ExternalIPs);
            Ports = string.Join(",", _service.Spec.Ports.Select(x => $"{x?.Port}{(x?.NodePort == null ? "" : ":" + x?.NodePort)}/{x?.Protocol}"));
            Age = DateTimeExtensions.GetPrettyDate(_service.Metadata.CreationTimestamp ?? DateTime.Now);
            RawObject = _service;
        }
        [DataName("Namespace")]
        public string Namespace { get; set; }
        [DataName("Name")]
        public string Name { get; set; }
        [DataName("Type")]
        public string Type { get; set; }
        [DataName("ClusterIP")]
        public string ClusterIP { get; set; }
        [DataName("ExternalIP")]
        public string ExternalIP { get; set; }
        [DataName("Ports")]
        public string Ports { get; set; }
        [DataName("Age")]
        public string Age { get; set; }
        [DataName("RawObject")]
        public object RawObject { get; set; }
    }
}
