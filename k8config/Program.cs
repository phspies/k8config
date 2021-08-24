using k8config.DataModels;
using k8config.Utilities;
using k8s;
using k8s.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terminal.Gui;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace k8config
{
    class Program
    {
        static string autoCompleteInterruptText = "";
        static int autoCompleteInterruptIndex = 0;
        static bool autoCompleteMode = false;
        static ListView availableKindsList = new ListView();
        static Label commandPromptLabel = new Label();
        static TextField commandPromptTextField = new TextField("");
        static Window definedYAMLWindow = new Window();
        static Window availableKindsPopup = new Window();
        static void Main(string[] args)
        {
            AssemblySubsystem.BuildAvailableAssemblyList();
            Application.Init();
            var top = Application.Top;
            top.TabStop = true;
            top.ColorScheme.Normal = new Terminal.Gui.Attribute(Color.Black, Color.White);

            definedYAMLWindow = new Window() { Title = "Selected Definition", X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill() - 11 };
            var commandWindow = new Window() { Title = "Command Line", Y = Pos.Bottom(definedYAMLWindow) + 1, Width = Dim.Fill(), Height = 3 };
            Label messageBarItem = new Label("This is a test");
            top.DrawContent += (e) => { commandWindow.Y = top.Bounds.Height - 4; };
            top.Add(commandWindow, definedYAMLWindow);

            commandPromptLabel = new Label(string.Join(":", GlobalVariables.promptArray) + ">") { TextAlignment = TextAlignment.Left, X = 1 };
            commandPromptTextField = new TextField("") { X = Pos.Right(commandPromptLabel) + 1, Width = Dim.Fill(), TabStop = true };
            availableKindsPopup = new Window() { Title = "Available Commands", TabStop = false, CanFocus = false };
            availableKindsPopup.Y = Pos.Top(commandWindow) - 7;
            availableKindsPopup.Width = Dim.Fill();
            availableKindsPopup.Height = 7;
            top.Add(availableKindsPopup);
            availableKindsList = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                AllowsMultipleSelection = false,
                AllowsMarking = false,
                CanFocus = false,
                TabStop = false
            };
            availableKindsPopup.Add(availableKindsList);

            messageBarItem.ColorScheme = new ColorScheme() { Normal = new Terminal.Gui.Attribute(Color.Red, Color.White) };
            messageBarItem.Width = Dim.Fill();
            messageBarItem.Text = " No definitions found";
            messageBarItem.CanFocus = false;
            messageBarItem.Y = Pos.Bottom(commandWindow);

            top.Add(messageBarItem);

            updateAvailableKindsList();

            commandPromptTextField.KeyPress += (e) =>
            {
                if (e.KeyEvent.KeyValue == 1048599 && !string.IsNullOrEmpty(commandPromptTextField.Text.ToString()))
                {
                    List<string> possibleOptions = new List<string>();
                    string[] args = commandPromptTextField.Text.ToString().Split();
                    if (args.Count() > 1)
                    {
                        if (String.IsNullOrEmpty(autoCompleteInterruptText)) autoCompleteInterruptText = args[1];
                        possibleOptions = retrieveAvailableOptions(true, false, autoCompleteInterruptText).Item2.Where(x => x.name.StartsWith(autoCompleteInterruptText)).Select(x => x.name).ToList();
                        availableKindsList.SetSource(possibleOptions.ToList());
                        commandPromptTextField.Text = $"{args[0]} {NextAutoComplete(autoCompleteInterruptText, possibleOptions)}";
                    }
                    else if (args.Count() == 1 && args[0] != "")
                    {
                        if (String.IsNullOrEmpty(autoCompleteInterruptText)) autoCompleteInterruptText = args[0];
                        possibleOptions = retrieveAvailableOptions(true, false, autoCompleteInterruptText).Item2.Where(x => x.name.StartsWith(autoCompleteInterruptText)).Select(x => x.name).ToList();
                        availableKindsList.SetSource(possibleOptions.ToList());
                        commandPromptTextField.Text = $"{NextAutoComplete(autoCompleteInterruptText, possibleOptions)}";
                    }
                    else
                    {
                        autoCompleteInterruptText = "";
                        possibleOptions = retrieveAvailableOptions(false).Item2.ToList().Select(x=> $"{x.name} [{x.displayType}]").ToList();
                        availableKindsList.SetSource(possibleOptions.ToList());
                    }


                    //if (possibleOptions.Count != 0)
                    //{
                    //    for (int index = 0; possibleOptions.Count() > (index - 1); index++)
                    //    {
                    //        //availableKindsList(); need to set the focus of list view to the current displayed autocomplete option shown
                    //        if (possibleOptions[index] == optionAvailable)
                    //        { break; }
                    //    }
                    //}
                    e.Handled = true;
                }
            };
            commandPromptTextField.KeyUp += (e) =>
            {
                if (e.KeyEvent.KeyValue == 10 && !string.IsNullOrEmpty(commandPromptTextField.Text.ToString()))
                {
                    string currentInputText = commandPromptTextField.Text.ToString();
                    //direct commands
                    if (currentInputText == "..")
                    {
                        if (GlobalVariables.promptArray.Count() > 1)
                        {
                            GlobalVariables.promptArray.Remove(GlobalVariables.promptArray.Last());
                            repositionCommandInput();
                            drawYAML();
                        }
                    }
                    else if (GlobalVariables.sessionDefinedKinds.Count() > 0 && currentInputText == "list")
                    {
                        if (GlobalVariables.promptArray.Count() == 1)
                        {
                            GlobalVariables.sessionDefinedKinds.ForEach(x => definedYAMLWindow.Text += $"[{x.index}] {x.name} {x.kind}\n");
                        }
                        else if (KubeObject.GetCurrentObject().IsListType())
                        {
                            definedYAMLWindow.Text = "";
                            KubeObject.GetNestedList(KubeObject.GetCurrentObject()).ForEach(x =>
                            {
                                definedYAMLWindow.Text += $"[{x.index}] {x.name}\n";
                            });
                        }
                    }
                    else if (currentInputText == "new")
                    {
                        //add new object to the List<T> object
                        object currentObject = KubeObject.GetCurrentObject();
                        if (currentObject.IsListType())
                        {
                            Type selectListType = currentObject.GetType().GetTypeInfo().GetGenericArguments()[0];
                            currentObject.GetType().GetMethod("Add").Invoke(currentObject, new[] { Activator.CreateInstance(selectListType) });
                            messageBarItem.Text = $" {selectListType.Name} added to {GlobalVariables.promptArray.Last()}";
                            GlobalVariables.promptArray.Add(KubeObject.GetNestedList(currentObject).Last().index.ToString());
                            repositionCommandInput();
                            drawYAML();
                        }
                    }
                    else
                    {
                        //inline value setting command
                        if (currentInputText.Split("=").Count() > 1)
                        {
                            string[] args = commandPromptTextField.Text.ToString().CommandToArray();


                        }
                        //inline type commands
                        if (currentInputText.Split(" ").Count() > 1)
                        {
                            string[] args = commandPromptTextField.Text.ToString().Split();
                            if (args[0] == "new")
                            {
                                if (GlobalVariables.availableKubeTypes.Exists(x => x.kind == args[1]))
                                {
                                    var kubeObject = GlobalVariables.availableKubeTypes.FirstOrDefault(x => x.kind == args[1]);
                                    Type currentType = Type.GetType(GlobalVariables.availableKubeTypes.FirstOrDefault(x => x.kind == args[1]).assemblyFullName);
                                    object _object = Activator.CreateInstance(currentType);
                                    _object.GetType().GetProperty("Kind").SetValue(_object, kubeObject.kind);
                                    _object.GetType().GetProperty("ApiVersion").SetValue(_object, $"{kubeObject.group}/{kubeObject.version}");
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
                                    messageBarItem.Text = $" {newSessionKind.kind} created and selected";
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
                                            messageBarItem.Text = $" {selectedKind.kind}{(string.IsNullOrEmpty(selectedKind.name) ? "" : selectedKind.name)} selected";
                                            GlobalVariables.promptArray.Add(index.ToString());
                                            repositionCommandInput();
                                            drawYAML();
                                        }
                                        else
                                        {
                                            messageBarItem.Text = " Item not found";
                                        }
                                    }
                                    else if (KubeObject.DoesNestedIndexExist(KubeObject.GetCurrentObject(), index))
                                    {
                                        messageBarItem.Text = $" {index} selected";
                                        GlobalVariables.promptArray.Add(index.ToString());
                                        repositionCommandInput();
                                        drawYAML();
                                    }
                                    else
                                    {
                                        messageBarItem.Text = $" {index} not found";
                                    }
                                }
                                else
                                {
                                    messageBarItem.Text = $" Value [{args[1]}] entered is not a integer";
                                }
                            }
                            else if (args[0] == "delete")
                            {
                                int index = 0;
                                if (int.TryParse(args[1], out index))
                                {
                                    if (GlobalVariables.sessionDefinedKinds.Exists(x => x.index == index))
                                    {
                                        GlobalVariables.sessionDefinedKinds.Remove(GlobalVariables.sessionDefinedKinds.FirstOrDefault(x => x.index == index));
                                        messageBarItem.Text = " Definition deleted";
                                        repositionCommandInput();
                                        drawYAML();
                                    }
                                    else
                                    {
                                        messageBarItem.Text = " Item not found";
                                    }
                                }
                                else
                                {
                                    messageBarItem.Text = " Value entered is not a integer";
                                }
                            }

                            else
                            {
                                messageBarItem.Text = " Command not found";
                            }
                        }
                        else
                        {
                            if (retrieveAvailableOptions(false, false).Item2.Exists(x => x.name == currentInputText))
                            {
                                object tmpObject = KubeObject.GetCurrentObject();
                                if (tmpObject.RetrieveAttributeValues().Exists(x => x.name.ToLower() == currentInputText.ToLower()))
                                {
                                    var currentObject = tmpObject.GetType().GetProperties().ToList().FirstOrDefault(x => x.Name.ToLower() == currentInputText.ToLower());
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
                            else
                            {
                                messageBarItem.Text = " Command not found";
                            }
                        }
                    }
                    commandPromptTextField.Text = "";
                    autoCompleteInterruptText = "";

                }
                updateAvailableKindsList();
            };
            commandWindow.Add(
                commandPromptLabel, commandPromptTextField
                );

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
                Tuple<string, List<OptionsSlimType>> returnValues = retrieveAvailableOptions();
                availableKindsPopup.Title = (returnValues.Item1);
                availableKindsList.SetSourceAsync(returnValues.Item2.Select(x => $"{x.name}").ToList());
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
                            new OptionsSlimType() { name = "new" },
                            new OptionsSlimType() { name = "delete" },
                            new OptionsSlimType() { name = "select" },
                            new OptionsSlimType() { name = "list" },
                            new OptionsSlimType() { name = "exit" },
                        };
                    }
                }
                if (GlobalVariables.promptArray.Count() == 2)
                {
                    returnHeader = "Available Options";
                    if (includeCommands) tmpAvailableOptions.Add(new OptionsSlimType() { name = ".." });
                    tmpAvailableOptions.AddRange(GlobalVariables.sessionDefinedKinds[int.Parse(GlobalVariables.promptArray[1]) - 1].KubeObject.RetrieveAttributeValues().Select(x => new OptionsSlimType() { name = x.name.ToLower() }).ToList());
                }
                else if (GlobalVariables.promptArray.Count() > 2)
                {
                    object tmpObject = KubeObject.GetCurrentObject();
                    if (includeCommands) tmpAvailableOptions.Add(new OptionsSlimType() { name = ".." });
                    if (tmpObject.IsListType())
                    {
                        tmpAvailableOptions = new List<OptionsSlimType>() {
                            new OptionsSlimType() { name = "new" },
                            new OptionsSlimType() { name = "delete" },
                            new OptionsSlimType() { name = "select" },
                            new OptionsSlimType() { name = "list" },
                        };
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
                    SessionDefinedKind currentSelectedKind = GlobalVariables.sessionDefinedKinds.FirstOrDefault(x => x.index == int.Parse(GlobalVariables.promptArray[1]));
                    definedYAMLWindow.Text = JsonConvert.SerializeObject(currentSelectedKind.KubeObject, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }
                else
                {
                    definedYAMLWindow.Text = "";
                }
            }


            Application.Run();

            //    ReadLine.AutoCompletionHandler = new AutoCompletionHandler();
            //    ReadLine.HistoryEnabled = true;


            //    GlobalVariables.availbleDefinitions["definitions"].SelectTokens("$..x-kubernetes-group-version-kind").ToList().ForEach(x =>
            //    {
            //        if ((x.First["group"] != null) && (x.First["kind"] != null) && (x.First["version"] != null) && (!((JProperty)(x.Parent.Parent.Parent)).Name.Contains(".apis.apiextensions")))
            //        {
            //            GlobalVariables.groupSourceKinds.Add(BuildSourceRootObject(x.Parent.Parent));
            //        }
            //    });

            //    while (true)
            //    {
            //        string input = ReadLine.Read(string.Join(":", GlobalVariables.promptArray) + ">");
            //        if (GlobalVariables.valuePromptMode)
            //        {
            //            switch (GlobalVariables.promptArray.Last())
            //            {
            //                case "comment":
            //                    GlobalVariables.promptArray.Remove(GlobalVariables.promptArray.Last());
            //                    FindTargetDefinedObject().comment = input;
            //                    break;
            //                case "select":
            //                    GlobalVariables.promptArray.Remove(GlobalVariables.promptArray.Last());
            //                    int index;
            //                    if (int.TryParse(input, out index))
            //                    {
            //                        if (GlobalVariables.promptArray.Count == 1)
            //                        {
            //                            if (GlobalVariables.groupTargetKinds.Exists(x => x.index == index))
            //                            {
            //                                var selectedDefined = GlobalVariables.groupTargetKinds.FirstOrDefault(x => x.index == index);
            //                                GlobalVariables.promptArray.Add(index.ToString());
            //                                WriteOutput.WriteInformationLine($"Selected [{selectedDefined.index}] {selectedDefined.comment}");
            //                            }
            //                            else
            //                            {
            //                                WriteOutput.WriteErrorLine("Definition does not exist");
            //                            }
            //                        }
            //                        else if (FindTargetDefinedObject().format == FieldFormat.Array)
            //                        {
            //                            var _tempObject = FindTargetDefinedObject();
            //                            if (_tempObject.properties.Count > 0)
            //                            {
            //                                var selectedDefined = _tempObject.properties.FirstOrDefault(x => x.index == index);
            //                                GlobalVariables.promptArray.Add(index.ToString());
            //                                WriteOutput.WriteInformationLine($"Selected [{selectedDefined.index}] {selectedDefined.comment}");
            //                            }
            //                            else
            //                            {
            //                                WriteOutput.WriteErrorLine("No objects defined");
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        WriteOutput.WriteErrorLine("Value not an integer");
            //                    }
            //                    break;
            //                default:
            //                    TargetGroupType _definedObject = FindTargetDefinedObject();
            //                    GlobalVariables.promptArray.Remove(GlobalVariables.promptArray.Last());
            //                    switch (_definedObject.type)
            //                    {
            //                        case FieldType.Integer:
            //                            Int32 _tempValue;
            //                            if (Int32.TryParse(input, out _tempValue))
            //                            {
            //                                _definedObject.value = _tempValue.ToString();
            //                            }
            //                            else
            //                            {
            //                                WriteOutput.WriteErrorLine("value entered is not a int32 value");
            //                            }
            //                            break;
            //                        case FieldType.String:
            //                            _definedObject.value = input;
            //                            break;
            //                        case FieldType.Boolean:
            //                            bool _tempBool;
            //                            if (Boolean.TryParse(input, out _tempBool))
            //                            {
            //                                _definedObject.value = _tempBool.ToString().ToLower();
            //                            }
            //                            else
            //                            {
            //                                WriteOutput.WriteErrorLine("value entered is not a boolean value");
            //                            }
            //                            break;

            //                    }
            //                    break;
            //            }
            //            GlobalVariables.valuePromptMode = false;
            //            continue;
            //        }
            //        else
            //        {
            //            switch (input)
            //            {
            //                case "print":
            //                case "p":
            //                    var outputObj = new ConstructOutputYAML();
            //                    outputObj.Build();
            //                    outputObj.PrintYAML();
            //                    break;
            //                case "new":
            //                    if (GlobalVariables.promptArray.Count == 1)
            //                    {
            //                        WriteOutput.WriteInformationLine("Creating new definition");
            //                        initPrompt();
            //                        if (GlobalVariables.groupTargetKinds.Count == 0)
            //                        {
            //                            GlobalVariables.groupTargetKinds.Add(new TargetGroupType() { rootObject = true, index = 0 });
            //                        }
            //                        else
            //                        {
            //                            GlobalVariables.groupTargetKinds.Add(new TargetGroupType() { rootObject = true, index = GlobalVariables.groupTargetKinds.Last().index + 1 });
            //                        }
            //                        GlobalVariables.promptArray.Add(GlobalVariables.groupTargetKinds.OrderBy(x => x.index).Last().index.ToString());
            //                    }
            //                    else if (FindTargetDefinedObject().type == FieldType.Array)
            //                    {
            //                        var currentObject = FindTargetDefinedObject();
            //                        if (currentObject.properties.Count == 0)
            //                        {
            //                            currentObject.properties.Add(new TargetGroupType() { index = 0, format = FieldFormat.Item });
            //                        }
            //                        else
            //                        {
            //                            currentObject.properties.Add(new TargetGroupType() { index = currentObject.properties.Last().index + 1, format = FieldFormat.Item });
            //                        }
            //                        GlobalVariables.promptArray.Add(currentObject.properties.OrderBy(x => x.index).Last().index.ToString());
            //                    }
            //                    break;
            //                case "select":
            //                    GlobalVariables.valuePromptMode = true;
            //                    GlobalVariables.promptArray.Add("select");
            //                    break;
            //                case "list":
            //                    if (GlobalVariables.promptArray.Count() == 1)
            //                    {
            //                        if (GlobalVariables.groupTargetKinds.Count > 0)
            //                        {
            //                            WriteOutput.WriteInformationLine("Current definitions");
            //                            GlobalVariables.groupTargetKinds.ForEach(kind =>
            //                            {
            //                                WriteOutput.WriteInformationLine($"> [{kind.index}] {kind.name} : {kind.comment}");
            //                            });
            //                        }
            //                        else
            //                        {
            //                            WriteOutput.WriteErrorLine("No definitions, please create one");
            //                        }
            //                    }
            //                    else if (FindTargetDefinedObject().format == FieldFormat.Array)
            //                    {
            //                        var _tempObject = FindTargetDefinedObject();
            //                        if (_tempObject.properties.Count > 0)
            //                        {
            //                            string _lineprop = "";
            //                            _tempObject.properties.ForEach(prop =>
            //                            {
            //                                if (prop.properties.Exists(x => !string.IsNullOrEmpty(x.name)))
            //                                {
            //                                    _lineprop = prop.properties.FirstOrDefault(x => !string.IsNullOrEmpty(x.name)).value;
            //                                }
            //                                if (!string.IsNullOrEmpty(prop.comment))
            //                                {
            //                                    _lineprop = string.Format($"{_lineprop} : {prop.comment}");
            //                                }
            //                                WriteOutput.WriteInformationLine($"[{prop.index}] {_lineprop}");
            //                            });
            //                        }
            //                        else
            //                        {
            //                            WriteOutput.WriteErrorLine("No objects defined");
            //                        }
            //                    }
            //                    else
            //                    {
            //                        WriteOutput.WriteErrorLine($"> {input} not found");
            //                    }
            //                    break;
            //                case "?":
            //                    break;
            //                case "desc":
            //                case "description":
            //                    var _object = FindSourceNestedObject();
            //                    if (_object.type != FieldType.Object)
            //                    {
            //                        WriteOutput.WriteInformationLine($"> [{FindSourceNestedObject().type}] {FindSourceNestedObject().description}");
            //                    }
            //                    else
            //                    {
            //                        WriteOutput.WriteInformationLine($"> {FindSourceNestedObject().description}");
            //                    }
            //                    break;
            //                case "comment":
            //                    GlobalVariables.valuePromptMode = true;
            //                    GlobalVariables.promptArray.Add("comment");
            //                    break;
            //                case "exit":
            //                case "..":
            //                    if (GlobalVariables.promptArray.Count() == 1)
            //                    {
            //                        System.Environment.Exit(0);
            //                    }
            //                    GlobalVariables.promptArray.RemoveAt(GlobalVariables.promptArray.Count() - 1);
            //                    break;
            //                default:
            //                    if (!string.IsNullOrEmpty(input))
            //                    {
            //                        if (GetSourceAvailableOptions("", true).Exists(x => x.name == input))
            //                        {
            //                            GlobalVariables.promptArray.Add(input);
            //                            TargetGroupType currentDefinedGroup = FindTargetDefinedObject();
            //                            if (currentDefinedGroup.type == FieldType.String || currentDefinedGroup.type == FieldType.Boolean || currentDefinedGroup.type == FieldType.Integer)
            //                            {
            //                                GlobalVariables.valuePromptMode = true;
            //                            }
            //                            if (GlobalVariables.promptArray.Count() == 3)
            //                            {
            //                                SourceGroupType currentGroup = GlobalVariables.groupSourceKinds.Find(x => x.name == input);
            //                                currentDefinedGroup.kubedetails = new KubeObjectType() { group = currentGroup.kubedetails.FirstOrDefault().group, kind = currentGroup.kubedetails.FirstOrDefault().kind, version = currentGroup.kubedetails.FirstOrDefault().version };
            //                                currentDefinedGroup.name = currentDefinedGroup.kubedetails.kind;
            //                                currentDefinedGroup.type = FieldType.Object;
            //                                currentDefinedGroup.format = FieldFormat.Object;
            //                            }
            //                        }
            //                        else if (input.Split(" ").Count() > 1)
            //                        {
            //                            List<string> _options = input.Split(" ").ToList();
            //                            if (GlobalVariables.knownCommands.Exists(x => x == _options.First()))
            //                            {
            //                                TargetGroupType currentDefinedGroup = FindTargetDefinedObject();
            //                                int _index = 0;
            //                                switch (_options.First())
            //                                {
            //                                    case "delete":

            //                                        if (Int32.TryParse(_options[1], out _index))
            //                                        {
            //                                            if (currentDefinedGroup.rootObject && GlobalVariables.promptArray.Count() == 1)
            //                                            {
            //                                                if (GlobalVariables.groupTargetKinds.Exists(x => x.index == _index))
            //                                                {
            //                                                    GlobalVariables.groupTargetKinds.Remove(GlobalVariables.groupTargetKinds.Find(x => x.index == _index));
            //                                                    WriteOutput.WriteInformationLine($"Object {_index} deleted");
            //                                                }
            //                                                else
            //                                                {
            //                                                    WriteOutput.WriteErrorLine($"Object {_index} not found");
            //                                                }
            //                                            }
            //                                            else if (currentDefinedGroup.properties.Exists(x => x.index == _index))
            //                                            {
            //                                                currentDefinedGroup.properties.Remove(currentDefinedGroup.properties.Find(x => x.index == _index));
            //                                                WriteOutput.WriteInformationLine($"Object {_index} deleted");
            //                                            }
            //                                            else
            //                                            {
            //                                                WriteOutput.WriteErrorLine($"Object {_index} not found");
            //                                            }
            //                                        }
            //                                        else
            //                                        {
            //                                            WriteOutput.WriteInformationHighlightLine($"Value [{input}] is not a integer", ConsoleColor.Red);
            //                                        }
            //                                        break;
            //                                    case "select":
            //                                        if (Int32.TryParse(_options[1], out _index))
            //                                        {
            //                                            if (GlobalVariables.promptArray.Count == 1)
            //                                            {
            //                                                if (GlobalVariables.groupTargetKinds.Exists(x => x.index == _index))
            //                                                {
            //                                                    var selectedDefined = GlobalVariables.groupTargetKinds.FirstOrDefault(x => x.index == _index);
            //                                                    GlobalVariables.promptArray.Add(_index.ToString());
            //                                                    WriteOutput.WriteInformationLine($"Selected [{selectedDefined.index}] {selectedDefined.comment}");
            //                                                }
            //                                                else
            //                                                {
            //                                                    WriteOutput.WriteErrorLine("Definition does not exist");
            //                                                }
            //                                            }
            //                                            else if (FindTargetDefinedObject().type == FieldType.Array)
            //                                            {
            //                                                var _tempObject = FindTargetDefinedObject();
            //                                                if (_tempObject.properties.Count > 0)
            //                                                {
            //                                                    if (_tempObject.properties.Exists(x => x.index == _index))
            //                                                    {
            //                                                        var selectedDefined = _tempObject.properties.FirstOrDefault(x => x.index == _index);
            //                                                        GlobalVariables.promptArray.Add(_index.ToString());
            //                                                        WriteOutput.WriteInformationLine($"Selected [{selectedDefined.index}] {selectedDefined.comment}");
            //                                                    }
            //                                                    else
            //                                                    {
            //                                                        WriteOutput.WriteErrorLine($"Object [{_index}] does not exist");
            //                                                    }
            //                                                }
            //                                                else
            //                                                {
            //                                                    WriteOutput.WriteErrorLine("No objects defined");
            //                                                }
            //                                            }
            //                                        }
            //                                        else
            //                                        {
            //                                            WriteOutput.WriteInformationHighlightLine($"Value [{input}] is not a integer", ConsoleColor.Red);
            //                                        }
            //                                        break;
            //                                }
            //                            }
            //                        }
            //                        else
            //                        {
            //                            WriteOutput.WriteErrorLine($"> {input} not found");
            //                        }

            //                    }
            //                    break;
            //            }
            //        }
            //    }
            //}

            //static public SourceGroupType BuildSourceRootObject(JToken _json)
            //{
            //    SourceGroupType _prop = new SourceGroupType();
            //    _prop.name = ((JProperty)_json.Parent).Name.Split(".").Last();
            //    _prop.description = _json.SelectToken("description")?.Value<string>();
            //    _prop.kubedetails = new List<KubeObjectType>();
            //    _prop.rootObject = true;
            //    _prop.type = FieldType.Object;
            //    _prop.format = FieldFormat.Object;
            //    _json.SelectToken("x-kubernetes-group-version-kind").Children().ToList().ForEach(x =>
            //    {
            //        _prop.kubedetails.Add(new KubeObjectType()
            //        {
            //            group = x.SelectToken("..group")?.Value<string>(),
            //            kind = x.SelectToken("..kind")?.Value<string>(),
            //            version = x.SelectToken("..version")?.Value<string>(),
            //        });

            //    });
            //    _json["properties"].ToList().ForEach(_property =>
            //    {
            //        if (!GlobalVariables.ignoreProperties.Any(x => x == ((JProperty)_property).Name))
            //        {
            //            if (_property.SelectToken("$..$ref") != null)
            //            {
            //                var objectref = _property.SelectToken("$..$ref")?.Value<string>();
            //                _prop.properties.Add(new SourceGroupType()
            //                {
            //                    name = ((JProperty)_property).Name,
            //                    type = FieldType.Object,
            //                    format = FieldFormat.Object,
            //                    description = _property.SelectToken("$..description")?.Value<string>(),
            //                    properties = BuildSourcePropTree(GlobalVariables.availbleDefinitions["definitions"][(_property.SelectToken("$..$ref")?.Value<string>()).Split("/").Last()])
            //                }); ;
            //            }
            //        }
            //    });
            //    return _prop;
            //}

            //static public List<SourceGroupType> BuildSourcePropTree(JToken _json)
            //{
            //    List<SourceGroupType> _proptree = new List<SourceGroupType>();
            //    List<string> required = _json["required"]?.ToObject<string[]>().ToList();
            //    string description = _json["description"]?.Value<string>();
            //    _json.SelectTokens("$..properties").Children().ToList().ForEach(node =>
            //    {
            //        if (!GlobalVariables.ignoreProperties.Any(x => x == ((JProperty)node).Name))
            //        {
            //            SourceGroupType _prop = new SourceGroupType();
            //            _prop.name = ((JProperty)node).Name;
            //            if (required != null) { _prop.required = (bool)required?.Any(x => x == ((JProperty)node).Name); }
            //            _prop.description = (string)node.First["description"];
            //            _proptree.Add(_prop);
            //            var _ref = node.First["$ref"];
            //            if (node.First["$ref"] == null)
            //            {
            //                _prop.format = String.IsNullOrEmpty((string)(node.First["format"])) ? FieldFormat.Object : ParseEnum.ParseFormat((string)node.First["format"]);
            //                _prop.type = ParseEnum.ParseType((string)node.First["type"]);
            //                if (_prop.format == FieldFormat.Array || _prop.type == FieldType.Array)
            //                {
            //                    if (node.First["items"]["type"] != null)
            //                    {
            //                        _prop.type = ParseEnum.ParseType((string)node.First["items"]["type"]);
            //                        _prop.format = ParseEnum.ParseFormat((string)node.First["items"]["format"]);
            //                        _prop.properties.Add(new SourceGroupType() { type = ParseEnum.ParseType(node.First["items"]["type"].Value<string>()) });
            //                    }
            //                    else if (node.First["items"]["$ref"] != null)
            //                    {
            //                        _prop.properties = BuildSourcePropTree(GlobalVariables.availbleDefinitions["definitions"][node.First["items"]["$ref"]?.Value<string>().Split("/").Last()]);
            //                    }
            //                    else
            //                    {
            //                        _prop.type = ParseEnum.ParseType((string)node.First["items"]["type"]);
            //                        _prop.format = ParseEnum.ParseFormat((string)node.First["items"]["format"]);
            //                        _prop.properties = BuildSourcePropTree(GlobalVariables.availbleDefinitions["definitions"][node.First["items"]["$ref"]?.Value<string>().Split("/").Last()]);
            //                    }
            //                }
            //                else if (node.SelectToken("$..additionalProperties") != null)
            //                {

            //                    _prop.type = ParseEnum.ParseType((string)node.First["additionalProperties"]["type"]);
            //                    if (node.First["additionalProperties"]["$ref"] != null)
            //                    {
            //                        var test = node.First["additionalProperties"]["$ref"];
            //                        _prop.properties = BuildSourcePropTree(GlobalVariables.availbleDefinitions["definitions"][node.First["additionalProperties"]["$ref"]?.Value<string>().Split("/").Last()]);
            //                    }
            //                    _prop.format = FieldFormat.Map;
            //                }
            //            }
            //            else
            //            {
            //                _prop.properties = BuildSourcePropTree(GlobalVariables.availbleDefinitions["definitions"][node.First["$ref"]?.Value<string>().Split("/").Last()]);
            //            }
            //        }
            //    });
            //    return _proptree;
            //}




            //class AutoCompletionHandler : IAutoCompleteHandler
            //{
            //    // characters to start completion from
            //    public char[] Separators { get; set; } = new char[] { ' ', '.', '/' };

            //    // text - The current text entered in the console
            //    // index - The index of the terminal cursor within {text}
            //    public string[] GetSuggestions(string text, int index)
            //    {
            //        return FindAvailableOptions(text).Select(x => x.name).ToArray();
            //    }
            //}
            //public static List<GroupSourceSlimType> FindAvailableOptions(string text)
            //{
            //    var _currentObject = FindSourceNestedObject();
            //    if (_currentObject.type == FieldType.Array && !int.TryParse(GlobalVariables.promptArray.Last(), out _))
            //    {
            //        List<GroupSourceSlimType> tempOptions = new List<GroupSourceSlimType>();
            //        //GlobalVariables.knownCommands.Where(x => x.StartsWith(text)).ForEach(x => tempOptions.Add(new GroupSourceSlimType() { name = x }));
            //        return tempOptions;
            //    }
            //    else
            //    {
            //        return GetSourceAvailableOptions(text);
            //    }
            //}
            //protected static void CtlrCHandler(object sender, ConsoleCancelEventArgs args)
            //{
            //    initPrompt();
            //    args.Cancel = true;
            //}
            //static public List<GroupSourceSlimType> GetSourceAvailableOptions(string text, bool includeCommands = false)
            //{
            //    SourceGroupType currentObject = FindSourceNestedObject();
            //    List<GroupSourceSlimType> options = new List<GroupSourceSlimType>();
            //    if (GlobalVariables.promptArray.Count() == 1)
            //    {
            //        GlobalVariables.rootKnownCommands.ForEach(x => options.Add(new GroupSourceSlimType() { name = x }));
            //    }
            //    if (GlobalVariables.promptArray.Count() == 2)
            //    {
            //        //GlobalVariables.groupSourceKinds.Where(x => x.name.StartsWith(text)).ForEach(x => options.Add(new GroupSourceSlimType() { name = x.name, isRequired = x.required, type = x.type, format = x.format }));
            //    }
            //    else if (currentObject.format == FieldFormat.Object && currentObject.type == FieldType.String)
            //    {

            //    }
            //    else
            //    {
            //        //FindSourceNestedObject().properties.Where(x => x.name.StartsWith(text)).ForEach(x => options.Add(new GroupSourceSlimType() { name = x.name, isRequired = x.required, type = x.type, format = x.format }));
            //    }
            //    if (includeCommands == true)
            //    {
            //        GlobalVariables.knownCommands.ForEach(x => options.Add(new GroupSourceSlimType() { name = x }));
            //    }
            //    return options;
            //}
            //static public SourceGroupType FindSourceNestedObject()
            //{
            //    SourceGroupType tempGroupType = new SourceGroupType() { description = "You must select one root object" };
            //    if (GlobalVariables.promptArray.Count() > 2)
            //    {
            //        tempGroupType = GlobalVariables.groupSourceKinds.FirstOrDefault(x => x.name == GlobalVariables.promptArray[2]);
            //        if (GlobalVariables.promptArray.Count() > 3)
            //        {
            //            for (int index = 3; index < (GlobalVariables.promptArray.Count()); index++)
            //            {
            //                //if we find a array identifier, just to the next value in prompt array
            //                if (int.TryParse(GlobalVariables.promptArray[index], out _))
            //                {
            //                    continue;
            //                }
            //                tempGroupType = tempGroupType.properties.FirstOrDefault(x => x.name == GlobalVariables.promptArray[index]);
            //            }
            //        }
            //    }
            //    if (tempGroupType == null)
            //    {
            //        throw new Exception("This object cannot be empty");
            //    }
            //    return tempGroupType;
            //}
            //static public TargetGroupType FindTargetDefinedObject()
            //{
            //    TargetGroupType tempGroupType = new TargetGroupType() { rootObject = true };
            //    if (GlobalVariables.promptArray.Count() > 1)
            //    {
            //        tempGroupType = GlobalVariables.groupTargetKinds.Find(x => x.index == int.Parse(GlobalVariables.promptArray[1]));
            //    }
            //    if (GlobalVariables.promptArray.Count() > 2)
            //    {
            //        for (int index = 3; index < GlobalVariables.promptArray.Count(); index++)
            //        {
            //            //skip index if value is a index number
            //            int indexNumber = 0;
            //            if (int.TryParse(GlobalVariables.promptArray[index], out indexNumber))
            //            {
            //                if (!tempGroupType.properties.Exists(x => x.index == indexNumber))
            //                {
            //                    var nestedSourceObject = FindSourceNestedObject();
            //                    tempGroupType.properties.Add(new TargetGroupType()
            //                    {
            //                        required = nestedSourceObject.required,
            //                        format = nestedSourceObject.format,
            //                        type = nestedSourceObject.type
            //                    }); ;
            //                }
            //                tempGroupType = tempGroupType.properties.FirstOrDefault(x => x.index == indexNumber);
            //            }
            //            else
            //            {
            //                if (!tempGroupType.properties.Exists(x => x.name == GlobalVariables.promptArray[index]))
            //                {
            //                    var nestedSourceObject = FindSourceNestedObject();
            //                    tempGroupType.properties.Add(new TargetGroupType()
            //                    {
            //                        name = nestedSourceObject.name,
            //                        required = nestedSourceObject.required,
            //                        format = nestedSourceObject.format,
            //                        type = nestedSourceObject.type
            //                    });
            //                }
            //                tempGroupType = tempGroupType.properties.FirstOrDefault(x => x.name == GlobalVariables.promptArray[index]);
            //            }
            //        }
            //    }
            //    return tempGroupType;
        }


    }
}
