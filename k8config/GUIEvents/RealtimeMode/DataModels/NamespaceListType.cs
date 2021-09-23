using k8config.Utilities;
using k8s.Models;
using System;
using System.Collections.Concurrent;
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
        private MemberInfo[] Members { get { return typeof(NamespaceType).GetMembers().Where(x => x.GetCustomAttribute(typeof(DataNameAttribute), false) != null).ToArray(); } }
        public DataTable DataTable = new DataTable();
        public ConcurrentDictionary<long, NamespaceType> Dictionary = new ConcurrentDictionary<long, NamespaceType>();

        public NamespaceListType()
        {
            DataTable.TableName = typeof(NamespaceType).FullName;
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
        public void AddUpdateDelete(NamespaceType newObject, CRUDOperation _operation)
        {
            NamespaceType temp;
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
                (from x in Dictionary orderby x.Value.Name select x).ToList().ForEach(record =>
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
    public class NamespaceType
    {
        public NamespaceType(V1Namespace _namespace)
        {
            Name = _namespace.Name();
            Status = _namespace.Status.Phase;
            Age = DateTimeExtensions.GetPrettyDate(_namespace.Metadata.CreationTimestamp ?? DateTime.Now);
            RawObject = _namespace;
        }
        [DataName("Index", false)]
        public long Index { get; set; }
        [DataName("Name")]
        public string Name { get; set; }
        [DataName("Status")]
        public string Status { get; set; }
        [DataName("Age")]
        public string Age { get; set; }
        [DataName("RawObject", false)]
        public object RawObject { get; set; }
    }
}
