using k8config.GUIDeployments.RealtimeMode.DataModels;
using k8config.GUIEvents.RealtimeMode.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace k8config.DataModels
{
    public class RealtimeModeControls
    {
        public static StatusItem[] realtimeStatusBarItems = new StatusItem[] {
                new StatusItem(Key.F9, "~F9~ Apply", () => Program.ApplyDefinitions()),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => Program.ToggleDisplayMode()),
                new StatusItem (Key.CharMask, "", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        public static StatusItem[] realtimeStatusBarPodsSelected = new StatusItem[] {
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    if (podsTableView.SelectedRow >= 0)
                    {
                        Program.RAWYamlView(podsList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(podsTableView.Table.Select()[podsTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                    }
                }),
                new StatusItem(Key.F8, "~F8~ Import", () =>
                {
                    if (podsTableView.SelectedRow >= 0)
                    {
                        Program.ImportDefinition(podsList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(podsTableView.Table.Select()[podsTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                    }
                }),
                new StatusItem(Key.F9, "~F9~ Apply", () => Program.ApplyDefinitions()),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => Program.ToggleDisplayMode()),
                new StatusItem (Key.CharMask, "", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        public static StatusItem[] realtimeStatusBarServicesSelected = new StatusItem[] {
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    if (servicesTableView.SelectedRow >= 0)
                    {
                        Program.RAWYamlView(servicesList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(servicesTableView.Table.Select()[servicesTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                    }
                }),
                new StatusItem(Key.F8, "~F8~ Import", () =>
                {
                    if (servicesTableView.SelectedRow >= 0)
                    {
                        Program.ImportDefinition(servicesList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(servicesTableView.Table.Select()[servicesTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                    }
                }),
                new StatusItem(Key.F9, "~F9~ Apply", () => Program.ApplyDefinitions()),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => Program.ToggleDisplayMode()),
                new StatusItem (Key.CharMask, "", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        public static StatusItem[] realtimeStatusBarDeploymentsSelected = new StatusItem[] {
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    if (deploymentsTableView.SelectedRow >= 0)
                    {
                        Program.RAWYamlView(deploymentsList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(deploymentsTableView.Table.Select()[deploymentsTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                    }
                }),
                new StatusItem(Key.F8, "~F8~ Import", () =>
                {
                    if (deploymentsTableView.SelectedRow >= 0)
                    {
                        Program.ImportDefinition(deploymentsList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(deploymentsTableView.Table.Select()[deploymentsTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                    }
                }),
                new StatusItem(Key.F9, "~F9~ Apply", () => Program.ApplyDefinitions()),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => Program.ToggleDisplayMode()),
                new StatusItem (Key.CharMask, "", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        public static StatusItem[] realtimeStatusBarNamespacesSelected = new StatusItem[] {
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    if (namespaceTableView.SelectedRow >= 0)
                    {
                        Program.RAWYamlView(namespacesList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(namespaceTableView.Table.Select()[namespaceTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                    }
                }),
                new StatusItem(Key.F8, "~F8~ Import", () =>
                {
                    if (namespaceTableView.SelectedRow >= 0)
                    {
                        Program.ImportDefinition(namespacesList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(namespaceTableView.Table.Select()[namespaceTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                    }
                }),
                new StatusItem(Key.F9, "~F9~ Apply", () => Program.ApplyDefinitions()),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => Program.ToggleDisplayMode()),
                new StatusItem (Key.CharMask, "", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        public static StatusItem[] realtimeStatusBarRepSetsSelected = new StatusItem[] {
                new StatusItem(Key.F2, "~F2~ RAW Yaml View", () =>
                {
                    if (replicasetsTableView.SelectedRow >= 0)
                    {
                        Program.RAWYamlView(replicasetsList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(replicasetsTableView.Table.Select()[replicasetsTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                    }
                }),
                new StatusItem(Key.F8, "~F8~ Import", () =>
                {
                    if (replicasetsTableView.SelectedRow >= 0)
                    {
                        Program.ImportDefinition(replicasetsList.Dictionary.FirstOrDefault(x => x.Key == Int64.Parse(replicasetsTableView.Table.Select()[replicasetsTableView.SelectedRow]["RawObject"].ToString())).Value.RawObject);
                    }
                }),
                new StatusItem(Key.F9, "~F9~ Apply", () => Program.ApplyDefinitions()),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => Program.ToggleDisplayMode()),
                new StatusItem (Key.CharMask, "", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        public static StatusItem[] realtimeStatusBarEventsSelected = new StatusItem[] {
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => Program.ToggleDisplayMode()),
                new StatusItem (Key.CharMask, "", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        public static Window availableContextsWindow = new Window();
        public static ListView availableContextsListView = new ListView();
        public static TableView namespaceTableView = new TableView();
        public static TableView podsTableView = new TableView();
        public static TableView servicesTableView = new TableView();
        public static TableView deploymentsTableView = new TableView();
        public static TableView replicasetsTableView = new TableView();
        public static TableView eventsTableView = new TableView();
        public static TabView.Tab namespacesTab = new TabView.Tab();
        public static TabView.Tab podsTab = new TabView.Tab();
        public static TabView.Tab servicesTab = new TabView.Tab();
        public static TabView.Tab deploymentsTab = new TabView.Tab();
        public static TabView.Tab replicasetsTab = new TabView.Tab();
        public static TabView.Tab eventsTab = new TabView.Tab();

        public static NamespaceListType namespacesList = new NamespaceListType();
        public static PodListType podsList = new PodListType();
        public static ServiceListType servicesList = new ServiceListType();
        public static DeploymentListType deploymentsList = new DeploymentListType();
        public static ReplicaSetListType replicasetsList = new ReplicaSetListType();
        public static EventListType eventsList = new EventListType();

        public static TabView contextDetailTabs = new TabView()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        public static Window busyWorkingWindow = new Window()
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
            ColorScheme = Program.colorSelector,
        };

        public static Window realtimeModeWindow = new Window()
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
            ColorScheme = Program.colorSelector,
        };
        public static Button closeRAWWindowButton = new Button("Close", true);
        public static Dialog rawYAMLWindow = new Dialog("RAW YAML View", 80, 40, closeRAWWindowButton)
        {
            Visible = false,

            X = Pos.Center(),
            Y = Pos.Center(),
        };
        public static List<string> rawYAMLList = new List<string>();
        public static ListView rawYAMLListView = new ListView(rawYAMLList)
        {
            Width = Dim.Fill() - 2,
            Height = Dim.Fill() - 3,
            X = 2,
            Y = 1
        };
        public static Button closeApplyWindowButton = new Button("Close", true);
        public static Dialog ApplyStatusWindow = new Dialog("Apply Definition Status", 80, 40, closeApplyWindowButton)
        {
            Visible = false,
            X = Pos.Center(),
            Y = Pos.Center(),
        };
        public static List<string> applyStatusList = new List<string>();
        public static ListView applyListView = new ListView(applyStatusList)
        {
            X = 2,
            Y = 1
        };
    }
}
