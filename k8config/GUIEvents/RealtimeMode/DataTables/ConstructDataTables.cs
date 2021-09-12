using k8config.GUIEvents.RealtimeMode.DataModels;
using System.Data;

namespace k8config.GUIEvents.RealtimeMode.DataTables
{
    public static class ConstructDataTables
    {
        public static DataTable Events()
        {
            DataTable dt = DataTableExtention.CreateDataTable<EventType>();
            dt.DefaultView.Sort = "TimeStamp desc";
            return dt.DefaultView.ToTable();
        }
        public static DataTable Namespaces()
        {
            DataTable dt = DataTableExtention.CreateDataTable<NamespaceType>();
            dt.DefaultView.Sort = "Name ASC";
            return dt.DefaultView.ToTable();
        }
        public static DataTable Pods()
        {
            DataTable dt = DataTableExtention.CreateDataTable<PodType>();
            dt.DefaultView.Sort = "Namespace ASC, Name ASC";
            return dt.DefaultView.ToTable();
        }
        public static DataTable Services()
        {
            DataTable dt = DataTableExtention.CreateDataTable<ServiceType>();
            dt.DefaultView.Sort = "Namespace ASC, Name ASC";
            return dt.DefaultView.ToTable();
        }
        public static DataTable ReplicaSets()
        {
            DataTable dt = DataTableExtention.CreateDataTable<ReplicaSetType>();
            dt.DefaultView.Sort = "Namespace ASC, Name ASC";
            return dt.DefaultView.ToTable();
        }
        public static DataTable Deployments()
        {
            DataTable dt = DataTableExtention.CreateDataTable<DeploymentType>();
            dt.DefaultView.Sort = "Namespace ASC, Name ASC";
            return dt.DefaultView.ToTable();
        }
    }
}
