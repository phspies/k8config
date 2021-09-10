using k8config.DataModels;
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
        static Window InteractiveModeWindow = new Window()
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

        static ListView availableKindsListView = new ListView();
        static Label commandPromptLabel = new Label();
        static Window commandWindow = new Window();
        static TextField commandPromptTextField = new TextField("");
        static Window definedYAMLWindow = new Window();
        static Window availableKindsWindow = new Window();
        static ListView definedYAMLListView = new ListView();
        static Window descriptionWindow = new Window();
        static TextView descriptionView = new TextView();
        //static Label messageBarItem = new Label();
        static KubernetesHelp kubeHelp = new KubernetesHelp();
        static StatusItem[] interactiveStatusBarItems = new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { Environment.Exit(0); }}),
                new StatusItem(Key.F10, "~F10~ Interactive Mode", () => ToggleDisplayMode()),
               new StatusItem (Key.CharMask, "No Defintions Found", null, true, new Terminal.Gui.Attribute(Color.BrightYellow, Color.DarkGray))
            };
        
        static public void InteractiveYAMLMode()
        {
            statusBar = new StatusBar(interactiveStatusBarItems);
            topLevelWindowObject.Add(statusBar);

            availableKindsWindow = new Window()
            {
                Title = "Available Commands",
                X = 0,
                Y = 0,
                Width = 50,
                Height = Dim.Fill() - 9,
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
                Text = "",
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
                Height = Dim.Fill() - 10,
                TabStop = false,
                CanFocus = false,
                ColorScheme = colorNormal

            };
            commandWindow = new Window()
            {
                Title = "Command Line",
                Y = descriptionWindow.Bounds.Bottom,
                Width = Dim.Fill(),
                Height = 4,
                ColorScheme = colorNormal,
                TabStop = false,
                CanFocus = false,
            };


            commandPromptLabel = new Label(string.Join(":", GlobalVariables.promptArray) + ">") { TextAlignment = TextAlignment.Left, X = 1 };
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
            //messageBarItem.ColorScheme = new ColorScheme() { Normal = new Terminal.Gui.Attribute(Color.Red, Color.White) };
            //messageBarItem.Width = Dim.Fill();
            //displayMessageStatusBar("No definitions found";
            //messageBarItem.CanFocus = false;
            //messageBarItem.Y = 1;
            //messageBarItem.X = 1;
            //displayMessageStatusBar("No definitions found";


            InteractiveModeWindow.Add(commandWindow, definedYAMLWindow);
            InteractiveModeWindow.Add(availableKindsWindow);

            //commandWindow.Add(messageBarItem);
            descriptionWindow.Add(descriptionView);
            InteractiveModeWindow.Add(descriptionWindow);
            commandWindow.Add(commandPromptLabel, commandPromptTextField);

            topLevelWindowObject.Add(InteractiveModeWindow);
            topLevelWindowObject.Add(statusBar);
        }
    }
}
