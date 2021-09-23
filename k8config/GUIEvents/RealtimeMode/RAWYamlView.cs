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
            rawYAMLList = new List<string>();
            rawYAMLListView.SetSourceAsync(Yaml.YAMLSerializer.Serialize(currentSelected).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList());
            rawYAMLWindow.Title = $"{((IKubernetesObject<V1ObjectMeta>)currentSelected).Name()} Yaml View"; 
            rawYAMLWindow.Width = realtimeModeWindow.Bounds.Width - 5;
            rawYAMLWindow.Height = realtimeModeWindow.Bounds.Height - 5;
            rawYAMLWindow.Visible = true;

        }
    }
}
