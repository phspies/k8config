using k8config.DataModels;
using System;
using System.Collections.Generic;

namespace k8config
{
    class ConstructOutputYAML
    {
        List<string> _list = new List<string>();
        public List<string> Build(TargetGroupType _object = null, int indent = 0, bool tagfirst = false)
        {
            if (_object == null)
            {
                GlobalVariables.definedKinds.ForEach(kind =>
                {
                    if (!String.IsNullOrEmpty(kind.comment)) { _list.Add($"#{kind.comment}"); }
                    _list.Add($"apiVersion: {kind.kubedetails.group}/{kind.kubedetails.version}");
                    _list.Add($"kind: {kind.kubedetails.kind}");
                    List<TargetGroupType> _objectProperties = kind.properties;
                    _list.AddRange(new ConstructOutputYAML().Build(kind, indent + 2));
                });
            }
            else
            {
                foreach (var property in _object.properties)
                {
                    if (property.isItem)
                    {
                        if (!String.IsNullOrEmpty(property.comment)) { _list.Add(padLeftString($"#{property.comment}", indent)); }
                        _list.AddRange(new ConstructOutputYAML().Build(property, indent, true));
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(property.comment)) { _list.Add(padLeftString($"#{property.comment}", indent)); }
                        if (!string.IsNullOrEmpty(property.type))
                        {
                            if (tagfirst)
                            {
                                _list.Add(padLeftString($"- {property.name}: {property.value}", indent - 2));
                                tagfirst = false;
                            }
                            else
                            {
                                _list.Add(padLeftString($"{property.name}: {property.value}", indent));
                            }
                        }
                        else
                        {
                            _list.Add(padLeftString($"{property.name}:", indent));
                        }
                        if (property.properties.Count > 0)
                        {

                            _list.AddRange(new ConstructOutputYAML().Build(property, indent + 2));
                        }
                    }
                };
            }
            return _list;

        }
        public void PrintYAML()
        {
            _list.ForEach(x => WriteOutput.WriteInformationLine(x));
        }
        static public string padLeftString(string _str, int padding)
        {
            for (int i = 0; i < padding; i++)
            {
                _str = _str.Insert(0, " ");
            }
            return _str;
        }
    }
}
