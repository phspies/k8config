using k8s;
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
        static KubernetesClientConfiguration k8config;
        public static void realtimeModeKeyEvents()
        {

            availableContextsListView.KeyUp += (e) =>
            {
                if (e.KeyEvent.Key == Key.Enter)
                {
                    var selectedContext = ((List<string>)availableContextsListView.Source.ToList())[availableContextsListView.SelectedItem];
                    k8config = KubernetesClientConfiguration.BuildConfigFromConfigObject(KubernetesClientConfiguration.LoadKubeConfig(), selectedContext);
                    updateMessageBar($"{k8config.CurrentContext} selected");

                }
            };
        }
    }
}
