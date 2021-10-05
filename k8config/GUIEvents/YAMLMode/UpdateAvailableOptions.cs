using k8config.DataModels;
using k8config.GUIEvents;
using k8config.Utilities;
using System;
using System.Collections.Generic;

namespace k8config
{
    partial class Program
    {
        public static void UpdateAvailableOptions()
        {
            List<string> outputList = new List<string>();
            if (YAMLModePromptObject.CurrentPromptPositionIsRoot)
            { 
                GlobalVariables.sessionDefinedKinds.ForEach(x =>
                {
                    if (x.KubeObject.GetNestedPropertyValue("Metadata.Name") != null)
                    {
                        outputList.Add($"[{x.index}] {x.KubeObject.GetNestedPropertyValue("Metadata.Name")} ({x.displayKind})");
                    }
                    else if (x.KubeObject.GetDataNamePropValue("Name") != null)
                    {
                        outputList.Add($"[{x.index}] {x.KubeObject.GetDataNamePropValue("Name")} ({x.displayKind})");
                    }
                    else
                    {
                        outputList.Add($"[{x.index}] {x.kind}");
                    }
                });
            }
            else if (KubeObject.GetCurrentObject().IsList())
            {
                KubeObject.GetNestedList(KubeObject.GetCurrentObject()).ForEach(x =>
                {
                    if (!String.IsNullOrWhiteSpace(x.name))
                    {
                        outputList.Add($"[{x.index}] {x.name} ({x.sanatizedDisplayType})");
                    }
                    else
                    {
                        outputList.Add($"[{x.index}] {x.sanatizedDisplayType}");
                    }
                });
            }
            if (outputList.Count == 0)
            {
                outputList.Add("empty");
                YAMLModelControls.definedYAMLWindow.Title = "No objects available to choose from";
            }
            else
            {
                YAMLModelControls.definedYAMLWindow.Title = "Available objects to choose from";
            }
            YAMLModelControls.definedYAMLListView.SetSourceAsync(outputList);
            
        }
    }
}
