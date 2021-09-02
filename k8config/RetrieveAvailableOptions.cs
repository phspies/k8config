using k8config.DataModels;
using k8config.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config
{
    partial class Program
    {
        static Tuple<string, List<OptionsSlimType>> retrieveAvailableOptions(bool includeCommands = true, bool autocompleteInAction = false, string searchValue = "")
        {
            List<OptionsSlimType> tmpAvailableOptions = new List<OptionsSlimType>();
            string returnHeader = "";
            object currentObject = new object();
            if (GlobalVariables.promptArray.Count() == 1 && includeCommands)
            {
                returnHeader = "Available Commands";
                if (GlobalVariables.sessionDefinedKinds.Count() == 0)
                {
                    tmpAvailableOptions = new List<OptionsSlimType>() {
                            new OptionsSlimType() { name = "new" },
                            new OptionsSlimType() { name = "exit" }
                        };
                }
                else
                {
                    tmpAvailableOptions = new List<OptionsSlimType>() {
                            new OptionsSlimType() { name = "new",propertyIsCommand = true, displayType = "create new object" },
                            new OptionsSlimType() { name = "delete",propertyIsCommand = true, displayType = "delete object" },
                            new OptionsSlimType() { name = "select",propertyIsCommand = true, displayType = "select object" },
                            new OptionsSlimType() { name = "list" ,propertyIsCommand = true, displayType = "list current objects" },
                            new OptionsSlimType() { name = "exit",propertyIsCommand = true, displayType = "exit" },
                        };
                    UpdateAvailableOptions();

                }
            }
            if (GlobalVariables.promptArray.Count() == 2)
            {
                returnHeader = "Available Options";
                if (includeCommands)
                {
                    tmpAvailableOptions = new List<OptionsSlimType>() {
                            new OptionsSlimType() { name = "..", propertyIsCommand = true, displayType = "back one folder" },
                            new OptionsSlimType() { name = "/" , propertyIsCommand = true, displayType = "back to root"}
                        };
                }
                tmpAvailableOptions.AddRange(GlobalVariables.sessionDefinedKinds[int.Parse(GlobalVariables.promptArray[1]) - 1].KubeObject.RetrieveAttributeValues().Select(x => new OptionsSlimType() { name = x.name.ToLower() }).ToList());
            }
            else if (GlobalVariables.promptArray.Count() > 2)
            {
                object tmpObject = KubeObject.GetCurrentObject();
                if (includeCommands)
                {
                    tmpAvailableOptions = new List<OptionsSlimType>() {
                            new OptionsSlimType() { name = "..", propertyIsCommand = true, displayType = "back one folder" },
                            new OptionsSlimType() { name = "/" , propertyIsCommand = true, displayType = "back to root"}
                        };
                }
                if (tmpObject.IsList())
                {
                    tmpAvailableOptions = new List<OptionsSlimType>() {
                            new OptionsSlimType() { name = "..", propertyIsCommand = true, displayType = "back one folder" },
                            new OptionsSlimType() { name = "/" , propertyIsCommand = true, displayType = "back to root"},
                            new OptionsSlimType() { name = "new",propertyIsCommand = true, displayType = "create new object" },
                            new OptionsSlimType() { name = "delete",propertyIsCommand = true, displayType = "delete object" },
                            new OptionsSlimType() { name = "select",propertyIsCommand = true, displayType = "select object" },
                            new OptionsSlimType() { name = "list" ,propertyIsCommand = true, displayType = "list current objects" }
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
                if (GlobalVariables.promptArray.Count() == 1 && commandPromptTextField.Text.StartsWith("new"))
                {
                    returnHeader = "Available Kinds";
                    if (String.IsNullOrWhiteSpace(searchValue))
                    {
                        string[] args = commandPromptTextField.Text.ToString().Split(" ");
                        if (args.Count() > 1 && !String.IsNullOrWhiteSpace(args[1]))
                        {
                            searchValue = args[1].ToString();
                        }
                    }
                    tmpAvailableOptions = GlobalVariables.availableKubeTypes.Select(x => new OptionsSlimType() { name = x.kind }).Where(x => x.name.StartsWith(searchValue)).ToList();


                }
                if (commandPromptTextField.Text.StartsWith("select") || commandPromptTextField.Text.StartsWith("delete"))
                {
                    if (GlobalVariables.promptArray.Count() == 1)
                    {
                        returnHeader = "Available Defined Kinds";
                        tmpAvailableOptions = GlobalVariables.sessionDefinedKinds.Select(x => new OptionsSlimType() { name = x.kind, index = x.index }).Where(x => x.name.StartsWith(searchValue)).ToList();

                    }
                }
            }
            return Tuple.Create(returnHeader, tmpAvailableOptions);
        }

    }
}
