using k8config.DataModels;
using k8config.Utilities;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        static public void YamlModeKeyEvents()
        {
            YAMLModeWindow.KeyUp += (e) =>
            {
                string currentInputText = commandPromptTextField.Text.ToString();
                if (e.KeyEvent.Key == (Key.CtrlMask | Key.C))
                {
                    commandPromptTextField.Text = "";
                    commandPromptTextField.CursorPosition = commandPromptTextField.Text.Length;
                }
                if (e.KeyEvent.Key == (Key.CtrlMask | Key.Z) || e.KeyEvent.Key == (Key.CtrlMask | Key.A) || e.KeyEvent.Key == Key.CursorUp || e.KeyEvent.Key == Key.CursorDown)
                {
                    switch (e.KeyEvent.Key)
                    {
                        case (Key.CursorUp):
                            if (sessionHistory.Count != 0)
                            {
                                if (sessionHistoryIndex != 0)
                                {
                                    sessionHistoryIndex--;
                                }
                                else
                                {
                                    sessionHistoryIndex = sessionHistory.Count - 1;
                                }
                                commandPromptTextField.Text = sessionHistory[sessionHistoryIndex];
                                commandPromptTextField.CursorPosition = commandPromptTextField.Text.Length;
                            }
                            break;
                        case (Key.CursorDown):
                            if (sessionHistory.Count != 0)
                            {
                                if (sessionHistoryIndex < (sessionHistory.Count - 1))
                                {
                                    sessionHistoryIndex++;
                                }
                                else
                                {
                                    sessionHistoryIndex = sessionHistory.Count == 0 ? (sessionHistory.Count - 1) : 0;
                                }
                                commandPromptTextField.Text = sessionHistory[sessionHistoryIndex];
                                commandPromptTextField.CursorPosition = commandPromptTextField.Text.Length;
                            }

                            break;
                        case (Key.CtrlMask | Key.A):
                            currentavailableListUpDown = true;
                            if (availableKindsListView.SelectedItem > 0)
                            {
                                availableKindsListView.SelectedItem = availableKindsListView.SelectedItem - 1;
                                availableKindsListView.TopItem = availableKindsListView.SelectedItem;
                                commandPromptTextField.Text = currentAvailableOptions[availableKindsListView.SelectedItem].name;
                                commandPromptTextField.CursorPosition = commandPromptTextField.Text.Length;
                                UpdateDescriptionView(commandPromptTextField.Text.ToString());
                            }
                            commandPromptTextField.SetFocus();
                            break;
                        case (Key.CtrlMask | Key.Z):
                            currentavailableListUpDown = true;
                            if (availableKindsListView.SelectedItem < availableKindsListView.Source.ToList().Count - 1)
                            {
                                availableKindsListView.SelectedItem = availableKindsListView.SelectedItem + 1;
                                availableKindsListView.TopItem = availableKindsListView.SelectedItem;
                                commandPromptTextField.Text = currentAvailableOptions[availableKindsListView.SelectedItem].name;
                                commandPromptTextField.CursorPosition = commandPromptTextField.Text.Length;
                                UpdateDescriptionView(commandPromptTextField.Text.ToString());
                            }
                            commandPromptTextField.SetFocus();
                            break;
                        default:

                            break;
                    }
                    if (String.IsNullOrWhiteSpace(currentInputText))
                    {
                        autoCompleteInterruptText = "";

                    }
                }

            };
            commandPromptTextField.KeyPress += (e) =>
            {
                string currentInputText = commandPromptTextField.Text.ToString();
                string[] args = currentInputText.Trim().Split();
                List<string> validCompleteCommands = new List<string>() { "no" };
                if (e.KeyEvent.Key == Key.Tab && !string.IsNullOrEmpty(currentInputText))
                {
                    currentavailableListUpDown = false;
                    List<string> possibleOptions = new List<string>();

                    if (GlobalVariables.promptArray.Count() == 1 && args.Count() > 1)
                    {
                        if (String.IsNullOrEmpty(autoCompleteInterruptText)) autoCompleteInterruptText = args[1];
                        possibleOptions = retrieveAvailableOptions(true, false, autoCompleteInterruptText).Item2.Where(x => x.name.StartsWith(autoCompleteInterruptText)).Select(x => x.name).ToList();
                        availableKindsListView.SetSource(possibleOptions.ToList());
                        commandPromptTextField.Text = $"{args[0]} {NextAutoComplete(possibleOptions)}";
                        commandPromptTextField.CursorPosition = commandPromptTextField.Text.Length;
                    }
                    else if (args.Count() == 1 && args[0] != "")
                    {
                        if (String.IsNullOrEmpty(autoCompleteInterruptText)) autoCompleteInterruptText = args[0].Trim();
                        possibleOptions = retrieveAvailableOptions(true, false, autoCompleteInterruptText).Item2.Where(x => x.name.StartsWith(autoCompleteInterruptText)).Select(x => x.name).ToList();
                        availableKindsListView.SetSource(possibleOptions.ToList());
                        commandPromptTextField.Text = $"{NextAutoComplete(possibleOptions)}";
                        commandPromptTextField.CursorPosition = commandPromptTextField.Text.Length;
                    }
                    else if (args.Count() == 2 && args[1] != "" && validCompleteCommands.Exists(x => x == args[0]))
                    {
                        if (String.IsNullOrEmpty(autoCompleteInterruptText)) autoCompleteInterruptText = args[1].Trim();
                        possibleOptions = retrieveAvailableOptions(true, false, autoCompleteInterruptText).Item2.Where(x => x.name.StartsWith(autoCompleteInterruptText)).Select(x => x.name).ToList();
                        availableKindsListView.SetSource(possibleOptions.ToList());
                        commandPromptTextField.Text = $"{args[0]} {NextAutoComplete(possibleOptions)}";
                        commandPromptTextField.CursorPosition = commandPromptTextField.Text.Length;
                    }
                    else
                    {
                        autoCompleteInterruptText = "";
                        possibleOptions = retrieveAvailableOptions(false).Item2.ToList().Select(x => x.TableView()).ToList();
                        availableKindsListView.SetSource(possibleOptions.ToList());

                    }
                    e.Handled = true;
                }
                else
                {
                    autoCompleteInterruptText = "";

                }

            };
            commandPromptTextField.KeyUp += (e) =>
            {
                List<string> allowedDescriptionCommands = new List<string>() { "new", "no" };
                string currentInputText = commandPromptTextField.Text.ToString();
                string[] args = currentInputText.Trim().Split();
                if (e.KeyEvent.Key == Key.Enter)
                {
                    UpdateMessageBar("");
                    if (!string.IsNullOrEmpty(currentInputText))
                    {
                        if (sessionHistory.Count == 0)
                        {
                            sessionHistory.Add(currentInputText);
                        }
                        else if (sessionHistory.Last() != currentInputText)
                        {
                            sessionHistory.Add(currentInputText);
                        }
                        currentavailableListUpDown = false;
                        if (currentInputText == "..")
                        {
                            if (GlobalVariables.promptArray.Count() > 1)
                            {
                                GlobalVariables.promptArray.RemoveAt(GlobalVariables.promptArray.Count - 1);

                                repositionCommandInput();
                                paintYAML();
                            }
                        }
                        else if (currentInputText == "/")
                        {
                            if (GlobalVariables.promptArray.Count() > 1)
                            {
                                GlobalVariables.promptArray = new List<string>() { "yaml" };
                                repositionCommandInput();
                                paintYAML();
                            }
                        }
                        else if (GlobalVariables.sessionDefinedKinds.Count() > 0 && currentInputText == "list")
                        {
                            List<String> outputList = new List<string>();
                            if (GlobalVariables.promptArray.Count() == 1)
                            {

                                GlobalVariables.sessionDefinedKinds.ForEach(x =>
                                {
                                    outputList.Add($"[{x.index}] {x.kind}");
                                });
                            }
                            else if (KubeObject.GetCurrentObject().IsList())
                            {
                                KubeObject.GetNestedList(KubeObject.GetCurrentObject()).ForEach(x =>
                                {
                                    outputList.Add($"[{x.index}] {x.name}");
                                });
                            }
                            else
                            {
                                UpdateMessageBar("The current object is not a list");
                            }
                            definedYAMLListView.SetSourceAsync(outputList);
                        }
                        else if (currentInputText == "new")
                        {
                            //add new object to the List<T> object
                            object currentObject = KubeObject.GetCurrentObject();
                            if (currentObject.IsList())
                            {
                                Type[] selectListTypes = currentObject.GetType().GetTypeInfo().GetGenericArguments();
                                if (selectListTypes.Length == 1)
                                {
                                    if (selectListTypes[0].Name == "String")
                                    {
                                        currentObject.GetType().GetMethod("Add").Invoke(currentObject, new[] { new String("") });
                                    }
                                    else
                                    {
                                        currentObject.GetType().GetMethod("Add").Invoke(currentObject, new[] { Activator.CreateInstance(selectListTypes[0]) });
                                    }
                                    UpdateMessageBar($"{selectListTypes[0].Name.Replace("V1", "")} added to {GlobalVariables.promptArray.Last()}");
                                    AddToPrompt(KubeObject.GetNestedList(currentObject).Last().index.ToString());
                                }
                                repositionCommandInput();
                                paintYAML();
                            }

                        }
                        else
                        {
                            //inline type commands
                            if (args.Length > 1)
                            {
                                if (args[0] == "import")
                                {
                                    if (!string.IsNullOrWhiteSpace(args[1]))
                                    {
                                        try
                                        {
                                            YAMLHandeling.DeserializeFile(args[1]);
                                            updateAvailableKindsList();
                                            UpdateMessageBar($"YAML file loaded with {GlobalVariables.sessionDefinedKinds.Count()} definitions");
                                        }
                                        catch (Exception ex)
                                        {
                                            UpdateMessageBar($"Error loading YAML file {args[1]} - {ex.Message}");
                                        }
                                    }
                                }
                                else if (args[0] == "export")
                                {
                                    try
                                    {
                                        YAMLHandeling.SerializeToFile(args[1]);
                                        UpdateMessageBar($"YAML file writen to {args[1]}");
                                    }
                                    catch (Exception ex)
                                    {
                                        UpdateMessageBar($"Error writing YAML to file {args[1]} - {ex.Message}");
                                    }
                                }

                                else if (args[0] == "new")
                                {
                                    if (GlobalVariables.availableKubeTypes.Exists(x => x.kind == args[1]))
                                    {
                                        if (GlobalVariables.promptArray.Count == 1)
                                        {
                                            var kubeObject = GlobalVariables.availableKubeTypes.FirstOrDefault(x => x.kind == args[1]);
                                            Type currentType = Type.GetType(GlobalVariables.availableKubeTypes.FirstOrDefault(x => x.kind == args[1]).assemblyFullName);
                                            object _object = Activator.CreateInstance(currentType);
                                            _object.SetObjectPropertyValue("kind", kubeObject.kind);
                                            string apiVersion = (string.IsNullOrWhiteSpace(kubeObject.group) ? kubeObject.version : $"{kubeObject.group}/{kubeObject.version}");
                                            _object.SetObjectPropertyValue("apiversion", apiVersion);
                                            SessionDefinedKind newSessionKind = new SessionDefinedKind()
                                            {
                                                index = GlobalVariables.sessionDefinedKinds.Count() == 0 ? 1 : GlobalVariables.sessionDefinedKinds.Last().index + 1,
                                                kind = kubeObject.kind,
                                                KubeObject = _object
                                            };
                                            GlobalVariables.sessionDefinedKinds.Add(newSessionKind);
                                            AddToPrompt(newSessionKind.index.ToString());
                                            UpdateMessageBar($"{newSessionKind.kind} created and selected");
                                            repositionCommandInput();
                                            paintYAML();
                                        }
                                        else
                                        {
                                            UpdateMessageBar($"\"{currentInputText}\" command not found");
                                        }
                                    }
                                    else //this is the direct new commands to dictionaries and arrays
                                    {
                                        object currentObject = KubeObject.GetCurrentObject();
                                        Type[] selectListTypes = KubeObject.GetCurrentObject().GetType().GetGenericArguments();
                                        string[] values = args[1].Split(":");
                                        if (selectListTypes.Length == values.Length)
                                        {
                                            if (selectListTypes.Length == 1)
                                            {
                                                currentObject.GetType().GetMethod("Add").Invoke(currentObject, new[] { args[1] });
                                            }
                                            else
                                            {
                                                currentObject.GetType().GetMethod("Add").Invoke(currentObject, new object[] { values[0], values[1] });
                                            }
                                        }
                                        else
                                        {
                                            UpdateMessageBar($"wrong amount of variables supplied. {selectListTypes.Length} variables expected");
                                        }
                                        repositionCommandInput();
                                        paintYAML();
                                    }
                                }
                                else if (args[0] == "select")
                                {
                                    int index = 0;
                                    if (int.TryParse(args[1], out index))
                                    {
                                        if (KubeObject.IsCurrentObjectRoot())
                                        {
                                            if (KubeObject.DoesRootIndexExist(index))
                                            {
                                                var selectedKind = GlobalVariables.sessionDefinedKinds.FirstOrDefault(x => x.index == index);
                                                UpdateMessageBar($"{selectedKind.kind} selected");
                                                AddToPrompt(index.ToString());
                                                repositionCommandInput();
                                                paintYAML();
                                            }
                                            else
                                            {
                                                UpdateMessageBar("Item not found");
                                            }
                                        }
                                        else if (KubeObject.DoesNestedIndexExist(KubeObject.GetCurrentObject(), index))
                                        {
                                            UpdateMessageBar($"{index} selected");
                                            AddToPrompt(index.ToString());
                                            repositionCommandInput();
                                            paintYAML();
                                        }
                                        else
                                        {
                                            UpdateMessageBar($"{index} not found");
                                        }
                                    }
                                    else
                                    {
                                        UpdateMessageBar($"Value [{args[1]}] entered is not a integer");
                                    }
                                }
                                else if (args[0] == "no")
                                {
                                    int index = 0;
                                    if (int.TryParse(args[1], out index))
                                    {
                                        if (GlobalVariables.promptArray.Count() == 1 && GlobalVariables.sessionDefinedKinds.Exists(x => x.index == index))
                                        {
                                            GlobalVariables.sessionDefinedKinds.Remove(GlobalVariables.sessionDefinedKinds.FirstOrDefault(x => x.index == index));
                                            UpdateMessageBar("Definition deleted");
                                            repositionCommandInput();
                                            paintYAML();
                                        }
                                        else if (KubeObject.GetCurrentObject().IsList() && KubeObject.DoesNestedIndexExist(KubeObject.GetCurrentObject(), index))
                                        {
                                            KubeObject.DeleteNestedAtIndex(KubeObject.GetCurrentObject(), index);
                                            paintYAML();
                                        }
                                        else
                                        {
                                            UpdateMessageBar($"Item ({index}) not found");
                                        }
                                    }
                                    else
                                    {
                                        object currentKubeObject = KubeObject.GetCurrentObject();
                                        PropertyInfo propertyInfo = currentKubeObject.GetJsonProperty(args[1]);
                                        if (propertyInfo != null)
                                        {
                                            UpdateMessageBar($"Cleared {args[1]} value");
                                            try
                                            {
                                                if (propertyInfo.GetValue(currentKubeObject) != null)
                                                {
                                                    propertyInfo.SetValue(currentKubeObject, null);
                                                }
                                                else
                                                {
                                                    UpdateMessageBar($"\"{args[1]}\" does not contain a value");
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                UpdateMessageBar($"Cannot clear attribute value: {ex.Message}");
                                            }
                                            paintYAML();
                                        }
                                        else
                                        {
                                            UpdateMessageBar($"Attribute {args[1]} not found");
                                        }

                                    }
                                }
                                else if (retrieveAvailableOptions(false, false).Item2.Exists(x => x.name == args[0] && !x.propertyIsList))
                                {
                                    object currentKubeObject = KubeObject.GetCurrentObject();
                                    if (currentKubeObject.JsonPropertyExists(args[0]))
                                    {
                                        UpdateDescriptionView(args[0].ToLower());

                                        OptionsSlimType currentSlimKubeObject = retrieveAvailableOptions(false, false).Item2.FirstOrDefault(x => x.name == args[0] && !x.propertyIsList);
                                        var propertyInfo = currentKubeObject.GetJsonProperty(args[0]);
                                        Type propertyType = (propertyInfo.PropertyType.Name == typeof(Nullable<>).Name ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType);
                                        try
                                        {
                                            if (propertyType.Name == "Int64" || propertyType.Name == "Int32" || propertyType.Name == "String" || propertyType.Name == "Boolean")
                                            {
                                                propertyInfo.SetValue(currentKubeObject, args[1].CastToReflected(propertyType));
                                            }
                                            else if (currentSlimKubeObject.propertyIsArray)
                                            {
                                                string[] valueArray = currentInputText.Trim().Split("|");
                                                string[] tmpArray = new string[valueArray.Length - 1];
                                                int size = 0;
                                                for (int i = 0; valueArray.Length > i; i++)
                                                {
                                                    if (i == 0)
                                                    {
                                                        continue;
                                                    }
                                                    if (!string.IsNullOrWhiteSpace(valueArray[i]))
                                                    {
                                                        size += 1;
                                                        tmpArray[i - 1] = valueArray[i];
                                                    }
                                                }
                                                Array.Resize(ref tmpArray, size);
                                                propertyInfo.SetValue(currentKubeObject, tmpArray);
                                            }
                                            else if (currentSlimKubeObject.propertyIsDictionary)
                                            {
                                                object dictObject = Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(new Type[] { currentSlimKubeObject.primaryType, currentSlimKubeObject.secondaryType }));
                                                string tmpArray = "";
                                                for (int i = 1; args.Length > i; i++)
                                                {
                                                    tmpArray += $"{args[i]}";
                                                }
                                                string[] arrayVars = tmpArray.Trim().Split("|");
                                                arrayVars.ForEach(x => x.Trim());
                                                arrayVars.ForEach(x =>
                                                {
                                                    dictObject.GetType().GetMethod("Add").Invoke(dictObject, new object[] {
                                                        x.Split(":")[0].CastToReflected(currentSlimKubeObject.primaryType),
                                                        x.Split(":")[1].CastToReflected(currentSlimKubeObject.secondaryType) });
                                                });
                                                propertyInfo.SetValue(currentKubeObject, dictObject);
                                            }
                                            else
                                            {

                                                UpdateMessageBar($"Don't know how to cast value {args[1]} to type {propertyType.Name}");

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            if (ex.InnerException != null)
                                            {
                                                UpdateMessageBar($"{args[1]} cannot be casted ({ex.InnerException.Message})");
                                            }
                                            else
                                            {
                                                UpdateMessageBar($"{args[1]} cannot be casted ({ex.Message})");
                                            }
                                        }
                                    }
                                    paintYAML();
                                }
                                else
                                {
                                    UpdateMessageBar("Command not found");
                                }
                            }
                            else
                            {
                                if (retrieveAvailableOptions(false, false).Item2.Exists(x => x.name == currentInputText))
                                {
                                    object tmpObject = KubeObject.GetCurrentObject();
                                    UpdateDescriptionView(currentInputText);
                                    OptionsSlimType currentKubeObject = tmpObject.RetrieveAttributeValue(currentInputText.ToLower());
                                    if (currentKubeObject != null)
                                    {
                                        var currentProperty = tmpObject.GetJsonProperty(currentInputText);
                                        Type currentObjectType = Nullable.GetUnderlyingType(currentProperty.PropertyType) != null ? Nullable.GetUnderlyingType(currentProperty.PropertyType) : currentProperty.PropertyType;
                                        if (currentObjectType.IsPrimitive || (currentObjectType == typeof(String)) || currentKubeObject.propertyIsArray || currentKubeObject.propertyIsDictionary)
                                        {
                                            UpdateMessageBar($"({currentInputText}) requires a value of type \"{currentKubeObject.displayType}\"");
                                        }
                                        else
                                        {
                                            if (tmpObject.GetObjectPropertyValue(currentProperty.Name) == null)
                                            {
                                                if (currentProperty.PropertyType.IsGenericType)
                                                {
                                                    if (currentProperty.PropertyType.GetGenericTypeDefinition() == typeof(IList<>))
                                                    {
                                                        tmpObject.SetObjectPropertyValue(currentInputText, Activator.CreateInstance(typeof(List<>).MakeGenericType(currentProperty.PropertyType.GetGenericArguments()[0])));
                                                    }
                                                }
                                                else
                                                {
                                                    tmpObject.SetObjectPropertyValue(currentInputText, Activator.CreateInstance(currentProperty.PropertyType));
                                                }

                                            }
                                            AddToPrompt(currentInputText.ToLower());
                                            UpdateDescriptionView();
                                            repositionCommandInput();
                                            paintYAML();
                                        }
                                    }
                                }
                                else
                                {
                                    UpdateMessageBar($"Command \"{currentInputText}\" not found");
                                }
                            }
                        }
                        commandPromptTextField.Text = "";
                        autoCompleteInterruptText = "";
                    }
                }
                updateAvailableKindsList();
                currentInputText = commandPromptTextField.Text.ToString();
                args = currentInputText.Trim().Split();
                if (KubeObject.GetCurrentObject().JsonPropertyExists(args[0].ToLower()))
                {
                    UpdateDescriptionView(args[0]);
                }
                else if (args.Length == 2 && args[0] == "new" && GlobalVariables.availableKubeTypes.Exists(x => x.kind == args[1]))
                {
                    UpdateDescriptionView(args[1]);
                }
                else if (args.Length == 2 && args[0] == "no")
                {
                    UpdateDescriptionView(args[1]);
                }
                else
                {
                    UpdateDescriptionView();
                }
            };
        }
    }
}
