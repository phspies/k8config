using k8config.GUIEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config
{
    partial class Program
    {
        static void AddToPrompt(string promptString)
        {
            YAMLModePromptObject.EnterFolder(promptString);
            repositionCommandInput();
            UpdateDescriptionView();
        }
    }
}
