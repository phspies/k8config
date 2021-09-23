using NStack;
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
        static void UpdateStatusBar()
        {
            Application.MainLoop.Invoke(() =>
            {
                switch (contextDetailTabs.SelectedTab.Text)
                {
                    case ustring a when a.ToLower().Contains("namespace"):
                        statusBar.Items = realtimeStatusBarNamespacesSelected;
                        break;
                    case ustring a when a.ToLower().Contains("pod"):
                        statusBar.Items = realtimeStatusBarPodsSelected;
                        break;
                    case ustring a when a.ToLower().Contains("service"):
                        statusBar.Items = realtimeStatusBarServicesSelected;
                        break;
                    case ustring a when a.ToLower().Contains("deployment"):
                        statusBar.Items = realtimeStatusBarDeploymentsSelected;
                        break;
                    case ustring a when a.ToLower().Contains("replica"):
                        statusBar.Items = realtimeStatusBarRepSetsSelected;
                        break;
                    case ustring a when a.ToLower().Contains("events"):
                        statusBar.Items = realtimeStatusBarEventsSelected;
                        break;
                };
                statusBar.SetNeedsDisplay();
            });
        }
    }
}
