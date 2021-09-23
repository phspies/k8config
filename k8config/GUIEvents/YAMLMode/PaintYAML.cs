using k8config.DataModels;
using k8s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace k8config
{
    partial class Program
    {
        static void paintYAML()
        {
            definedYAMLWindow.Text = "";
            if (GlobalVariables.promptArray.Count() > 1)
            {
                object currentSelectedKind = GlobalVariables.sessionDefinedKinds.FirstOrDefault(x => x.index == int.Parse(GlobalVariables.promptArray[1])).KubeObject;
                definedYAMLListView.SetSourceAsync(Yaml.YAMLSerializer.Serialize(currentSelectedKind).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList());
                definedYAMLWindow.Title = $"{GlobalVariables.sessionDefinedKinds.Find(x => x.index == int.Parse(GlobalVariables.promptArray[1])).kind} YAML";
            }
            else
            {
                definedYAMLListView.SetSourceAsync(new List<string>());
            }
            if (GlobalVariables.promptArray.Count > 2)
            {
                string startObject = ((List<string>)definedYAMLListView.Source.ToList()).Find(x => x.Contains(GlobalVariables.promptArray[2]));
                int startIndex = definedYAMLListView.Source.ToList().IndexOf(startObject);
                for (int x = 2; x > (GlobalVariables.promptArray.Count-1); x++)
                {
                    if (int.TryParse(GlobalVariables.promptArray[x], out _))
                    {
                        continue;
                    }
                }
            }
            var currentListObect = ((List<String>)definedYAMLListView.Source.ToList()).Find(x => x.Contains(GlobalVariables.promptArray.Last()));
            if (!string.IsNullOrWhiteSpace(currentListObect))
            {
                definedYAMLListView.SelectedItem = definedYAMLListView.Source.ToList().IndexOf(currentListObect);
            }
            else
            {
                currentListObect = ((List<String>)definedYAMLListView.Source.ToList()).Find(x => x.Contains(GlobalVariables.promptArray[GlobalVariables.promptArray.Count - 2]));
                if (!string.IsNullOrWhiteSpace(currentListObect))
                {
                    definedYAMLListView.SelectedItem = definedYAMLListView.Source.ToList().IndexOf(currentListObect);
                }
            }
        }
    }
}
