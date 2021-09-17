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
        static void repositionCommandInput()
        {
            commandPromptLabel.Text = string.Join(":", GlobalVariables.promptArray) + ">";
            commandPromptLabel.Width = commandPromptLabel.Text.Count() + 1;
            commandPromptTextField.X = Pos.Right(commandPromptLabel);
        }
    }
}
