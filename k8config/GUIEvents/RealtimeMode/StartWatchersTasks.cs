using k8config.GUIEvents.RealtimeMode.DataModels;
using k8config.GUIEvents.RealtimeMode.DataTables;
using k8s;
using k8s.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        /// <summary>
        /// Defines the namespacelistResp.
        /// </summary>
        static public Task<HttpOperationResponse<V1NamespaceList>> namespacelistResp;

        /// <summary>
        /// Defines the podlistResp.
        /// </summary>
        static public Task<HttpOperationResponse<V1PodList>> podlistResp;

        /// <summary>
        /// Defines the deploymentlistResp.
        /// </summary>
        static public Task<HttpOperationResponse<V1DeploymentList>> deploymentlistResp;

        /// <summary>
        /// Defines the servicelistResp.
        /// </summary>
        static public Task<HttpOperationResponse<V1ServiceList>> servicelistResp;

        /// <summary>
        /// Defines the replicasetlistResp.
        /// </summary>
        static public Task<HttpOperationResponse<V1ReplicaSetList>> replicasetlistResp;

        /// <summary>
        /// Defines the watchersRunning.
        /// </summary>
        internal static bool watchersRunning;

        /// <summary>
        /// Defines the watchersCancelationTokenSource.
        /// </summary>
        static public CancellationTokenSource watchersCancelationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Defines the watchersTask.
        /// </summary>
        static public List<Task> watchersTasks = new List<Task>();
        static void StartWatchersTasks()
        {
            busyWorkingWindow.Visible = true;
            watchersCancelationTokenSource.Cancel();
            watchersTasks.ForEach(task =>
            {
                while (task.Status == TaskStatus.Running)
                {
                    task.Wait(); 
                    Thread.Sleep(10); 
                }
            });
            watchersTasks = new List<Task>();
            watchersCancelationTokenSource = new CancellationTokenSource();
     
            if (string.IsNullOrWhiteSpace(selectedContext))
            {
                selectedContext = KubernetesClientConfiguration.BuildDefaultConfig().CurrentContext;
            }
            busyWorkingWindow.Text = $"Connecting to {selectedContext} context";

            watchersTasks.Add(Task.Run(() =>
            {
                try
                {
                    k8Client = new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigObject(KubernetesClientConfiguration.LoadKubeConfig(), selectedContext));
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
                    UpdateMessageBar($"Error connecting to {selectedContext} context {ex.Message}");
                }
            }, watchersCancelationTokenSource.Token));
            busyWorkingWindow.Visible = false;
        }
        /// <summary>
        /// The processPodEvents.
        /// </summary>
        internal static void processPodEvents()
        {
            watchersTasks.Add(Task.Run(() =>
            {
                Watcher<V1Pod> watcher = podlistResp.Watch<V1Pod, V1PodList>((eventType, pod) =>
                {
                    CRUDEventDataTable.Add(eventsTable, eventType, pod);
                    Monitor.Enter(eventsTableView);
                    eventsTableView.Table = DataTableExtention.Sort<EventType>(eventsTable);
                    eventsTableView.Update();
                    Monitor.Exit(eventsTableView);
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
                    podsTableView.Table = DataTableExtention.Sort<PodType>(podsTable);
                    podsTableView.Update();
                    UpdateTabHeaders();
                });
            }, watchersCancelationTokenSource.Token));
        }

        /// <summary>
        /// The processReplicaSetEvents.
        /// </summary>
        internal static void processReplicaSetEvents()
        {
            watchersTasks.Add(Task.Run(() =>
            {
                Watcher<V1ReplicaSet> watcher = replicasetlistResp.Watch<V1ReplicaSet, V1ReplicaSetList>((eventType, ReplicaSet) =>
                {
                    CRUDEventDataTable.Add(eventsTable, eventType, ReplicaSet);
                    Monitor.Enter(eventsTableView);
                    eventsTableView.Table = DataTableExtention.Sort<EventType>(eventsTable);
                    eventsTableView.Update();
                    Monitor.Exit(eventsTableView);
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
                    replicasetsTableView.Table = DataTableExtention.Sort<ReplicaSetType>(replicasetsTable);
                    replicasetsTableView.Update();
                    UpdateTabHeaders();
                });
            }, watchersCancelationTokenSource.Token));
        }

        /// <summary>
        /// The processServiceEvents.
        /// </summary>
        internal static void processServiceEvents()
        {
            watchersTasks.Add(Task.Run(() =>
            {
                Watcher<V1Service> watcher = servicelistResp.Watch<V1Service, V1ServiceList>((eventType, service) =>
                {
                    CRUDEventDataTable.Add(eventsTable, eventType, service);
                    Monitor.Enter(eventsTableView);
                    eventsTableView.Table = DataTableExtention.Sort<EventType>(eventsTable);
                    eventsTableView.Update();
                    Monitor.Exit(eventsTableView);
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
                    servicesTableView.Table = DataTableExtention.Sort<ServiceType>(servicesTable);
                    servicesTableView.Update();
                    UpdateTabHeaders();
                });
            }, watchersCancelationTokenSource.Token));
        }

        /// <summary>
        /// The processDeploymentEvents.
        /// </summary>
        internal static void processDeploymentEvents()
        {
            watchersTasks.Add(Task.Run(() =>
            {
                Watcher<V1Deployment> watcher = deploymentlistResp.Watch<V1Deployment, V1DeploymentList>((eventType, deployment) =>
                {
                    CRUDEventDataTable.Add(eventsTable, eventType, deployment);
                    Monitor.Enter(eventsTableView);
                    eventsTableView.Table = DataTableExtention.Sort<EventType>(eventsTable);
                    eventsTableView.Update();
                    Monitor.Exit(eventsTableView);
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
                    deploymentsTableView.Table = DataTableExtention.Sort<DeploymentType>(deploymentsTable); ;
                    deploymentsTableView.Update();
                    UpdateTabHeaders();
                });
            }, watchersCancelationTokenSource.Token));
        }

        /// <summary>
        /// The processNamespaceEvents.
        /// </summary>
        internal static void processNamespaceEvents()
        {
            watchersTasks.Add(Task.Run(() =>
            {
                Watcher<V1Namespace> watcher = namespacelistResp.Watch<V1Namespace, V1NamespaceList>((eventType, _namespace) =>
                {
                    CRUDEventDataTable.Add(eventsTable, eventType, _namespace);
                    Monitor.Enter(eventsTableView);
                    eventsTableView.Table = DataTableExtention.Sort<EventType>(eventsTable);
                    eventsTableView.Update();
                    Monitor.Exit(eventsTableView);
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
                    namespaceTableView.Table = DataTableExtention.Sort<NamespaceType>(namespacesTable); ;
                    namespaceTableView.Update();
                    UpdateTabHeaders();
                });
            }, watchersCancelationTokenSource.Token));
        }
    }
}
