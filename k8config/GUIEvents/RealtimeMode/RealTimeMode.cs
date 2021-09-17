using k8config.DataModels;
using k8config.GUIDeployments.RealtimeMode.DataModels;
using k8config.GUIEvents.RealtimeMode.DataModels;
using k8s;
using NStack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        static StatusItem[] realtimeStatusBarItems = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, GlobalVariables.k8configVersion, null, false, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        static StatusItem[] realtimeStatusBarPodsSelected = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    PodType pod = podsList.Dictionary.FirstOrDefault(x => x.Key == long.Parse(podsTableView.Table.Select()[podsTableView.SelectedRow]["Index"].ToString())).Value;
                    RAWYamlView(pod?.Name, pod?.RawObject);
                }),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, GlobalVariables.k8configVersion, null, false, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        static StatusItem[] realtimeStatusBarServicesSelected = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    DataRow row =servicesTableView.Table.Select()[servicesTableView.SelectedRow];
                    RAWYamlView(row["Name"].ToString(), row["RawObject"]);
                }),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, GlobalVariables.k8configVersion, null, false, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        static StatusItem[] realtimeStatusBarDeploymentsSelected = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    DataRow row =deploymentsTableView.Table.Select()[deploymentsTableView.SelectedRow];
                    RAWYamlView(row["Name"].ToString(), row["RawObject"]);
                }),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, GlobalVariables.k8configVersion, null, false, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        static StatusItem[] realtimeStatusBarNamespacesSelected = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    DataRow row =namespaceTableView.Table.Select()[namespaceTableView.SelectedRow];
                    RAWYamlView(row["Name"].ToString(), row["RawObject"]);
                }),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, GlobalVariables.k8configVersion, null, false, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        static StatusItem[] realtimeStatusBarRepSetsSelected = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    DataRow row =replicasetsTableView.Table.Select()[replicasetsTableView.SelectedRow];
                    RAWYamlView(row["Name"].ToString(), row["RawObject"]);
                }),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, GlobalVariables.k8configVersion, null, false, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        static StatusItem[] realtimeStatusBarEventsSelected = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    DataRow eventRecord = eventsList.DataTableConstruct.Rows[eventsTableView.SelectedRow];
                    RAWYamlView(eventRecord["Name"].ToString(), eventRecord["RawObject"]);
                }),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, GlobalVariables.k8configVersion, null, false, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        static Window availableContextsWindow = new Window();
        static ListView availableContextsListView = new ListView();
        static TableView namespaceTableView = new TableView();
        static TableView podsTableView = new TableView();
        static TableView servicesTableView = new TableView();
        static TableView deploymentsTableView = new TableView();
        static TableView replicasetsTableView = new TableView();
        static TableView eventsTableView = new TableView();
        static TabView.Tab namespacesTab = new TabView.Tab();
        static TabView.Tab podsTab = new TabView.Tab();
        static TabView.Tab servicesTab = new TabView.Tab();
        static TabView.Tab deploymentsTab = new TabView.Tab();
        static TabView.Tab replicasetsTab = new TabView.Tab();
        static TabView.Tab eventsTab = new TabView.Tab();

        static NamespaceListType namespacesList = new NamespaceListType();
        static PodListType podsList = new PodListType();
        static ServiceListType servicesList = new ServiceListType();
        static DeploymentListType deploymentsList = new DeploymentListType();
        static ReplicaSetListType replicasetsList = new ReplicaSetListType();
        static EventListType eventsList = new EventListType();

        static TabView contextDetailTabs = new TabView()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        static Window busyWorkingWindow = new Window()
        {
            Border = new Border()
            {
                Effect3D = true,
                BorderStyle = BorderStyle.Single
            },
            LayoutStyle = LayoutStyle.Computed,
            TextAlignment = TextAlignment.Centered,
            VerticalTextAlignment = VerticalTextAlignment.Middle,
            Visible = false,
            Width = 50,
            Height = 3,
            X = Pos.Center(),
            Y = Pos.Center(),
            ColorScheme = colorSelector,
        };

        static Window realtimeModeWindow = new Window()
        {
            Border = new Border()
            {
                BorderStyle = BorderStyle.None,
                DrawMarginFrame = false,
                Effect3D = false,
            },
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Visible = false,
            TabStop = false,
            CanFocus = false,
            ColorScheme = colorSelector,
        };
        static Button closeRAWWindowButton = new Button("Close", true);
        static Dialog rawYAMLWindow = new Dialog("RAW YAML View", 80, 40, closeRAWWindowButton)
        {
            Visible = false,

            X = Pos.Center(),
            Y = Pos.Center(),
        };
        static List<string> rawYAMLList = new List<string>();
        static ListView rawYAMLListView = new ListView(rawYAMLList)
        {
            Width = Dim.Fill() - 2,
            Height = Dim.Fill() - 3,
            X = 2,
            Y = 1
        };


        static public void RealTimeMode()
        {
            topLevelWindowObject.Add(InteractiveModeWindow);
            k8Client = new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile());

            TableView.TableStyle tableStyle = new TableView.TableStyle() { AlwaysShowHeaders = true, ShowHorizontalHeaderOverline = false, ShowHorizontalHeaderUnderline = false, ShowVerticalCellLines = false, ShowVerticalHeaderLines = false, ExpandLastColumn = false };

            availableContextsWindow = new Window()
            {
                Title = "Available Contexts",
                X = 0,
                Y = 0,
                Width = 30,
                Height = Dim.Fill(),
                TabStop = false,
                CanFocus = false,
                ColorScheme = colorNormal,
            };
            availableContextsListView = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                AllowsMultipleSelection = false,
                AllowsMarking = false,
                CanFocus = true,
                TabStop = true,
                ColorScheme = colorSelector
            };


            namespaceTableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                LayoutStyle = LayoutStyle.Computed,
                Style = tableStyle,
                Table = namespacesList.ToDataTable(),
                Border = new Border()
                {
                    BorderStyle = BorderStyle.None,
                    DrawMarginFrame = false,
                    Effect3D = false,
                },
                CanFocus = true,
                TabStop = true,
                ColorScheme = colorSelector
            };
            podsTableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                LayoutStyle = LayoutStyle.Computed,
                Style = tableStyle,
                Table = podsList.DataTable,
                Border = new Border()
                {
                    BorderStyle = BorderStyle.None,
                    DrawMarginFrame = false,
                    Effect3D = false,
                },
                CanFocus = true,
                TabStop = true,
                ColorScheme = colorSelector
            };



            servicesTableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                LayoutStyle = LayoutStyle.Computed,
                Style = tableStyle,
                Table = servicesList.ToDataTable(),
                Border = new Border()
                {
                    BorderStyle = BorderStyle.None,
                    DrawMarginFrame = false,
                    Effect3D = false,
                },
                CanFocus = true,
                TabStop = true,
                ColorScheme = colorSelector
            };
            deploymentsTableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                LayoutStyle = LayoutStyle.Computed,
                Style = tableStyle,
                Table = deploymentsList.DataTable,
                Border = new Border()
                {
                    BorderStyle = BorderStyle.None,
                    DrawMarginFrame = false,
                    Effect3D = false,
                },
                CanFocus = true,
                TabStop = true,
                ColorScheme = colorSelector
            };


            replicasetsTableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                LayoutStyle = LayoutStyle.Computed,
                Style = tableStyle,
                Table = replicasetsList.ToDataTable(),
                Border = new Border()
                {
                    BorderStyle = BorderStyle.None,
                    DrawMarginFrame = false,
                    Effect3D = false,
                },
                CanFocus = true,
                TabStop = true,
                ColorScheme = colorSelector
            };
            eventsTableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                LayoutStyle = LayoutStyle.Computed,
                Style = tableStyle,
                Table = eventsList.DataTableConstruct,
                Border = new Border()
                {
                    BorderStyle = BorderStyle.None,
                    DrawMarginFrame = false,
                    Effect3D = false,
                },
                CanFocus = true,
                TabStop = true,
                ColorScheme = colorSelector
            };
            //eventsTableView.Style.GetOrCreateColumnStyle(eventsList.DataTableConstruct.Columns["Name"]).RepresentationGetter = (i) => eventsList.DictionaryConstruct.FirstOrDefault(x => x.Key == (long)i).Value.Name.ToString();
            //eventsTableView.Style.GetOrCreateColumnStyle(eventsList.DataTableConstruct.Columns["TimeStamp"]).RepresentationGetter = (i) => eventsList.DictionaryConstruct.FirstOrDefault(x => x.Key == (long)i).Value.TimeStamp.ToString();
            //eventsTableView.Style.GetOrCreateColumnStyle(eventsList.DataTableConstruct.Columns["Kubernetes Type"]).RepresentationGetter = (i) => eventsList.DictionaryConstruct.FirstOrDefault(x => x.Key == (long)i).Value.Type.ToString();
            //eventsTableView.Style.GetOrCreateColumnStyle(eventsList.DataTableConstruct.Columns["Action"]).RepresentationGetter = (i) => eventsList.DictionaryConstruct.FirstOrDefault(x => x.Key == (long)i).Value.Action.ToString();


            namespacesTab = new TabView.Tab($" Namespaces ", namespaceTableView);
            podsTab = new TabView.Tab($" Pods ", podsTableView);
            servicesTab = new TabView.Tab($" Services ", servicesTableView);
            deploymentsTab = new TabView.Tab($" Deployments ", deploymentsTableView);
            replicasetsTab = new TabView.Tab($" Replica Sets ", replicasetsTableView);
            eventsTab = new TabView.Tab($" Events ", eventsTableView);
            contextDetailTabs.AddTab(namespacesTab, false);
            contextDetailTabs.AddTab(podsTab, true);
            contextDetailTabs.AddTab(servicesTab, false);
            contextDetailTabs.AddTab(deploymentsTab, false);
            contextDetailTabs.AddTab(replicasetsTab, false);
            contextDetailTabs.AddTab(eventsTab, false);
            contextDetailTabs.ColorScheme = colorSelector;
            contextDetailTabs.SelectedTabChanged += (s, e) =>
            {
                switch (e.NewTab.Text)
                {
                    case ustring a when a.ToLower().Contains("namespace"):
                        UpdateMessageBar("Namespaces selected");
                        statusBar.Items = realtimeStatusBarNamespacesSelected;
                        break;
                    case ustring a when a.ToLower().Contains("pod"):
                        UpdateMessageBar("Pods selected");
                        statusBar.Items = realtimeStatusBarPodsSelected;
                        break;
                    case ustring a when a.ToLower().Contains("service"):
                        UpdateMessageBar("Services selected");
                        statusBar.Items = realtimeStatusBarServicesSelected;
                        break;
                    case ustring a when a.ToLower().Contains("deployment"):
                        UpdateMessageBar("Deployments selected");
                        statusBar.Items = realtimeStatusBarDeploymentsSelected;
                        break;
                    case ustring a when a.ToLower().Contains("replica"):
                        UpdateMessageBar("ReplicSets selected");
                        statusBar.Items = realtimeStatusBarRepSetsSelected;
                        break;
                    case ustring a when a.ToLower().Contains("events"):
                        UpdateMessageBar("Events selected");
                        statusBar.Items = realtimeStatusBarEventsSelected;
                        break;
                };
            };

            topLevelWindowObject.Add(realtimeModeWindow);
            realtimeModeWindow.Add(availableContextsWindow);

            realtimeModeWindow.Add(contextDetailTabs);
            realtimeModeWindow.Add(busyWorkingWindow);

            availableContextsWindow.Add(availableContextsListView);
            rawYAMLWindow.Add(rawYAMLListView);
            closeRAWWindowButton.Clicked += () =>
            {
                rawYAMLWindow.Visible = false;

            };
            realtimeModeWindow.Add(rawYAMLWindow);

            try
            {
                availableContextsListView.SetSource(KubernetesClientConfiguration.LoadKubeConfig().Contexts.Select(x => x.Name).ToList());
            }
            catch (Exception ex)
            {
                UpdateMessageBar(ex.Message);
            }
            realtimeModeKeyEvents();


            //.BuildConfigFromConfigFile();
            //config.SkipTlsVerify = true;
            //var config = new KubernetesClientConfiguration { Host = "http://127.0.0.1:8000" };
            //var config = KubernetesClientConfiguration.BuildConfigFromConfigObject(KubernetesClientConfiguration.LoadKubeConfig(), "docker-desktop");
            //var client = new Kubernetes(config);
            //var namespaces = client.ListNamespace();
        }

    }
}
