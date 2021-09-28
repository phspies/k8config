using k8config.DataModels;
using k8config.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace k8config
{
    partial class Program
    {
        public static void UpdateAvailableOptions()
        {
            List<String> outputList = new List<string>();
            if (GlobalVariables.promptArray.Count() == 1)
            { 
                GlobalVariables.sessionDefinedKinds.ForEach(x =>
                {
                    if (x.KubeObject.GetNestedPropertyValue("Metadata.Name") != null)
                    {
                        outputList.Add($"[{x.index}] {x.KubeObject.GetNestedPropertyValue("Metadata.Name")} ({x.kind})");
                    }
                    else if (x.KubeObject.GetDataNamePropValue("Name") != null)
                    {
                        outputList.Add($"[{x.index}] {x.KubeObject.GetDataNamePropValue("Name")} ({x.kind})");
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
                        outputList.Add($"[{x.index}] {x.name} ({x.displayType})");
                    }
                    else
                    {
                        outputList.Add($"[{x.index}] {x.displayType}");
                    }
                });
            }
            if (outputList.Count == 0)
            {
                outputList.Add("empty");
                definedYAMLWindow.Title = "No objects available to choose from";
            }
            else
            {
                definedYAMLWindow.Title = "Available objects to choose from";
            }    
            definedYAMLListView.SetSourceAsync(outputList);
            
        }
    }
}
