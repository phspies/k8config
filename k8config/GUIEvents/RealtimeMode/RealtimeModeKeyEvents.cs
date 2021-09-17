using k8s;
using System.Collections.Generic;
using Terminal.Gui;

namespace k8config
{
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
