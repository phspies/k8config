using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace k8config
{
    class Program
    {
        static JObject definitions;
        static int level = 0;
        static string currentDefinition = "";
        static List<GroupType> groupKinds = new List<GroupType>();

        static void Main(string[] args)
        {

            var MyFilePath = "swagger.json";
            definitions = JObject.Parse(File.ReadAllText(MyFilePath));

            ReadLine.AutoCompletionHandler = new AutoCompletionHandler();

            definitions.SelectTokens("$..spec").ToList().ForEach(x =>
            {

                //"x-kubernetes-group-version-kind";
                var parentobject = x.Parent.Parent.Parent.Parent;
                var groupobject = parentobject.SelectToken("$..x-kubernetes-group-version-kind");

                if (groupobject != null)
                {
                    string group = groupobject.SelectToken("..group")?.Value<string>();
                    string kind = groupobject.SelectToken("..kind")?.Value<string>();
                    string version = groupobject.SelectToken("..version")?.Value<string>();
                    if (!groupKinds.Exists(y => y.group == group && y.kind == kind && y.version == version))
                    {
                        string description = parentobject.SelectToken("description")?.Value<string>();

                        List<GroupProperty> _tmpList = new List<GroupProperty>();
                        var metadata = parentobject.SelectToken("$..properties").SelectToken("$..metadata")?.SelectToken("$ref")?.Value<string>();
                        if (metadata != null) { _tmpList.Add(new GroupProperty() { property = "metadata", reference = metadata }); }
                        var spec = parentobject.SelectToken("$..properties").SelectToken("$..spec")?.SelectToken("$ref")?.Value<string>();
                        if (spec != null) { _tmpList.Add(new GroupProperty() { property = "spec", reference = spec }); }
                        var items = parentobject.SelectToken("$..properties").SelectToken("$..items")?.SelectToken("$ref")?.Value<string>();
                        if (items != null) { _tmpList.Add(new GroupProperty() { property = "items", reference = items }); }
                        groupKinds.Add(new GroupType() { description = description, fullpath = parentobject.Path, group = group, kind = kind, version = version, properties = _tmpList }); ;                        
                    }
                }

            });

            Console.WriteLine($"{groupKinds.Count()} Definitions found \n");
            var prompt = "K8:> ";


            while (true)
            {
                string input = ReadLine.Read(prompt);

                if (groupKinds.Any(x => x.kind == input))
                {

                    level = 1;
                    currentDefinition = input;
                    prompt = String.Format($"{input}> ");
                }
                switch (input)
                {
                    case "exit":
                        System.Environment.Exit(0);
                        break;
                    default:
                        continue;
                }
            }

        }
        public class GroupType
        {
            public GroupType()
            {
                properties = new List<GroupProperty>();
            }
            public string fullpath { get; set; }
            public string description { get; set; }
            public string group { get; set; }
            public string kind { get; set; }
            public string version { get; set; }
            public List<GroupProperty> properties { get; set; }
        }
        public class GroupProperty
        {
            public GroupProperty()
            {
                required = false;
            }
            public string property { get; set; }
            public string reference {get; set;}
            public bool required { get; set; }
        }

        class AutoCompletionHandler : IAutoCompleteHandler
        {
            // characters to start completion from
            public char[] Separators { get; set; } = new char[] { ' ', '.', '/' };

            // text - The current text entered in the console
            // index - The index of the terminal cursor within {text}
            public string[] GetSuggestions(string text, int index)
            {
                switch (level)
                {
                    case 0:
                        return groupKinds.Select(x => x.kind).Where(x => x.Contains(text)).ToArray();
                    case 1:
                        //var search = definitions.SelectToken("definitions").Where(x => ((JProperty)x).Name == currentDefinition).FirstOrDefault();
                        var search = definitions["definitions"][currentDefinition]["properties"];
                        var properties = search;
                       
                        return new string[0];
                    default:
                        return new string[0];

                }
                   
            }
        }

    }


   
}
