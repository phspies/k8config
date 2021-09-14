using k8config.DataModels;
using k8config.Utilities;
using k8s;
using k8s.Models;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        static string autoCompleteInterruptText = "";
        static int autoCompleteInterruptIndex = 0;
        static bool currentavailableListUpDown = false;

        static void Main(string[] args)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Application.UseSystemConsole = true;
            }
            Application.Init();
            AssemblySubsystem.BuildAvailableAssemblyList();

            SetupTopLevelView();
            YAMLMode();
            RealTimeMode();
            YamlModeKeyEvents();

            updateAvailableKindsList();

            //run the GUI
            Application.Run(topLevelWindowObject);

        }



        static void repositionCommandInput()
        {
            commandPromptLabel.Text = string.Join("/", GlobalVariables.promptArray) + ">";
            commandPromptLabel.Width = commandPromptLabel.Text.Count() + 1;
            commandPromptTextField.X = Pos.Right(commandPromptLabel);
        }

        static void AddToPrompt(string _promptString)
        {
            GlobalVariables.promptArray.Add(_promptString);
            repositionCommandInput();
            UpdateDescriptionView();
        }
    }
}
