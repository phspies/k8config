using k8config.DataModels;
using k8config.Utilities;
using k8s;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        static public void YamlModeKeyEvents()
        {
            InteractiveModeWindow.KeyUp += (e) =>
            {
                string currentInputText = commandPromptTextField.Text.ToString();
                if (e.KeyEvent.Key == Key.CursorDown || e.KeyEvent.Key == Key.CursorUp)
                {
                    switch (e.KeyEvent.Key)
                    {
                        case Key.CursorUp:
                            currentavailableListUpDown = true;
                            if (availableKindsListView.SelectedItem > 0)
                            {
                                availableKindsListView.SelectedItem = availableKindsListView.SelectedItem - 1;
                                availableKindsListView.TopItem = availableKindsListView.SelectedItem;
                            }
                            break;
                        case Key.CursorDown:
                            currentavailableListUpDown = true;
                            if (availableKindsListView.SelectedItem < availableKindsListView.Source.ToList().Count - 1)
                            {
                                availableKindsListView.SelectedItem = availableKindsListView.SelectedItem + 1;
                                availableKindsListView.TopItem = availableKindsListView.SelectedItem;
                            }
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
                string currentInputText = commandPromptTextField.Text.ToString();
                string[] args = currentInputText.Trim().Split();
                if (e.KeyEvent.Key == Key.Enter)
                {
                    if (!string.IsNullOrEmpty(currentInputText))
                    {
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
                        if (currentInputText == "/")
                        {
                            if (GlobalVariables.promptArray.Count() > 1)
                            {
                                GlobalVariables.promptArray = new List<string>() { "k8config" };
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
                                    UpdateMessageBar($"{selectListTypes[0].Name} added to {GlobalVariables.promptArray.Last()}");
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
                                        var kubeObject = GlobalVariables.availableKubeTypes.FirstOrDefault(x => x.kind == args[1]);
                                        Type currentType = Type.GetType(GlobalVariables.availableKubeTypes.FirstOrDefault(x => x.kind == args[1]).assemblyFullName);
                                        object _object = Activator.CreateInstance(currentType);
                                        _object.GetType().GetProperty("Kind").SetValue(_object, kubeObject.kind);
                                        string apiVersion = (string.IsNullOrWhiteSpace(kubeObject.group) ? kubeObject.version : $"{kubeObject.group}/{kubeObject.version}");
                                        _object.GetType().GetProperty("ApiVersion").SetValue(_object, apiVersion);
                                        if (_object is IMetadata<V1ObjectMeta> withMetadata && withMetadata.Metadata == null)
                                        {
                                            withMetadata.Metadata = new V1ObjectMeta();
                                        }
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
                                else if (args[0] == "delete")
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
                                            UpdateMessageBar("Item not found");
                                        }
                                    }
                                    else
                                    {
                                        UpdateMessageBar("Value entered is not a integer");
                                    }
                                }
                                else if (retrieveAvailableOptions(false, false).Item2.Exists(x => x.name == args[0] && !x.propertyIsList))
                                {
                                    object currentKubeObject = KubeObject.GetCurrentObject();
                                    if (currentKubeObject.GetType().GetProperties().ToList().Exists(x => x.Name.ToLower() == args[0].ToLower()))
                                    {
                                        UpdateDescriptionView(args[0].ToLower());

                                        OptionsSlimType currentSlimKubeObject = retrieveAvailableOptions(false, false).Item2.FirstOrDefault(x => x.name == args[0] && !x.propertyIsList);
                                        var propertyInfo = currentKubeObject.GetType().GetProperties().ToList().FirstOrDefault(x => x.Name.ToLower() == args[0].ToLower());
                                        Type propertyType = (propertyInfo.PropertyType.Name == typeof(Nullable<>).Name ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType);
                                        try
                                        {
                                            if (propertyType.Name == "Int64" || propertyType.Name == "Int32" || propertyType.Name == "String" || propertyType.Name == "Boolean")
                                            {
                                                propertyInfo.SetValue(currentKubeObject, args[1].CastToReflected(propertyType));
                                            }
                                            else if (currentSlimKubeObject.propertyIsArray)
                                            {
                                                string tmpArray = "";
                                                for (int i = 1; args.Length > i; i++)
                                                {
                                                    tmpArray += $"{args[i]}";
                                                }
                                                string[] arrayVars = tmpArray.Trim().Split("|");
                                                arrayVars.ForEach(x => x.Trim());
                                                propertyInfo.SetValue(currentKubeObject, arrayVars.ToList());
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
                                    UpdateDescriptionView(currentInputText);
                                    object tmpObject = KubeObject.GetCurrentObject();
                                    if (tmpObject.RetrieveAttributeValues().Exists(x => x.name.ToLower() == currentInputText.ToLower()))
                                    {
                                        var currentKubeObject = tmpObject.RetrieveAttributeValues().FirstOrDefault(x => x.name.ToLower() == currentInputText.ToLower());
                                        var currentProperty = tmpObject.GetType().GetProperties().ToList().FirstOrDefault(x => x.Name.ToLower() == currentInputText.ToLower());
                                        Type currentObjectType = Nullable.GetUnderlyingType(currentProperty.PropertyType) != null ? Nullable.GetUnderlyingType(currentProperty.PropertyType) : currentProperty.PropertyType;
                                        if (currentObjectType.IsPrimitive || (currentObjectType == typeof(String)) || currentKubeObject.propertyIsArray || currentKubeObject.propertyIsDictionary)
                                        {
                                            UpdateMessageBar($"({currentInputText}) requires a value of type {currentKubeObject.displayType}");
                                        }
                                        else
                                        {
                                            if (tmpObject.GetType().GetProperty(currentProperty.Name).GetValue(tmpObject) == null)
                                            {
                                                if (currentProperty.PropertyType.IsGenericType)
                                                {
                                                    var test = currentProperty.PropertyType.GetGenericTypeDefinition();
                                                    if (currentProperty.PropertyType.GetGenericTypeDefinition() == typeof(IList<>))
                                                    {
                                                        var listType = typeof(List<>);
                                                        var constructedListType = listType.MakeGenericType(currentProperty.PropertyType.GetGenericArguments()[0]);
                                                        tmpObject.GetType().GetProperty(currentProperty.Name).SetValue(tmpObject, Activator.CreateInstance(constructedListType));
                                                    }
                                                }
                                                else
                                                {
                                                    tmpObject.GetType().GetProperty(currentProperty.Name).SetValue(tmpObject, Activator.CreateInstance(currentProperty.PropertyType));
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
                                    UpdateMessageBar("Command not found");
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
                if (KubeObject.GetCurrentObject().GetType().GetProperties().ToList().Exists(x => x.Name.ToLower() == args[0].ToLower()))
                {
                    UpdateDescriptionView(args[0]);
                }
                else if (args.Length == 2 && args[0] == "new" && GlobalVariables.availableKubeTypes.Exists(x => x.kind == args[1]))
                {
                    UpdateDescriptionView(args[1]);
                }
                else
                {
                    UpdateDescriptionView();
                }
                //repositionCommandInput();
                //drawYAML();
            };
        }
    }
}
