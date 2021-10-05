using k8config.DataModels;
using k8config.GUIEvents.YAMLMode;
using k8s;
using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


namespace k8config.Utilities
{
    public static class YAMLHandeling
    {
        public static void DeserializeFile(string fileLocation)
        {
            Logger Log = LogManager.GetCurrentClassLogger();
            var stream = new StreamReader(fileLocation);
            var reader = new Parser(stream);
            reader.Consume<StreamStart>();
            DocumentStart outdocument;
            GlobalVariables.sessionDefinedKinds = new List<SessionDefinedKind>();
            int index = 1;
            while (reader.Accept(out outdocument))
            {
                ExpandoObject tempExpandoObject = (new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build()).Deserialize<ExpandoObject>(reader);
                if (tempExpandoObject != null)
                {
                    var dict = (IDictionary<string, object>)tempExpandoObject;
                    if (!string.IsNullOrWhiteSpace(dict["kind"] as string))
                    {
                        string apiVersion = String.Empty;
                        if (((string)dict["apiVersion"]).Contains("/"))
                        {
                            apiVersion = ((string)dict["apiVersion"]).Split("/")[1];
                        }
                        else
                        {
                            apiVersion = ((string)dict["apiVersion"]);
                        }
                        
                        GlobalAssemblyKubeType _type = GlobalVariables.availableKubeTypes.Find(x => (string.Compare(x.kind,(string)dict["kind"],StringComparison.OrdinalIgnoreCase) == 0) && (string.Compare(x.version, apiVersion, StringComparison.OrdinalIgnoreCase) == 0));
                        if (_type != null)
                        {
                            Log.Info($"Definition imported - kind:{dict["kind"]} apiVerion:{dict["apiVersion"]} kubeType: {_type.classKind}");
                            GlobalVariables.sessionDefinedKinds.Add(new SessionDefinedKind()
                            {
                                index = index,
                                kind = _type.classKind,
                                name = _type.classKind,
                                KubeObject = Yaml.Deserializer.Deserialize(Newtonsoft.Json.JsonConvert.SerializeObject(tempExpandoObject), Type.GetType(_type.assemblyFullName))
                            });;
                            index++;
                        }
                        else
                        {
                            Log.Debug($"Definition importefailed - Type not found in available types: kind:{dict["kind"]} apiVerion:{dict["apiVersion"]}");
                        }
                    }
                }
            }
            stream.Close();
        }
        public static void SerializeToFile(string filePath)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull).Build();
            GlobalVariables.sessionDefinedKinds.Select(x => x.KubeObject);
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach (object _kubeobject in GlobalVariables.sessionDefinedKinds.Select(x => x.KubeObject))
                {
                    YAMLOperations.SerializeObjectToList(_kubeobject).ForEach(line => sw.WriteLine(line));
                    if (!_kubeobject.Equals(GlobalVariables.sessionDefinedKinds.Last().KubeObject))
                        sw.WriteLine("---");
                }
                
            }
        }
    }
}