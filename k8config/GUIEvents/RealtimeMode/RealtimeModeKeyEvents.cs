using k8config.GUIEvents.RealtimeMode.DataModels;
using k8config.GUIEvents.RealtimeMode.DataTables;
using k8s;
using k8s.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        static Kubernetes k8Client;
        static string selectedContext;
        static public Task<HttpOperationResponse<V1NamespaceList>> namespacelistResp;
        static public Task<HttpOperationResponse<V1PodList>> podlistResp;
        static public Task<HttpOperationResponse<V1DeploymentList>> deploymentlistResp;
        static public Task<HttpOperationResponse<V1ServiceList>> servicelistResp;
        static public Task<HttpOperationResponse<V1ReplicaSetList>> replicasetlistResp;
        static bool watchersRunning;
        public static void realtimeModeKeyEvents()
        {

            availableContextsListView.KeyUp += async (e) =>
            {
                if (e.KeyEvent.Key == Key.Enter)
                {
                    busyWorkingWindow.Visible = true;
                    selectedContext = ((List<string>)availableContextsListView.Source.ToList())[availableContextsListView.SelectedItem];
                    busyWorkingWindow.Text = $"Connecting to {selectedContext} context";
                    await Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var config = KubernetesClientConfiguration.BuildConfigFromConfigObject(KubernetesClientConfiguration.LoadKubeConfig(), selectedContext);
                            k8Client = new Kubernetes(config);
                            loadContextData();
                            if (!watchersRunning)
                            {
                                namespacelistResp = k8Client.ListNamespaceWithHttpMessagesAsync(watch: true);
                                podlistResp = k8Client.ListPodForAllNamespacesWithHttpMessagesAsync(watch: true);
                                deploymentlistResp = k8Client.ListDeploymentForAllNamespacesWithHttpMessagesAsync(watch: true);
                                servicelistResp = k8Client.ListServiceForAllNamespacesWithHttpMessagesAsync(watch: true);
                                replicasetlistResp = k8Client.ListReplicaSetForAllNamespacesWithHttpMessagesAsync(watch: true);
                                Application.MainLoop.Invoke(processPodEvents);
                                Application.MainLoop.Invoke(processDeploymentEvents);
                                Application.MainLoop.Invoke(processReplicaSetEvents);
                                Application.MainLoop.Invoke(processNamespaceEvents);
                                Application.MainLoop.Invoke(processServiceEvents);
                                watchersRunning = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            updateMessageBar($"Error connecting to {selectedContext} context {ex.Message}");
                        }
                    });
                    busyWorkingWindow.Visible = false;
                }
            };
        }
        static void processPodEvents()
        {
            Watcher<V1Pod> watcher = podlistResp.Watch<V1Pod, V1PodList>((eventType, pod) =>
            {
                CRUDEventDataTable.Add(eventsTable, eventType, pod);
                eventsListView.Update();
                switch (eventType)
                {
                    case WatchEventType.Added:
                        CRUDPodDataTable.AddOrUpdate(podsTable, pod);
                        break;
                    case WatchEventType.Modified:
                        CRUDPodDataTable.AddOrUpdate(podsTable, pod);
                        break;
                    case WatchEventType.Deleted:
                        CRUDPodDataTable.Delete(podsTable, pod);
                        break;
                    case WatchEventType.Error:
                        break;
                    case WatchEventType.Bookmark:
                        break;
                }
                DataTableExtention.Sort<PodType>(ref podsTable);
                podsListView.Table = podsTable;
                podsListView.Update();
                UpdateTabHeaders();
            });
        }
        static void processReplicaSetEvents()
        {
            Watcher<V1ReplicaSet> watcher = replicasetlistResp.Watch<V1ReplicaSet, V1ReplicaSetList>((eventType, ReplicaSet) =>
            {
                CRUDEventDataTable.Add(eventsTable, eventType, ReplicaSet);
                eventsListView.Update();
                switch (eventType)
                {
                    case WatchEventType.Added:
                        CRUDReplicaSetDataTable.AddOrUpdate(replicasetsTable, ReplicaSet);
                        break;
                    case WatchEventType.Modified:
                        CRUDReplicaSetDataTable.AddOrUpdate(replicasetsTable, ReplicaSet);
                        break;
                    case WatchEventType.Deleted:
                        CRUDReplicaSetDataTable.Delete(replicasetsTable, ReplicaSet);
                        break;
                    case WatchEventType.Error:
                        break;
                    case WatchEventType.Bookmark:
                        break;
                }
                DataTableExtention.Sort<ReplicaSetType>(ref replicasetsTable);
                replicasetsListView.Table = replicasetsTable;
                replicasetsListView.Update();
                UpdateTabHeaders();
            });

        }
        static void processServiceEvents()
        {
            Watcher<V1Service> watcher = servicelistResp.Watch<V1Service, V1ServiceList>((eventType, service) =>
            {
                CRUDEventDataTable.Add(eventsTable, eventType, service);
                eventsListView.Update();
                switch (eventType)
                {
                    case WatchEventType.Added:
                        CRUDServiceDataTable.AddOrUpdate(servicesTable, service);
                        break;
                    case WatchEventType.Modified:
                        CRUDServiceDataTable.AddOrUpdate(servicesTable, service);
                        break;
                    case WatchEventType.Deleted:
                        CRUDServiceDataTable.Delete(servicesTable, service);
                        break;
                    case WatchEventType.Error:
                        break;
                    case WatchEventType.Bookmark:
                        break;
                }
                DataTableExtention.Sort<ServiceType>(ref servicesTable);
                servicesListView.Table = servicesTable;
                servicesListView.Update();
                UpdateTabHeaders();
            });
        }
        static void processDeploymentEvents()
        {
            Watcher<V1Deployment> watcher = deploymentlistResp.Watch<V1Deployment, V1DeploymentList>((eventType, deployment) =>
            {
                CRUDEventDataTable.Add(eventsTable, eventType, deployment);
                eventsListView.Update();
                switch (eventType)
                {
                    case WatchEventType.Added:
                        CRUDDeploymentDataTable.AddOrUpdate(deploymentsTable, deployment);
                        break;
                    case WatchEventType.Modified:
                        CRUDDeploymentDataTable.AddOrUpdate(deploymentsTable, deployment);
                        break;
                    case WatchEventType.Deleted:
                        CRUDDeploymentDataTable.Delete(deploymentsTable, deployment);
                        break;
                    case WatchEventType.Error:
                        break;
                    case WatchEventType.Bookmark:
                        break;
                }
                DataTableExtention.Sort<DeploymentType>(ref deploymentsTable);
                deploymentsListView.Table = deploymentsTable;
                deploymentsListView.Update();
                UpdateTabHeaders();
            });

        }
        static void processNamespaceEvents()
        {
            Watcher<V1Namespace> watcher = namespacelistResp.Watch<V1Namespace, V1NamespaceList>((eventType, _namespace) =>
            {
                CRUDEventDataTable.Add(eventsTable, eventType, _namespace);
                eventsListView.Update();
                switch (eventType)
                {
                    case WatchEventType.Added:
                        CRUDNamespaceDataTable.AddOrUpdate(namespacesTable, _namespace);
                        break;
                    case WatchEventType.Modified:
                        CRUDNamespaceDataTable.AddOrUpdate(namespacesTable, _namespace);
                        break;
                    case WatchEventType.Deleted:
                        CRUDNamespaceDataTable.Delete(namespacesTable, _namespace);
                        break;
                    case WatchEventType.Error:
                        break;
                    case WatchEventType.Bookmark:
                        break;
                }
                DataTableExtention.Sort<NamespaceType>(ref namespacesTable);
                namespaceListView.Table = namespacesTable;
                namespaceListView.Update();
                UpdateTabHeaders();
            });

        }

    }
}
