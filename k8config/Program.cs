using k8config.DataModels;
using k8config.Utilities;
using k8s;
using k8s.Models;
using NLog;
using System;
using System.Runtime.InteropServices;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        static string autoCompleteInterruptText = "";
        static int autoCompleteInterruptIndex = 0;
        static bool currentavailableListUpDown = false;
        static Logger Log;
        static string proxyHost = string.Empty;
        static void Main(string[] args)
        {
            Application.Init();

            if (args.Length > 0)
            {
                args.ForEach(x =>
                {
                    string[] argvals = x.Split("=");
                    switch(argvals[0])
                    {
                        case "--proxyHost":
                            proxyHost = argvals[1];
                            break;
                        default:
                            Console.WriteLine($"{argvals[0]} not known");
                            break;
                    }
                });
            }

            var config = new NLog.Config.LoggingConfiguration();
            // Targets where to log to: File and Console
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, new NLog.Targets.FileTarget("logfile") { FileName = "k8config.log" });

            // Apply config           
            LogManager.Configuration = config;
            Log = LogManager.GetCurrentClassLogger();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Log.Debug($"This is a OSX client, using system console to GUI");
                Application.UseSystemConsole = true;
            }
            else
            {
                Log.Debug($"This is a {RuntimeInformation.OSDescription} client");
            }
            AssemblySubsystem.BuildAvailableAssemblyList();

            try
            {
                k8Client = new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile());
                Array.Resize(ref interactiveStatusBarItems, 4);
                interactiveStatusBarItems[3] = interactiveStatusBarItems[2];
                interactiveStatusBarItems[2] = new StatusItem(Key.F10, "~F10~ Interactive Mode", () => { ToggleDisplayMode(); });
            }
            catch (Exception e)
            {
                Log.Error($"Cannot load kubeconfig: {e.Message}");
            }


            SetupTopLevelView();
            YAMLMode();
            RealTimeMode();
            YamlModeKeyEvents();

            updateAvailableKindsList();

            Application.Run(topLevelWindowObject);

        }

        static void AddToPrompt(string _promptString)
        {
            GlobalVariables.promptArray.Add(_promptString);
            repositionCommandInput();
            UpdateDescriptionView();
        }
    }
}
