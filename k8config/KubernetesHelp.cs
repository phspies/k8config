using k8config.DataModels;
using k8config.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace k8config
{
    class KubernetesHelp
    {
        JToken K8HelpObject;
        JToken definitions;
        public KubernetesHelp()
        {
            var reader = new StreamReader(new MemoryStream(Properties.Resources.k8help), Encoding.Default);
            K8HelpObject = JObject.ReadFrom(new JsonTextReader(reader));
        }
        public string getCurrentObjectHelp(string _nestedProperty = null)
        {

            string description = "";
            DescriptionType _prop = new DescriptionType();
            definitions = K8HelpObject.SelectToken("definitions");
            if (GlobalVariables.promptArray.Count > 1)
            {
                var currentRoot = GlobalVariables.sessionDefinedKinds.Find(x => x.index == int.Parse(GlobalVariables.promptArray[1]));
                var found = definitions.Children().FirstOrDefault(x => ((x as JProperty).Name.Contains(currentRoot.kind))).Select(x => x);
                var _property = (JToken)found.First().Parent;

                _prop.name = ((JProperty)_property).Name;
                _prop.description = _property.First["description"]?.Value<string>();


                _prop.properties = BuildPropTree(_property.First);
                if (GlobalVariables.promptArray.Count == 2)
                {
                    description = _prop.description;
                    if (!string.IsNullOrWhiteSpace(_nestedProperty) && _prop.properties.Exists(x => x.name.ToLower() == _nestedProperty.ToLower()))
                    {
                        description = _prop.properties.FirstOrDefault(x => x.name.ToLower() == _nestedProperty.ToLower())?.description;
                    }
                }
                DescriptionType tempHelpObject = _prop.Clone();
                for (int i = 2; GlobalVariables.promptArray.Count > i; i++)
                {
                    if (Int16.TryParse(GlobalVariables.promptArray[i], out _))
                    {
                        continue;
                    }
                    tempHelpObject = tempHelpObject.properties.FirstOrDefault(x => x.name.ToLower() == GlobalVariables.promptArray[i].ToLower());
                    if (!string.IsNullOrWhiteSpace(_nestedProperty) && tempHelpObject.properties.Exists(x => x.name.ToLower() == _nestedProperty.ToLower()))
                    {
                        description = tempHelpObject.properties.FirstOrDefault(x => x.name.ToLower() == _nestedProperty.ToLower())?.description;
                    }
                    else
                    {
                        description = tempHelpObject.description;
                    }
                }

            }
            else
            {
                if (!string.IsNullOrWhiteSpace(_nestedProperty))
                {
                    var _property = (JToken)definitions.Children().FirstOrDefault(x => (x.First["x-kubernetes-group-version-kind"]["kind"]?.Value<string>() == _nestedProperty))?.Select(x => x).First().Parent;
                    description = _property?.First["description"]?.Value<string>();
                }
            }

            return string.IsNullOrWhiteSpace(description) ? "Description not found" : description;
        }

        public List<DescriptionType> BuildPropTree(JToken _json)
        {
            List<DescriptionType> _proptree = new List<DescriptionType>();
            List<string> required = _json["required"]?.ToObject<string[]>().ToList();
            string description = _json["description"]?.Value<string>();
            _json["properties"]?.Children().ToList().ForEach(node =>
            {
                if (!GlobalVariables.ignoreProperties.Any(x => x == ((JProperty)node).Name))
                {
                    DescriptionType _prop = new DescriptionType();
                    _prop.name = ((JProperty)node).Name;
                    if (required != null) { _prop.required = (bool)required?.Any(x => x == ((JProperty)node).Name); }
                    _prop.description = (string)(node.First["description"]);
                    _prop.type = (string)(node.First["type"]);
                    _proptree.Add(_prop);
                    var _ref = node.First["$ref"];
                    if (node.First["$ref"] == null)
                    {
                        if (_prop.type == "array")
                        {
                            if (node.First["items"]["$ref"] != null)
                            {
                                var test = node.First["items"]["$ref"]?.Value<string>().Split("/").Last();
                                _prop.properties = BuildPropTree(definitions[test]);
                            }
                        }
                    }
                    else
                    {
                        _prop.properties = BuildPropTree(definitions[node.First["$ref"]?.Value<string>().Split("/").Last()]);
                    }
                }
            });
            return _proptree;
        }
    }

}
