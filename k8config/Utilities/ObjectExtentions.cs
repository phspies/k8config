using k8config.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace k8config.Utilities
{
    public static class ObjectExtensions
    {
        public static object ConstructDictionary(Type KeyType, Type ValueType)
        {
            Type[] TemplateTypes = new Type[] { KeyType, ValueType };
            Type DictionaryType = typeof(Dictionary<,>).MakeGenericType(TemplateTypes);

            return Activator.CreateInstance(DictionaryType);
        }
        public static void ForEach<T>(this IList<T> list, Action<T> action)
        {
            foreach (T t in list)
                action(t);
        }
        public static void SetValue(this object instance, PropertyInfo info, object value)
        {
            info.SetValue(instance, Convert.ChangeType(value, info.PropertyType));
        }
        public static void AddToDictionary(object DictionaryObject, object KeyObject, object ValueObject)
        {
            Type DictionaryType = DictionaryObject.GetType();

            if (!(DictionaryType.IsGenericType && DictionaryType.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                throw new Exception("sorry object is not a dictionary");

            Type[] TemplateTypes = DictionaryType.GetGenericArguments();
            var add = DictionaryType.GetMethod("Add", new[] { TemplateTypes[0], TemplateTypes[1] });
            add.Invoke(DictionaryObject, new object[] { KeyObject, ValueObject });
        }
        public static T CastTo<T>(this object o) => (T)o;
        public static dynamic CastToReflected(this object o, Type type)
        {
            var methodInfo = typeof(ObjectExtensions).GetMethod(nameof(CastTo), BindingFlags.Static | BindingFlags.Public);
            var genericArguments = new[] { type };
            var genericMethodInfo = methodInfo?.MakeGenericMethod(genericArguments);
            return genericMethodInfo?.Invoke(null, new[] { o });
        }
        public static bool IsList(this object o)
        {
            if (o is Type)
            {
                return (o as Type).IsGenericType && ((o as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)) || (o as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)));
            }
            else if (o is PropertyInfo)
            {
                return (o as PropertyInfo).PropertyType.IsGenericType && ((o as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)) || (o as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)));
            }
            else
            {
                return o is IList &&
                   o.GetType().IsGenericType &&
                   (o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)) || o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)));
            }
        }
        public static Type GetKubeType(this object o)
        {
            Type returnType = null;
            if (o is Type)
            {
                if ((o as Type).IsGenericType && ((o as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)) || (o as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>))))
                {
                    returnType = (o as Type).GetGenericArguments()[0];
                }
                else
                {
                    returnType = (o as Type);
                }
            }
            else if (o is PropertyInfo)
            {
                if ((o as PropertyInfo).PropertyType.IsGenericType && ((o as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)) || (o as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>))))
                {
                    returnType = (o as PropertyInfo).PropertyType.GetGenericArguments()[0];
                }
                else
                {
                    returnType = (o as PropertyInfo).PropertyType;
                }
            }
            else if (o is IList)
            { 
                if (o.GetType().IsGenericType && (o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)) || o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>))))
                {
                    returnType = o.GetType().GetGenericArguments()[0];
                }
            }
            else
            {
                returnType = o.GetType();
            }
            return returnType;
        }
        public static bool IsStringArray(this object o)
        {
            if (o is Type)
            {
                return (o as Type).IsGenericType && ((o as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)) || (o as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>))) && (o as Type).GetGenericArguments()[0] == typeof(String);
            }
            else if (o is PropertyInfo)
            {
                return (o as PropertyInfo).PropertyType.IsGenericType && ((o as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)) || (o as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>))) && (o as PropertyInfo).PropertyType.GetGenericArguments()[0] == typeof(String);
            }
            else
            {
                return o is IList &&
                   o.GetType().IsGenericType &&
                   (o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)) || o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>))) && o.GetType().GetGenericArguments()[0] == typeof(String);
            }
        }

        public static bool IsDictionary(this object o)
        {
            return o is IDictionary &&
               o.GetType().IsGenericType &&
               o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<Object, Object>));
        }
        public static ObjectPropertyType RetrieveAttributeValue(this object o, string attributeName)
        {
            OptionsSlimType tmp = o.RetrieveAttributeValues().FirstOrDefault(x => x.name == attributeName);
            return new ObjectPropertyType() { name = attributeName, kubeType = tmp.GetType(), kubeObject = tmp };
        }

        public static List<OptionsSlimType> RetrieveAttributeValues(this object o)
        {
            List<OptionsSlimType> tmpAttributes = new List<OptionsSlimType>();
            List<string> removeAttributes = new List<string>() { "Kind", "ApiVersion", "Status" };
            if (o != null)
            {
                if (o.GetType().IsGenericType)
                {
                    var test = o.GetType().GetGenericTypeDefinition();
                    if (o.GetType().GetGenericTypeDefinition() == typeof(List<>))
                    {

                        o.GetType().GetGenericArguments()[0].GetProperties().ToList().ForEach(x =>
                        {
                            //var test2= Activator.CreateInstance(x.PropertyType); 
                            //var test = x.GetValue(o, null);      
                            if (x.PropertyType.Name == typeof(IList<>).Name)
                            {
                                if (!removeAttributes.Contains(x.Name))
                                {
                                    var tmpObject = new OptionsSlimType() { name = x.Name.ToLower(), type = x.PropertyType, isList = (x.PropertyType.Name == typeof(IList<>).Name) };
                                    if (x.GetType().Name != typeof(Nullable).Name || x.GetType().Name != typeof(IList).Name || x.GetType().Name != typeof(IDictionary).Name)
                                    {
                                        tmpObject.isRequired = true;
                                    }
                                    tmpAttributes.Add(tmpObject);
                                }
                            }
                            else
                            {
                                if (!removeAttributes.Contains(x.Name))
                                {
                                    tmpAttributes.Add(new OptionsSlimType() { name = x.Name.ToLower(), type = x.PropertyType, isList = false, });
                                    //kubeObject = x.GetValue(x, null)
                                }

                            }
                        });

                    }
                }
                else
                {
                    o.GetType().GetProperties().ToList().ForEach(x =>
                        {
                            if (!removeAttributes.Contains(x.Name))
                            {
                                var tmpObject = new OptionsSlimType() { name = x.Name.ToLower(), type = x.GetKubeType(), value = x.GetValue(o, null) };
                                //if (x.PropertyType.Name != typeof(Nullable<>).Name)
                                //{
                                //    tmpObject.isRequired = true;
                                //}

                                if (x.IsStringArray())
                                {
                                    tmpObject.displayType = $"Array<{x.PropertyType.GetGenericArguments()[0].Name}>";
                                    tmpObject.isArray = true;
                                }
                                else if (x.IsList())
                                {
                                    tmpObject.displayType = $"List<{x.PropertyType.GetGenericArguments()[0].Name}>";
                                    tmpObject.isList = x.IsList();
                                }
                                else if (x.PropertyType.GetGenericArguments().Length > 0)
                                {
                                    tmpObject.displayType = x.PropertyType.GetGenericArguments()[0].Name;
                                }
                                else
                                {
                                    tmpObject.displayType = x.PropertyType.Name.Replace("V1", "");
                                }
                                tmpAttributes.Add(tmpObject);
                            }
                        });
                }
            }
            
            return tmpAttributes;

        }
        public static Dictionary<string, object> GetPropertyAttributes(PropertyInfo property)
        {
            Dictionary<string, object> attribs = new Dictionary<string, object>();
            // look for attributes that takes one constructor argument
            foreach (CustomAttributeData attribData in property.GetCustomAttributesData())
            {

                if (attribData.ConstructorArguments.Count == 1)
                {
                    string typeName = attribData.Constructor.DeclaringType.Name;
                    if (typeName.EndsWith("Attribute")) typeName = typeName.Substring(0, typeName.Length - 9);
                    attribs[typeName] = attribData.ConstructorArguments[0].Value;
                }

            }
            return attribs;
        }
    }
}
