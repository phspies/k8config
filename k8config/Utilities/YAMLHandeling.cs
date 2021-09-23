using k8config.DataModels;
using k8s;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
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
                    if (!String.IsNullOrWhiteSpace(dict["kind"] as string))
                    {
                        GlobalAssemblyKubeType _type = GlobalVariables.availableKubeTypes.Find(x => x.kind.ToLower() == ((string)dict["kind"]).ToLower());
                        if (_type != null)
                        {
                            GlobalVariables.sessionDefinedKinds.Add(new SessionDefinedKind()
                            {
                                index = index,
                                kind = dict["kind"] as string,
                                name = dict["kind"] as string,
                                KubeObject = Yaml.Deserializer.Deserialize(Newtonsoft.Json.JsonConvert.SerializeObject(tempExpandoObject), Type.GetType(_type.assemblyFullName))
                            });;
                            index++;
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
                    Yaml.YAMLSerializer.Serialize(sw, _kubeobject);
                    if (!_kubeobject.Equals(GlobalVariables.sessionDefinedKinds.Last().KubeObject))
                        sw.WriteLine("---");
                }
                
            }
        }
    }
}