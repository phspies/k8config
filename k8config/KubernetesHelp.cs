using k8config.DataModels;
using k8config.Utilities;
using k8s.Models;
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
        int? currentIndexObject;
        DescriptionType descriptionObject;
        public KubernetesHelp()
        {
            var reader = new StreamReader(new MemoryStream(Properties.Resources.k8help), Encoding.Default);
            K8HelpObject = JObject.ReadFrom(new JsonTextReader(reader));
            definitions = K8HelpObject.SelectToken("definitions");
        }
        public DescriptionType getCurrentObjectHelp(string _nestedProperty = null)
        {

            DescriptionType returnObject = new DescriptionType();

            if (GlobalVariables.promptArray.Count > 1)
            {
                var currentRoot = GlobalVariables.sessionDefinedKinds.Find(x => x.index == int.Parse(GlobalVariables.promptArray[1]));
                if (currentIndexObject == null || currentIndexObject != int.Parse(GlobalVariables.promptArray[1]) || descriptionObject.name != currentRoot.kind)
                {
                    currentIndexObject = int.Parse(GlobalVariables.promptArray[1]);
                    descriptionObject = new DescriptionType();
                    var _property = ((JToken)definitions).SelectTokens($"$..x-kubernetes-group-version-kind[?(@.kind == '{currentRoot.kind}')]")?.First().Parent.Parent.Parent.Parent.First.Parent;
                    descriptionObject.name = ((JProperty)_property).Name.Split(".").Last();
                    descriptionObject.description = _property.First["description"]?.Value<string>();
                    descriptionObject.properties = BuildPropTree(_property.First);
                }
                if (GlobalVariables.promptArray.Count == 2)
                {
                    returnObject = descriptionObject;
                    if (!string.IsNullOrWhiteSpace(_nestedProperty) && descriptionObject.properties.Exists(x => x.name.ToLower() == _nestedProperty.ToLower()))
                    {
                        returnObject = descriptionObject.properties.FirstOrDefault(x => x.name.ToLower() == _nestedProperty.ToLower());
                    }
                }
                else
                {
                    DescriptionType tempHelpObject = descriptionObject.Clone();
                    for (int i = 2; GlobalVariables.promptArray.Count > i; i++)
                    {
                        if (Int16.TryParse(GlobalVariables.promptArray[i], out _))
                        {
                            continue;
                        }
                        tempHelpObject = tempHelpObject.properties.FirstOrDefault(x => x.name.ToLower().Equals(GlobalVariables.promptArray[i].ToLower()));

                        if (tempHelpObject != null && !string.IsNullOrWhiteSpace(_nestedProperty) && tempHelpObject != null && tempHelpObject.properties.Exists(x => x.name.ToLower() == _nestedProperty.ToLower()))
                        {
                            returnObject = tempHelpObject.properties.FirstOrDefault(x => x.name.ToLower() == _nestedProperty.ToLower());
                        }
                    }
                }

            }
            else
            {
                if (!string.IsNullOrWhiteSpace(_nestedProperty))
                {
                    returnObject.description = ((JToken)definitions).SelectTokens($"$..x-kubernetes-group-version-kind[?(@.kind == '{_nestedProperty}')]")?.First().Parent.Parent.Parent.Parent.First["description"]?.Value<string>();
                }
            }

            return  returnObject;
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
