using k8config.Utilities;
using k8s.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terminal.Gui;

namespace k8config.GUIEvents.RealtimeMode.DataModels
{
    public class PodListType
    {
        private MemberInfo[] Members { get { return typeof(PodType).GetMembers().Where(x => x.GetCustomAttribute(typeof(DataNameAttribute), false) != null).ToArray(); } }
        public DataTable DataTable = new DataTable();
        public ConcurrentDictionary<long, PodType> Dictionary = new ConcurrentDictionary<long, PodType>();

        public PodListType()
        {
            DataTable.TableName = typeof(PodType).FullName;
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
        public void AddUpdateDelete(PodType newObject, CRUDOperation _operation)
        {
            PodType temp;
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

    public class PodType
    {
        public PodType(V1Pod _pod)
        {
            Namespace = _pod.Namespace();
            Name = _pod.Name();
            Ready = _pod.Status.ContainerStatuses == null ? "0/0" : $"{_pod.Status.ContainerStatuses?.Where(x => x.Ready == true).Count()}/{_pod.Status.ContainerStatuses?.Count()}";
            Status = _pod.Status?.Phase;
            Restarts = _pod.Status.ContainerStatuses?.Select(x => x.RestartCount)?.Sum() ?? 0;
            Age = DateTimeExtensions.GetPrettyDate(_pod.Metadata.CreationTimestamp ?? DateTime.Now);
            RawObject = _pod;
        }
        [DataName("Index", false)]
        public long Index { get; set; }
        [DataName("Namespace")]
        public string Namespace { get; set; }
        [DataName("Name")]
        public string Name { get; set; }
        [DataName("Ready")]
        public string Ready { get; set; }
        [DataName("Status")]
        public string Status { get; set; }
        [DataName("Restarts")]
        public int Restarts { get; set; }
        [DataName("Age")]
        public string Age { get; set; }
        [DataName("RawObject", false)]
        public object RawObject { get; set; }
    }
}
