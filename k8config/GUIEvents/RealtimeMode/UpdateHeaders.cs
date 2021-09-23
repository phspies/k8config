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
            namespacesTab.Text = $" Namespaces ({namespacesList.Dictionary.Count}) ";
            podsTab.Text = $" Pods ({podsList.Dictionary.Count}) ";
            servicesTab.Text = $" Services ({servicesList.Dictionary.Count}) ";
            deploymentsTab.Text = $" Deployments ({deploymentsList.Dictionary.Count}) ";
            replicasetsTab.Text = $" Replica Sets ({replicasetsList.Dictionary.Count}) ";
            eventsTab.Text = $" Events ({eventsList.DataTableConstruct.Rows.Count}) ";

            //move table down to the latest entry
            //eventsTableView.Move(0, eventsTable.Rows.Count - 1);

            //update message bar
            //UpdateMessageBar($"Connected to {selectedContext}");
        }
    }
}
