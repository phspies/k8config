using k8config.DataModels;
using k8s;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        static public void RAWYamlView(object currentSelected)
        {
            RealtimeModeControls.rawYAMLList = new List<string>();
            RealtimeModeControls.rawYAMLList = Yaml.YAMLSerializer.Serialize(currentSelected).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            RealtimeModeControls.rawYAMLList.ForEach(line => line.Replace("{}","empty"));
            RealtimeModeControls.rawYAMLListView.SetSourceAsync(RealtimeModeControls.rawYAMLList);
            RealtimeModeControls.rawYAMLWindow.Title = $"{((IKubernetesObject<V1ObjectMeta>)currentSelected).Name()} Yaml View";
            RealtimeModeControls.rawYAMLWindow.Width = RealtimeModeControls.realtimeModeWindow.Bounds.Width - 5;
            RealtimeModeControls.rawYAMLWindow.Height = RealtimeModeControls.realtimeModeWindow.Bounds.Height - 5;
            RealtimeModeControls.rawYAMLWindow.Visible = true;
            ;
        }
    }
}
