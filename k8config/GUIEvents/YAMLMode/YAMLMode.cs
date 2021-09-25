using k8config.DataModels;
using k8config.GUIEvents.YAMLMode;
using k8s.Models;
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
        static Window YAMLModeWindow = new Window()
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
        static List<string> sessionHistory = new List<string>();
        static int sessionHistoryIndex = 0;
        static ListView availableKindsListView = new ListView();
        static Label commandPromptLabel = new Label();
        static Window commandWindow = new Window();
        static TextField commandPromptTextField = new TextField("");
        static Window definedYAMLWindow = new Window();
        static Window availableKindsWindow = new Window();
        static ListView definedYAMLListView = new ListView();
        static Window descriptionWindow = new Window();
        static TextView descriptionView = new TextView();
        static Button closeValidationWindowButton = new Button("Close", true);
        static Dialog validationWindow = new Dialog("Validation Status", 80, 17, closeValidationWindowButton)
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            Visible = false,
        };
        static StatusItem[] interactiveStatusBarItems = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
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

                new StatusItem(Key.F10, "~F10~ Interactive Mode", () => { ToggleDisplayMode(); }),
               new StatusItem(Key.CharMask, "No Defintions Found", null, true, new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray))

            };

        static public void YAMLMode()
        {

            statusBar.Items = interactiveStatusBarItems;
            availableKindsWindow = new Window()
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
            descriptionWindow = new Window()
            {
                Title = "Description",
                X = 0,
                Y = availableKindsWindow.Bounds.Bottom,
                Width = Dim.Fill(),
                Height = 5,
                TabStop = false,
                CanFocus = false,
                ColorScheme = colorNormal,
            };
            descriptionView = new TextView()
            {

                Y = 0,
                X = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                Text = "No definitions found. Please create a definition with the 'new <kind>' command.",
                ColorScheme = colorNormal,
                ReadOnly = true,
                WordWrap = true,
                TabStop = false,
                CanFocus = false,
                AllowsTab = false,
                Multiline = true,

            };


            definedYAMLWindow = new Window()
            {
                Title = "Selected Definition",
                X = availableKindsWindow.Bounds.Right,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 9,
                TabStop = false,
                CanFocus = false,
                ColorScheme = colorNormal

            };
            commandWindow = new Window()
            {
                Title = "Command Line",
                Y = descriptionWindow.Bounds.Bottom,
                Width = Dim.Fill(),
                Height = 3,
                ColorScheme = colorNormal,
                TabStop = false,
                CanFocus = false,
            };


            commandPromptLabel = new Label(string.Join(":", GlobalVariables.promptArray) + ">") { TextAlignment = TextAlignment.Left, X = 1, TabStop = false };
            commandPromptTextField = new TextField("") { X = Pos.Right(commandPromptLabel) + 1, Width = Dim.Fill(), TabStop = true };

            definedYAMLListView = new ListView()
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
            definedYAMLWindow.Add(definedYAMLListView);
            availableKindsListView = new ListView()
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

            availableKindsWindow.Add(availableKindsListView);
            YAMLModeWindow.Add(commandWindow, definedYAMLWindow);
            YAMLModeWindow.Add(availableKindsWindow);
            //commandWindow.Add(messageBarItem);
            descriptionWindow.Add(descriptionView);
            YAMLModeWindow.Add(descriptionWindow);
            commandWindow.Add(commandPromptLabel, commandPromptTextField);
            topLevelWindowObject.Add(YAMLModeWindow);
            YAMLModeWindow.Add(validationWindow);
            closeValidationWindowButton.Clicked += () =>
            {
                validationWindow.Visible = false;
                commandPromptTextField.SetFocus();
            };

            ToggleDisplayMode();

        }
    }
}
