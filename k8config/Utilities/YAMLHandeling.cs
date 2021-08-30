using k8config.DataModels;
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
            var input = new StreamReader(fileLocation);
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var reader = new Parser(input);
            reader.Consume<StreamStart>();
            DocumentStart outdocument;
            int index = 1;
            while (reader.Accept(out outdocument))
            {
                var tempExpandoObject = deserializer.Deserialize<ExpandoObject>(reader);
                if (tempExpandoObject != null)
                {
                    var dict = (IDictionary<string, object>)tempExpandoObject;
                    if (!String.IsNullOrWhiteSpace(dict["kind"] as string))
                    {
                        GlobalAssemblyKubeType _type = GlobalVariables.availableKubeTypes.FirstOrDefault(x => x.kind.ToLower() == ((string)dict["kind"]).ToLower());
                        if (_type != null)
                        {
                            GlobalVariables.sessionDefinedKinds.Add(new SessionDefinedKind()
                            {
                                index = index,
                                kind = dict["kind"] as string,
                                name = dict["kind"] as string,
                                KubeObject = Newtonsoft.Json.JsonConvert.DeserializeObject(Newtonsoft.Json.JsonConvert.SerializeObject(tempExpandoObject), Type.GetType(_type.assemblyFullName))
                            });
                            index++;
                        }
                    }
                }
            }
        }
    }
}