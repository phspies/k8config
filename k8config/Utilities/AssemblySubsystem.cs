using k8config.DataModels;
using k8s;
using k8s.Models;
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
            List<GlobalAssemblyKubeType> tmpList = new List<GlobalAssemblyKubeType>();
            GlobalVariables.Log.Debug($"Loading Kubernetes models");
            try
            {
                foreach (var assembly in GetAvailableAssemblyList().DistinctBy(x => x.Name))
                {
                    if (typeof(IKubernetesObject<V1ObjectMeta>).IsAssignableFrom(Type.GetType(assembly.AssemblyQualifiedName)) && (assembly?.CustomAttributes.Count() > 0) && assembly?.CustomAttributes?.First()?.NamedArguments.Count() > 0)
                    {
                        var namedArgs = assembly.CustomAttributes.First().NamedArguments;
                        tmpList.Add(new GlobalAssemblyKubeType()
                        {
                            classKind = assembly.Name.Replace("V1", ""),
                            kind = namedArgs.FirstOrDefault(x => x.MemberName == "Kind").TypedValue.Value.ToString(),
                            version = namedArgs.FirstOrDefault(x => x.MemberName == "ApiVersion").TypedValue.Value.ToString(),
                            group = namedArgs.FirstOrDefault(x => x.MemberName == "Group").TypedValue.Value.ToString(),
                            assemblyFullName = assembly.AssemblyQualifiedName
                        });
                    }
                };
            }
            catch (Exception ex)
            {
                GlobalVariables.Log.Error($"Error loading Kubernetes models: {ex.Message}");
            }
            GlobalVariables.availableKubeTypes = new List<GlobalAssemblyKubeType>(tmpList.OrderBy(x => x.classKind));
            GlobalVariables.Log.Debug($"Done loading Kubernetes models, {GlobalVariables.availableKubeTypes.Count} found!");
        }
        public static IEnumerable<Type> GetAvailableAssemblyList()
        {
            //return Assembly.Load("KubernetesClient").GetTypes().Where(t => t.IsClass && t.Namespace == "k8s.Models");
            return Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.Namespace == "k8s.Models");
        }
    }
}
