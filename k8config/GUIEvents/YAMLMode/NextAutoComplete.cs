using k8config.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace k8config
{
    partial class  Program
    {
        static string NextAutoComplete(List<string> _availableOptions)
        {
            string availableOption = "";
            if (_availableOptions.Count() > 0)
            {
                if (GlobalVariables.autoCompleteInterruptIndex > _availableOptions.Count - 1)
                {
                    GlobalVariables.autoCompleteInterruptIndex = 0;
                }
                availableOption = _availableOptions[GlobalVariables.autoCompleteInterruptIndex];
                GlobalVariables.autoCompleteInterruptIndex++;
            }
            return availableOption;
        }
    }
}
