using k8config.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace k8config
{
    partial class Program
    {
        static List<OptionsSlimType> currentAvailableOptions = new List<OptionsSlimType>();
        static void updateAvailableKindsList()
        {
            if (GlobalVariables.currentavailableListUpDown)
            {
                GlobalVariables.currentavailableListUpDown = false;
            }
            else
            {
                Tuple<string, List<OptionsSlimType>> returnValues = retrieveAvailableOptions();
                YAMLModelControls.availableKindsWindow.Title = returnValues.Item1;
                currentAvailableOptions = returnValues.Item2;
                YAMLModelControls.availableKindsListView.SetSource(returnValues.Item2.Select(x => x.TableView).ToList());
                if (!string.IsNullOrWhiteSpace(YAMLModelControls.commandPromptTextField.Text.ToString()))
                {
                    var currentListObect = ((List<String>)YAMLModelControls.availableKindsListView.Source.ToList()).Find(x => x.StartsWith(YAMLModelControls.commandPromptTextField.Text.ToString()));
                    if (!string.IsNullOrWhiteSpace(currentListObect))
                    {
                        YAMLModelControls.availableKindsListView.SelectedItem = YAMLModelControls.availableKindsListView.Source.ToList().IndexOf(currentListObect);
                    }
                };
            }
        }
    }
}
