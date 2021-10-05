using k8config.GUIEvents.YAMLMode;
using System;
using System.Collections.Generic;
using Terminal.Gui;

namespace k8config.DataModels
{
    public class YAMLModelControls
    {
        public static Window YAMLModeWindow = new Window()
        {
            Border = new Border()
            {
                BorderStyle = BorderStyle.None,
                DrawMarginFrame = false,
                Effect3D = false,
            },
            Width = Dim.Fill(),
            Height = Dim.Fill(),

        };
        public static List<string> sessionHistory = new List<string>();
        public static int sessionHistoryIndex = 0;
        public static ListView availableKindsListView = new ListView();
        public static Label commandPromptLabel = new Label();
        public static Window commandWindow = new Window();
        public static TextField commandPromptTextField = new TextField("");
        public static Window definedYAMLWindow = new Window();
        public static Window availableKindsWindow = new Window();
        public static ListView definedYAMLListView = new ListView();
        public static Window descriptionWindow = new Window();
        public static TextView descriptionView = new TextView() { Width = 80, Height = 5 };
        public static Button closeValidationWindowButton = new Button("Close", true);
        public static Dialog validationWindow = new Dialog("Validation Status", 80, 17, closeValidationWindowButton)
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            Visible = false,
        };
        public static StatusItem[] interactiveStatusBarItems = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Program.Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F2, "~F2~ Validate", () => {
                    if (!validationWindow.Visible)
                    {
                        List<string> objectionsList = YAMLOperations.Validate();
                        var validationlistView = new ListView(objectionsList)
                        {
                            Width = validationWindow.Bounds.Width - 2,
                            Height = validationWindow.Bounds.Height - 4,
                            X = validationWindow.Bounds.Top + 2,
                            Y = Pos.Center()
                        };
                        validationWindow.Add(validationlistView);

                        validationWindow.Visible = true;
                        closeValidationWindowButton.SetFocus();
                    }
                }),
                new StatusItem(Key.CharMask, "No Defintions Found", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))
            };
        public static Label k8configVersion = new Label() { Text = GlobalVariables.k8configVersion, Visible = true };
    }
}
