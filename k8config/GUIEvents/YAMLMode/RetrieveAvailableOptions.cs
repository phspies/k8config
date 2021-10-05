using k8config.DataModels;
using k8config.GUIEvents;
using k8config.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace k8config
{
    partial class Program
    {
        static Tuple<string, List<OptionsSlimType>> retrieveAvailableOptions(bool includeCommands = true, bool autocompleteInAction = false, string searchValue = "")
        {
            List<OptionsSlimType> tmpAvailableOptions = new List<OptionsSlimType>();
            string returnHeader = "";
            object currentObject = new object();
            if (YAMLModePromptObject.CurrentPromptPositionIsRoot && includeCommands)
            {
                returnHeader = "Available Commands";
                if (GlobalVariables.sessionDefinedKinds.Count() == 0)
                {
                    tmpAvailableOptions = new List<OptionsSlimType>() {
                            new OptionsSlimType() { name = "new" ,propertyIsCommand = true, displayType = "create new object" },
                            new OptionsSlimType() { name = "exit" ,propertyIsCommand = true, displayType = "exit" },
                            new OptionsSlimType() { name = "import" ,propertyIsCommand = true, displayType = "Import YAML file" },
                            new OptionsSlimType() { name = "export" ,propertyIsCommand = true, displayType = "Export to YAML file" },
                        };
                }
                else
                {
                    tmpAvailableOptions = new List<OptionsSlimType>() {
                            new OptionsSlimType() { name = "new",propertyIsCommand = true, displayType = "create new object" },
                            new OptionsSlimType() { name = "select",propertyIsCommand = true, displayType = "select object" },
                            new OptionsSlimType() { name = "no",propertyIsCommand = true, displayType = "delete attribute" },
                            new OptionsSlimType() { name = "import" ,propertyIsCommand = true, displayType = "Import YAML file" },
                            new OptionsSlimType() { name = "export" ,propertyIsCommand = true, displayType = "Export to YAML file" },
                            new OptionsSlimType() { name = "exit",propertyIsCommand = true, displayType = "exit" },
                        };
                    UpdateAvailableOptions();

                }
            }
            if (YAMLModePromptObject.Count == 2)
            {
                returnHeader = "Available Commands/Options";
                if (includeCommands)
                {
                    tmpAvailableOptions = new List<OptionsSlimType>() {
                            new OptionsSlimType() { name = "..", propertyIsCommand = true, displayType = "back one folder" },
                            new OptionsSlimType() { name = "/" , propertyIsCommand = true, displayType = "back to root"},
                            new OptionsSlimType() { name = "no",propertyIsCommand = true, displayType = "delete attribute" },
                        };
                }
                tmpAvailableOptions.AddRange(GlobalVariables.sessionDefinedKinds.FirstOrDefault(x => x.index == int.Parse(YAMLModePromptObject.GetFolderAt(1))).KubeObject.RetrieveAttributeValues());
            }
            else if (YAMLModePromptObject.Count > 2)
            {
                returnHeader = "Available Commands/Options";
                object tmpObject = KubeObject.GetCurrentObject();
                if (includeCommands)
                {
                    tmpAvailableOptions = new List<OptionsSlimType>() {
                            new OptionsSlimType() { name = "..", propertyIsCommand = true, displayType = "back one folder" },
                            new OptionsSlimType() { name = "/" , propertyIsCommand = true, displayType = "back to root"},
                            new OptionsSlimType() { name = "no" , propertyIsCommand = true, displayType = "deleted defined property"},
                        };
                }
                if (tmpObject.IsList())
                {
                    tmpAvailableOptions = new List<OptionsSlimType>() {
                            new OptionsSlimType() { name = "..", propertyIsCommand = true, displayType = "back one folder" },
                            new OptionsSlimType() { name = "/" , propertyIsCommand = true, displayType = "back to root"},
                            new OptionsSlimType() { name = "new",propertyIsCommand = true, displayType = "create new object" },
                            new OptionsSlimType() { name = "select",propertyIsCommand = true, displayType = "select object" },
                            new OptionsSlimType() { name = "no",propertyIsCommand = true, displayType = "delete attribute" },
                        };
                    UpdateAvailableOptions();

                }
                else
                {
                    tmpAvailableOptions.AddRange(tmpObject.RetrieveAttributeValues());
                }
            }

            if (!autocompleteInAction)
            {
                if (YAMLModePromptObject.CurrentPromptPositionIsRoot && YAMLModelControls.commandPromptTextField.Text.StartsWith("new"))
                {
                    returnHeader = "Available Kinds";
                    if (String.IsNullOrWhiteSpace(searchValue))
                    {
                        string[] args = YAMLModelControls.commandPromptTextField.Text.ToString().Split(" ");
                        if (args.Count() > 1 && !String.IsNullOrWhiteSpace(args[1]))
                        {
                            searchValue = args[1].ToString();
                        }
                    }
                    tmpAvailableOptions = GlobalVariables.availableKubeTypes.Select(x => new OptionsSlimType() { name = x.classKind }).Where(x => x.name.StartsWith(searchValue)).ToList();


                }
            }
            return Tuple.Create(returnHeader, tmpAvailableOptions);
        }
    }
}
