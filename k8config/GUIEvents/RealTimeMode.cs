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
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { topLevelWindowObject.Running = false; }}),
                new StatusItem(Key.F10, "~F10~ YAML Mode", () => ToggleDisplayMode())
            };
        static Window ReatimeModeWindow = new Window()
        {
            Border = new Border()
            {
                BorderStyle = BorderStyle.None,
                DrawMarginFrame = false,
                Effect3D = false,
            },
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        static public void RealTimeMode()
        {
            topLevelWindowObject.Add(InteractiveModeWindow);

            var config = KubernetesClientConfiguration.LoadKubeConfig();
            //.BuildConfigFromConfigFile();
            //config.SkipTlsVerify = true;
            //var config = new KubernetesClientConfiguration { Host = "http://127.0.0.1:8000" };
            //var client = new Kubernetes(config);
            //var namespaces = client.ListNamespace();
        }
        static public void ToggleDisplayMode()
        {
            if (GlobalVariables.displayMode == 0)
            {
                GlobalVariables.displayMode = 1;
                InteractiveModeWindow.Visible = false;
                //topLevelWindowObject.Remove(statusBar);
                statusBar.Items = realtimeStatusBarItems;
                ReatimeModeWindow.Visible = true;
                //topLevelWindowObject.Add(statusBar);
            }
            else
            {
                GlobalVariables.displayMode = 0;
                InteractiveModeWindow.Visible = true;
                ReatimeModeWindow.Visible = false;
                //topLevelWindowObject.Remove(statusBar);
                statusBar.Items = interactiveStatusBarItems;
                //topLevelWindowObject.Add(statusBar);
            }
        }
    }
}
