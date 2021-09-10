using k8config.GUIEvents.RealtimeMode.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

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
        public static DataTable PupulateDataTable<T>(IEnumerable<T> list, DataTable dataTable)
        {
            var properties = typeof(T).GetProperties();
            foreach (T entity in list)
            {
                if (!dataTable.AsEnumerable().Any(row => row.Field<string>("Name") == properties.FirstOrDefault(x=> x.Name == "Name").GetValue(entity).ToString()))
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
            var properties = typeof(T).GetProperties();
            DataRow _row = dataTable.NewRow();
            for (int i = 0; i < properties.Length; i++)
            {
                _row.SetField((properties[i].GetCustomAttribute(typeof(DataNameAttribute), false) as DataNameAttribute).ValueName, properties[i].GetValue(newObject));

            }
            dataTable.Rows.Add(_row);
        }
        public static void UpdateDataTable<T>(this DataTable _datatable, DataRow _row, T entity)
        {
            var properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                _row.SetField((properties[i].GetCustomAttribute(typeof(DataNameAttribute), false) as DataNameAttribute).ValueName, properties[i].GetValue(entity));
            }
            //_row.SetModified();
        }
        public static void Sort<T> (ref DataTable dataTable)
        {
            DataView dv = dataTable.DefaultView;
            var properties = typeof(T).GetProperties();
            if (properties.Any(x => x.Name == "Namespace"))
            {
                dv.Sort = "Namespace, Name ASC";
            }
            else
            {
                dv.Sort = "Name ASC";
            }
            DataTable sortedDT = dv.ToTable();
            dataTable = sortedDT;
        }

    }
}
