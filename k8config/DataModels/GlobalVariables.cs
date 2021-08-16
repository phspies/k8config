using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace k8config.DataModels
{
    public static class GlobalVariables
    {
        public static JObject availbleDefinitions;
        public static List<string> promptArray = new List<string>() { "K8" };
        public static List<GroupType> groupKinds = new List<GroupType>();
        public static List<string> ignoreProperties = new List<string>() { "kind", "apiVersion", "status" };
        public static List<string> knownCommands = new List<string>() { "exit", "new", "list", "comment", ".." };
        public static List<DefinedGroupType> definedKinds = new List<DefinedGroupType>();
        public static bool valuePromptMode = false;
    }
}
