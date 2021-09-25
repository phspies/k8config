using k8config.DataModels;
using k8s;
using System;
using System.Collections.Generic;
using System.Linq;

namespace k8config
{
    partial class Program
    {
        static void paintYAML()
        {
            List<string> yamlList = new List<string>();
            definedYAMLWindow.Text = "";
            if (GlobalVariables.promptArray.Count() > 1)
            {
                object currentSelectedKind = GlobalVariables.sessionDefinedKinds.FirstOrDefault(x => x.index == int.Parse(GlobalVariables.promptArray[1])).KubeObject;
                yamlList = Yaml.YAMLSerializer.Serialize(currentSelectedKind).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
                if (string.IsNullOrWhiteSpace(yamlList.Last()))
                {
                    yamlList.Remove(yamlList.Last());
                }
                definedYAMLListView.SetSource(yamlList);
                definedYAMLWindow.Title = $"{GlobalVariables.sessionDefinedKinds.Find(x => x.index == int.Parse(GlobalVariables.promptArray[1])).kind} YAML";
            }
            else
            {
                definedYAMLListView.SetSourceAsync(new List<string>());
            }
            if (GlobalVariables.promptArray.Count > 2)
            {
                string startObject = yamlList.Find(x => x.Contains(GlobalVariables.promptArray[2]));
                int currentIndex = yamlList.IndexOf(startObject) < 1 ? 1 : yamlList.IndexOf(startObject);
                string[] yamlArray = yamlList.ToArray();
                List<int> indexArray = new List<int>();
                currentIndex--;
                for (int x = 2; x < (GlobalVariables.promptArray.Count); x++)
                {
                    int subindex;
                    if (int.TryParse(GlobalVariables.promptArray[x], out subindex))
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
                                if (currentIndex > yamlArray.Length-1)
                                {
                                    currentIndex = fallbackIndex;
                                    break;
                                }
                                if (x == 2 && yamlArray[currentIndex].ToLower().StartsWith($"{GlobalVariables.promptArray[x].ToLower()}:"))
                                {
                                    fallbackIndex = currentIndex;
                                    break;
                                }
                                else if (x != 2 && yamlArray[currentIndex].ToLower().Contains(GlobalVariables.promptArray[x].ToLower()))
                                {
                                    fallbackIndex = currentIndex;
                                    break;
                                }
                                
                            }
                        }
                    }
                    definedYAMLListView.SelectedItem = currentIndex;
                    if (currentIndex < 8)
                    {
                        definedYAMLListView.TopItem = 0;
                    }
                    else
                    {
                        definedYAMLListView.TopItem = currentIndex - 5;
                    }

                    if (currentIndex == yamlArray.Length)
                    {
                        break;
                    }
                }
            }
        }
    }
}
