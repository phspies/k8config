using k8config.DataModels;
using k8config.GUIDeployments.RealtimeMode.DataModels;
using k8config.GUIEvents.RealtimeMode.DataModels;
using k8s;
using NStack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Terminal.Gui;

namespace k8config
{

    partial class Program
    {
        static bool realtimeAvailable = false;
        static StatusItem[] realtimeStatusBarItems = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F9, "~F9~ Apply", () => ApplyDefinitions()),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, GlobalVariables.k8configVersion, null, false, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray)),
                new StatusItem (Key.CharMask, "", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        static StatusItem[] realtimeStatusBarPodsSelected = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    RAWYamlView(podsList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(podsTableView.Table.Select()[podsTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                }),
                new StatusItem(Key.F8, "~F8~ Import", () =>
                {
                    ImportDefinition(podsList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(podsTableView.Table.Select()[podsTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                }),
                new StatusItem(Key.F9, "~F9~ Apply", () => ApplyDefinitions()),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, GlobalVariables.k8configVersion, null, false, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray)),
                new StatusItem (Key.CharMask, "", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        static StatusItem[] realtimeStatusBarServicesSelected = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    RAWYamlView(servicesList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(servicesTableView.Table.Select()[servicesTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                }),
                new StatusItem(Key.F8, "~F8~ Import", () =>
                {
                    ImportDefinition(servicesList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(servicesTableView.Table.Select()[servicesTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                }),
                new StatusItem(Key.F9, "~F9~ Apply", () => ApplyDefinitions()),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, GlobalVariables.k8configVersion, null, false, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray)),
                new StatusItem (Key.CharMask, "", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        static StatusItem[] realtimeStatusBarDeploymentsSelected = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    RAWYamlView(deploymentsList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(deploymentsTableView.Table.Select()[deploymentsTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                }),
                new StatusItem(Key.F8, "~F8~ Import", () =>
                {
                    ImportDefinition(deploymentsList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(deploymentsTableView.Table.Select()[deploymentsTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                }),
                new StatusItem(Key.F9, "~F9~ Apply", () => ApplyDefinitions()),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, GlobalVariables.k8configVersion, null, false, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray)),
                new StatusItem (Key.CharMask, "", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        static StatusItem[] realtimeStatusBarNamespacesSelected = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    RAWYamlView(namespacesList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(namespaceTableView.Table.Select()[namespaceTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                }),
                new StatusItem(Key.F8, "~F8~ Import", () =>
                {
                    ImportDefinition(namespacesList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(namespaceTableView.Table.Select()[namespaceTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                }),
                new StatusItem(Key.F9, "~F9~ Apply", () => ApplyDefinitions()),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, GlobalVariables.k8configVersion, null, false, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray)),
                new StatusItem (Key.CharMask, "", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        static StatusItem[] realtimeStatusBarRepSetsSelected = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    RAWYamlView(replicasetsList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(replicasetsTableView.Table.Select()[replicasetsTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                }),
                new StatusItem(Key.F8, "~F8~ Import", () =>
                {
                    ImportDefinition(replicasetsList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(replicasetsTableView.Table.Select()[replicasetsTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);

                }),
                new StatusItem(Key.F9, "~F9~ Apply", () => ApplyDefinitions()),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, GlobalVariables.k8configVersion, null, false, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray)),
                new StatusItem (Key.CharMask, "", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        static StatusItem[] realtimeStatusBarEventsSelected = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, GlobalVariables.k8configVersion, null, false, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray)),
                new StatusItem (Key.CharMask, "", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
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
        static Button closeApplyWindowButton = new Button("Close", true);
        static Dialog ApplyStatusWindow = new Dialog("Apply Definition Status", 80, 40, closeApplyWindowButton)
        {
            Visible = false,
            X = Pos.Center(),
            Y = Pos.Center(),
        };
        static List<string> applyStatusList = new List<string>();
        static ListView applyListView = new ListView(applyStatusList)
        {
            X = 2,
            Y = 1
        };

        static public void RealTimeMode()
        {
            topLevelWindowObject.Add(YAMLModeWindow);


            TableView.TableStyle tableStyle = new TableView.TableStyle() { AlwaysShowHeaders = true, ShowHorizontalHeaderOverline = false, ShowHorizontalHeaderUnderline = true, ShowVerticalCellLines = true, ShowVerticalHeaderLines = true, ExpandLastColumn = false };

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
                Table = namespacesList.DataTable,
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
                Table = servicesList.DataTable,
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
                Table = replicasetsList.DataTable,
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


            ApplyStatusWindow.Add(applyListView);

            ApplyStatusWindow.Height = Dim.Fill() - 10;
            ApplyStatusWindow.Width = Dim.Fill() - 10;
            applyListView.Width = Dim.Fill() - 2;
            applyListView.Height = Dim.Fill() - 4;

            closeApplyWindowButton.Clicked += () =>
            {
                ApplyStatusWindow.Visible = false;

            };


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
                UpdateStatusBar();
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

            realtimeModeWindow.Add(ApplyStatusWindow);

            //.BuildConfigFromConfigFile();
            //config.SkipTlsVerify = true;
            //var config = new KubernetesClientConfiguration { Host = "http://127.0.0.1:8000" };
            //var config = KubernetesClientConfiguration.BuildConfigFromConfigObject(KubernetesClientConfiguration.LoadKubeConfig(), "docker-desktop");
            //var client = new Kubernetes(config);
            //var namespaces = client.ListNamespace();
        }

    }
}
