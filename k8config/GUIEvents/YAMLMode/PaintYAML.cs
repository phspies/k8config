using k8config.DataModels;
using k8config.GUIEvents;
using k8config.GUIEvents.YAMLMode;
using k8s.Models;
using System.Collections.Generic;
using System.Linq;

namespace k8config
{
    partial class Program
    {
        static void paintYAML()
        {
            List<string> yamlList = new List<string>();
            YAMLModelControls.definedYAMLWindow.Text = "";
            if (YAMLModePromptObject.CurrentPromptPositionIsNotRoot)
            {
                object currentSelectedKind = GlobalVariables.sessionDefinedKinds.FirstOrDefault(x => x.index == int.Parse(YAMLModePromptObject.GetFolderAt(1))).KubeObject;
                yamlList = YAMLOperations.SerializeObjectToList(currentSelectedKind);
                YAMLModelControls.definedYAMLListView.SetSource(yamlList);
                var kubeObject = GlobalVariables.sessionDefinedKinds.Find(x => x.index == int.Parse(YAMLModePromptObject.GetFolderAt(1)));
                YAMLModelControls.definedYAMLWindow.Title = $"{kubeObject.metaData.Name()} ({kubeObject.metaData.Kind}) YAML";
            }
            else
            {
                YAMLModelControls.definedYAMLListView.SetSource(new List<string>() { "No definition defined" });
                YAMLModelControls.definedYAMLWindow.Title = "";
            }

            //move select  bar to current selected prompt object 
            if (YAMLModePromptObject.CurrentPromptPositionIsNotRoot)
            {
                string startObject = yamlList.Find(x => x.Contains(YAMLModePromptObject.GetFolderAt(2)));
                int currentIndex = yamlList.IndexOf(startObject) < 1 ? 1 : yamlList.IndexOf(startObject);
                string[] yamlArray = yamlList.ToArray();
                List<int> indexArray = new List<int>();
                currentIndex--;
                for (int x = 2; x < YAMLModePromptObject.Count; x++)
                {
                    int subindex;
                    if (int.TryParse(YAMLModePromptObject.GetFolderAt(x), out subindex))
                    {
                        indexArray.Add(subindex);
                        continue;
                    }
                    else
                    {
                        int fallbackIndex = 0;
                        int i = 0;
                        while (i != (indexArray.Sum(x => x) + 1))
                        {
                            i++;
                            while (true)
                            {
                                currentIndex += 1;
                                if (currentIndex > yamlArray.Length - 1)
                                {
                                    currentIndex = fallbackIndex;
                                    break;
                                }
                                if (x == 2 && yamlArray[currentIndex].ToLower().StartsWith($"{YAMLModePromptObject.GetFolderAt(x).ToLower()}:"))
                                {
                                    fallbackIndex = currentIndex;
                                    break;
                                }
                                else if (x != 2 && yamlArray[currentIndex].ToLower().Contains(YAMLModePromptObject.GetFolderAt(x).ToLower()))
                                {
                                    fallbackIndex = currentIndex;
                                    break;
                                }
                            }
                        }
                    }
                    YAMLModelControls.definedYAMLListView.SelectedItem = currentIndex;
                    YAMLModelControls.definedYAMLListView.TopItem = currentIndex < 8 ? 0 : currentIndex - 5;
                    if (currentIndex == yamlArray.Length)
                    {
                        break;
                    }
                }
            }
        }
    }
}
