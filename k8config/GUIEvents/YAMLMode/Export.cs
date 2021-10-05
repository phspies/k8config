using k8config.Utilities;
using System;
using System.Collections.Generic;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        public static void Export()
        {
            var d = new SaveDialog("Save", "Save to YAML file", new List<string>() { "yaml" });
            Application.Run(d);
            if (!d.Canceled)
            {
                try
                {
                    YAMLHandeling.SerializeToFile(d.FilePath.ToString());
                    UpdateMessageBar($"YAML file writen to {d.FilePath}");
                }
                catch (Exception ex)
                {
                    UpdateMessageBar($"Error writing YAML to file {d.FilePath} - {ex.Message}");
                }
            }
        }
    }
}
