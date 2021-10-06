using k8config.DataModels;
using k8config.Utilities;
using k8s;
using k8s.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace k8config.GUIEvents.YAMLMode
{
    public static class YAMLOperations
    {
        static List<string> removeAttributes = new List<string>() { "Kind", "ApiVersion", "Status" };
        static List<string> concernList = new List<string>();
        public static List<string> SerializeObjectToList(object srcObject)
        {
            List<string> yamlList = Yaml.YAMLSerializer.Serialize(srcObject).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            for (int i = 0; i < yamlList.Count; i++)
            {
                string line = yamlList[i];
                if (yamlList[i].StartsWith("apiVersion:")) { yamlList.RemoveAt(i); yamlList.Insert(0, line); }
                if (yamlList[i].StartsWith("kind:")) { yamlList.RemoveAt(i); yamlList.Insert(1, line); }
            };
            if (string.IsNullOrWhiteSpace(yamlList.Last()))
            {
                yamlList.Remove(yamlList.Last());
            }
            return yamlList;
        }
        public static List<string> Validate()
        {
            concernList = new List<string>();
            GlobalVariables.sessionDefinedKinds.ForEach(definedKind =>
            {
                nestedLoopValidation(definedKind.KubeObject, $"/{definedKind.metaData.Metadata.Name}({definedKind.metaData.Kind})");
            });
            return concernList.Count > 0 ? concernList : new List<string>() { "All validations passed" };
        }
        private static void nestedLoopValidation(object _object, string _currentPath)
        {
            if (_object.IsList())
            {
                var _list = (_object as IEnumerable);
                int index = 0;
                foreach (var nestedObject in _list)
                {
                    string currentPath = $"{_currentPath}/{index}";
                    nestedLoopValidation(nestedObject, currentPath);
                    index += 1;
                }
            }
            else
            {
                foreach (var _property in _object.GetObjectProperties())
                {
                    if (!removeAttributes.Contains(_property.Name) && _property.CanWrite)
                    {
                        string currentPath = $"{_currentPath}/{(_property.GetCustomAttributes(typeof(JsonPropertyAttribute), false)?.First() as JsonPropertyAttribute)?.PropertyName}";
                        if (((_property.GetCustomAttributes(typeof(KubernetesPropertyAttribute), false)?.First() as KubernetesPropertyAttribute)?.IsRequired ?? false) && _property.GetValue(_object) == null)
                        {
                            concernList.Add($"{currentPath} is required");
                        }
                        else if ((_property.GetValue(_object) != null) && _property.PropertyType.FullName.Contains("k8s.Models"))
                        {
                            nestedLoopValidation(_property.GetValue(_object), currentPath);
                        }
                    }
                }
            }
        }
    }
}
