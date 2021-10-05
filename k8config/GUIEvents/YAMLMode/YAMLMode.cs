using k8config.DataModels;
using k8config.GUIEvents;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
  

        static public void YAMLMode()
        {
            topLevelWindowObject.Add(YAMLModelControls.YAMLModeWindow);
            statusBar.Items = YAMLModelControls.interactiveStatusBarItems;
            YAMLModelControls.availableKindsWindow = new Window()
            {
                Title = "Available Commands",
                X = 0,
                Y = 0,
                Width = 50,
                Height = Dim.Fill() - 8,
                TabStop = false,
                CanFocus = false,
                ColorScheme = colorNormal,

            };
            YAMLModelControls.descriptionWindow = new Window()
            {
                Title = "Description",
                X = 0,
                Y = YAMLModelControls.availableKindsWindow.Bounds.Bottom,
                Width = Dim.Fill(),
                Height = 5,
                TabStop = false,
                CanFocus = false,
                ColorScheme = colorNormal,
            };
            YAMLModelControls.descriptionView = new TextView()
            {

                Y = 0,
                X = 0,
                Width = 80,
                Height = 1,
                Text = "No definitions found. Please create a definition with the 'new <kind>' command.",
                ColorScheme = colorNormal,
                ReadOnly = true,
                WordWrap = true,
                TabStop = false,
                CanFocus = false,
                AllowsTab = false,
                Multiline = true,
            };


            YAMLModelControls.definedYAMLWindow = new Window()
            {
                Title = "Selected Definition",
                X = YAMLModelControls.availableKindsWindow.Bounds.Right,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 9,
                TabStop = false,
                CanFocus = false,
                ColorScheme = colorNormal

            };
            YAMLModelControls.commandWindow = new Window()
            {
                Title = "Command Line",
                Y = YAMLModelControls.descriptionWindow.Bounds.Bottom,
                Width = Dim.Fill(),
                Height = 3,
                ColorScheme = colorNormal,
                TabStop = false,
                CanFocus = false,
            };


            YAMLModelControls.commandPromptLabel = new Label(YAMLModePromptObject.PromptConstructor) { TextAlignment = TextAlignment.Left, X = 1, TabStop = false };
            YAMLModelControls.commandPromptTextField = new TextField("") { X = Pos.Right(YAMLModelControls.commandPromptLabel) + 1, Width = Dim.Fill(), TabStop = true };

            YAMLModelControls.definedYAMLListView = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                AllowsMultipleSelection = false,
                AllowsMarking = false,
                CanFocus = false,
                TabStop = false,
                ColorScheme = colorSelector,

            };
            YAMLModelControls.definedYAMLWindow.Add(YAMLModelControls.definedYAMLListView);
            YAMLModelControls.availableKindsListView = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                AllowsMultipleSelection = false,
                AllowsMarking = false,
                CanFocus = false,
                TabStop = false,
                ColorScheme = colorSelector
            };

            YAMLModelControls.availableKindsWindow.Add(YAMLModelControls.availableKindsListView);
            YAMLModelControls.YAMLModeWindow.Add(YAMLModelControls.commandWindow, YAMLModelControls.definedYAMLWindow);
            YAMLModelControls.YAMLModeWindow.Add(YAMLModelControls.availableKindsWindow);
            //commandWindow.Add(messageBarItem);
            YAMLModelControls.descriptionWindow.Add(YAMLModelControls.descriptionView);
            YAMLModelControls.YAMLModeWindow.Add(YAMLModelControls.descriptionWindow);
            YAMLModelControls.commandWindow.Add(YAMLModelControls.commandPromptLabel, YAMLModelControls.commandPromptTextField);
            topLevelWindowObject.Add(YAMLModelControls.YAMLModeWindow);
            YAMLModelControls.YAMLModeWindow.Add(YAMLModelControls.validationWindow);
            YAMLModelControls.closeValidationWindowButton.Clicked += () =>
            {
                YAMLModelControls.validationWindow.Visible = false;
                YAMLModelControls.commandPromptTextField.SetFocus();
            };
            

            ToggleDisplayMode();
            paintYAML();
            if (!string.IsNullOrWhiteSpace(GlobalVariables.startupString))
            {
                UpdateMessageBar(GlobalVariables.startupString);
                updateAvailableKindsList();
                
            }
            UpdateDescriptionView();
        }
    }
}
