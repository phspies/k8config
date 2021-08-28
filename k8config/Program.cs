using k8config.DataModels;
using k8config.Utilities;
using k8s;
using k8s.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Terminal.Gui;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace k8config
{
    class Program
    {


        static string autoCompleteInterruptText = "";
        static int autoCompleteInterruptIndex = 0;
        static ListView availableKindsList = new ListView();
        static Label commandPromptLabel = new Label();
        static TextField commandPromptTextField = new TextField("");
        static Window definedYAMLWindow = new Window();
        static Window availableKindsPopup = new Window();
        static ListView definedYAMLList = new ListView();
        static bool currentavailableListUpDown = false;

        
        static void Main(string[] args)
        {
            AssemblySubsystem.BuildAvailableAssemblyList();

            YAMLHandeling.DeserializeFile(@"C:\Users\Phillip\mysql.yaml");


  
            Application.Init();
            var top = Application.Top;

            top.TabStop = true;
            top.ColorScheme.Normal = new Terminal.Gui.Attribute(Color.Black, Color.White);

            availableKindsPopup = new Window()
            {
                Title = "Available Commands",
                X = 0,
                Y = 0,
                Width = 50,
                Height = Dim.Fill() - 4,
                TabStop = false,
                CanFocus = false
            };
            definedYAMLWindow = new Window()
            {
                Title = "Selected Definition",
                X = availableKindsPopup.Bounds.Right,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 4,
                TabStop = false,
                CanFocus = false
            };
            var commandWindow = new Window()
            {
                Title = "Command Line",
                Y = definedYAMLWindow.Bounds.Bottom + 1,
                Width = Dim.Fill(),
                Height = 3
            };
            top.DrawContent += (e) =>
            {
                availableKindsPopup.Width = 50;
                definedYAMLWindow.X = availableKindsPopup.Bounds.Right;
                commandWindow.Y = definedYAMLWindow.Bounds.Bottom;
            };

            Label messageBarItem = new Label("");
            top.Add(commandWindow, definedYAMLWindow);

            commandPromptLabel = new Label(string.Join(":", GlobalVariables.promptArray) + ">") { TextAlignment = TextAlignment.Left, X = 1 };
            commandPromptTextField = new TextField("") { X = Pos.Right(commandPromptLabel) + 1, Width = Dim.Fill(), TabStop = true };

            top.Add(availableKindsPopup);
            definedYAMLList = new ListView()
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
            definedYAMLWindow.Add(definedYAMLList);
            availableKindsList = new ListView()
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

            availableKindsPopup.Add(availableKindsList);
            availableKindsList.ColorScheme = new ColorScheme()
            {
                Normal = new Terminal.Gui.Attribute(Color.Black, Color.White),
                Focus = new Terminal.Gui.Attribute(Color.White, Color.Cyan),
                HotNormal = new Terminal.Gui.Attribute(Color.White, Color.Red),
                HotFocus = new Terminal.Gui.Attribute(Color.Blue, Color.Cyan),
                Disabled = new Terminal.Gui.Attribute(Color.DarkGray, Color.Black)
            };
            messageBarItem.ColorScheme = new ColorScheme() { Normal = new Terminal.Gui.Attribute(Color.Red, Color.White) };
            messageBarItem.Width = Dim.Fill();
            messageBarItem.Text = " No definitions found";
            messageBarItem.CanFocus = false;
            messageBarItem.Y = Pos.Bottom(commandWindow);

            top.Add(messageBarItem);

            updateAvailableKindsList();

            top.KeyUp += (e) =>
            {
                if (e.KeyEvent.Key == Key.CursorDown || e.KeyEvent.Key == Key.CursorUp)
                {
                    string currentInputText = commandPromptTextField.Text.ToString();
                    switch (e.KeyEvent.Key)
                    {
                        case Key.CursorUp:
                            currentavailableListUpDown = true;
                            if (availableKindsList.SelectedItem > 0)
                            {
                                availableKindsList.SelectedItem = availableKindsList.SelectedItem - 1;
                                availableKindsList.TopItem = availableKindsList.SelectedItem;
                            }
                            break;
                        case Key.CursorDown:
                            currentavailableListUpDown = true;
                            if (availableKindsList.SelectedItem < availableKindsList.Source.ToList().Count - 1)
                            {
                                availableKindsList.SelectedItem = availableKindsList.SelectedItem + 1;
                                availableKindsList.TopItem = availableKindsList.SelectedItem;
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
                if (e.KeyEvent.Key == Key.Tab && !string.IsNullOrEmpty(commandPromptTextField.Text.ToString()))
                {
                    currentavailableListUpDown = false;
                    List<string> possibleOptions = new List<string>();
                    string[] args = currentInputText.Split();
                    if (GlobalVariables.promptArray.Count() == 1 && args.Count() > 1)
                    {
                        if (String.IsNullOrEmpty(autoCompleteInterruptText)) autoCompleteInterruptText = args[1];
                        possibleOptions = retrieveAvailableOptions(true, false, autoCompleteInterruptText).Item2.Where(x => x.name.StartsWith(autoCompleteInterruptText)).Select(x => x.name).ToList();
                        availableKindsList.SetSource(possibleOptions.ToList());
                        commandPromptTextField.Text = $"{args[0]} {NextAutoComplete(autoCompleteInterruptText, possibleOptions)}";
                        commandPromptTextField.CursorPosition = currentInputText.Length;

                    }
                    else if (args.Count() == 1 && args[0] != "")
                    {
                        if (String.IsNullOrEmpty(autoCompleteInterruptText)) autoCompleteInterruptText = args[0];
                        possibleOptions = retrieveAvailableOptions(true, false, autoCompleteInterruptText).Item2.Where(x => x.name.StartsWith(autoCompleteInterruptText)).Select(x => x.name).ToList();
                        availableKindsList.SetSource(possibleOptions.ToList());
                        commandPromptTextField.Text = $"{NextAutoComplete(autoCompleteInterruptText, possibleOptions)}";
                        commandPromptTextField.CursorPosition = currentInputText.Length;
                    }
                    else
                    {
                        autoCompleteInterruptText = "";
                        possibleOptions = retrieveAvailableOptions(false).Item2.ToList().Select(x => x.TableView()).ToList();
                        availableKindsList.SetSource(possibleOptions.ToList());
                    }

                    e.Handled = true;
                }

            };
            commandPromptTextField.KeyUp += (e) =>
            {
                if (e.KeyEvent.Key == Key.Enter)
                {
                    string currentInputText = commandPromptTextField.Text.ToString();

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
                                messageBarItem.Text = " The current object is not a list";
                            }
                            definedYAMLList.SetSourceAsync(outputList);
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
                                    messageBarItem.Text = $" {selectListTypes[0].Name} added to {GlobalVariables.promptArray.Last()}";
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
                                            messageBarItem.Text = $" wrong amount of variable supplied. {selectListTypes.Length} variables expected";
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
                                        if (GlobalVariables.promptArray.Count() == 1 && GlobalVariables.sessionDefinedKinds.Exists(x => x.index == index))
                                        {
                                            GlobalVariables.sessionDefinedKinds.Remove(GlobalVariables.sessionDefinedKinds.FirstOrDefault(x => x.index == index));
                                            messageBarItem.Text = " Definition deleted";
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
                                            messageBarItem.Text = " Item not found";
                                        }
                                    }
                                    else
                                    {
                                        messageBarItem.Text = " Value entered is not a integer";
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
                                                    tmpArray += $" {args[i]}";
                                                }
                                                string[] arrayVars = tmpArray.Trim().Split("|");
                                                arrayVars.ForEach(x => x.Trim());
                                                propertyInfo.SetValue(tmpObject, arrayVars.ToList());
                                            }
                                            else
                                            {
                                                messageBarItem.Text = $" Don't know how to cast value {args[1]} to type {propertyType.Name}";

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            messageBarItem.Text = $" {args[1]} cannot be casted to {propertyType.Name} ({ex.Message})";
                                        }
                                    }
                                    drawYAML();
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
                                        var currentKubeObject = tmpObject.RetrieveAttributeValues().FirstOrDefault(x => x.name.ToLower() == currentInputText.ToLower());
                                        var currentObject = tmpObject.GetType().GetProperties().ToList().FirstOrDefault(x => x.Name.ToLower() == currentInputText.ToLower());
                                        Type currentObjectType = Nullable.GetUnderlyingType(currentObject.PropertyType) != null ? Nullable.GetUnderlyingType(currentObject.PropertyType) : currentObject.PropertyType;
                                        if (currentObjectType.IsPrimitive || currentObjectType == typeof(String) || currentObjectType == typeof(IList<String>))
                                        {
                                            messageBarItem.Text = $" ({currentInputText}) requires a value of type {currentKubeObject.displayType}";
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
                                    messageBarItem.Text = " Command not found";
                                }
                            }
                        }
                        commandPromptTextField.Text = "";
                        autoCompleteInterruptText = "";

                    }
                    updateAvailableKindsList();
                    //repositionCommandInput();
                    //drawYAML();
                };
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
                    availableKindsPopup.Title = (returnValues.Item1);
                    availableKindsList.SetSource(returnValues.Item2.Select(x => x.TableView()).ToList());
                    if (!string.IsNullOrWhiteSpace(commandPromptTextField.Text.ToString()))
                    {
                        var currentListObect = ((List<String>)availableKindsList.Source.ToList()).Find(x => x.StartsWith(commandPromptTextField.Text.ToString()));
                        if (!string.IsNullOrWhiteSpace(currentListObect))
                        {
                            availableKindsList.SelectedItem = availableKindsList.Source.ToList().IndexOf(currentListObect);
                        }
                    }
                   ;
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
                    //List<string> jsonList = JsonConvert.SerializeObject(currentSelectedKind.KubeObject, Formatting.Indented, new JsonSerializerSettings
                    //{
                    //    NullValueHandling = NullValueHandling.Ignore
                    //}).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
                    definedYAMLList.SetSourceAsync(jsonList);
                }
                else
                {
                    definedYAMLList.SetSourceAsync(new List<string>());
                }
            }
            Application.Run();
        }
    }
}
