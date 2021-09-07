using k8config.DataModels;
using k8config.Utilities;
using k8s;
using System.Linq;
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
            //Application.UseSystemConsole = true;
            Application.Init();
            AssemblySubsystem.BuildAvailableAssemblyList();

            SetupTopLevelView();
            InteractiveYAMLMode();
            RealTimeMode();
            SetupKeyEvents();

            updateAvailableKindsList();

            //run the GUI
            Application.Run(topLevelWindowObject);

        }



        static void repositionCommandInput()
        {
            commandPromptLabel.Text = string.Join(":", GlobalVariables.promptArray) + ">";
            commandPromptLabel.Width = commandPromptLabel.Text.Count() + 1;
            commandPromptTextField.X = Pos.Right(commandPromptLabel);
        }

        static void AddToPrompt(string _promptString)
        {
            GlobalVariables.promptArray.Add(_promptString);
            repositionCommandInput();
            UpdateDescriptionView();
        }
        static void UpdateDescriptionView(string _nestedObject = null)
        {
            DescriptionType descriptionObject = kubeHelp.getCurrentObjectHelp(_nestedObject);
            if (descriptionObject.description != null)
                descriptionView.Text = descriptionObject?.description;
        }
    }
}
