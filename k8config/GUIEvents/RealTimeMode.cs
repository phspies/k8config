using k8config.DataModels;
using k8s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        static StatusItem[] realtimeStatusBarItems = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode()),
                 new StatusItem (Key.CharMask, "No connection Found", null)
            };
        static Window availableContextsWindow = new Window();
        static ListView availableContextsListView = new ListView();
        static Window ReatimeModeWindow = new Window()
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
        };
        static public void RealTimeMode()
        {
            topLevelWindowObject.Add(InteractiveModeWindow);
            statusBar = new StatusBar(realtimeStatusBarItems);
            topLevelWindowObject.Add(statusBar);

            availableContextsWindow = new Window()
            {
                Title = "Available Contexts",
                X = 0,
                Y = 0,
                Width = 30,
                Height = Dim.Fill() - 9,
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
            
            ReatimeModeWindow.Add(availableContextsWindow);
            availableContextsWindow.Add(availableContextsListView);
            topLevelWindowObject.Add(ReatimeModeWindow);

            var config = KubernetesClientConfiguration.LoadKubeConfig();
            
            availableContextsListView.SetSource(config.Contexts.Select(x => x.Name).ToList());


            //.BuildConfigFromConfigFile();
            //config.SkipTlsVerify = true;
            //var config = new KubernetesClientConfiguration { Host = "http://127.0.0.1:8000" };
            //var client = new Kubernetes(config);
            //var namespaces = client.ListNamespace();
        }
        
    }
}
