using k8config.DataModels;
using k8config.GUIEvents;
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
            YAMLModelControls.YAMLModeWindow.KeyUp += (e) =>
            {
                string currentInputText = YAMLModelControls.commandPromptTextField.Text.ToString();
                if (e.KeyEvent.Key == (Key.CtrlMask | Key.C))
                {
                    YAMLModelControls.commandPromptTextField.Text = "";
                    YAMLModelControls.commandPromptTextField.CursorPosition = YAMLModelControls.commandPromptTextField.Text.Length;
                }
                if (e.KeyEvent.Key == (Key.CtrlMask | Key.Z) || e.KeyEvent.Key == (Key.CtrlMask | Key.A) || e.KeyEvent.Key == Key.CursorUp || e.KeyEvent.Key == Key.CursorDown)
                {
                    string[] args = YAMLModelControls.commandPromptTextField.Text.ToString().Split(" ");
                    switch (e.KeyEvent.Key)
                    {
                        case (Key.CursorUp):
                            if (YAMLModelControls.sessionHistory.Count != 0)
                            {
                                if (YAMLModelControls.sessionHistoryIndex != 0)
                                {
                                    YAMLModelControls.sessionHistoryIndex--;
                                }
                                else
                                {
                                    YAMLModelControls.sessionHistoryIndex = YAMLModelControls.sessionHistory.Count - 1;
                                }
                                YAMLModelControls.commandPromptTextField.Text = YAMLModelControls.sessionHistory[YAMLModelControls.sessionHistoryIndex];
                                YAMLModelControls.commandPromptTextField.CursorPosition = YAMLModelControls.commandPromptTextField.Text.Length;
                            }
                            break;
                        case (Key.CursorDown):
                            if (YAMLModelControls.sessionHistory.Count != 0)
                            {
                                if (YAMLModelControls.sessionHistoryIndex < (YAMLModelControls.sessionHistory.Count - 1))
                                {
                                    YAMLModelControls.sessionHistoryIndex++;
                                }
                                else
                                {
                                    YAMLModelControls.sessionHistoryIndex = YAMLModelControls.sessionHistory.Count == 0 ? (YAMLModelControls.sessionHistory.Count - 1) : 0;
                                }
                                YAMLModelControls.commandPromptTextField.Text = YAMLModelControls.sessionHistory[YAMLModelControls.sessionHistoryIndex];
                                YAMLModelControls.commandPromptTextField.CursorPosition = YAMLModelControls.commandPromptTextField.Text.Length;
                            }

                            break;
                        case (Key.CtrlMask | Key.A):
                            GlobalVariables.currentavailableListUpDown = true;
                            if (YAMLModelControls.availableKindsListView.SelectedItem > 0)
                            {

                                YAMLModelControls.availableKindsListView.SelectedItem = YAMLModelControls.availableKindsListView.SelectedItem - 1;
                                YAMLModelControls.availableKindsListView.TopItem = YAMLModelControls.availableKindsListView.SelectedItem;
                                YAMLModelControls.commandPromptTextField.Text = args[0] == "new" ? $"{args[0]} {currentAvailableOptions[YAMLModelControls.availableKindsListView.SelectedItem].name}" : currentAvailableOptions[YAMLModelControls.availableKindsListView.SelectedItem].name;
                                YAMLModelControls.commandPromptTextField.CursorPosition = YAMLModelControls.commandPromptTextField.Text.Length;
                                UpdateDescriptionView(YAMLModelControls.commandPromptTextField.Text.ToString());
                            }
                            YAMLModelControls.commandPromptTextField.SetFocus();
                            break;
                        case (Key.CtrlMask | Key.Z):
                            GlobalVariables.currentavailableListUpDown = true;
                            if (YAMLModelControls.availableKindsListView.SelectedItem < YAMLModelControls.availableKindsListView.Source.ToList().Count - 1)
                            {
                                YAMLModelControls.availableKindsListView.SelectedItem = YAMLModelControls.availableKindsListView.SelectedItem + 1;
                                YAMLModelControls.availableKindsListView.TopItem = YAMLModelControls.availableKindsListView.SelectedItem;
                                YAMLModelControls.commandPromptTextField.Text = args[0] == "new" ? $"{args[0]} {currentAvailableOptions[YAMLModelControls.availableKindsListView.SelectedItem].name}" : currentAvailableOptions[YAMLModelControls.availableKindsListView.SelectedItem].name;
                                YAMLModelControls.commandPromptTextField.CursorPosition = YAMLModelControls.commandPromptTextField.Text.Length;
                                UpdateDescriptionView(YAMLModelControls.commandPromptTextField.Text.ToString());
                            }
                            YAMLModelControls.commandPromptTextField.SetFocus();
                            break;
                        default:

                            break;
                    }
                    if (String.IsNullOrWhiteSpace(currentInputText))
                    {
                        GlobalVariables.autoCompleteInterruptText = "";
                    }
                }
            };
            YAMLModelControls.commandPromptTextField.KeyPress += (e) =>
            {
                string currentInputText = YAMLModelControls.commandPromptTextField.Text.ToString();
                string[] args = currentInputText.Trim().Split();
                List<string> validCompleteCommands = new List<string>() { "no" };
                if (e.KeyEvent.Key == Key.Tab && !string.IsNullOrEmpty(currentInputText))
                {
                    GlobalVariables.currentavailableListUpDown = false;
                    List<string> possibleOptions = new List<string>();

                    if (YAMLModePromptObject.CurrentPromptPositionIsRoot && args.Count() > 1)
                    {
                        if (String.IsNullOrEmpty(GlobalVariables.autoCompleteInterruptText)) GlobalVariables.autoCompleteInterruptText = args[1];
                        possibleOptions = retrieveAvailableOptions(true, false, GlobalVariables.autoCompleteInterruptText).Item2.Where(x => x.name.StartsWith(GlobalVariables.autoCompleteInterruptText)).Select(x => x.name).ToList();
                        YAMLModelControls.availableKindsListView.SetSource(possibleOptions.ToList());
                        YAMLModelControls.commandPromptTextField.Text = $"{args[0]} {NextAutoComplete(possibleOptions)}";
                        YAMLModelControls.commandPromptTextField.CursorPosition = YAMLModelControls.commandPromptTextField.Text.Length;
                    }
                    else if (args.Count() == 1 && args[0] != "")
                    {
                        if (String.IsNullOrEmpty(GlobalVariables.autoCompleteInterruptText)) GlobalVariables.autoCompleteInterruptText = args[0].Trim();
                        possibleOptions = retrieveAvailableOptions(true, false, GlobalVariables.autoCompleteInterruptText).Item2.Where(x => x.name.StartsWith(GlobalVariables.autoCompleteInterruptText)).Select(x => x.name).ToList();
                        YAMLModelControls.availableKindsListView.SetSource(possibleOptions.ToList());
                        YAMLModelControls.commandPromptTextField.Text = $"{NextAutoComplete(possibleOptions)}";
                        YAMLModelControls.commandPromptTextField.CursorPosition = YAMLModelControls.commandPromptTextField.Text.Length;
                    }
                    else if (args.Count() == 2 && args[1] != "" && validCompleteCommands.Exists(x => x == args[0]))
                    {
                        if (String.IsNullOrEmpty(GlobalVariables.autoCompleteInterruptText)) GlobalVariables.autoCompleteInterruptText = args[1].Trim();
                        possibleOptions = retrieveAvailableOptions(true, false, GlobalVariables.autoCompleteInterruptText).Item2.Where(x => x.name.StartsWith(GlobalVariables.autoCompleteInterruptText)).Select(x => x.name).ToList();
                        YAMLModelControls.availableKindsListView.SetSource(possibleOptions.ToList());
                        YAMLModelControls.commandPromptTextField.Text = $"{args[0]} {NextAutoComplete(possibleOptions)}";
                        YAMLModelControls.commandPromptTextField.CursorPosition = YAMLModelControls.commandPromptTextField.Text.Length;
                    }
                    else
                    {
                        GlobalVariables.autoCompleteInterruptText = "";
                        possibleOptions = retrieveAvailableOptions(false).Item2.ToList().Select(x => x.TableView).ToList();
                        YAMLModelControls.availableKindsListView.SetSource(possibleOptions.ToList());

                    }
                    e.Handled = true;
                }
                else
                {
                    GlobalVariables.autoCompleteInterruptText = "";
                }
            };
            YAMLModelControls.commandPromptTextField.KeyUp += (e) =>
            {
                List<string> allowedDescriptionCommands = new List<string>() { "new", "no" };
                string currentInputText = YAMLModelControls.commandPromptTextField.Text.ToString();
                string[] args = currentInputText.Trim().Split();
                if (e.KeyEvent.Key == Key.Enter)
                {
                    UpdateMessageBar("");
                    if (!string.IsNullOrEmpty(currentInputText))
                    {
                        if (YAMLModelControls.sessionHistory.Count == 0)
                        {
                            YAMLModelControls.sessionHistory.Add(currentInputText);
                        }
                        else if (YAMLModelControls.sessionHistory.Last() != currentInputText)
                        {
                            YAMLModelControls.sessionHistory.Add(currentInputText);
                        }
                        GlobalVariables.currentavailableListUpDown = false;
                        if (currentInputText.Contains(".."))
                        {
                            if (YAMLModePromptObject.CurrentPromptPositionIsNotRoot)
                            {
                                YAMLModePromptObject.ExitMultipleFolders(currentInputText);
                                repositionCommandInput();
                                paintYAML();
                            }
                        }
                        else if (currentInputText == "/")
                        {
                            if (YAMLModePromptObject.CurrentPromptPositionIsNotRoot)
                            {
                                YAMLModePromptObject.Init();
                                repositionCommandInput();
                                paintYAML();
                            }
                        }
                        else if (GlobalVariables.sessionDefinedKinds.Count() > 0 && currentInputText == "list")
                        {
                            List<string> outputList = new List<string>();
                            if (YAMLModePromptObject.CurrentPromptPositionIsRoot)
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
                            YAMLModelControls.definedYAMLListView.SetSourceAsync(outputList);
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
                                    UpdateMessageBar($"{selectListTypes[0].Name.Replace("V1", "")} added to {YAMLModePromptObject.Last}");
                                    AddToPrompt(KubeObject.GetNestedList(currentObject).Last().index.ToString());
                                }
                                repositionCommandInput();
                                paintYAML();
                            }

                        }
                        else if (YAMLModePromptObject.CurrentPromptPositionIsRoot && currentInputText == "exit")
                        {
                            Environment.Exit(0);
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
                                            GlobalVariables.Log.Error(ex, $"Error loading YAML file {args[1]}");
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
                                        GlobalVariables.Log.Error(ex, $"Error writing YAML file {args[1]}");
                                        UpdateMessageBar($"Error writing YAML to file {args[1]} - {ex.Message}");
                                    }
                                }

                                else if (args[0] == "new")
                                {
                                    if (GlobalVariables.availableKubeTypes.Exists(x => x.classKind == args[1]))
                                    {
                                        if (YAMLModePromptObject.CurrentPromptPositionIsRoot)
                                        {
                                            var kubeObject = GlobalVariables.availableKubeTypes.FirstOrDefault(x => x.classKind == args[1]);
                                            Type currentType = Type.GetType(GlobalVariables.availableKubeTypes.FirstOrDefault(x => x.classKind == args[1]).assemblyFullName);
                                            object _object = Activator.CreateInstance(currentType);
                                            _object.SetObjectPropertyValue("kind", kubeObject.kind);
                                            string apiVersion = (string.IsNullOrWhiteSpace(kubeObject.group) ? kubeObject.version : $"{kubeObject.group}/{kubeObject.version}");
                                            _object.SetObjectPropertyValue("apiversion", apiVersion);
                                            _object.SetObjectPropertyValue("metadata", new V1ObjectMeta() {  Name = $"new{kubeObject.kind}" });
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
                                        if (YAMLModePromptObject.CurrentPromptPositionIsRoot)
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
                                        if (YAMLModePromptObject.CurrentPromptPositionIsRoot && GlobalVariables.sessionDefinedKinds.Exists(x => x.index == index))
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
                                        PropertyInfo propertyInfo = currentKubeObject.GetJsonPropertyInfo(args[1]);
                                        if (propertyInfo != null)
                                        {
                                            UpdateMessageBar($"\"{args[1]}\" attribute removed");
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
                                                GlobalVariables.Log.Error(ex, "Cannot clear attribute value");
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
                                        var propertyInfo = currentKubeObject.GetJsonPropertyInfo(args[0]);
                                        Type propertyType = (propertyInfo.PropertyType.Name == typeof(Nullable<>).Name ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType);
                                        try
                                        {
                                            if (propertyType.Name == "Int64" || propertyType.Name == "Int32" || propertyType.Name == "String" || propertyType.Name == "Boolean")
                                            {
                                                propertyInfo.SetValue(currentKubeObject, args[1].CastToReflected(propertyType));
                                            }
                                            else if (propertyType.Name == "IntstrIntOrString")
                                            {
                                                propertyInfo.SetValue(currentKubeObject, Activator.CreateInstance(typeof(IntstrIntOrString), args[1]));
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
                                                GlobalVariables.Log.Error(ex.InnerException, $"{args[1]} cannot be casted");
                                                UpdateMessageBar($"{args[1]} cannot be casted ({ex.InnerException.Message})");
                                            }
                                            else
                                            {
                                                GlobalVariables.Log.Error(ex, $"{args[1]} cannot be casted");
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
                                object currentPromptObject = KubeObject.GetCurrentObject();
                                if (currentPromptObject.GetJsonObjectPropertyValueExists(currentInputText))
                                {
                                    UpdateDescriptionView(currentInputText);
                                    OptionsSlimType currentKubeObject = currentPromptObject.RetrieveAttributeValue(currentInputText);
                                    if (currentKubeObject != null)
                                    {
                                        var currentProperty = currentPromptObject.GetJsonPropertyInfo(currentInputText);
                                        Type currentObjectType = Nullable.GetUnderlyingType(currentProperty.PropertyType) != null ? Nullable.GetUnderlyingType(currentProperty.PropertyType) : currentProperty.PropertyType;
                                        if (currentObjectType.IsPrimitive || (currentObjectType == typeof(String)) || currentKubeObject.propertyIsArray || currentKubeObject.propertyIsDictionary)
                                        {
                                            UpdateMessageBar($"{currentInputText} requires a value of type \"{currentKubeObject.displayType}\"");
                                        }
                                        else
                                        {
                                            if (currentPromptObject.GetJsonObjectPropertyValue(currentInputText) == null)
                                            {
                                                if (currentProperty.PropertyType.IsGenericType)
                                                {
                                                    if (currentProperty.PropertyType.GetGenericTypeDefinition() == typeof(IList<>))
                                                    {
                                                        currentPromptObject.SetObjectPropertyValue(currentInputText, Activator.CreateInstance(typeof(List<>).MakeGenericType(currentProperty.PropertyType.GetGenericArguments()[0])));
                                                    }
                                                }
                                                else
                                                {
                                                    currentPromptObject.SetObjectPropertyValue(currentInputText, Activator.CreateInstance(currentProperty.PropertyType));
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
                        YAMLModelControls.commandPromptTextField.Text = "";
                    }
                    GlobalVariables.autoCompleteInterruptText = "";
                }
                updateAvailableKindsList();
                currentInputText = YAMLModelControls.commandPromptTextField.Text.ToString();
                args = currentInputText.Trim().Split();
                if (KubeObject.GetCurrentObject().JsonPropertyExists(args[0]))
                {
                    UpdateDescriptionView(args[0]);
                }
                else if (args.Length == 2 && args[0] == "new" && GlobalVariables.availableKubeTypes.Exists(x => x.classKind == args[1]))
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
