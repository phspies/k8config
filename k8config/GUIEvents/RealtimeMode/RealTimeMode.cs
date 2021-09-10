using k8config.GUIEvents.RealtimeMode.DataModels;
using k8config.GUIEvents.RealtimeMode.DataTables;
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
        static StatusItem[] realtimeStatusBarItems = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, "No context selected", null, true, new Terminal.Gui.Attribute(Color.BrightYellow, Color.DarkGray))
            };
        static StatusItem[] realtimeStatusBarItemsSelected = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                new StatusItem (Key.CharMask, "No context selected", null, true, new Terminal.Gui.Attribute(Color.BrightYellow, Color.DarkGray))
            };
        static List<PodType> podList = new List<PodType>();

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
        static DataTable namespacesTable = ConstructDataTables.Namespaces();
        static DataTable podsTable = ConstructDataTables.Pods();
        static DataTable servicesTable = ConstructDataTables.Services();
        static DataTable deploymentsTable = ConstructDataTables.Deployments();
        static DataTable replicasetsTable = ConstructDataTables.ReplicaSets();
        static DataTable eventsTable = ConstructDataTables.Events();
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
      

        static public void RealTimeMode()
        {
            topLevelWindowObject.Add(InteractiveModeWindow);
            statusBar = new StatusBar(realtimeStatusBarItems);
            topLevelWindowObject.Add(statusBar);
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
                Table = namespacesTable,
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
                Table = podsTable,
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

            //podsTableView.Style.GetOrCreateColumnStyle(podsTable.Columns["Namespace"]).RepresentationGetter = (i) => podList.FirstOrDefault(x => x.Name == (string)i).Namespace.ToString();
            //podsTableView.Style.GetOrCreateColumnStyle(podsTable.Columns["Name"]).RepresentationGetter = (i) => podList.FirstOrDefault(x => x.Name == (string)i).Name.ToString();
            //podsTableView.Style.GetOrCreateColumnStyle(podsTable.Columns["Ready"]).RepresentationGetter = (i) => podList.FirstOrDefault(x => x.Name == (string)i).Ready.ToString();
            //podsTableView.Style.GetOrCreateColumnStyle(podsTable.Columns["Status"]).RepresentationGetter = (i) => podList.FirstOrDefault(x => x.Name == (string)i).Status.ToString();
            //podsTableView.Style.GetOrCreateColumnStyle(podsTable.Columns["Restarts"]).RepresentationGetter = (i) => podList.FirstOrDefault(x => x.Name == (string)i).Restarts.ToString();
            //podsTableView.Style.GetOrCreateColumnStyle(podsTable.Columns["Age"]).RepresentationGetter = (i) => podList.FirstOrDefault(x => x.Name == (string)i).Age.ToString();

            servicesTableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                LayoutStyle = LayoutStyle.Computed,
                Style = tableStyle,
                Table = servicesTable,
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
                Table = deploymentsTable,
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
                Table = replicasetsTable,
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
                Table = eventsTable,
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
                        updateMessageBar("Namespaces selected");
                        break;
                    case ustring a when a.ToLower().Contains("pod"):
                        updateMessageBar("Pods selected");
                        break;
                    case ustring a when a.ToLower().Contains("service"):
                        updateMessageBar("Services selected");
                        break;
                    case ustring a when a.ToLower().Contains("deployment"):
                        updateMessageBar("Deployments selected");
                        break;
                    case ustring a when a.ToLower().Contains("replica"):
                        updateMessageBar("ReplicSets selected");
                        break;
                    case ustring a when a.ToLower().Contains("events"):
                        updateMessageBar("Events selected");
                        break;
                };
            };

            topLevelWindowObject.Add(realtimeModeWindow);
            realtimeModeWindow.Add(availableContextsWindow);

            realtimeModeWindow.Add(contextDetailTabs);
            realtimeModeWindow.Add(busyWorkingWindow);

            availableContextsWindow.Add(availableContextsListView);


            try
            {
                availableContextsListView.SetSource(KubernetesClientConfiguration.LoadKubeConfig().Contexts.Select(x => x.Name).ToList());
            }
            catch (Exception ex)
            {
                updateMessageBar(ex.Message);
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
