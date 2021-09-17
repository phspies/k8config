using k8config.DataModels;
using k8config.Utilities;
using k8s.Models;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace k8config.GUIEvents.YAMLMode
{
    public static class YAMLOperations
    {
        static List<string> removeAttributes = new List<string>() { "Kind", "ApiVersion", "Status" };
        static List<string> concernList = new List<string>();
        public static List<string> Validate()
        {
            concernList = new List<string>();
            GlobalVariables.sessionDefinedKinds.ForEach(definedKind =>
            {
                nestedLoopValidation(definedKind.KubeObject, $":{definedKind.index}:{definedKind.kind}");
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
                        string currentPath = $"{_currentPath}:{_property.Name}";
                        bool isRequired = (_property.GetCustomAttributes(typeof(KubernetesPropertyAttribute), false)?.First() as KubernetesPropertyAttribute)?.IsRequired ?? false;
                        if (isRequired && _property.GetValue(_object) == null)
                        {
                            concernList.Add(currentPath);
                        }
                        else if (_property.GetValue(_object) != null && _property.PropertyType.FullName.Contains("k8s.Models"))
                        {
                            nestedLoopValidation(_property.GetValue(_object), currentPath);
                        }
                    }
                }
            }
        }
    }
}
