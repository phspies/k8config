using k8config.DataModels;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config
{
    partial class Program
    {
        static public void ToggleDisplayMode()
        {
            if (GlobalVariables.displayMode == 0)
            {
                GlobalVariables.displayMode = 1;
                YAMLModeWindow.Visible = false;
                statusBar.Items = realtimeStatusBarItems;
                realtimeModeWindow.Visible = true;
                UpdateStatusBar();
                StartWatchersTasks();
            }
            else
            {
                DisposeAllWatchers();
                GlobalVariables.displayMode = 0;
                YAMLModeWindow.Visible = true;
                realtimeModeWindow.Visible = false;
                statusBar.Items = interactiveStatusBarItems;
                commandPromptTextField.SetFocus();
                updateAvailableKindsList();
                //UpdateDescriptionView();
            }
        }
    }
}
