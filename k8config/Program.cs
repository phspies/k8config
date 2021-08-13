using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq;
using Newtonsoft.Json.Linq;

namespace k8config
{
    class Program
    {
        static JObject availbleDefinitions;
        static int level = 0;
        static string prompt = "";
        static List<string> promptArray = new List<string>() { "K8" };
        static List<GroupType> groupKinds = new List<GroupType>();
        static List<GroupType> definitions = new List<GroupType>();
        static List<string> ignoreProperties = new List<string>() { "kind", "apiVersion", "status" };

        static void Main(string[] args)
        {

            var MyFilePath = "swagger.json";
            availbleDefinitions = JObject.Parse(File.ReadAllText(MyFilePath));

            ReadLine.AutoCompletionHandler = new AutoCompletionHandler();

            availbleDefinitions.SelectTokens("$..x-kubernetes-group-version-kind").ToList().ForEach(x =>
            {
                if (String.IsNullOrEmpty(x.SelectToken(".group")?.Value<string>()) &&
                    String.IsNullOrEmpty(x.SelectToken(".kind")?.Value<string>()) &&
                    String.IsNullOrEmpty(x.SelectToken(".version")?.Value<string>()))
                {
                    JContainer rootobject = x.Parent.Parent;
                    groupKinds.Add(BuildRootObject(rootobject));
                }
            });


            WriteInformationLine($"{groupKinds.DistinctBy(x => x.name).Count()} Definitions found \n");


            while (true)
            {
                prompt = string.Join(":", promptArray) + ">";
                string input = ReadLine.Read(prompt);


                switch (input)
                {
                    case "new":
                        if (promptArray.Count() == 1)
                        {
                            WriteInformationLine("Creating new definition");
                            promptArray = new List<string>() { "K8" };
                        }
                        break;
                    case "list":
                        if (promptArray.Count() == 1)
                        {
                            if (definitions.Count > 0)
                            {
                                WriteInformationLine("Current definitions");
                                definitions.ForEach(definition => {
                                    WriteInformationLine($"> {definition.name}");
                                });
                            }
                            else
                            {
                                WriteErrorLine("No definitions, please create one");
                            }
                        }
                        break;
                    case "?":
                        GetAvailableOptions("").ToList().ForEach(x => WriteInformationLine($"> {x}"));
                        break;
                    case "desc":
                    case "description":
                        var _object = FindNestedObject();
                        if (!string.IsNullOrEmpty(_object.type))
                        {
                            WriteInformationLine($"> [{FindNestedObject().type}] {FindNestedObject().description}");
                        }
                        else
                        {
                            WriteInformationLine($"> {FindNestedObject().description}");
                        }
                        break;
                    case "exit":
                    case "..":
                        if (promptArray.Count() == 1)
                        {
                            System.Environment.Exit(0);
                        }
                        promptArray.RemoveAt(promptArray.Count() - 1);
                        break;
                    default:
                        if (!string.IsNullOrEmpty(input))
                        {
                            if (GetAvailableOptions("").Exists(x => x == input))
                            {
                                promptArray.Add(input);
                            }
                            else
                            {
                                WriteErrorLine($"> {input} not found");
                            }
                        }
                        break;
                }
            }
        }
        static public GroupType BuildRootObject(JToken _json)
        {
            GroupType _prop = new GroupType();
            _prop.name = ((JProperty)_json.Parent).Name.Split(".").Last();
            _prop.description = _json.SelectToken("description")?.Value<string>();
            _prop.kubedetails = new List<KubeObjectType>();
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
                if (!ignoreProperties.Any(x => x == ((JProperty)_property).Name))
                {
                    if (_property.SelectToken("$..$ref") != null)
                    {
                        var objectref = _property.SelectToken("$..$ref")?.Value<string>();
                        _prop.properties.Add(new GroupType()
                        {
                            name = ((JProperty)_property).Name,
                            description = _property.SelectToken("$..description")?.Value<string>(),
                            properties = BuildPropTree(availbleDefinitions["definitions"][(_property.SelectToken("$..$ref")?.Value<string>()).Split("/").Last()])
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
                if (!ignoreProperties.Any(x => x == ((JProperty)node).Name))
                {
                    GroupType _prop = new GroupType();
                    _prop.name = ((JProperty)node).Name;
                    if (required != null) { _prop.required = (bool)required?.Any(x => x == ((JProperty)node).Name); }
                    _prop.description = (string)(node.First["description"]);
                    _proptree.Add(_prop);
                    var _ref = node.First["$ref"];
                    if (node.First["$ref"] == null)
                    {
                        _prop.format = (string)(node.First["format"]);
                        _prop.type = (string)(node.First["type"]);
                    }
                    else
                    {
                        _prop.properties = BuildPropTree(availbleDefinitions["definitions"][node.First["$ref"]?.Value<string>().Split("/").Last()]);
                    }
                }
            });
            return _proptree;
        }

        public class GroupType
        {
            public GroupType()
            {
                properties = new List<GroupType>();
            }
            public bool required { get; set; }
            public List<KubeObjectType> kubedetails { get; set; }
            public string name { get; set; }
            public string reference { get; set; }
            public string fullpath { get; set; }
            public string description { get; set; }
            public string format { get; set; }
            public string type { get; set; }
            public List<GroupType> properties { get; set; }
        }
        public class KubeObjectType
        {
            public string group { get; set; }
            public string kind { get; set; }
            public string version { get; set; }
        }
        public class GroupProperty
        {
            public GroupProperty()
            {
                required = false;
            }
            public string property { get; set; }
            public string reference { get; set; }
            public bool required { get; set; }
            public string description { get; set; }
            public string format { get; set; }
            public string type { get; set; }
            public List<GroupProperty> properties { get; set; }
        }

        class AutoCompletionHandler : IAutoCompleteHandler
        {
            // characters to start completion from
            public char[] Separators { get; set; } = new char[] { ' ', '.', '/' };

            // text - The current text entered in the console
            // index - The index of the terminal cursor within {text}
            public string[] GetSuggestions(string text, int index)
            {

                return GetAvailableOptions(text).ToArray();
            }
        }
        static public List<string> GetAvailableOptions(string text)
        {
            List<string> options;
            if (promptArray.Count() == 1)
            {
                options = groupKinds.Select(x => x.name).Where(x => x.Contains(text)).ToList();
            }
            else
            {
                options = FindNestedObject().properties.Select(x => x.name).Where(x => x.Contains(text)).ToList();
            }
            return options;
        }
        static public GroupType FindNestedObject()
        {
            GroupType tempGroupType = new GroupType() { description = "You must select one root object" };
            if (promptArray.Count() > 1)
            {
                tempGroupType = groupKinds.FirstOrDefault(x => x.name == promptArray[1]);
                if (promptArray.Count() > 2)
                {
                    for (int index = 2; index < (promptArray.Count()); index++)
                    {
                        tempGroupType = tempGroupType.properties.FirstOrDefault(x => x.name == promptArray[index]);
                    }
                }
            }
            return tempGroupType;
        }
        static public void WriteNormalLine(string _line)
        {
            Console.WriteLine(_line);
        }
        static public void WriteInformationLine(string _line)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(_line);
            Console.ResetColor();
        }
        static public void WriteErrorLine(string _line)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(_line);
            Console.ResetColor();
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
