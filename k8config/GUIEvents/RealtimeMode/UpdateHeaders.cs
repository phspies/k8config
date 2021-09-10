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
            namespacesTab.Text = $" Namespaces ({namespaceListView.Table.Rows.Count}) ";
            podsTab.Text = $" Pods ({podsListView.Table.Rows.Count}) ";
            servicesTab.Text = $" Services ({servicesListView.Table.Rows.Count}) ";
            deploymentsTab.Text = $" Deployments ({deploymentsListView.Table.Rows.Count}) ";
            replicasetsTab.Text = $" Replica Sets ({replicasetsListView.Table.Rows.Count}) ";
            eventsTab.Text = $" Events ({eventsListView.Table.Rows.Count}) ";
            updateMessageBar($"Connected to {selectedContext}");
        }
    }
}
