using k8config.Utilities;
using k8s.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace k8config.GUIEvents.RealtimeMode.DataModels
{
    public class ServiceListType
    {
        private MemberInfo[] Members { get { return typeof(ServiceType).GetMembers().Where(x => x.GetCustomAttribute(typeof(DataNameAttribute), false) != null).ToArray(); } }
        public DataTable DataTable = new DataTable();
        public ConcurrentDictionary<long, ServiceType> Dictionary = new ConcurrentDictionary<long, ServiceType>();

        public ServiceListType()
        {
            DataTable.TableName = typeof(ServiceType).FullName;
            foreach (MemberInfo info in Members)
            {
                DataNameAttribute customerAttributes = info.GetCustomAttribute(typeof(DataNameAttribute), false) as DataNameAttribute;
                DataColumn newColumn = new DataColumn(customerAttributes.Column, typeof(long));
                if (customerAttributes.Visible == false)
                {
                    newColumn.ColumnMapping = MappingType.Hidden;
                }
                DataTable.Columns.Add(newColumn);
            }
        }
        public void AddUpdateDelete(ServiceType newObject, CRUDOperation _operation)
        {
            ServiceType temp;
            var Index = Dictionary.FirstOrDefault(x => x.Value.Name == newObject.Name);
            switch (_operation)
            {
                case CRUDOperation.Add:
                    if (Index.Value == null)
                    {
                        newObject.Index = Dictionary.Count > 0 ? Dictionary.Max(x => x.Key) + 1 : 0;
                        Dictionary.TryAdd(newObject.Index, newObject);
                    }
                    break;
                case CRUDOperation.Change:
                    if (Index.Value != null && Dictionary.TryGetValue(Index.Key, out temp))
                    {
                        newObject.Index = Index.Key;
                        Dictionary.TryUpdate(newObject.Index, newObject, temp);
                    }
                    break;
                case CRUDOperation.Delete:
                    if (Index.Value != null && Dictionary.TryGetValue(Index.Key, out temp))
                    {
                        Dictionary.TryRemove(Index.Key, out temp);
                    }
                    break;
            }
            lock (DataTable.Rows.SyncRoot)
            {
                DataTable.Rows.Clear();
                (from x in Dictionary orderby x.Value.Namespace, x.Value.Name select x).ToList().ForEach(record =>
                {
                    DataRow _row = DataTable.NewRow();
                    for (int i = 0; i < Members.Length; i++)
                    {
                        _row.SetField((Members[i].GetCustomAttribute(typeof(DataNameAttribute), false) as DataNameAttribute).Column, record.Key);
                    }
                    DataTable.Rows.Add(_row);
                });
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
        [DataName("Index", false)]
        public long Index { get; set; }
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
        [DataName("RawObject", false)]
        public object RawObject { get; set; }
    }
}
