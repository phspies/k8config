using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace k8config.DataModels
{
    public static class GlobalVariables
    {
        public static List<string> knownCommands = new List<string>() { "select", "delete", "exit", "new", "list", "comment", ".." };
        public static List<string> rootKnownCommands = new List<string>() { "select", "delete", "exit", "new", "list" };
        public static JObject availbleDefinitions;
        public static List<string> promptArray = new List<string>() { "K8" };
        public static List<SourceGroupType> groupSourceKinds = new List<SourceGroupType>();
        public static List<TargetGroupType> groupTargetKinds = new List<TargetGroupType>();
        public static List<string> ignoreProperties = new List<string>() { "kind", "apiVersion", "status" };
        public static bool valuePromptMode = false;
    }
}
