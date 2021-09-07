using k8config.DataModels;
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
                InteractiveModeWindow.Visible = false;
                statusBar.Items = realtimeStatusBarItems;
                realtimeModeWindow.Visible = true;
            }
            else
            {
                GlobalVariables.displayMode = 0;
                InteractiveModeWindow.Visible = true;
                realtimeModeWindow.Visible = false;
                statusBar.Items = interactiveStatusBarItems;
            }
        }
    }
}
