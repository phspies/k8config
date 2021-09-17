﻿using k8config.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace k8config.Utilities
{
    public static class AssemblySubsystem
    {
        public static void BuildAvailableAssemblyList()
        {
            NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
            Log.Debug($"Loading Kubernetes models");
            try
            {
                foreach (var assembly in GetAvailableAssemblyList())
                {
                    if (assembly?.CustomAttributes.Count() > 0)
                    {
                        if (assembly?.CustomAttributes?.First()?.NamedArguments.Count() > 0)
                        {
                            var namedArgs = assembly.CustomAttributes.First().NamedArguments;
                            if (namedArgs.Any(x => (new List<string>() { "ApiVersion", "Group", "Kind" }).Contains(x.MemberName)))
                            {
                                GlobalVariables.availableKubeTypes.Add(new GlobalAssemblyKubeType()
                                {
                                    kind = namedArgs.FirstOrDefault(x => x.MemberName == "Kind").TypedValue.Value.ToString(),
                                    version = namedArgs.FirstOrDefault(x => x.MemberName == "ApiVersion").TypedValue.Value.ToString(),
                                    group = namedArgs.FirstOrDefault(x => x.MemberName == "Group").TypedValue.Value.ToString(),
                                    assemblyFullName = assembly.AssemblyQualifiedName
                                });
                            }
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Log.Error($"Error loading Kubernetes models: {ex.Message}");
            }
            Log.Debug($"Done loading Kubernetes models, {GlobalVariables.availableKubeTypes.Count} found!");
        }
        public static IEnumerable<Type> GetAvailableAssemblyList()
        {
            //return Assembly.Load("KubernetesClient").GetTypes().Where(t => t.IsClass && t.Namespace == "k8s.Models");
            return Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.Namespace == "k8s.Models");
        }
    }
}
