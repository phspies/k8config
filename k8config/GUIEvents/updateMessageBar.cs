using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config
{
    partial class Program
    {
        static void UpdateMessageBar(string _string)
        {
            statusBar.UpdateMessageText(_string);
        }

    }
}
