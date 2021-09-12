using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config
{
    partial class  Program
    {
        static string NextAutoComplete(List<string> _availableOptions)
        {
            string availableOption = "";
            if (_availableOptions.Count() > 0)
            {
                if (autoCompleteInterruptIndex > _availableOptions.Count - 1)
                {
                    autoCompleteInterruptIndex = 0;
                }
                availableOption = _availableOptions[autoCompleteInterruptIndex];
                autoCompleteInterruptIndex++;
            }
            return availableOption;
        }
    }
}
