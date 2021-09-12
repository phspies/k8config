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
            namespacesTab.Text = $" Namespaces ({namespaceTableView.Table.Rows.Count}) ";
            podsTab.Text = $" Pods ({podsTableView.Table.Rows.Count}) ";
            servicesTab.Text = $" Services ({servicesTableView.Table.Rows.Count}) ";
            deploymentsTab.Text = $" Deployments ({deploymentsTableView.Table.Rows.Count}) ";
            replicasetsTab.Text = $" Replica Sets ({replicasetsTableView.Table.Rows.Count}) ";
            eventsTab.Text = $" Events ({eventsTableView.Table.Rows.Count}) ";

            //move table down to the latest entry
            //eventsTableView.Move(0, eventsTable.Rows.Count - 1);

            //update message bar
            UpdateMessageBar($"Connected to {selectedContext}");
        }
    }
}
