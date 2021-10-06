using k8config.DataModels;
using k8config.Utilities;
using k8s;
using k8s.Models;
using NLog;
using NLog.Targets;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Terminal.Gui;
using static System.Environment;

namespace k8config
{
    partial class Program
    {

        static void Main(string[] args)
        {
            try
            {
                GlobalVariables.k8configAppFolder = Path.Combine(GetFolderPath(SpecialFolder.ApplicationData, SpecialFolderOption.DoNotVerify), "k8config");
                Directory.CreateDirectory(GlobalVariables.k8configAppFolder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot create local data folder for k8config: {GlobalVariables.k8configAppFolder} : {ex.Message}");
                Exit(1);
            }
            if (args.Length > 0)
            {
                args.ForEach(x =>
                {
                    string[] argvals = x.Split("=");
                    switch (argvals[0])
                    {
                        case "--proxyHost":
                            GlobalVariables.proxyHost = argvals[1];
                            break;
                        case "--file":
                            try
                            {
                                GlobalVariables.startupFileLoad = argvals[1];
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Cannot open file specified: {argvals[1]} : {ex.Message}");
                                Exit(1);
                            }
                            break;
                        case "/h":
                        case "/?":
                        case "-h":
                        case "--help":
                            Console.WriteLine($"{GlobalVariables.k8configVersion}");
                            Console.WriteLine($"");
                            Console.WriteLine($"--proxyHost=hostname:port \t Specify proxy host url to use in realtime mode");
                            Console.WriteLine($"--file=filename.yaml \t\t Specify YAML file to load on startup");
                            Exit(0);
                            break;
                        default:
                            Console.WriteLine($"{argvals[0]} not known");
                            Exit(1);
                            break;
                    }
                });
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Application.UseSystemConsole = true;
            }
            Application.Init();

            var config = new NLog.Config.LoggingConfiguration();
            config.AddRule(
                LogLevel.Debug,
                LogLevel.Fatal,
                new FileTarget
                {
                    Name = "logfile",
                    FileName = $"{GlobalVariables.k8configAppFolder}{Path.DirectorySeparatorChar}k8config.log",
                    Layout = "${longdate}|${level:uppercase=true}|${message}",
                },
                "*");
            LogManager.Configuration = config;
            GlobalVariables.Log = LogManager.GetCurrentClassLogger();

            AssemblySubsystem.BuildAvailableAssemblyList();
            GlobalVariables.Log.Debug($"This is a {RuntimeInformation.OSDescription} client");

            try
            {
                if (!string.IsNullOrWhiteSpace(GlobalVariables.proxyHost))
                {
                    k8Client = new Kubernetes(new KubernetesClientConfiguration { Host = GlobalVariables.proxyHost });
                }
                else
                {
                    k8Client = new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile());
                }
                Array.Resize(ref YAMLModelControls.interactiveStatusBarItems, 3);
                YAMLModelControls.interactiveStatusBarItems[2] = YAMLModelControls.interactiveStatusBarItems[1];
                YAMLModelControls.interactiveStatusBarItems[1] = new StatusItem(Key.F10, "~F10~ Interactive Mode", () => { ToggleDisplayMode(); });
            }
            catch (Exception e)
            {
                GlobalVariables.Log.Error($"Cannot load kubeconfig: {e.Message}");
            }
            SetupTopLevelView();
            YAMLMode();
            RealTimeMode();
            YamlModeKeyEvents();
            updateAvailableKindsList();
            topLevelWindowObject.Add(YAMLModelControls.k8configVersion);

            Application.Run(topLevelWindowObject);

        }
    }
}
