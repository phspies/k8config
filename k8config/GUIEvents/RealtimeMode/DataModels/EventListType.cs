using AutoMapper.Internal;
using k8config.Utilities;
using k8s;
using k8s.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terminal.Gui;

namespace k8config.GUIEvents.RealtimeMode.DataModels
{
    public class EventListType
    {
        private MemberInfo[] Members { get { return typeof(EventType).GetMembers().Where(x => x.GetCustomAttribute(typeof(DataNameAttribute), false) != null).ToArray(); } }
        private ConcurrentDictionary<long, EventType> Dictionary = new ConcurrentDictionary<long, EventType>();
        private DataTable dataTable = new DataTable();
        public DataTable DataTableConstruct { get { return this.dataTable; } }
        public ConcurrentDictionary<long, EventType> DictionaryConstruct { get { return this.Dictionary; } }
        public EventListType()
        {
            dataTable.TableName = typeof(EventType).FullName;
            foreach (MemberInfo info in Members)
            {
                DataNameAttribute customerAttributes = info.GetCustomAttribute(typeof(DataNameAttribute), false) as DataNameAttribute;
                DataColumn newColumn = new DataColumn(customerAttributes.Column, ((PropertyInfo)info).PropertyType);
                if (customerAttributes.Visible == false)
                {
                    newColumn.ColumnMapping = MappingType.Hidden;
                }
                dataTable.Columns.Add(newColumn);
            }
        }

        public void Add(EventType newObject)
        {
            lock (dataTable.Rows.SyncRoot)
            {
                DataRow _row = dataTable.NewRow();
                for (int i = 0; i < Members.Length; i++)
                {
                    _row.SetField((Members[i].GetCustomAttribute(typeof(DataNameAttribute), false) as DataNameAttribute).Column, Members[i].GetMemberValue(newObject));
                }
                dataTable.Rows.InsertAt(_row,0);
            }
        }
        public DataTable RebuilDataTable()
        {
            lock (dataTable.Rows.SyncRoot)
            {
                dataTable.Rows.Clear();
                (from x in Dictionary orderby x.Value.TimeStamp descending select x).ToList().ForEach(record =>
                {
                    DataRow _row = dataTable.NewRow();
                    for (int i = 0; i < Members.Length; i++)
                    {
                        _row.SetField((Members[i].GetCustomAttribute(typeof(DataNameAttribute), false) as DataNameAttribute).Column, record.Key);
                    }
                    dataTable.Rows.Add(_row);
                });
                Debug.WriteLine($"{dataTable.Rows.Count} EventType rows to return");
            }
            return dataTable;
        }
    }
    public class EventType
    {
        public EventType(WatchEventType _eventType, object _object)
        {
            TimeStamp = DateTime.Now;
            Action = _eventType.ToString();
            RawObject = _object;
            Name = ((IKubernetesObject<V1ObjectMeta>)_object).Metadata.Name;
            Type = ((IKubernetesObject<V1ObjectMeta>)RawObject).Kind;
        }
       
        [DataName(column: "TimeStamp", visible: true)]
        public DateTime TimeStamp { get; set; }
        [DataName(column: "Name", visible: true)]
        public string Name { get; set; }
        [DataName(column: "Kubernetes Type", visible: true)]
        public string Type { get; set; }
        [DataName(column: "Action", visible: true)]
        public string Action { get; set; }
        [DataName(column: "RawObject", visible: false)]
        public object RawObject { get; set; }
    }
}
