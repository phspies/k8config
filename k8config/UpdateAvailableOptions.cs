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
                    outputList.Add($"[{x.index}] {x.kind}");
                });
            }
            else if (KubeObject.GetCurrentObject().IsList())
            {
                KubeObject.GetNestedList(KubeObject.GetCurrentObject()).ForEach(x =>
                {
                    outputList.Add($"[{x.index}] {x.name}");
                });
            }
            definedYAMLListView.SetSourceAsync(outputList);
            definedYAMLWindow.Title = "Available kinds to choose from";
        }
    }
}
