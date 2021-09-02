using k8config.DataModels;
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
                var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull).Build();
                definedYAMLListView.SetSourceAsync(serializer.Serialize(currentSelectedKind).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList());
                definedYAMLWindow.Title = $"{GlobalVariables.sessionDefinedKinds.Find(x => x.index == int.Parse(GlobalVariables.promptArray[1])).kind} YAML";
            }
            else
            {
                definedYAMLListView.SetSourceAsync(new List<string>());
            }
        }
    }
}
