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
                    namespacesList = new NamespaceListType();
                    podsList = new PodListType();
                    servicesList = new ServiceListType();
                    deploymentsList = new DeploymentListType();
                    replicasetsList = new ReplicaSetListType();
                    eventsList = new EventListType();

                    podsTableView.Table = podsList.DataTable;
                    deploymentsTableView.Table = deploymentsList.DataTable;
                    eventsTableView.Table = eventsList.DataTableConstruct;
                    namespaceTableView.Table = namespacesList.DataTable;
                    replicasetsTableView.Table = replicasetsList.DataTable;
                    servicesTableView.Table = servicesList.DataTable;

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

                    namespaceTableView.Style.GetOrCreateColumnStyle(namespacesList.DataTable.Columns["Name"]).RepresentationGetter = (i) => namespacesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Name.ToString();
                    namespaceTableView.Style.GetOrCreateColumnStyle(namespacesList.DataTable.Columns["Status"]).RepresentationGetter = (i) => namespacesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Status.ToString();
                    namespaceTableView.Style.GetOrCreateColumnStyle(namespacesList.DataTable.Columns["Age"]).RepresentationGetter = (i) => namespacesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Age.ToString();

                    servicesTableView.Style.GetOrCreateColumnStyle(servicesList.DataTable.Columns["Namespace"]).RepresentationGetter = (i) => servicesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Namespace.ToString();
                    servicesTableView.Style.GetOrCreateColumnStyle(servicesList.DataTable.Columns["Name"]).RepresentationGetter = (i) => servicesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Name.ToString();
                    servicesTableView.Style.GetOrCreateColumnStyle(servicesList.DataTable.Columns["ClusterIP"]).RepresentationGetter = (i) => servicesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.ClusterIP.ToString();
                    servicesTableView.Style.GetOrCreateColumnStyle(servicesList.DataTable.Columns["ExternalIP"]).RepresentationGetter = (i) => servicesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.ExternalIP.ToString();
                    servicesTableView.Style.GetOrCreateColumnStyle(servicesList.DataTable.Columns["Ports"]).RepresentationGetter = (i) => servicesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.Ports.ToString();
                    servicesTableView.Style.GetOrCreateColumnStyle(servicesList.DataTable.Columns["Age"]).RepresentationGetter = (i) => servicesList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Age.ToString();

                    replicasetsTableView.Style.GetOrCreateColumnStyle(replicasetsList.DataTable.Columns["Namespace"]).RepresentationGetter = (i) => replicasetsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Namespace.ToString();
                    replicasetsTableView.Style.GetOrCreateColumnStyle(replicasetsList.DataTable.Columns["Name"]).RepresentationGetter = (i) => replicasetsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Name.ToString();
                    replicasetsTableView.Style.GetOrCreateColumnStyle(replicasetsList.DataTable.Columns["Current"]).RepresentationGetter = (i) => replicasetsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Current.ToString();
                    replicasetsTableView.Style.GetOrCreateColumnStyle(replicasetsList.DataTable.Columns["Desired"]).RepresentationGetter = (i) => replicasetsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Desired.ToString();
                    replicasetsTableView.Style.GetOrCreateColumnStyle(replicasetsList.DataTable.Columns["Ready"]).RepresentationGetter = (i) => replicasetsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value.Ready.ToString();
                    replicasetsTableView.Style.GetOrCreateColumnStyle(replicasetsList.DataTable.Columns["Age"]).RepresentationGetter = (i) => replicasetsList.Dictionary.FirstOrDefault(x => x.Key == (long)i).Value?.Age.ToString();
                }
            });
            try
            {
                if (!string.IsNullOrWhiteSpace(proxyHost))
                {
                    Log.Debug($"Starting with proxy host {proxyHost}");
                    var config = new KubernetesClientConfiguration { Host = proxyHost };
                    config.TcpKeepAlive = false;
                    config.SkipTlsVerify = true;
                    k8Client = new Kubernetes(config);
                }
                else if (string.IsNullOrWhiteSpace(selectedContext))
                {
                    selectedContext = KubernetesClientConfiguration.BuildDefaultConfig().CurrentContext;
                    Log.Debug($"Starting with default context {selectedContext}");                   
                    var config = KubernetesClientConfiguration.BuildConfigFromConfigObject(KubernetesClientConfiguration.LoadKubeConfig(), selectedContext);
                    config.TcpKeepAlive = false;
                    config.SkipTlsVerify = true;
                    k8Client = new Kubernetes(config);
                }
                else
                {
                    Log.Debug($"Starting with selected context {selectedContext}");
                    var config = KubernetesClientConfiguration.BuildConfigFromConfigObject(KubernetesClientConfiguration.LoadKubeConfig(), selectedContext);
                    config.TcpKeepAlive = false;
                    config.SkipTlsVerify = true;
                    k8Client = new Kubernetes(config);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error building new connection context {selectedContext} context", ex, k8Client);
                return;
            }

            try
            {

                Log.Info("Starting Service Watcher");
                Task<HttpOperationResponse<V1ServiceList>> servicelistResp = k8Client.ListServiceForAllNamespacesWithHttpMessagesAsync(watch: true);
                serviceWatcher = servicelistResp.Watch<V1Service, V1ServiceList>(onEvent: (eventType, service) =>
                {
                    Application.MainLoop.Invoke(() =>
                    {
                        Log.Info($"Processing Service Event: {eventType} : {JsonConvert.SerializeObject(service)}");
                        processEventDetails(eventType, service);
                        switch (eventType)
                        {
                            case WatchEventType.Added:
                                servicesList.AddUpdateDelete(new ServiceType(service), CRUDOperation.Add);
                                break;
                            case WatchEventType.Modified:
                                servicesList.AddUpdateDelete(new ServiceType(service), CRUDOperation.Change);
                                break;
                            case WatchEventType.Deleted:
                                servicesList.AddUpdateDelete(new ServiceType(service), CRUDOperation.Delete);
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
                        servicesTableView.SetNeedsDisplay();
                        UpdateTabHeaders();
                    });

                }, onError: (ex) =>
                {
                    Log.Error(ex);
                },
                    onClosed: () =>
                    {
                        Log.Error("Namespace Watcher closed connection");
                    });
                Log.Info("Starting Namespace Watcher");
                Task<HttpOperationResponse<V1NamespaceList>> namespacelistResp = k8Client.ListNamespaceWithHttpMessagesAsync(watch: true);
                namespaceWatcher = namespacelistResp.Watch<V1Namespace, V1NamespaceList>(
                    onEvent: (eventType, _namespace) =>
                    {
                        Application.MainLoop.Invoke(() =>
                        {
                            Log.Info($"Processing Namespace Event: {eventType} : {JsonConvert.SerializeObject(_namespace)}");
                            processEventDetails(eventType, _namespace);
                            switch (eventType)
                            {
                                case WatchEventType.Added:
                                    namespacesList.AddUpdateDelete(new NamespaceType(_namespace), CRUDOperation.Add);
                                    break;
                                case WatchEventType.Modified:
                                    namespacesList.AddUpdateDelete(new NamespaceType(_namespace), CRUDOperation.Change);
                                    break;
                                case WatchEventType.Deleted:
                                    namespacesList.AddUpdateDelete(new NamespaceType(_namespace), CRUDOperation.Delete);
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
                            namespaceTableView.SetNeedsDisplay();
                            UpdateTabHeaders();
                        });

                    },
                    onError: (ex) =>
                            {
                                Log.Error(ex);
                            },
                    onClosed: () =>
                    {
                        Log.Error("Namespace Watcher closed connection");
                    });

                Log.Info("Starting Pod Watcher");
                Task<HttpOperationResponse<V1PodList>> podlistResp = k8Client.ListPodForAllNamespacesWithHttpMessagesAsync(watch: true);
                podWatcher = podlistResp.Watch<V1Pod, V1PodList>(onEvent: (eventType, pod) =>
                {
                    Application.MainLoop.Invoke(() =>
                    {
                        Log.Info($"Processing Pod Event: {eventType} : {JsonConvert.SerializeObject(pod)}");
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
                        podsTableView.SetNeedsDisplay();
                        UpdateTabHeaders();
                    });

                }, onError: (ex) =>
                {
                    Log.Error(ex);
                },
                    onClosed: () =>
                    {
                        Log.Error("Pod Watcher closed connection");
                    });
                Log.Info("Starting Deployment Watcher");
                Task<HttpOperationResponse<V1DeploymentList>> deploymentlistResp = k8Client.ListDeploymentForAllNamespacesWithHttpMessagesAsync(watch: true);
                deploymentWatcher = deploymentlistResp.Watch<V1Deployment, V1DeploymentList>((eventType, deployment) =>
                {
                    Application.MainLoop.Invoke(() =>
                    {
                        Log.Info($"Processing Deployment Event: {eventType} : {JsonConvert.SerializeObject(deployment)}");
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
                        deploymentsTableView.SetNeedsDisplay();
                        UpdateTabHeaders();
                    });
                }, onError: (ex) =>
                {
                    Log.Error(ex);
                },
                    onClosed: () =>
                    {
                        Log.Error("Deployments Watcher closed connection");
                    });
                Log.Info("Starting ReplicationSet Watcher");
                Task<HttpOperationResponse<V1ReplicaSetList>> replicasetlistResp = k8Client.ListReplicaSetForAllNamespacesWithHttpMessagesAsync(watch: true);
                replicationsetWatcher = replicasetlistResp.Watch<V1ReplicaSet, V1ReplicaSetList>(
                    onEvent: (eventType, replicationset) =>
                {
                    Application.MainLoop.Invoke(() =>
                    {
                        Log.Info($"Processing ReplicationSet Event: {eventType} : {JsonConvert.SerializeObject(replicationset)}");
                        processEventDetails(eventType, replicationset);
                        switch (eventType)
                        {
                            case WatchEventType.Added:
                                replicasetsList.AddUpdateDelete(new ReplicaSetType(replicationset), CRUDOperation.Add);
                                break;
                            case WatchEventType.Modified:
                                replicasetsList.AddUpdateDelete(new ReplicaSetType(replicationset), CRUDOperation.Change);
                                break;
                            case WatchEventType.Deleted:
                                replicasetsList.AddUpdateDelete(new ReplicaSetType(replicationset), CRUDOperation.Delete);
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
                        replicasetsTableView.SetNeedsDisplay();
                        UpdateTabHeaders();
                    });
                }, onError: (ex) =>
                {
                    Log.Error(ex);
                },
                    onClosed: () =>
                    {
                        Log.Error("ReplicaSets Watcher closed connection");
                    });
            }
            catch (Exception ex)
            {
                Log.Error($"Error starting watchers to {selectedContext} context", ex, null);
            }
        }

        static void processEventDetails(WatchEventType _eventType, object _object)
        {
            eventsTableView.SetNeedsDisplay();
            eventsList.Add(new EventType(_eventType, _object));
        }
    }
}
