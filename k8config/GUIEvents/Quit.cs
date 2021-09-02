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
        static bool Quit()
        {
            int selectedOption = MessageBox.Query(50, 5, "Quit", "Are you sure you want to quit k8config?", "Yes", "No");
            return (selectedOption == 0);

        }
    }
}
