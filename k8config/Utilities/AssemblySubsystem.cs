using k8config.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace k8config.Utilities
{
    public static class AssemblySubsystem
    {
        public static void BuildAvailableAssemblyList()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.IsClass && t.Namespace == "k8s.Models");
            foreach (var assembly in types)
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
        public static Type[] LoadType(string typeName)
        {
            return LoadType(typeName, true);
        }

        public static Type[] LoadType(string typeName, bool referenced)
        {
            return LoadType(typeName, referenced, true);
        }

        private static Type[] LoadType(string typeName, bool referenced, bool gac)
        {
            //check for problematic work
            if (string.IsNullOrEmpty(typeName) || !referenced && !gac)
                return new Type[] { };

            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            List<string> assemblyFullnames = new List<string>();
            List<Type> types = new List<Type>();

            if (referenced)
            {            //Check refrenced assemblies
                foreach (AssemblyName assemblyName in currentAssembly.GetReferencedAssemblies())
                {
                    //Load method resolve refrenced loaded assembly
                    Assembly assembly = Assembly.Load(assemblyName.FullName);

                    //Check if type is exists in assembly
                    var type = assembly.GetType(typeName, false, true);

                    if (type != null && !assemblyFullnames.Contains(assembly.FullName))
                    {
                        types.Add(type);
                        assemblyFullnames.Add(assembly.FullName);
                    }
                }
            }

            if (gac)
            {
                //GAC files
                string gacPath = Environment.GetFolderPath(System.Environment.SpecialFolder.Windows) + "\\assembly";
                var files = GetGlobalAssemblyCacheFiles(gacPath);
                foreach (string file in files)
                {
                    try
                    {
                        //reflection only
                        Assembly assembly = Assembly.ReflectionOnlyLoadFrom(file);

                        //Check if type is exists in assembly
                        var type = assembly.GetType(typeName, false, true);

                        if (type != null && !assemblyFullnames.Contains(assembly.FullName))
                        {
                            types.Add(type);
                            assemblyFullnames.Add(assembly.FullName);
                        }
                    }
                    catch
                    {
                        //your custom handling
                    }
                }
            }

            return types.ToArray();
        }

        public static string[] GetGlobalAssemblyCacheFiles(string path)
        {
            List<string> files = new List<string>();

            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo fi in di.GetFiles("*.dll"))
            {
                files.Add(fi.FullName);
            }

            foreach (DirectoryInfo diChild in di.GetDirectories())
            {
                var files2 = GetGlobalAssemblyCacheFiles(diChild.FullName);
                files.AddRange(files2);
            }

            return files.ToArray();
        }
        public static Type ReconstructType(string assemblyQualifiedName, bool throwOnError = true, params Assembly[] referencedAssemblies)
        {
            foreach (Assembly asm in referencedAssemblies)
            {
                var fullNameWithoutAssemblyName = assemblyQualifiedName.Replace($", {asm.FullName}", "");
                var type = asm.GetType(fullNameWithoutAssemblyName, throwOnError: false);
                if (type != null) return type;
            }

            if (assemblyQualifiedName.Contains("[["))
            {
                Type type = ConstructGenericType(assemblyQualifiedName, throwOnError);
                if (type != null)
                    return type;
            }
            else
            {
                Type type = Type.GetType(assemblyQualifiedName, false);
                if (type != null)
                    return type;
            }

            if (throwOnError)
                throw new Exception($"The type \"{assemblyQualifiedName}\" cannot be found in referenced assemblies.");
            else
                return null;
        }

        private static Type ConstructGenericType(string assemblyQualifiedName, bool throwOnError = true)
        {
            Regex regex = new Regex(@"^(?<name>\w+(\.\w+)*)`(?<count>\d)\[(?<subtypes>\[.*\])\](, (?<assembly>\w+(\.\w+)*)[\w\s,=\.]+)$?", RegexOptions.Singleline | RegexOptions.ExplicitCapture);
            Match match = regex.Match(assemblyQualifiedName);
            if (!match.Success)
                if (!throwOnError) return null;
                else throw new Exception($"Unable to parse the type's assembly qualified name: {assemblyQualifiedName}");

            string typeName = match.Groups["name"].Value;
            int n = int.Parse(match.Groups["count"].Value);
            string asmName = match.Groups["assembly"].Value;
            string subtypes = match.Groups["subtypes"].Value;

            typeName = typeName + $"`{n}";
            Type genericType = ReconstructType(typeName, throwOnError);
            if (genericType == null) return null;

            List<string> typeNames = new List<string>();
            int ofs = 0;
            while (ofs < subtypes.Length && subtypes[ofs] == '[')
            {
                int end = ofs, level = 0;
                do
                {
                    switch (subtypes[end++])
                    {
                        case '[': level++; break;
                        case ']': level--; break;
                    }
                } while (level > 0 && end < subtypes.Length);

                if (level == 0)
                {
                    typeNames.Add(subtypes.Substring(ofs + 1, end - ofs - 2));
                    if (end < subtypes.Length && subtypes[end] == ',')
                        end++;
                }

                ofs = end;
                n--;  // just for checking the count
            }

            if (n != 0)
                // This shouldn't ever happen!
                throw new Exception("Generic type argument count mismatch! Type name: " + assemblyQualifiedName);

            Type[] types = new Type[typeNames.Count];
            for (int i = 0; i < types.Length; i++)
            {
                try
                {
                    types[i] = ReconstructType(typeNames[i], throwOnError);
                    if (types[i] == null)  // if throwOnError, should not reach this point if couldn't create the type
                        return null;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unable to reconstruct generic type. Failed on creating the type argument {(i + 1)}: {typeNames[i]}. Error message: {ex.Message}");
                }
            }

            Type resultType = genericType.MakeGenericType(types);
            return resultType;
        }
    }

}
