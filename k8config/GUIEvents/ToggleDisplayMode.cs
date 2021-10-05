using k8config.DataModels;

namespace k8config
{
    partial class Program
    {
        static public void ToggleDisplayMode()
        {
            if (GlobalVariables.displayMode == 0)
            {
                GlobalVariables.displayMode = 1;
                YAMLModelControls.YAMLModeWindow.Visible = false;
                statusBar.Items = RealtimeModeControls.realtimeStatusBarItems;
                RealtimeModeControls.realtimeModeWindow.Visible = true;
                UpdateStatusBar();
                StartWatchersTasks();
            }
            else
            {
                DisposeAllWatchers();
                GlobalVariables.displayMode = 0;
                YAMLModelControls.YAMLModeWindow.Visible = true;
                RealtimeModeControls.realtimeModeWindow.Visible = false;
                statusBar.Items = YAMLModelControls.interactiveStatusBarItems;
                YAMLModelControls.commandPromptTextField.SetFocus();
                updateAvailableKindsList();
                //UpdateDescriptionView();
            }
        }
    }
}
