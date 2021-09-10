using k8config.GUIEvents.RealtimeMode.DataModels;
using k8config.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Terminal.Gui;

namespace k8config.GUIEvents.RealtimeMode.DataTables
{
    public static class DataTableExtention
    {
        public static DataTable CreateDataTable<T>()
        {
            var properties = typeof(T).GetProperties();
            DataTable dataTable = new DataTable();
            dataTable.TableName = typeof(T).FullName;
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn((info.GetCustomAttribute(typeof(DataNameAttribute), false) as DataNameAttribute).ValueName, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }
            return dataTable;
        }
        public static DataTable CreateDataTablePointer<T>()
        {
            var properties = typeof(T).GetProperties();
            DataTable dataTable = new DataTable();
            dataTable.TableName = typeof(T).FullName;
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn((info.GetCustomAttribute(typeof(DataNameAttribute), false) as DataNameAttribute).ValueName, typeof(string)));
            }
            return dataTable;
        }
        public static DataTable PupulateDataTable<T>(IEnumerable<T> list, DataTable dataTable)
        {
            var properties = typeof(T).GetProperties();
            foreach (T entity in list)
            {
                if (!dataTable.AsEnumerable().Any(row => row.Field<string>("Name") == properties.FirstOrDefault(x => x.Name == "Name").GetValue(entity).ToString()))
                {
                    DataRow _row = dataTable.NewRow();
                    for (int i = 0; i < properties.Length; i++)
                    {
                        _row.SetField((properties[i].GetCustomAttribute(typeof(DataNameAttribute), false) as DataNameAttribute).ValueName, properties[i].GetValue(entity));

                    }
                    dataTable.Rows.Add(_row);
                }
            }
            return dataTable;
        }
        public static void AddDataTable<T>(this DataTable dataTable, T newObject)
        {
            Monitor.Enter(dataTable.Rows.SyncRoot);
            try
            {
                var properties = typeof(T).GetProperties();
                DataRow _row = dataTable.NewRow();
                for (int i = 0; i < properties.Length; i++)
                {
                    _row.SetField((properties[i].GetCustomAttribute(typeof(DataNameAttribute), false) as DataNameAttribute).ValueName, properties[i].GetValue(newObject));

                }
                dataTable.Rows.Add(_row);
            }
            finally
            {
                Monitor.Exit(dataTable.Rows.SyncRoot);
            }
        }
        public static void AddDataTablePointer<T>(this DataTable dataTable, T newObject)
        {
            var properties = typeof(T).GetProperties();
            DataRow _row = dataTable.NewRow();
            for (int i = 0; i < properties.Length; i++)
            {
                _row.SetField((properties[i].GetCustomAttribute(typeof(DataNameAttribute), false) as DataNameAttribute).ValueName, newObject.GetPropValue("Name"));

            }
            dataTable.Rows.Add(_row);
        }
        public static void UpdateDataTable<T>(this DataTable _datatable, DataRow _row, T entity)
        {
            Monitor.Enter(_datatable.Rows.SyncRoot);
            try
            {
                var properties = typeof(T).GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    _row.SetField((properties[i].GetCustomAttribute(typeof(DataNameAttribute), false) as DataNameAttribute).ValueName, properties[i].GetValue(entity));
                }
            }
            finally
            {
                Monitor.Exit(_datatable.Rows.SyncRoot);
            }

        }
        public static DataTable Sort<T>(DataTable dataTable)
        {
            DataTable _returnTable = new DataTable();
            Monitor.Enter(dataTable.Rows.SyncRoot);
            try
            {
                DataView dv = dataTable.DefaultView;
                dv.ApplyDefaultSort = false;
                var properties = typeof(T).GetProperties();
                if (properties.Any(x => x.Name == "Namespace"))
                {
                    dv.Sort = "Namespace asc, Name asc";
                }
                else if (properties.Any(x => x.Name == "TimeStamp"))
                {
                    dv.Sort = "TimeStamp desc";
                }
                else
                {
                    dv.Sort = "Name asc";
                }
                _returnTable = dv.ToTable();
            }
            finally
            {
                Monitor.Exit(dataTable.Rows.SyncRoot);
            }

            //if (dataTable.TableName.Contains("EventType"))
            //{
            //    string filename = $"C:\\Users\\Phillip\\events.csv";
            //    _returnTable.ToCSV(filename);
            //}
            return _returnTable;
        }
        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();
        public static void ToCSV(this DataTable dtDataTable, string strFilePath)
        {
            _readWriteLock.EnterWriteLock();
            using (StreamWriter sw = new StreamWriter(strFilePath, true))
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    sw.Write(dtDataTable.Columns[i]);
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
                foreach (DataRow dr in dtDataTable.Rows)
                {
                    for (int i = 0; i < dtDataTable.Columns.Count; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            string value = dr[i].ToString();
                            if (value.Contains(','))
                            {
                                value = String.Format("\"{0}\"", value);
                                sw.Write(value);
                            }
                            else
                            {
                                sw.Write(dr[i].ToString());
                            }
                        }
                        if (i < dtDataTable.Columns.Count - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                sw.Close();
            }
            _readWriteLock.ExitWriteLock();
        }
    }
}
