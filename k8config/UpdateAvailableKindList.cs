using k8config.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace k8config
{
    partial class Program
    {
        static void updateAvailableKindsList()
        {
            if (currentavailableListUpDown)
            {
                currentavailableListUpDown = false;
            }
            else
            {
                Tuple<string, List<OptionsSlimType>> returnValues = retrieveAvailableOptions();
                availableKindsWindow.Title = returnValues.Item1;
                availableKindsListView.SetSource(returnValues.Item2.Select(x => x.TableView()).ToList());
                if (!string.IsNullOrWhiteSpace(commandPromptTextField.Text.ToString()))
                {
                    var currentListObect = ((List<String>)availableKindsListView.Source.ToList()).Find(x => x.StartsWith(commandPromptTextField.Text.ToString()));
                    if (!string.IsNullOrWhiteSpace(currentListObect))
                    {
                        availableKindsListView.SelectedItem = availableKindsListView.Source.ToList().IndexOf(currentListObect);
                    }
                };
            }
        }
    }
}
