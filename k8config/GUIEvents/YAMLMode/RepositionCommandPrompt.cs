using k8config.DataModels;
using k8config.GUIEvents;
using System.Linq;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        static void repositionCommandInput()
        {
            YAMLModelControls.commandPromptLabel.Text = YAMLModePromptObject.PromptConstructor;
            YAMLModelControls.commandPromptLabel.Width = YAMLModelControls.commandPromptLabel.Text.Count() + 1;
            YAMLModelControls.commandPromptTextField.X = Pos.Right(YAMLModelControls.commandPromptLabel);
        }
    }
}
