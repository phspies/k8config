using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace k8config.DataModels
{
    public static class GlobalVariables
    {
        public static JObject availbleDefinitions;
        public static List<string> promptArray = new List<string>() { "k8config" };
        public static List<SourceGroupType> groupSourceKinds = new List<SourceGroupType>();
        public static List<TargetGroupType> groupTargetKinds = new List<TargetGroupType>();
        public static List<string> ignoreProperties = new List<string>() { "kind", "apiVersion", "status" };
        public static bool valuePromptMode = false;


        public static int displayMode = 1;
        public static List<SessionDefinedKind> sessionDefinedKinds = new List<SessionDefinedKind>();
        public static List<GlobalAssemblyKubeType> availableKubeTypes = new List<GlobalAssemblyKubeType>();
    }

  
    public class GlobalAssemblyKubeType
    {
        public string kind { get; set;}
        public string assemblyFullName { get; set; }
        public string group { get; set; }
        public string version { get; set; }
 
    }
    public class SessionDefinedKind
    {
        public string name { get; set; }
        public int index { get; set; }
        public string kind { get; set; }
        public Object KubeObject { get; set; }
    }
}
