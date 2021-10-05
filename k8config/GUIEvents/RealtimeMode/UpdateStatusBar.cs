using k8config.DataModels;
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
                switch (RealtimeModeControls.contextDetailTabs.SelectedTab.Text)
                {
                    case ustring a when a.ToLower().Contains("namespace"):
                        statusBar.Items = RealtimeModeControls.realtimeStatusBarNamespacesSelected;
                        break;
                    case ustring a when a.ToLower().Contains("pod"):
                        statusBar.Items = RealtimeModeControls.realtimeStatusBarPodsSelected;
                        break;
                    case ustring a when a.ToLower().Contains("service"):
                        statusBar.Items = RealtimeModeControls.realtimeStatusBarServicesSelected;
                        break;
                    case ustring a when a.ToLower().Contains("deployment"):
                        statusBar.Items = RealtimeModeControls.realtimeStatusBarDeploymentsSelected;
                        break;
                    case ustring a when a.ToLower().Contains("replica"):
                        statusBar.Items = RealtimeModeControls.realtimeStatusBarRepSetsSelected;
                        break;
                    case ustring a when a.ToLower().Contains("events"):
                        statusBar.Items = RealtimeModeControls.realtimeStatusBarEventsSelected;
                        break;
                };
                statusBar.SetNeedsDisplay();
            });
        }
    }
}
