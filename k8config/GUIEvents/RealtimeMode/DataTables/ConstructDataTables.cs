using k8config.GUIEvents.RealtimeMode.DataModels;
using k8config.Utilities;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace k8config.GUIEvents.RealtimeMode.DataTables
{
    public static class ConstructDataTables
    {
        public static DataTable Events()
        {
            return DataTableExtention.CreateDataTable<EventType>();
        }
        public static DataTable Namespaces()
        {
            return DataTableExtention.CreateDataTable<NamespaceType>();
        }
        public static DataTable Pods()
        {
            return DataTableExtention.CreateDataTable<PodType>();
        }
        public static DataTable Services()
        {
            return DataTableExtention.CreateDataTable<ServiceType>();
        }
        public static DataTable ReplicaSets()
        {
            return DataTableExtention.CreateDataTable<ReplicaSetType>();
        }
        public static DataTable Deployments()
        {
            return DataTableExtention.CreateDataTable<DeploymentType>();
        }
    }
}
