using k8config.GUIDeployments.RealtimeMode.DataModels;
using k8config.GUIEvents.RealtimeMode.DataModels;
using k8s;
using k8s.Models;
using Microsoft.Rest;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        static public CancellationTokenSource namespacesCancelationTokenSource = new CancellationTokenSource();
        static public CancellationTokenSource podsCancelationTokenSource = new CancellationTokenSource();
        static public CancellationTokenSource deploymentsCancelationTokenSource = new CancellationTokenSource();
        static public CancellationTokenSource repsetsCancelationTokenSource = new CancellationTokenSource();
        static public CancellationTokenSource servicesCancelationTokenSource = new CancellationTokenSource();

        static private object eventLockObject = new object();
        static private object podLockObject = new object();
        static private object deploymentLockObject = new object();

        static void CancelAllWatchers()
        {
            namespacesCancelationTokenSource.Cancel();
            podsCancelationTokenSource.Cancel();
            deploymentsCancelationTokenSource.Cancel();
            servicesCancelationTokenSource.Cancel();
            repsetsCancelationTokenSource.Cancel();
        }
        static void StartWatchersTasks()
        {
            CancelAllWatchers();
            namespacesCancelationTokenSource = new CancellationTokenSource();
            podsCancelationTokenSource = new CancellationTokenSource();
            deploymentsCancelationTokenSource = new CancellationTokenSource();
            repsetsCancelationTokenSource = new CancellationTokenSource();
            servicesCancelationTokenSource = new CancellationTokenSource();

            //clear current eventtable
            Application.MainLoop.Invoke(() =>
            {
                lock (eventLockObject)
                {
                    namespacesList = new NamespaceListType();
                    podsList = new PodListType();
                    servicesList = new ServiceListType();
                    deploymentsList = new DeploymentListType();
                    replicasetsList = new ReplicaSetListType();
                    eventsList = new EventListType();

                    podsTableView.Table = podsList.DataTable;
                    deploymentsTableView.Table = deploymentsList.DataTable;
                    eventsTableView.Table = eventsList.DataTableConstruct;


                    deploymentsTableView.Style.GetOrCreateColumnStyle(deploymentsList.DataTable.Columns["Namespace"]).RepresentationGetter = (i) => deploymentsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.Namespace.ToString();
                    deploymentsTableView.Style.GetOrCreateColumnStyle(deploymentsList.DataTable.Columns["Name"]).RepresentationGetter = (i) => deploymentsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.Name.ToString();
                    deploymentsTableView.Style.GetOrCreateColumnStyle(deploymentsList.DataTable.Columns["Ready"]).RepresentationGetter = (i) => deploymentsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.Ready.ToString();
                    deploymentsTableView.Style.GetOrCreateColumnStyle(deploymentsList.DataTable.Columns["Up-To-Date"]).RepresentationGetter = (i) => deploymentsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.UpToDate.ToString();
                    deploymentsTableView.Style.GetOrCreateColumnStyle(deploymentsList.DataTable.Columns["Available"]).RepresentationGetter = (i) => deploymentsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.Available.ToString();
                    deploymentsTableView.Style.GetOrCreateColumnStyle(deploymentsList.DataTable.Columns["Age"]).RepresentationGetter = (i) => deploymentsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.Age.ToString();

                    podsTableView.Style.GetOrCreateColumnStyle(podsList.DataTable.Columns["Namespace"]).RepresentationGetter = (i) => podsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Namespace.ToString();
                    podsTableView.Style.GetOrCreateColumnStyle(podsList.DataTable.Columns["Name"]).RepresentationGetter = (i) => podsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Name.ToString();
                    podsTableView.Style.GetOrCreateColumnStyle(podsList.DataTable.Columns["Ready"]).RepresentationGetter = (i) => podsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Ready.ToString();
                    podsTableView.Style.GetOrCreateColumnStyle(podsList.DataTable.Columns["Status"]).RepresentationGetter = (i) => podsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Status.ToString();
                    podsTableView.Style.GetOrCreateColumnStyle(podsList.DataTable.Columns["Restarts"]).RepresentationGetter = (i) => podsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Restarts.ToString();
                    podsTableView.Style.GetOrCreateColumnStyle(podsList.DataTable.Columns["Age"]).RepresentationGetter = (i) => podsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Age.ToString();
                }
            });
            try
            {
                if (!string.IsNullOrEmpty(proxyHost))
                {
                    Log.Debug($"Starting with proxy host {proxyHost}");
                    var config = new KubernetesClientConfiguration { Host = proxyHost };
                    k8Client = new Kubernetes(config);
                }
                else if (string.IsNullOrWhiteSpace(selectedContext))
                {
                    selectedContext = KubernetesClientConfiguration.BuildDefaultConfig().CurrentContext;
                    k8Client = new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigObject(KubernetesClientConfiguration.LoadKubeConfig(), selectedContext));
                    Log.Debug($"Starting with default context {selectedContext}");
                }
                else
                {
                    k8Client = new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigObject(KubernetesClientConfiguration.LoadKubeConfig(), selectedContext));
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error building new connection context {selectedContext} context", ex, k8Client);
            }

            try
            {
                Task<HttpOperationResponse<V1NamespaceList>> namespacelistResp = k8Client.ListNamespaceWithHttpMessagesAsync(watch: true, cancellationToken: namespacesCancelationTokenSource.Token);
                Task<HttpOperationResponse<V1PodList>> podlistResp = k8Client.ListPodForAllNamespacesWithHttpMessagesAsync(watch: true, cancellationToken: podsCancelationTokenSource.Token);
                var podWatcher = podlistResp.Watch<V1Pod, V1PodList>((eventType, pod) =>
                {
                    Application.MainLoop.Invoke(() =>
                    {
                        processEventDetails(eventType, pod);
                        switch (eventType)
                        {
                            case WatchEventType.Added:
                                podsList.AddUpdateDelete(new PodType(pod), CRUDOperation.Add);
                                break;
                            case WatchEventType.Modified:
                                podsList.AddUpdateDelete(new PodType(pod), CRUDOperation.Change);
                                break;
                            case WatchEventType.Deleted:
                                podsList.AddUpdateDelete(new PodType(pod), CRUDOperation.Delete);
                                break;
                            case WatchEventType.Error:
                                Log.Error("Error Event: error in watch thread");
                                break;
                            case WatchEventType.Bookmark:
                                Log.Error("Bookmark Event: error in watch thread");
                                break;
                            default:
                                Log.Error("default event: error in watch thread");
                                break;
                        }
                        lock (podLockObject)
                        {
                            podsTableView.SetNeedsDisplay();
                            UpdateTabHeaders();
                        }
                    });

                }, ctx: podsCancelationTokenSource.Token);
                Task<HttpOperationResponse<V1DeploymentList>> deploymentlistResp = k8Client.ListDeploymentForAllNamespacesWithHttpMessagesAsync(watch: true, cancellationToken: deploymentsCancelationTokenSource.Token);
                var deploymentWatcher = deploymentlistResp.Watch<V1Deployment, V1DeploymentList>((eventType, deployment) =>
                {
                    Application.MainLoop.Invoke(() =>
                    {
                        processEventDetails(eventType, deployment);
                        switch (eventType)
                        {
                            case WatchEventType.Added:
                                deploymentsList.AddUpdateDelete(new DeploymentType(deployment), CRUDOperation.Add);
                                break;
                            case WatchEventType.Modified:
                                deploymentsList.AddUpdateDelete(new DeploymentType(deployment), CRUDOperation.Change);
                                break;
                            case WatchEventType.Deleted:
                                deploymentsList.AddUpdateDelete(new DeploymentType(deployment), CRUDOperation.Delete);
                                break;
                            case WatchEventType.Error:
                                Log.Error("Error Event: error in watch thread");
                                break;
                            case WatchEventType.Bookmark:
                                Log.Error("Bookmark Event: error in watch thread");
                                break;
                            default:
                                Log.Error("default event: error in watch thread");
                                break;
                        }
                        lock (deploymentLockObject)
                        {
                            deploymentsTableView.SetNeedsDisplay();
                            UpdateTabHeaders();
                        }
                    });
                }, ctx: deploymentsCancelationTokenSource.Token);
                Task<HttpOperationResponse<V1ServiceList>> servicelistResp = k8Client.ListServiceForAllNamespacesWithHttpMessagesAsync(watch: true, cancellationToken: servicesCancelationTokenSource.Token);
                Task<HttpOperationResponse<V1ReplicaSetList>> replicasetlistResp = k8Client.ListReplicaSetForAllNamespacesWithHttpMessagesAsync(watch: true, cancellationToken: repsetsCancelationTokenSource.Token);

            }
            catch (Exception ex)
            {
                Log.Error($"Error starting watchers to {selectedContext} context", ex, null);
            }

        }

        static void processEventDetails(WatchEventType _eventType, object _object)
        {
            lock (eventLockObject)
            {
                eventsTableView.SetNeedsDisplay();
                eventsList.Add(new EventType(_eventType, _object));
            }
        }
        /// <summary>
        /// The processPodEvents.
        /// </summary>
        /// <summary>
        /// The processReplicaSetEvents.
        /// </summary>
        //internal static void processReplicaSetEvents()
        //{
        //    Action<CancellationToken> send = (token) =>
        //    {
        //        Watcher<V1ReplicaSet> watcher = replicasetlistResp.Watch<V1ReplicaSet, V1ReplicaSetList>(async (eventType, ReplicaSet) =>
        //        {
        //            eventsList.AddChange(new EventType(eventType, ReplicaSet));
        //            switch (eventType)
        //            {
        //                case WatchEventType.Added:
        //                    CRUDReplicaSetDataTable.AddOrUpdate(replicasetsTable, ReplicaSet);
        //                    break;
        //                case WatchEventType.Modified:
        //                    CRUDReplicaSetDataTable.AddOrUpdate(replicasetsTable, ReplicaSet);
        //                    break;
        //                case WatchEventType.Deleted:
        //                    CRUDReplicaSetDataTable.Delete(replicasetsTable, ReplicaSet);
        //                    break;
        //                case WatchEventType.Error:
        //                    break;
        //                case WatchEventType.Bookmark:
        //                    break;
        //            }
        //            replicasetsTableView.Table = await DataTableExtention.SortAsync<ReplicaSetType>(replicasetsTable);
        //            replicasetsTableView.Update();
        //            UpdateTabHeaders();
        //        });
        //    };
        //    send.BeginInvoke(watchersCancelationTokenSource.Token, null, null);
        //}

        /// <summary>
        /// The processServiceEvents.
        /// </summary>
        //internal static void processServiceEvents()
        //{
        //    watchersTasks.Add(Task.Run(() =>
        //    {
        //        Watcher<V1Service> watcher = servicelistResp.Watch<V1Service, V1ServiceList>(async (eventType, service) =>
        //        {
        //            CRUDEventDataTable.Add(eventsTable, eventType, service);
        //            eventsTableView.Table = await DataTableExtention.SortAsync<EventType>(eventsTable);
        //            eventsTableView.Update();
        //            switch (eventType)
        //            {
        //                case WatchEventType.Added:
        //                    CRUDServiceDataTable.AddOrUpdate(servicesTable, service);
        //                    break;
        //                case WatchEventType.Modified:
        //                    CRUDServiceDataTable.AddOrUpdate(servicesTable, service);
        //                    break;
        //                case WatchEventType.Deleted:
        //                    CRUDServiceDataTable.Delete(servicesTable, service);
        //                    break;
        //                case WatchEventType.Error:
        //                    break;
        //                case WatchEventType.Bookmark:
        //                    break;
        //            }
        //            servicesTableView.Table = await DataTableExtention.SortAsync<ServiceType>(servicesTable);
        //            servicesTableView.Update();
        //            UpdateTabHeaders();
        //        });
        //    }, watchersCancelationTokenSource.Token));
        //}




        /// <summary>
        /// The processNamespaceEvents.
        /// </summary>
        //internal static void processNamespaceEvents()
        //{
        //    watchersTasks.Add(Task.Run(() =>
        //    {
        //        Watcher<V1Namespace> watcher = namespacelistResp.Watch<V1Namespace, V1NamespaceList>(async (eventType, _namespace) =>
        //        {
        //            CRUDEventDataTable.Add(eventsTable, eventType, _namespace);
        //            eventsTableView.Table = await DataTableExtention.SortAsync<EventType>(eventsTable);
        //            eventsTableView.Update();
        //            switch (eventType)
        //            {
        //                case WatchEventType.Added:
        //                    CRUDNamespaceDataTable.AddOrUpdate(namespacesTable, _namespace);
        //                    break;
        //                case WatchEventType.Modified:
        //                    CRUDNamespaceDataTable.AddOrUpdate(namespacesTable, _namespace);
        //                    break;
        //                case WatchEventType.Deleted:
        //                    CRUDNamespaceDataTable.Delete(namespacesTable, _namespace);
        //                    break;
        //                case WatchEventType.Error:
        //                    break;
        //                case WatchEventType.Bookmark:
        //                    break;
        //            }
        //            namespaceTableView.Table = await DataTableExtention.SortAsync<NamespaceType>(namespacesTable);
        //            namespaceTableView.Update();
        //            UpdateTabHeaders();
        //        });
        //    }, watchersCancelationTokenSource.Token));
        //}
    }
}
