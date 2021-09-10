using k8config.GUIEvents.RealtimeMode.DataTables;
using k8s;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace k8config
{
    partial class Program
    {
        static public CancellationTokenSource watcherTokenSource = new CancellationTokenSource();
        static public CancellationToken watcherTokenSourceToken = watcherTokenSource.Token;
        static public void loadContextData()
        {
            Task.Run(async () =>
            {
                namespacesTable = PupulateDataTables.Namespaces(await k8Client.ListNamespaceAsync(), namespacesTable);
                podsTable = PupulateDataTables.Pods(await k8Client.ListPodForAllNamespacesAsync(), podsTable);
                servicesTable = PupulateDataTables.Services(await k8Client.ListServiceForAllNamespacesAsync(), servicesTable);
                deploymentsTable = PupulateDataTables.Deployments(await k8Client.ListDeploymentForAllNamespacesAsync(), deploymentsTable);
                replicasetsTable = PupulateDataTables.ReplicaSets(await k8Client.ListReplicaSetForAllNamespacesAsync(), replicasetsTable);
            }).Wait();
            UpdateTabHeaders();
        }
    }
}

