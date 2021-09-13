namespace k8config
{
    using k8config.GUIEvents.RealtimeMode.DataModels;
    using k8config.GUIEvents.RealtimeMode.DataTables;
    using k8s;
    using k8s.Models;
    using Microsoft.Rest;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Terminal.Gui;

    /// <summary>
    /// Defines the <see cref="Program" />.
    /// </summary>
    internal partial class Program
    {
        /// <summary>
        /// Defines the k8Client.
        /// </summary>
        internal static Kubernetes k8Client;

        /// <summary>
        /// Defines the selectedContext.
        /// </summary>
        internal static string selectedContext;



        /// <summary>
        /// The realtimeModeKeyEvents.
        /// </summary>
        public static void realtimeModeKeyEvents()
        {

            availableContextsListView.KeyUp += (e) =>
            {
                if (e.KeyEvent.Key == Key.Enter)
                {
                    selectedContext = ((List<string>)availableContextsListView.Source.ToList())[availableContextsListView.SelectedItem];
                    StartWatchersTasks();
                }
            };
        }

     
    }
}
