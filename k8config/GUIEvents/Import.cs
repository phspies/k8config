using k8config.DataModels;
using k8config.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        static public void Import()
        {
            var d = new OpenDialog("Open", "Open a YMAL file", new List<string>() { "yaml" }) { AllowsMultipleSelection = false };
            Application.Run(d);

            if (!d.Canceled)
            {
                try
                {
                    YAMLHandeling.DeserializeFile(d.FilePath.ToString());
                    UpdateMessageBar($"YAML file loaded with {GlobalVariables.sessionDefinedKinds.Count()} definitions");
                }
                catch (Exception ex)
                {
                    UpdateMessageBar($"Error loading YAML file {d.FilePath} - {ex.Message}");
                }
            }
        }
    }
}
