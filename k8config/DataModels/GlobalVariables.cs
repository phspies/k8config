using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace k8config.DataModels
{
    public static class GlobalVariables
    {
        private static List<string> _knownCommands = new List<string>() { "exit", "new", "list", "comment", ".." };
        public static JObject availbleDefinitions;
        public static List<string> promptArray = new List<string>() { "K8" };
        public static List<SourceGroupType> groupKinds = new List<SourceGroupType>();
        public static List<string> ignoreProperties = new List<string>() { "kind", "apiVersion", "status" };
        public static List<string> knownCommands
        {   get { return _knownCommands; }
            set { throw new Exception("cannot change this object"); }
        }
        public static List<TargetGroupType> definedKinds = new List<TargetGroupType>();
        public static bool valuePromptMode = false;
    }
}
