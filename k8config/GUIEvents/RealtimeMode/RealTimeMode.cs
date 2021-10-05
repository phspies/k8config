using k8config.DataModels;
using k8config.GUIDeployments.RealtimeMode.DataModels;
using k8config.GUIEvents.RealtimeMode.DataModels;
using k8s;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Terminal.Gui;

namespace k8config
{

    partial class Program
    { 
        static public void RealTimeMode()
        {
            TableView.TableStyle tableStyle = new TableView.TableStyle() { AlwaysShowHeaders = true, ShowHorizontalHeaderOverline = false, ShowHorizontalHeaderUnderline = true, ShowVerticalCellLines = true, ShowVerticalHeaderLines = true, ExpandLastColumn = false };
            RealtimeModeControls.availableContextsWindow = new Window()
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
            RealtimeModeControls.availableContextsListView = new ListView()
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


            RealtimeModeControls.namespaceTableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                LayoutStyle = LayoutStyle.Computed,
                Style = tableStyle,
                Table = RealtimeModeControls.namespacesList.DataTable,
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
            RealtimeModeControls.podsTableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                LayoutStyle = LayoutStyle.Computed,
                Style = tableStyle,
                Table = RealtimeModeControls.podsList.DataTable,
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



            RealtimeModeControls.servicesTableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                LayoutStyle = LayoutStyle.Computed,
                Style = tableStyle,
                Table = RealtimeModeControls.servicesList.DataTable,
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
            RealtimeModeControls.deploymentsTableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                LayoutStyle = LayoutStyle.Computed,
                Style = tableStyle,
                Table = RealtimeModeControls.deploymentsList.DataTable,
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


            RealtimeModeControls.replicasetsTableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                LayoutStyle = LayoutStyle.Computed,
                Style = tableStyle,
                Table = RealtimeModeControls.replicasetsList.DataTable,
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
            RealtimeModeControls.eventsTableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                LayoutStyle = LayoutStyle.Computed,
                Style = tableStyle,
                Table = RealtimeModeControls.eventsList.DataTableConstruct,
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


            RealtimeModeControls.ApplyStatusWindow.Add(RealtimeModeControls.applyListView);

            RealtimeModeControls.ApplyStatusWindow.Height = Dim.Fill() - 10;
            RealtimeModeControls.ApplyStatusWindow.Width = Dim.Fill() - 10;
            RealtimeModeControls.applyListView.Width = Dim.Fill() - 2;
            RealtimeModeControls.applyListView.Height = Dim.Fill() - 4;

            RealtimeModeControls.closeApplyWindowButton.Clicked += () =>
            {
                RealtimeModeControls.ApplyStatusWindow.Visible = false;

            };


            RealtimeModeControls.namespacesTab = new TabView.Tab($" Namespaces ", RealtimeModeControls.namespaceTableView);
            RealtimeModeControls.podsTab = new TabView.Tab($" Pods ", RealtimeModeControls.podsTableView);
            RealtimeModeControls.servicesTab = new TabView.Tab($" Services ", RealtimeModeControls.servicesTableView);
            RealtimeModeControls.deploymentsTab = new TabView.Tab($" Deployments ", RealtimeModeControls.deploymentsTableView);
            RealtimeModeControls.replicasetsTab = new TabView.Tab($" Replica Sets ", RealtimeModeControls.replicasetsTableView);
            RealtimeModeControls.eventsTab = new TabView.Tab($" Events ", RealtimeModeControls.eventsTableView);
            RealtimeModeControls.contextDetailTabs.AddTab(RealtimeModeControls.namespacesTab, false);
            RealtimeModeControls.contextDetailTabs.AddTab(RealtimeModeControls.podsTab, true);
            RealtimeModeControls.contextDetailTabs.AddTab(RealtimeModeControls.servicesTab, false);
            RealtimeModeControls.contextDetailTabs.AddTab(RealtimeModeControls.deploymentsTab, false);
            RealtimeModeControls.contextDetailTabs.AddTab(RealtimeModeControls.replicasetsTab, false);
            RealtimeModeControls.contextDetailTabs.AddTab(RealtimeModeControls.eventsTab, false);
            RealtimeModeControls.contextDetailTabs.ColorScheme = colorSelector;
            RealtimeModeControls.contextDetailTabs.SelectedTabChanged += (s, e) =>
            {
                UpdateStatusBar();
            };

            topLevelWindowObject.Add(RealtimeModeControls.realtimeModeWindow);
            RealtimeModeControls.realtimeModeWindow.Add(RealtimeModeControls.availableContextsWindow);

            RealtimeModeControls.realtimeModeWindow.Add(RealtimeModeControls.contextDetailTabs);
            RealtimeModeControls.realtimeModeWindow.Add(RealtimeModeControls.busyWorkingWindow);

            RealtimeModeControls.availableContextsWindow.Add(RealtimeModeControls.availableContextsListView);
            RealtimeModeControls.rawYAMLWindow.Add(RealtimeModeControls.rawYAMLListView);
            RealtimeModeControls.closeRAWWindowButton.Clicked += () =>
            {
                RealtimeModeControls.rawYAMLWindow.Visible = false;

            };
            RealtimeModeControls.realtimeModeWindow.Add(RealtimeModeControls.rawYAMLWindow);

            try
            {
                RealtimeModeControls.availableContextsListView.SetSource(KubernetesClientConfiguration.LoadKubeConfig().Contexts.Select(x => x.Name).ToList());
            }
            catch (Exception ex)
            {
                UpdateMessageBar(ex.Message);
            }
            realtimeModeKeyEvents();

            RealtimeModeControls.realtimeModeWindow.Add(RealtimeModeControls.ApplyStatusWindow);

            //.BuildConfigFromConfigFile();
            //config.SkipTlsVerify = true;
            //var config = new KubernetesClientConfiguration { Host = "http://127.0.0.1:8000" };
            //var config = KubernetesClientConfiguration.BuildConfigFromConfigObject(KubernetesClientConfiguration.LoadKubeConfig(), "docker-desktop");
            //var client = new Kubernetes(config);
            //var namespaces = client.ListNamespace();
        }

    }
}
