using k8config.GUIEvents.RealtimeMode.DataModels;
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

namespace k8config.GUIDeployments.RealtimeMode.DataModels
{
    public class DeploymentListType
    {
        private MemberInfo[] Members { get { return typeof(DeploymentType).GetMembers().Where(x => x.GetCustomAttribute(typeof(DataNameAttribute), false) != null).ToArray(); } }
        public DataTable DataTable = new DataTable();
        public ConcurrentDictionary<long, DeploymentType> Dictionary = new ConcurrentDictionary<long, DeploymentType>();


        public DeploymentListType()
        {
            DataTable.TableName = typeof(DeploymentType).FullName;
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
        public void AddUpdateDelete(DeploymentType newObject, CRUDOperation _operation)
        {
            DeploymentType temp;
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
    public class DeploymentType
    {
        public DeploymentType(V1Deployment _deployment)
        {
            Index = long.MinValue;
            Namespace = _deployment.Namespace();
            Name = _deployment.Name();
            Ready = $"{_deployment.Status.AvailableReplicas ?? 0}/{_deployment.Status.Replicas ?? 0}";
            UpToDate = _deployment.Status.UpdatedReplicas ?? 0;
            Available = _deployment.Status.AvailableReplicas ?? 0;
            Age = DateTimeExtensions.GetPrettyDate(_deployment.Metadata.CreationTimestamp ?? DateTime.Now);
            RawObject = _deployment;
        }
        [DataName(column: "Index", visible: false)]
        public long Index { get; set; }
        [DataName(column: "Namespace")]
        public string Namespace { get; set; }
        [DataName("Name")]
        public string Name { get; set; }
        [DataName("Ready")]
        public string Ready { get; set; }
        [DataName("Up-To-Date")]
        public int UpToDate { get; set; }
        [DataName("Available")]
        public int Available { get; set; }
        [DataName("Age")]
        public string Age { get; set; }
        [DataName(column: "RawObject", visible: false)]
        public object RawObject { get; set; }
    }
}
