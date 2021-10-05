using k8config.DataModels;
using k8config.GUIDeployments.RealtimeMode.DataModels;
using k8config.GUIEvents.RealtimeMode.DataModels;
using k8s;
using k8s.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        static private object LockObject = new object();

        static private Watcher<V1Service> serviceWatcher;
        static private Watcher<V1ReplicaSet> replicationsetWatcher;
        static private Watcher<V1Namespace> namespaceWatcher;
        static private Watcher<V1Pod> podWatcher;
        static private Watcher<V1Deployment> deploymentWatcher;

        static void DisposeAllWatchers()
        {
            namespaceWatcher?.Dispose();
            podWatcher?.Dispose();
            deploymentWatcher?.Dispose();
            serviceWatcher?.Dispose();
            replicationsetWatcher?.Dispose();
        }
        static void StartWatchersTasks()
        {
            DisposeAllWatchers();
            Application.MainLoop.Invoke(() =>
            {
                lock (LockObject)
                {
                    RealtimeModeControls.namespacesList = new NamespaceListType();
                    RealtimeModeControls.podsList = new PodListType();
                    RealtimeModeControls.servicesList = new ServiceListType();
                    RealtimeModeControls.deploymentsList = new DeploymentListType();
                    RealtimeModeControls.replicasetsList = new ReplicaSetListType();
                    RealtimeModeControls.eventsList = new EventListType();

                    RealtimeModeControls.podsTableView.Table = RealtimeModeControls.podsList.DataTable;
                    RealtimeModeControls.deploymentsTableView.Table = RealtimeModeControls.deploymentsList.DataTable;
                    RealtimeModeControls.eventsTableView.Table = RealtimeModeControls.eventsList.DataTableConstruct;
                    RealtimeModeControls.namespaceTableView.Table = RealtimeModeControls.namespacesList.DataTable;
                    RealtimeModeControls.replicasetsTableView.Table = RealtimeModeControls.replicasetsList.DataTable;
                    RealtimeModeControls.servicesTableView.Table = RealtimeModeControls.servicesList.DataTable;

                    RealtimeModeControls.deploymentsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.deploymentsList.DataTable.Columns["Namespace"]).RepresentationGetter = (i) => RealtimeModeControls.deploymentsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.Namespace.ToString();
                    RealtimeModeControls.deploymentsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.deploymentsList.DataTable.Columns["Name"]).RepresentationGetter = (i) => RealtimeModeControls.deploymentsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.Name.ToString();
                    RealtimeModeControls.deploymentsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.deploymentsList.DataTable.Columns["Ready"]).RepresentationGetter = (i) => RealtimeModeControls.deploymentsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.Ready.ToString();
                    RealtimeModeControls.deploymentsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.deploymentsList.DataTable.Columns["Up-To-Date"]).RepresentationGetter = (i) => RealtimeModeControls.deploymentsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.UpToDate.ToString();
                    RealtimeModeControls.deploymentsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.deploymentsList.DataTable.Columns["Available"]).RepresentationGetter = (i) => RealtimeModeControls.deploymentsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.Available.ToString();
                    RealtimeModeControls.deploymentsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.deploymentsList.DataTable.Columns["Age"]).RepresentationGetter = (i) => RealtimeModeControls.deploymentsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.Age.ToString();

                    RealtimeModeControls.podsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.podsList.DataTable.Columns["Namespace"]).RepresentationGetter = (i) => RealtimeModeControls.podsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Namespace.ToString();
                    RealtimeModeControls.podsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.podsList.DataTable.Columns["Name"]).RepresentationGetter = (i) => RealtimeModeControls.podsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Name.ToString();
                    RealtimeModeControls.podsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.podsList.DataTable.Columns["Ready"]).RepresentationGetter = (i) => RealtimeModeControls.podsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Ready.ToString();
                    RealtimeModeControls.podsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.podsList.DataTable.Columns["Status"]).RepresentationGetter = (i) => RealtimeModeControls.podsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Status.ToString();
                    RealtimeModeControls.podsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.podsList.DataTable.Columns["Restarts"]).RepresentationGetter = (i) => RealtimeModeControls.podsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Restarts.ToString();
                    RealtimeModeControls.podsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.podsList.DataTable.Columns["Age"]).RepresentationGetter = (i) => RealtimeModeControls.podsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Age.ToString();

                    RealtimeModeControls.namespaceTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.namespacesList.DataTable.Columns["Name"]).RepresentationGetter = (i) => RealtimeModeControls.namespacesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Name.ToString();
                    RealtimeModeControls.namespaceTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.namespacesList.DataTable.Columns["Status"]).RepresentationGetter = (i) => RealtimeModeControls.namespacesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Status.ToString();
                    RealtimeModeControls.namespaceTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.namespacesList.DataTable.Columns["Age"]).RepresentationGetter = (i) => RealtimeModeControls.namespacesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Age.ToString();

                    RealtimeModeControls.servicesTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.servicesList.DataTable.Columns["Namespace"]).RepresentationGetter = (i) => RealtimeModeControls.servicesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Namespace.ToString();
                    RealtimeModeControls.servicesTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.servicesList.DataTable.Columns["Name"]).RepresentationGetter = (i) => RealtimeModeControls.servicesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Name.ToString();
                    RealtimeModeControls.servicesTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.servicesList.DataTable.Columns["ClusterIP"]).RepresentationGetter = (i) => RealtimeModeControls.servicesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.ClusterIP.ToString();
                    RealtimeModeControls.servicesTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.servicesList.DataTable.Columns["ExternalIP"]).RepresentationGetter = (i) => RealtimeModeControls.servicesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.ExternalIP.ToString();
                    RealtimeModeControls.servicesTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.servicesList.DataTable.Columns["Ports"]).RepresentationGetter = (i) => RealtimeModeControls.servicesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.Ports.ToString();
                    RealtimeModeControls.servicesTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.servicesList.DataTable.Columns["Age"]).RepresentationGetter = (i) => RealtimeModeControls.servicesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Age.ToString();

                    RealtimeModeControls.replicasetsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.replicasetsList.DataTable.Columns["Namespace"]).RepresentationGetter = (i) => RealtimeModeControls.replicasetsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Namespace.ToString();
                    RealtimeModeControls.replicasetsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.replicasetsList.DataTable.Columns["Name"]).RepresentationGetter = (i) => RealtimeModeControls.replicasetsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Name.ToString();
                    RealtimeModeControls.replicasetsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.replicasetsList.DataTable.Columns["Current"]).RepresentationGetter = (i) => RealtimeModeControls.replicasetsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Current.ToString();
                    RealtimeModeControls.replicasetsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.replicasetsList.DataTable.Columns["Desired"]).RepresentationGetter = (i) => RealtimeModeControls.replicasetsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Desired.ToString();
                    RealtimeModeControls.replicasetsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.replicasetsList.DataTable.Columns["Ready"]).RepresentationGetter = (i) => RealtimeModeControls.replicasetsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.Ready.ToString();
                    RealtimeModeControls.replicasetsTableView.Style.GetOrCreateColumnStyle(RealtimeModeControls.replicasetsList.DataTable.Columns["Age"]).RepresentationGetter = (i) => RealtimeModeControls.replicasetsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Age.ToString();
                }
            });
            try
            {
                if (!string.IsNullOrWhiteSpace(GlobalVariables.proxyHost))
                {
                    GlobalVariables.Log.Debug($"Starting with proxy host {GlobalVariables.proxyHost}");
                    var config = new KubernetesClientConfiguration { Host = GlobalVariables.proxyHost };
                    config.TcpKeepAlive = false;
                    config.SkipTlsVerify = true;
                    k8Client = new Kubernetes(config);
                }
                else if (string.IsNullOrWhiteSpace(selectedContext))
                {
                    selectedContext = KubernetesClientConfiguration.BuildDefaultConfig().CurrentContext;
                    GlobalVariables.Log.Debug($"Starting with default context {selectedContext}");                   
                    var config = KubernetesClientConfiguration.BuildConfigFromConfigObject(KubernetesClientConfiguration.LoadKubeConfig(), selectedContext);
                    config.TcpKeepAlive = false;
                    config.SkipTlsVerify = true;
                    k8Client = new Kubernetes(config);
                }
                else
                {
                    GlobalVariables.Log.Debug($"Starting with selected context {selectedContext}");
                    var config = KubernetesClientConfiguration.BuildConfigFromConfigObject(KubernetesClientConfiguration.LoadKubeConfig(), selectedContext);
                    config.TcpKeepAlive = false;
                    config.SkipTlsVerify = true;
                    k8Client = new Kubernetes(config);
                }
            }
            catch (Exception ex)
            {
                GlobalVariables.Log.Error($"Error building new connection context {selectedContext} context", ex, k8Client);
                return;
            }

            try
            {
                GlobalVariables.Log.Info("Starting Service Watcher");
                Task<HttpOperationResponse<V1ServiceList>> servicelistResp = k8Client.ListServiceForAllNamespacesWithHttpMessagesAsync(watch: true);
                serviceWatcher = servicelistResp.Watch<V1Service, V1ServiceList>(onEvent: (eventType, service) =>
                {
                    Application.MainLoop.Invoke(() =>
                    {
                        GlobalVariables.Log.Info($"Processing Service Event: {eventType} : {JsonConvert.SerializeObject(service)}");
                        processEventDetails(eventType, service);
                        switch (eventType)
                        {
                            case WatchEventType.Added:
                                RealtimeModeControls.servicesList.AddUpdateDelete(new ServiceType(service), CRUDOperation.Add);
                                break;
                            case WatchEventType.Modified:
                                RealtimeModeControls.servicesList.AddUpdateDelete(new ServiceType(service), CRUDOperation.Change);
                                break;
                            case WatchEventType.Deleted:
                                RealtimeModeControls.servicesList.AddUpdateDelete(new ServiceType(service), CRUDOperation.Delete);
                                break;
                            case WatchEventType.Error:
                                GlobalVariables.Log.Error("Error Event: error in watch thread");
                                break;
                            case WatchEventType.Bookmark:
                                GlobalVariables.Log.Error("Bookmark Event: error in watch thread");
                                break;
                            default:
                                GlobalVariables.Log.Error("default event: error in watch thread");
                                break;
                        }
                        RealtimeModeControls.servicesTableView.SetNeedsDisplay();
                        UpdateTabHeaders();
                    });

                }, onError: (ex) =>
                {
                    GlobalVariables.Log.Error(ex);
                },
                    onClosed: () =>
                    {
                        GlobalVariables.Log.Error("Namespace Watcher closed connection");
                    });
                GlobalVariables.Log.Info("Starting Namespace Watcher");
                Task<HttpOperationResponse<V1NamespaceList>> namespacelistResp = k8Client.ListNamespaceWithHttpMessagesAsync(watch: true);
                namespaceWatcher = namespacelistResp.Watch<V1Namespace, V1NamespaceList>(
                    onEvent: (eventType, _namespace) =>
                    {
                        Application.MainLoop.Invoke(() =>
                        {
                            GlobalVariables.Log.Info($"Processing Namespace Event: {eventType} : {JsonConvert.SerializeObject(_namespace)}");
                            processEventDetails(eventType, _namespace);
                            switch (eventType)
                            {
                                case WatchEventType.Added:
                                    RealtimeModeControls.namespacesList.AddUpdateDelete(new NamespaceType(_namespace), CRUDOperation.Add);
                                    break;
                                case WatchEventType.Modified:
                                    RealtimeModeControls.namespacesList.AddUpdateDelete(new NamespaceType(_namespace), CRUDOperation.Change);
                                    break;
                                case WatchEventType.Deleted:
                                    RealtimeModeControls.namespacesList.AddUpdateDelete(new NamespaceType(_namespace), CRUDOperation.Delete);
                                    break;
                                case WatchEventType.Error:
                                    GlobalVariables.Log.Error("Error Event: error in watch thread");
                                    break;
                                case WatchEventType.Bookmark:
                                    GlobalVariables.Log.Error("Bookmark Event: error in watch thread");
                                    break;
                                default:
                                    GlobalVariables.Log.Error("default event: error in watch thread");
                                    break;
                            }
                            RealtimeModeControls.namespaceTableView.SetNeedsDisplay();
                            UpdateTabHeaders();
                        });
                    },
                    onError: (ex) =>
                            {
                                GlobalVariables.Log.Error(ex);
                            },
                    onClosed: () =>
                    {
                        GlobalVariables.Log.Error("Namespace Watcher closed connection");
                    });

                GlobalVariables.Log.Info("Starting Pod Watcher");
                Task<HttpOperationResponse<V1PodList>> podlistResp = k8Client.ListPodForAllNamespacesWithHttpMessagesAsync(watch: true);
                podWatcher = podlistResp.Watch<V1Pod, V1PodList>(onEvent: (eventType, pod) =>
                {
                    Application.MainLoop.Invoke(() =>
                    {
                        GlobalVariables.Log.Info($"Processing Pod Event: {eventType} : {JsonConvert.SerializeObject(pod)}");
                        processEventDetails(eventType, pod);
                        switch (eventType)
                        {
                            case WatchEventType.Added:
                                RealtimeModeControls.podsList.AddUpdateDelete(new PodType(pod), CRUDOperation.Add);
                                break;
                            case WatchEventType.Modified:
                                RealtimeModeControls.podsList.AddUpdateDelete(new PodType(pod), CRUDOperation.Change);
                                break;
                            case WatchEventType.Deleted:
                                RealtimeModeControls.podsList.AddUpdateDelete(new PodType(pod), CRUDOperation.Delete);
                                break;
                            case WatchEventType.Error:
                                GlobalVariables.Log.Error("Error Event: error in watch thread");
                                break;
                            case WatchEventType.Bookmark:
                                GlobalVariables.Log.Error("Bookmark Event: error in watch thread");
                                break;
                            default:
                                GlobalVariables.Log.Error("default event: error in watch thread");
                                break;
                        }
                        RealtimeModeControls.podsTableView.SetNeedsDisplay();
                        UpdateTabHeaders();
                    });

                }, onError: (ex) =>
                {
                    GlobalVariables.Log.Error(ex);
                },
                    onClosed: () =>
                    {
                        GlobalVariables.Log.Error("Pod Watcher closed connection");
                    });
                GlobalVariables.Log.Info("Starting Deployment Watcher");
                Task<HttpOperationResponse<V1DeploymentList>> deploymentlistResp = k8Client.ListDeploymentForAllNamespacesWithHttpMessagesAsync(watch: true);
                deploymentWatcher = deploymentlistResp.Watch<V1Deployment, V1DeploymentList>((eventType, deployment) =>
                {
                    Application.MainLoop.Invoke(() =>
                    {
                        GlobalVariables.Log.Info($"Processing Deployment Event: {eventType} : {JsonConvert.SerializeObject(deployment)}");
                        processEventDetails(eventType, deployment);
                        switch (eventType)
                        {
                            case WatchEventType.Added:
                                RealtimeModeControls.deploymentsList.AddUpdateDelete(new DeploymentType(deployment), CRUDOperation.Add);
                                break;
                            case WatchEventType.Modified:
                                RealtimeModeControls.deploymentsList.AddUpdateDelete(new DeploymentType(deployment), CRUDOperation.Change);
                                break;
                            case WatchEventType.Deleted:
                                RealtimeModeControls.deploymentsList.AddUpdateDelete(new DeploymentType(deployment), CRUDOperation.Delete);
                                break;
                            case WatchEventType.Error:
                                GlobalVariables.Log.Error("Error Event: error in watch thread");
                                break;
                            case WatchEventType.Bookmark:
                                GlobalVariables.Log.Error("Bookmark Event: error in watch thread");
                                break;
                            default:
                                GlobalVariables.Log.Error("default event: error in watch thread");
                                break;
                        }
                        RealtimeModeControls.deploymentsTableView.SetNeedsDisplay();
                        UpdateTabHeaders();
                    });
                }, onError: (ex) =>
                {
                    GlobalVariables.Log.Error(ex);
                },
                    onClosed: () =>
                    {
                        GlobalVariables.Log.Error("Deployments Watcher closed connection");
                    });
                GlobalVariables.Log.Info("Starting ReplicationSet Watcher");
                Task<HttpOperationResponse<V1ReplicaSetList>> replicasetlistResp = k8Client.ListReplicaSetForAllNamespacesWithHttpMessagesAsync(watch: true);
                replicationsetWatcher = replicasetlistResp.Watch<V1ReplicaSet, V1ReplicaSetList>(
                    onEvent: (eventType, replicationset) =>
                {
                    Application.MainLoop.Invoke(() =>
                    {
                        GlobalVariables.Log.Info($"Processing ReplicationSet Event: {eventType} : {JsonConvert.SerializeObject(replicationset)}");
                        processEventDetails(eventType, replicationset);
                        switch (eventType)
                        {
                            case WatchEventType.Added:
                                RealtimeModeControls.replicasetsList.AddUpdateDelete(new ReplicaSetType(replicationset), CRUDOperation.Add);
                                break;
                            case WatchEventType.Modified:
                                RealtimeModeControls.replicasetsList.AddUpdateDelete(new ReplicaSetType(replicationset), CRUDOperation.Change);
                                break;
                            case WatchEventType.Deleted:
                                RealtimeModeControls.replicasetsList.AddUpdateDelete(new ReplicaSetType(replicationset), CRUDOperation.Delete);
                                break;
                            case WatchEventType.Error:
                                GlobalVariables.Log.Error("Error Event: error in watch thread");
                                break;
                            case WatchEventType.Bookmark:
                                GlobalVariables.Log.Error("Bookmark Event: error in watch thread");
                                break;
                            default:
                                GlobalVariables.Log.Error("default event: error in watch thread");
                                break;
                        }
                        RealtimeModeControls.replicasetsTableView.SetNeedsDisplay();
                        UpdateTabHeaders();
                    });
                }, onError: (ex) =>
                {
                    GlobalVariables.Log.Error(ex);
                },
                    onClosed: () =>
                    {
                        GlobalVariables.Log.Error("ReplicaSets Watcher closed connection");
                    });
            }
            catch (Exception ex)
            {
                GlobalVariables.Log.Error($"Error starting watchers to {selectedContext} context", ex, null);
            }
        }

        static void processEventDetails(WatchEventType _eventType, object _object)
        {
            RealtimeModeControls.eventsTableView.SetNeedsDisplay();
            RealtimeModeControls.eventsList.Add(new EventType(_eventType, _object));
        }
    }
}
