using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using k8config.DataModels;
using MoreLinq;
using Newtonsoft.Json.Linq;

namespace k8config
{
    class Program
    {


        static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CtlrCHandler);
            Console.BackgroundColor = ConsoleColor.White;
            var MyFilePath = @"c:\swagger.json";
            GlobalVariables.availbleDefinitions = JObject.Parse(File.ReadAllText(MyFilePath));

            ReadLine.AutoCompletionHandler = new AutoCompletionHandler();

            GlobalVariables.availbleDefinitions.SelectTokens("$..x-kubernetes-group-version-kind").ToList().ForEach(x =>
            {
                if (String.IsNullOrEmpty(x.SelectToken(".group")?.Value<string>()) &&
                    String.IsNullOrEmpty(x.SelectToken(".kind")?.Value<string>()) &&
                    String.IsNullOrEmpty(x.SelectToken(".version")?.Value<string>()))
                {
                    JContainer rootobject = x.Parent.Parent;
                    if (((JProperty)rootobject.Parent).Name.Contains("Deployment"))
                    {
                        GlobalVariables.groupKinds.Add(BuildRootObject(rootobject));
                    }
                }
            });


            WriteOutput.WriteInformationLine($"{GlobalVariables.groupKinds.DistinctBy(x => x.name).Count()} Definitions found \n");


            while (true)
            {
                string input = ReadLine.Read(string.Join(":", GlobalVariables.promptArray) + ">");
                if (GlobalVariables.valuePromptMode)
                {
                    switch (GlobalVariables.promptArray.Last())
                    {
                        case "comment":
                            GlobalVariables.promptArray.Remove(GlobalVariables.promptArray.Last());
                            FindCurrentDefinedObject().comment = input;
                            break;
                        case "select":
                            GlobalVariables.promptArray.Remove(GlobalVariables.promptArray.Last());
                            int index;
                            if (int.TryParse(input, out index))
                            {
                                if (GlobalVariables.definedKinds.Exists(x => x.index == index))
                                {
                                    var selectedDefined = GlobalVariables.definedKinds.FirstOrDefault(x => x.index == index);
                                    GlobalVariables.promptArray.Add(index.ToString());
                                    GlobalVariables.promptArray.Add(selectedDefined.name);
                                    WriteOutput.WriteInformationLine($"Selected {selectedDefined.name} : {selectedDefined.comment}");
                                }
                                else
                                {
                                    WriteOutput.WriteErrorLine("Definition does not exist");
                                }
                            }
                            else
                            {
                                WriteOutput.WriteErrorLine("Value not an integer");
                            }
                            break;
                        default:
                            DefinedGroupType _definedObject = FindCurrentDefinedObject();
                            GlobalVariables.promptArray.Remove(GlobalVariables.promptArray.Last());
                            switch (_definedObject.type)
                            {
                                case "integer":
                                    Int32 _tempValue;
                                    if (Int32.TryParse(input, out _tempValue))
                                    {
                                        _definedObject.value = _tempValue.ToString();
                                    }
                                    else
                                    {
                                        WriteOutput.WriteErrorLine("value entered is not a int32 value");
                                    }
                                    break;
                                case "string":
                                    _definedObject.value = input;
                                    break;

                            }
                            break;
                    }
                    GlobalVariables.valuePromptMode = false;
                    continue;
                }
                else
                {
                    switch (input)
                    {
                        case "print":
                        case "p":
                            var outputObj = new ConstructOutputYAML();
                            outputObj.Build();
                            outputObj.PrintYAML();
                            break;
                        case "new":

                            if (GlobalVariables.promptArray.Count == 1)
                            {
                                WriteOutput.WriteInformationLine("Creating new definition");
                                initPrompt();
                                if (GlobalVariables.definedKinds.Count == 0)
                                {
                                    GlobalVariables.definedKinds.Add(new DefinedGroupType() { rootObject = true, index = 1 });
                                }
                                else
                                {
                                    GlobalVariables.definedKinds.Add(new DefinedGroupType() { rootObject = true, index = GlobalVariables.definedKinds.Last().index + 1 });
                                }
                                GlobalVariables.promptArray.Add(GlobalVariables.definedKinds.OrderBy(x => x.index).Last().index.ToString());
                            }
                            else if (FindCurrentDefinedObject().type == "array")
                            {
                                var currentObject = FindCurrentDefinedObject();
                                if (currentObject.properties.Count == 0)
                                {
                                    currentObject.properties.Add(new DefinedGroupType() { index = 1 });
                                }
                                else
                                {
                                    currentObject.properties.Add(new DefinedGroupType() { index = currentObject.properties.Last().index + 1 });
                                }
                                GlobalVariables.promptArray.Add(currentObject.properties.OrderBy(x => x.index).Last().index.ToString());
                            }
                            break;
                        case "select":
                            GlobalVariables.valuePromptMode = true;
                            GlobalVariables.promptArray.Add("select");
                            break;
                        case "list":
                            if (GlobalVariables.promptArray.Count() == 1)
                            {
                                if (GlobalVariables.definedKinds.Count > 0)
                                {
                                    WriteOutput.WriteInformationLine("Current definitions");
                                    GlobalVariables.definedKinds.ForEach(kind =>
                                    {
                                        WriteOutput.WriteInformationLine($"> [{kind.index}] {kind.name} : {kind.comment}");
                                    });
                                }
                                else
                                {
                                    WriteOutput.WriteErrorLine("No definitions, please create one");
                                }
                            }
                            else if (FindCurrentDefinedObject().type == "array")
                            {
                                if (FindCurrentDefinedObject().properties.Count > 0)
                                {
                                    FindCurrentDefinedObject().properties.ForEach(prop => WriteOutput.WriteInformationLine($"[{prop.index}] {prop.name}"));
                                }
                                else
                                {
                                    WriteOutput.WriteErrorLine("No objects defined");
                                }
                            }
                            else
                            {
                                WriteOutput.WriteErrorLine($"> {input} not found");
                            }
                            break;
                        case "?":
                            var _currentObject = FindNestedObject();
                            if (_currentObject.type == "array" && _currentObject.format == "object")
                            {
                                GlobalVariables.knownCommands.ForEach(x => WriteOutput.WriteInformationLine($"> {x}"));
                            }
                            else
                            {
                                GetAvailableOptions("").ToList().ForEach(x => WriteOutput.WriteInformationLine($"> {x}"));
                            }
                            break;
                        case "desc":
                        case "description":
                            var _object = FindNestedObject();
                            if (!string.IsNullOrEmpty(_object.type))
                            {
                                WriteOutput.WriteInformationLine($"> [{FindNestedObject().type}] {FindNestedObject().description}");
                            }
                            else
                            {
                                WriteOutput.WriteInformationLine($"> {FindNestedObject().description}");
                            }
                            break;
                        case "comment":
                            GlobalVariables.valuePromptMode = true;
                            GlobalVariables.promptArray.Add("comment");
                            break;
                        case "exit":
                        case "..":
                            if (GlobalVariables.promptArray.Count() == 1)
                            {
                                System.Environment.Exit(0);
                            }
                            GlobalVariables.promptArray.RemoveAt(GlobalVariables.promptArray.Count() - 1);
                            break;
                        default:
                            if (!string.IsNullOrEmpty(input))
                            {
                                if (GetAvailableOptions("", true).Exists(x => x == input))
                                {
                                    GlobalVariables.promptArray.Add(input);
                                    DefinedGroupType currentDefinedGroup = FindCurrentDefinedObject();
                                    //currentDefinedGroup.name = input;
                                    List<string> inputTypes = new List<string>() { "string", "integer" };
                                    if (inputTypes.Exists(x => x == currentDefinedGroup.type))
                                    {
                                        GlobalVariables.valuePromptMode = true;
                                    }
                                    if (GlobalVariables.promptArray.Count() == 3)
                                    {
                                        GroupType currentGroup = GlobalVariables.groupKinds.Find(x => x.name == input);
                                        currentDefinedGroup.kubedetails = new KubeObjectType() { group = currentGroup.kubedetails.FirstOrDefault().group, kind = currentGroup.kubedetails.FirstOrDefault().kind, version = currentGroup.kubedetails.FirstOrDefault().version };
                                    }
                                }
                                else
                                {
                                    WriteOutput.WriteErrorLine($"> {input} not found");
                                }
                            }
                            break;
                    }
                }
            }
        }

        static public GroupType BuildRootObject(JToken _json)
        {
            GroupType _prop = new GroupType();
            _prop.name = ((JProperty)_json.Parent).Name.Split(".").Last();
            _prop.description = _json.SelectToken("description")?.Value<string>();
            _prop.kubedetails = new List<KubeObjectType>();
            _prop.rootObject = true;
            _json.SelectToken("x-kubernetes-group-version-kind").Children().ForEach(x =>
            {
                _prop.kubedetails.Add(new KubeObjectType()
                {
                    group = x.SelectToken("..group")?.Value<string>(),
                    kind = x.SelectToken("..kind")?.Value<string>(),
                    version = x.SelectToken("..version")?.Value<string>(),
                });

            });
            _json["properties"].ForEach(_property =>
            {
                if (!GlobalVariables.ignoreProperties.Any(x => x == ((JProperty)_property).Name))
                {
                    if (_property.SelectToken("$..$ref") != null)
                    {
                        var objectref = _property.SelectToken("$..$ref")?.Value<string>();
                        _prop.properties.Add(new GroupType()
                        {
                            name = ((JProperty)_property).Name,
                            description = _property.SelectToken("$..description")?.Value<string>(),
                            properties = BuildPropTree(GlobalVariables.availbleDefinitions["definitions"][(_property.SelectToken("$..$ref")?.Value<string>()).Split("/").Last()])
                        });
                    }
                }
            });
            return _prop;
        }

        static public List<GroupType> BuildPropTree(JToken _json)
        {
            List<GroupType> _proptree = new List<GroupType>();
            List<string> required = _json["required"]?.ToObject<string[]>().ToList();
            string description = _json["description"]?.Value<string>();
            _json.SelectTokens("$..properties").Children().ToList().ForEach(node =>
            {
                if (!GlobalVariables.ignoreProperties.Any(x => x == ((JProperty)node).Name))
                {
                    GroupType _prop = new GroupType();
                    _prop.name = ((JProperty)node).Name;
                    Console.Write($"{_prop.name}:");
                    if (required != null) { _prop.required = (bool)required?.Any(x => x == ((JProperty)node).Name); }
                    _prop.description = (string)(node.First["description"]);
                    _proptree.Add(_prop);
                    var _ref = node.First["$ref"];
                    if (node.First["$ref"] == null)
                    {
                        _prop.format = String.IsNullOrEmpty((string)(node.First["format"])) ? "object" : (string)(node.First["format"]);
                        _prop.type = (string)(node.First["type"]);
                        if (_prop.type == "array")
                        {
                            if (node.First["items"]["type"] != null)
                            {
                                _prop.properties.Add(new GroupType() { type = node.First["items"]["type"].Value<string>() });
                            }
                            else if (node.First["items"]["$ref"] != null)
                            {
                                _prop.properties = BuildPropTree(GlobalVariables.availbleDefinitions["definitions"][node.First["items"]["$ref"]?.Value<string>().Split("/").Last()]);
                            }
                        }
                    }
                    else
                    {
                        _prop.properties = BuildPropTree(GlobalVariables.availbleDefinitions["definitions"][node.First["$ref"]?.Value<string>().Split("/").Last()]);
                    }

                }

            });
            Console.Write($"\n");
            return _proptree;
        }




        class AutoCompletionHandler : IAutoCompleteHandler
        {
            // characters to start completion from
            public char[] Separators { get; set; } = new char[] { ' ', '.', '/' };

            // text - The current text entered in the console
            // index - The index of the terminal cursor within {text}
            public string[] GetSuggestions(string text, int index)
            {
                if (GlobalVariables.promptArray.Count > 3)
                {
                    if (FindNestedObject().type == "array" && FindNestedObject().format == "object")
                    {
                        return GlobalVariables.knownCommands.ToArray();
                    }
                }
                var templist = GetAvailableOptions("");
                templist.AddRange(GlobalVariables.knownCommands);
                return templist.Where(x => x.Contains(text)).ToArray();
            }
        }
        protected static void CtlrCHandler(object sender, ConsoleCancelEventArgs args)
        {
            initPrompt();
            args.Cancel = true;
        }
        static public List<string> GetAvailableOptions(string text, bool includeCommands = false)
        {
            GroupType currentObject = FindNestedObject();
            List<string> options = List<string>();
            if (GlobalVariables.promptArray.Count() == 2)
            {
                options = GlobalVariables.groupKinds.Select(x => x.name).Where(x => x.Contains(text)).ToList();
            }
            else if (currentObject.format == "object" && currentObject.type == "Array")
            {

            }
            else
            {
                options = FindNestedObject().properties.Select(x => x.name).Where(x => x.Contains(text)).ToList();
            }
            if (includeCommands == true) { options.AddRange(GlobalVariables.knownCommands); }
            return options;
        }
        static public GroupType FindNestedObject()
        {
            GroupType tempGroupType = new GroupType() { description = "You must select one root object" };
            if (GlobalVariables.promptArray.Count() > 2)
            {
                tempGroupType = GlobalVariables.groupKinds.FirstOrDefault(x => x.name == GlobalVariables.promptArray[2]);
                if (GlobalVariables.promptArray.Count() > 3)
                {
                    for (int index = 3; index < (GlobalVariables.promptArray.Count()); index++)
                    {
                        //if we find a array identifier, just to the next value in prompt array
                        if (int.TryParse(GlobalVariables.promptArray[index], out _))
                        {
                            continue;
                        }
                        tempGroupType = tempGroupType.properties.FirstOrDefault(x => x.name == GlobalVariables.promptArray[index]);
                    }
                }
            }
            return tempGroupType;
        }
        static public DefinedGroupType FindCurrentDefinedObject()
        {
            var tempGroupType = GlobalVariables.definedKinds.Find(x => x.index == int.Parse(GlobalVariables.promptArray[1]));
            if (GlobalVariables.promptArray.Count() > 2)
            {
                for (int index = 3; index < (GlobalVariables.promptArray.Count()); index++)
                {
                    if (!tempGroupType.properties.Exists(x => x.name == GlobalVariables.promptArray[index]))
                    {
                        var nestedObject = FindNestedObject();
                        tempGroupType.properties.Add(new DefinedGroupType()
                        {
                            name = nestedObject.name,
                            required = nestedObject.required,
                            format = nestedObject.format,
                            type = nestedObject.type
                        });
                        tempGroupType = tempGroupType.properties.FirstOrDefault(x => x.name == GlobalVariables.promptArray[index]);
                    }
                    else
                    {
                        tempGroupType = tempGroupType.properties.FirstOrDefault(x => x.name == GlobalVariables.promptArray[index]);
                    }
                }
            }
            return tempGroupType;
        }

        static void initPrompt()
        {
            GlobalVariables.promptArray = new List<string>() { "K8" };
        }
    }
    public static class BooleanExtensions
    {
        public static string ToRequiredString(this bool value)
        {
            return value ? "required" : "optional";
        }
    }


}
