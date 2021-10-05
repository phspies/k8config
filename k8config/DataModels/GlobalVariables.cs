using NLog;
using System.Collections.Generic;
using System.Reflection;

namespace k8config.DataModels
{
    public static class GlobalVariables
    {
        public static string k8configAppFolder = string.Empty;
        public static int displayMode = 1;
        public static List<SessionDefinedKind> sessionDefinedKinds = new List<SessionDefinedKind>();
        public static List<GlobalAssemblyKubeType> availableKubeTypes = new List<GlobalAssemblyKubeType>();
        public static string k8configVersion = "k8config " + Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        public static string autoCompleteInterruptText = "";
        public static int autoCompleteInterruptIndex = 0;
        public static bool currentavailableListUpDown = false;
        public static Logger Log;
        public static string proxyHost = string.Empty;
        public static string startupString = string.Empty;
    }
 
}
