using k8config.Utilities;
using System.Collections.Generic;

namespace k8config.GUIEvents
{
    static class YAMLModePromptObject
    {
        private static List<string> initArray = new List<string>() { "k8config" };
        public static List<string> promptArray = new List<string>(initArray);
        public static bool CurrentPromptPositionIsRoot { get { return promptArray.Count == 1 ? true : false; } }
        public static bool CurrentPromptPositionIsNotRoot { get { return promptArray.Count > 1 ? true : false; } }
        public static bool CurrentPromptDefinitionSelected { get { return promptArray.Count >= 2 ? true : false; } }
        public static int Count { get { return promptArray.Count; } }
        public static string Last { get { return promptArray[promptArray.Count - 1]; } }
        public static string PromptConstructor { get { return string.Join(":", promptArray) + ">"; } }
        public static void Init()
        {
            promptArray = new List<string>(initArray);
        }
        public static void EnterFolder(string folderName)
        {
            promptArray.Add(folderName);
        }
        public static string CurrentFolder { get { return promptArray[promptArray.Count - 1]; } }
        public static string GetFolderAt(int index)
        {
            if (index < promptArray.Count)
            {
                return promptArray[index];
            }
            return "";
        }
        public static void ExitFolder()
        {
            promptArray.RemoveAt(promptArray.Count - 1);
        }
        public static void ExitMultipleFolders(string changeString)
        {
            changeString.Split("/").ForEach(x =>
            {
                if (promptArray.Count > 1 && x == "..")
                {
                    ExitFolder();
                }
            });
        }
    }
}
