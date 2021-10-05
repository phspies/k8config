using k8config.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config
{
    partial class Program
    {
        public static void UpdateTabHeaders()
        {
            RealtimeModeControls.namespacesTab.Text = $" Namespaces ({RealtimeModeControls.namespacesList.Dictionary.Count}) ";
            RealtimeModeControls.podsTab.Text = $" Pods ({RealtimeModeControls.podsList.Dictionary.Count}) ";
            RealtimeModeControls.servicesTab.Text = $" Services ({RealtimeModeControls.servicesList.Dictionary.Count}) ";
            RealtimeModeControls.deploymentsTab.Text = $" Deployments ({RealtimeModeControls.deploymentsList.Dictionary.Count}) ";
            RealtimeModeControls.replicasetsTab.Text = $" Replica Sets ({RealtimeModeControls.replicasetsList.Dictionary.Count}) ";
            RealtimeModeControls.eventsTab.Text = $" Events ({RealtimeModeControls.eventsList.DataTableConstruct.Rows.Count}) ";
        }
    }
}
