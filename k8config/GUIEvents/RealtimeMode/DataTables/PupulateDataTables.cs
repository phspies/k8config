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
    public static class PupulateDataTables
    {
        public static DataTable Namespaces(V1NamespaceList namespaceList, DataTable dataTable)
        {
            return DataTableExtention.PupulateDataTable(namespaceList.Items.Select(ns => new NamespaceType(ns)), dataTable);
        }
        public static DataTable Pods(V1PodList podList, DataTable dataTable)
        {
            return DataTableExtention.PupulateDataTable(podList.Items.Select(pod => new PodType(pod)), dataTable);
        }
        public static DataTable Services(V1ServiceList serviceList, DataTable dataTable)
        {
            return DataTableExtention.PupulateDataTable(serviceList.Items.Select(service => new ServiceType(service)), dataTable);
        }
        public static DataTable ReplicaSets(V1ReplicaSetList replicasetList, DataTable dataTable)
        {
            return DataTableExtention.PupulateDataTable(replicasetList.Items.Select(replicaset => new ReplicaSetType(replicaset)), dataTable);
        }
        public static DataTable Deployments(V1DeploymentList deploymentList, DataTable dataTable)
        {
            return DataTableExtention.PupulateDataTable(deploymentList.Items.Select(deployment => new DeploymentType(deployment)), dataTable);
        }
    }
}
