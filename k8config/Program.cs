using k8config.DataModels;
using k8config.Utilities;
using k8s;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Terminal.Gui;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace k8config
{
    class Program
    {
        static string autoCompleteInterruptText = "";
        static int autoCompleteInterruptIndex = 0;
        static ListView availableKindsListView = new ListView();
        static Label commandPromptLabel = new Label();
        static TextField commandPromptTextField = new TextField("");
        static Window definedYAMLWindow = new Window();
        static Window availableKindsWindow = new Window();
        static ListView definedYAMLListView = new ListView();
        static bool currentavailableListUpDown = false;
        static Toplevel topLevelWindowObject = Application.Top;
        static Label messageBarItem = new Label("");
        static void Main(string[] args)
        {
            AssemblySubsystem.BuildAvailableAssemblyList();
            Application.Init();
            topLevelWindowObject.TabStop = true;
            topLevelWindowObject.ColorScheme.Normal = new Terminal.Gui.Attribute(Color.Black, Color.White);
            ColorScheme color = new ColorScheme()
            {
                Normal = new Terminal.Gui.Attribute(Color.Black, Color.White),
                Focus = new Terminal.Gui.Attribute(Color.White, Color.Blue),
                HotNormal = new Terminal.Gui.Attribute(Color.Red, Color.White),
                HotFocus = new Terminal.Gui.Attribute(Color.Blue, Color.White),
                Disabled = new Terminal.Gui.Attribute(Color.DarkGray, Color.Black)
            };
            ColorScheme selectColor = new ColorScheme()
            {
                Normal = new Terminal.Gui.Attribute(Color.Black, Color.White),
                Focus = new Terminal.Gui.Attribute(Color.White, Color.Blue),
                HotNormal = new Terminal.Gui.Attribute(Color.White, Color.Red),
                HotFocus = new Terminal.Gui.Attribute(Color.Blue, Color.White),
                Disabled = new Terminal.Gui.Attribute(Color.DarkGray, Color.Black)
            };


            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Quit", () => { if (Quit()) { topLevelWindowObject.Running = false; }}),
                new StatusItem(Key.F2, "~F2~ Open", () => {
                    Open();
                    updateAvailableKindsList();
                }),
                new StatusItem(Key.F3, "~F3~ Save", () => Save()),
            });
            topLevelWindowObject.Add(statusBar);

            availableKindsWindow = new Window()
            {
                Title = "Available Commands",
                X = 0,
                Y = 0,
                Width = 50,
                Height = Dim.Fill() - 5,
                TabStop = false,
                CanFocus = false,
                ColorScheme = color,

            };
            definedYAMLWindow = new Window()
            {
                Title = "Selected Definition",
                X = availableKindsWindow.Bounds.Right,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 5,
                TabStop = false,
                CanFocus = false,
                ColorScheme = color

            };
            var commandWindow = new Window()
            {
                Title = "Command Line",
                Y = definedYAMLWindow.Bounds.Bottom + 1,
                Width = Dim.Fill(),
                Height = 4,
                ColorScheme = color
            };

            topLevelWindowObject.DrawContent += (e) =>
            {
                availableKindsWindow.Width = 50;
                definedYAMLWindow.X = availableKindsWindow.Bounds.Right;
                commandWindow.Y = definedYAMLWindow.Bounds.Bottom;
            };

            topLevelWindowObject.Add(commandWindow, definedYAMLWindow);

            commandPromptLabel = new Label(string.Join(":", GlobalVariables.promptArray) + ">") { TextAlignment = TextAlignment.Left, X = 1 };
            commandPromptTextField = new TextField("") { X = Pos.Right(commandPromptLabel) + 1, Width = Dim.Fill(), TabStop = true };

            topLevelWindowObject.Add(availableKindsWindow);
            definedYAMLListView = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                AllowsMultipleSelection = false,
                AllowsMarking = false,
                CanFocus = false,
                TabStop = false,

            };
            definedYAMLWindow.Add(definedYAMLListView);
            availableKindsListView = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                AllowsMultipleSelection = false,
                AllowsMarking = false,
                CanFocus = false,
                TabStop = false,
                ColorScheme = selectColor
            };

            availableKindsWindow.Add(availableKindsListView);
            messageBarItem.ColorScheme = new ColorScheme() { Normal = new Terminal.Gui.Attribute(Color.Red, Color.White) };
            messageBarItem.Width = Dim.Fill();
            messageBarItem.Text = "No definitions found";
            messageBarItem.CanFocus = false;
            messageBarItem.Y = 1;
            messageBarItem.X = 1;
            commandWindow.Add(messageBarItem);

            updateAvailableKindsList();

            topLevelWindowObject.KeyUp += (e) =>
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
                if (e.KeyEvent.Key == Key.Tab && !string.IsNullOrEmpty(currentInputText))
                {
                    currentavailableListUpDown = false;
                    List<string> possibleOptions = new List<string>();
                    string[] args = currentInputText.Split();
                    if (GlobalVariables.promptArray.Count() == 1 && args.Count() > 1)
                    {
                        if (String.IsNullOrEmpty(autoCompleteInterruptText)) autoCompleteInterruptText = args[1];
                        possibleOptions = retrieveAvailableOptions(true, false, autoCompleteInterruptText).Item2.Where(x => x.name.StartsWith(autoCompleteInterruptText)).Select(x => x.name).ToList();
                        availableKindsListView.SetSource(possibleOptions.ToList());
                        commandPromptTextField.Text = $"{args[0]} {NextAutoComplete(autoCompleteInterruptText, possibleOptions)}";
                        commandPromptTextField.CursorPosition = currentInputText.Length;

                    }
                    else if (args.Count() == 1 && args[0] != "")
                    {
                        if (String.IsNullOrEmpty(autoCompleteInterruptText)) autoCompleteInterruptText = args[0];
                        possibleOptions = retrieveAvailableOptions(true, false, autoCompleteInterruptText).Item2.Where(x => x.name.StartsWith(autoCompleteInterruptText)).Select(x => x.name).ToList();
                        availableKindsListView.SetSource(possibleOptions.ToList());
                        commandPromptTextField.Text = $"{NextAutoComplete(autoCompleteInterruptText, possibleOptions)}";
                        commandPromptTextField.CursorPosition = currentInputText.Length;
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
                if (e.KeyEvent.Key == Key.Enter)
                {
                    if (!string.IsNullOrEmpty(currentInputText))
                    {
                        currentavailableListUpDown = false;
                        //direct commands

                        if (currentInputText == "..")
                        {
                            if (GlobalVariables.promptArray.Count() > 1)
                            {
                                GlobalVariables.promptArray.RemoveAt(GlobalVariables.promptArray.Count - 1);
                                repositionCommandInput();
                                drawYAML();
                            }
                        }
                        if (currentInputText == "/")
                        {
                            if (GlobalVariables.promptArray.Count() > 1)
                            {
                                GlobalVariables.promptArray = new List<string>() { "k8config" };
                                repositionCommandInput();
                                drawYAML();
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
                                messageBarItem.Text = "The current object is not a list";
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
                                    messageBarItem.Text = $"{selectListTypes[0].Name} added to {GlobalVariables.promptArray.Last()}";
                                    GlobalVariables.promptArray.Add(KubeObject.GetNestedList(currentObject).Last().index.ToString());
                                }
                                repositionCommandInput();
                                drawYAML();
                            }
                        }
                        else
                        {
                            //inline type commands
                            string[] args = currentInputText.Trim().Split();
                            if (args.Length > 1)
                            {
                                if (args[0] == "new")
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
                                        GlobalVariables.promptArray.Add(newSessionKind.index.ToString());
                                        messageBarItem.Text = $"{newSessionKind.kind} created and selected";
                                        repositionCommandInput();
                                        drawYAML();
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
                                            messageBarItem.Text = $"wrong amount of variables supplied. {selectListTypes.Length} variables expected";
                                        }
                                        repositionCommandInput();
                                        drawYAML();
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
                                                messageBarItem.Text = $"{selectedKind.kind} selected";
                                                GlobalVariables.promptArray.Add(index.ToString());
                                                repositionCommandInput();
                                                drawYAML();
                                            }
                                            else
                                            {
                                                messageBarItem.Text = "Item not found";
                                            }
                                        }
                                        else if (KubeObject.DoesNestedIndexExist(KubeObject.GetCurrentObject(), index))
                                        {
                                            messageBarItem.Text = $"{index} selected";
                                            GlobalVariables.promptArray.Add(index.ToString());
                                            repositionCommandInput();
                                            drawYAML();
                                        }
                                        else
                                        {
                                            messageBarItem.Text = $"{index} not found";
                                        }
                                    }
                                    else
                                    {
                                        messageBarItem.Text = $"Value [{args[1]}] entered is not a integer";
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
                                            messageBarItem.Text = "Definition deleted";
                                            repositionCommandInput();
                                            drawYAML();
                                        }
                                        else if (KubeObject.GetCurrentObject().IsList() && KubeObject.DoesNestedIndexExist(KubeObject.GetCurrentObject(), index))
                                        {
                                            KubeObject.DeleteNestedAtIndex(KubeObject.GetCurrentObject(), index);
                                            drawYAML();
                                        }
                                        else
                                        {
                                            messageBarItem.Text = "Item not found";
                                        }
                                    }
                                    else
                                    {
                                        messageBarItem.Text = "Value entered is not a integer";
                                    }
                                }
                                else if (retrieveAvailableOptions(false, false).Item2.Exists(x => x.name == args[0] && !x.isList))
                                {
                                    object tmpObject = KubeObject.GetCurrentObject();
                                    if (tmpObject.GetType().GetProperties().ToList().Exists(x => x.Name.ToLower() == args[0].ToLower()))
                                    {
                                        OptionsSlimType currentKubeObject = retrieveAvailableOptions(false, false).Item2.FirstOrDefault(x => x.name == args[0] && !x.isList);
                                        var propertyInfo = tmpObject.GetType().GetProperties().ToList().FirstOrDefault(x => x.Name.ToLower() == args[0].ToLower());
                                        Type propertyType;
                                        if (propertyInfo.PropertyType.Name == typeof(Nullable<>).Name)
                                        {
                                            propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                                        }
                                        else
                                        {
                                            propertyType = propertyInfo.PropertyType;
                                        }

                                        try
                                        {
                                            if (propertyType.Name == "Int64")
                                            {
                                                propertyInfo.SetValue(tmpObject, Convert.ChangeType(Int64.Parse(args[1]), propertyType));
                                            }
                                            else if (propertyType.Name == "Int32")
                                            {
                                                propertyInfo.SetValue(tmpObject, Convert.ChangeType(Int32.Parse(args[1]), propertyType));
                                            }
                                            else if (propertyType.Name == "String")
                                            {
                                                propertyInfo.SetValue(tmpObject, args[1]);
                                            }
                                            else if (propertyType.Name == "Boolean")
                                            {
                                                propertyInfo.SetValue(tmpObject, Convert.ChangeType(Boolean.Parse(args[1]), propertyType));
                                            }
                                            else if (currentKubeObject.isArray)
                                            {
                                                string tmpArray = "";
                                                for (int i = 1; args.Length > i; i++)
                                                {
                                                    tmpArray += $"{args[i]}";
                                                }
                                                string[] arrayVars = tmpArray.Trim().Split("|");
                                                arrayVars.ForEach(x => x.Trim());
                                                propertyInfo.SetValue(tmpObject, arrayVars.ToList());
                                            }
                                            else if (currentKubeObject.isDictionary)
                                            {
                                                object dictObject = Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(new Type[] { currentKubeObject.primaryType, currentKubeObject.secondaryType }));
                                                
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
                                                        x.Split(":")[0].CastToReflected(currentKubeObject.primaryType),
                                                        x.Split(":")[1].CastToReflected(currentKubeObject.secondaryType)
                                                    });
                                                });
                                                propertyInfo.SetValue(tmpObject, dictObject);
                                            }
                                            else
                                            {

                                                messageBarItem.Text = $"Don't know how to cast value {args[1]} to type {propertyType.Name}";

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            messageBarItem.Text = $"{args[1]} cannot be casted to {propertyType.Name} ({ex.Message})";
                                        }
                                    }
                                    drawYAML();
                                }
                                else
                                {
                                    messageBarItem.Text = "Command not found";
                                }
                            }
                            else
                            {
                                if (retrieveAvailableOptions(false, false).Item2.Exists(x => x.name == currentInputText))
                                {
                                    object tmpObject = KubeObject.GetCurrentObject();
                                    if (tmpObject.RetrieveAttributeValues().Exists(x => x.name.ToLower() == currentInputText.ToLower()))
                                    {
                                        var currentKubeObject = tmpObject.RetrieveAttributeValues().FirstOrDefault(x => x.name.ToLower() == currentInputText.ToLower());
                                        var currentObject = tmpObject.GetType().GetProperties().ToList().FirstOrDefault(x => x.Name.ToLower() == currentInputText.ToLower());
                                        Type currentObjectType = Nullable.GetUnderlyingType(currentObject.PropertyType) != null ? Nullable.GetUnderlyingType(currentObject.PropertyType) : currentObject.PropertyType;
                                        if (currentObjectType.IsPrimitive || currentObjectType == typeof(String) || currentObjectType == typeof(IList<String>))
                                        {
                                            messageBarItem.Text = $"({currentInputText}) requires a value of type {currentKubeObject.displayType}";
                                        }
                                        else
                                        {
                                            if (tmpObject.GetType().GetProperty(currentObject.Name).GetValue(tmpObject) == null)
                                            {
                                                if (currentObject.PropertyType.IsGenericType)
                                                {
                                                    var test = currentObject.PropertyType.GetGenericTypeDefinition();
                                                    if (currentObject.PropertyType.GetGenericTypeDefinition() == typeof(IList<>))
                                                    {
                                                        var listType = typeof(List<>);
                                                        var constructedListType = listType.MakeGenericType(currentObject.PropertyType.GetGenericArguments()[0]);
                                                        tmpObject.GetType().GetProperty(currentObject.Name).SetValue(tmpObject, Activator.CreateInstance(constructedListType));
                                                    }
                                                }
                                                else
                                                {
                                                    tmpObject.GetType().GetProperty(currentObject.Name).SetValue(tmpObject, Activator.CreateInstance(currentObject.PropertyType));
                                                }

                                            }
                                            GlobalVariables.promptArray.Add(currentInputText.ToLower());
                                            repositionCommandInput();
                                            drawYAML();
                                        }
                                    }
                                }


                                else
                                {
                                    messageBarItem.Text = "Command not found";
                                }
                            }
                        }
                        commandPromptTextField.Text = "";
                        autoCompleteInterruptText = "";

                    }

                }
                updateAvailableKindsList();
                //repositionCommandInput();
                //drawYAML();
            };
            commandWindow.Add(commandPromptLabel, commandPromptTextField);

            var config = new KubernetesClientConfiguration { Host = "http://127.0.0.1:8000" };
            var client = new Kubernetes(config);

            static string NextAutoComplete(string _text, List<string> _availableOptions)
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
            static void updateAvailableKindsList()
            {
                if (currentavailableListUpDown)
                {
                    currentavailableListUpDown = false;
                }
                else
                {

                    Tuple<string, List<OptionsSlimType>> returnValues = retrieveAvailableOptions();
                    availableKindsWindow.Title = (returnValues.Item1);
                    availableKindsListView.SetSource(returnValues.Item2.Select(x => x.TableView()).ToList());
                    if (!string.IsNullOrWhiteSpace(commandPromptTextField.Text.ToString()))
                    {
                        var currentListObect = ((List<String>)availableKindsListView.Source.ToList()).Find(x => x.StartsWith(commandPromptTextField.Text.ToString()));
                        if (!string.IsNullOrWhiteSpace(currentListObect))
                        {
                            availableKindsListView.SelectedItem = availableKindsListView.Source.ToList().IndexOf(currentListObect);
                        }
                    };
                }
            }
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
                            new OptionsSlimType() { name = "new",isCommand = true, displayType = "create new object" },
                            new OptionsSlimType() { name = "delete",isCommand = true, displayType = "delete object" },
                            new OptionsSlimType() { name = "select",isCommand = true, displayType = "select object" },
                            new OptionsSlimType() { name = "list" ,isCommand = true, displayType = "list current objects" },
                            new OptionsSlimType() { name = "exit",isCommand = true, displayType = "exit" },
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
                            new OptionsSlimType() { name = "..", isCommand = true, displayType = "back one folder" },
                            new OptionsSlimType() { name = "/" , isCommand = true, displayType = "back to root"}
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
                            new OptionsSlimType() { name = "..", isCommand = true, displayType = "back one folder" },
                            new OptionsSlimType() { name = "/" , isCommand = true, displayType = "back to root"}
                        };
                    }
                    if (tmpObject.IsList())
                    {
                        tmpAvailableOptions = new List<OptionsSlimType>() {
                            new OptionsSlimType() { name = "..", isCommand = true, displayType = "back one folder" },
                            new OptionsSlimType() { name = "/" , isCommand = true, displayType = "back to root"},
                            new OptionsSlimType() { name = "new",isCommand = true, displayType = "create new object" },
                            new OptionsSlimType() { name = "delete",isCommand = true, displayType = "delete object" },
                            new OptionsSlimType() { name = "select",isCommand = true, displayType = "select object" },
                            new OptionsSlimType() { name = "list" ,isCommand = true, displayType = "list current objects" }
                        };
                        UpdateAvailableOptions();

                    }
                    else
                    {
                        tmpAvailableOptions.AddRange(tmpObject.RetrieveAttributeValues().ToList());
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
            static void repositionCommandInput()
            {
                commandPromptLabel.Text = string.Join(":", GlobalVariables.promptArray) + ">";
                commandPromptLabel.Width = commandPromptLabel.Text.Count() + 1;
                commandPromptTextField.X = Pos.Right(commandPromptLabel);
            }
            static void drawYAML()
            {
                definedYAMLWindow.Text = "";
                if (GlobalVariables.promptArray.Count() > 1)
                {
                    object currentSelectedKind = GlobalVariables.sessionDefinedKinds.FirstOrDefault(x => x.index == int.Parse(GlobalVariables.promptArray[1])).KubeObject;
                    var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull).Build();
                    List<string> jsonList = serializer.Serialize(currentSelectedKind).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
                    definedYAMLListView.SetSourceAsync(jsonList);
                    definedYAMLWindow.Title = $"{GlobalVariables.sessionDefinedKinds.FirstOrDefault(x => x.index == int.Parse(GlobalVariables.promptArray[1])).kind} YAML";
                }
                else
                {
                    definedYAMLListView.SetSourceAsync(new List<string>());
                }
            }
            Application.Run(topLevelWindowObject);
        }
        public static void UpdateAvailableOptions()
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
            definedYAMLListView.SetSourceAsync(outputList);
            definedYAMLWindow.Title = "Available kinds to choose from";

        }
        static bool Quit()
        {
            int selectedOption = MessageBox.Query(50, 5, "Quit", "Are you sure you want to quit k8config?", "Yes", "No");
            return (selectedOption == 0);

        }
        public static void Open()
        {
            var d = new OpenDialog("Open", "Open a YMAL file", new List<string>() { "yaml" }) { AllowsMultipleSelection = false };
            Application.Run(d);

            if (!d.Canceled)
            {
                YAMLHandeling.DeserializeFile(d.FilePath.ToString());
                messageBarItem.Text = $"YAML file loaded with {GlobalVariables.sessionDefinedKinds.Count()} definitions";
            }
        }
        public static void Save()
        {

            var d = new SaveDialog("Save", "Save to YAML file", new List<string>() { "yaml" });
            Application.Run(d);

            if (!d.Canceled)
            {
                var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull).Build();
                GlobalVariables.sessionDefinedKinds.Select(x => x.KubeObject);
                using (StreamWriter sw = new StreamWriter(d.FilePath.ToString()))
                {
                    foreach (object _kubeobject in GlobalVariables.sessionDefinedKinds.Select(x => x.KubeObject))
                    {
                        serializer.Serialize(sw, _kubeobject);
                        if (!_kubeobject.Equals(GlobalVariables.sessionDefinedKinds.Last().KubeObject))
                        {
                            sw.WriteLine("---");
                        }
                    }
                }
                messageBarItem.Text = $"YAML file writen to {d.FilePath}";
            }
        }
    }
}
