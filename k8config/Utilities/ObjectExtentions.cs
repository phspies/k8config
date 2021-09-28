using k8config.DataModels;
using k8config.GUIEvents.RealtimeMode.DataModels;
using k8s.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace k8config.Utilities
{
    public static class ObjectExtensions
    {
        public static List<PropertyInfo> GetObjectProperties(this object srcObject)
        {
            return srcObject.GetType().GetProperties().ToList();
        }
        public static PropertyInfo GetObjectProperty(this object srcObject, string property)
        {
            return srcObject.GetType().GetProperties().ToList().FirstOrDefault(x => x.Name.ToLower() == property.ToLower());
        }
        public static KubernetesPropertyAttribute GetJsonKubernetesAttribute(this object sourceObject, string jsonPropertyName)
        {
            return sourceObject.GetJsonProperty(jsonPropertyName).GetCustomAttributes(typeof(KubernetesPropertyAttribute), false).First() as KubernetesPropertyAttribute;
        }
        public static PropertyInfo GetJsonProperty<T>(this T obj, string jsonPropertyName)
        {
            return obj.GetType().GetProperties().FirstOrDefault(x => string.Compare((x.GetCustomAttributes(typeof(JsonPropertyAttribute), false).First() as JsonPropertyAttribute).PropertyName, jsonPropertyName, StringComparison.OrdinalIgnoreCase) == 0);
        }
        public static bool JsonPropertyExists<T>(this T obj, string jsonPropertyName)
        {
            return obj.GetType().GetProperties().Any(x => string.Compare((x.GetCustomAttributes(typeof(JsonPropertyAttribute), false).First() as JsonPropertyAttribute).PropertyName, jsonPropertyName, StringComparison.OrdinalIgnoreCase) == 0);
        }
        public static bool GetJsonObjectPropertyValueIsNotNull(this object srcObject, string jsonPropertyName)
        {
            return srcObject.GetType().GetProperties().FirstOrDefault(x => string.Compare((x.GetCustomAttributes(typeof(JsonPropertyAttribute), false).First() as JsonPropertyAttribute).PropertyName, jsonPropertyName, StringComparison.OrdinalIgnoreCase) == 0)?.GetValue(srcObject) == null ? false : true;
        }
        public static object GetJsonObjectPropertyValue(this object srcObject, string jsonPropertyName)
        {
            return srcObject.GetType().GetProperties().FirstOrDefault(x => string.Compare((x.GetCustomAttributes(typeof(JsonPropertyAttribute), false).First() as JsonPropertyAttribute).PropertyName, jsonPropertyName, StringComparison.OrdinalIgnoreCase) == 0)?.GetValue(srcObject);
        }
        public static object GetObjectPropertyValue(this object srcObject, string property)
        {
            return srcObject.GetType()?.GetProperties()?.ToList()?.FirstOrDefault(x => x.Name.ToLower() == property.ToLower())?.GetValue(srcObject);
        }
        public static bool ObjectPropertyValueExist(this object srcObject, string property)
        {
            return srcObject.GetType()?.GetProperties()?.ToList()?.Exists(x => x.Name.ToLower() == property.ToLower()) ?? false;
        }
        public static void SetObjectPropertyValue(this object targetObject, string jsonPropertyName, object objectValue)
        {
            targetObject.GetType().GetProperties().FirstOrDefault(x => string.Compare((x.GetCustomAttributes(typeof(JsonPropertyAttribute), false).First() as JsonPropertyAttribute).PropertyName, jsonPropertyName, StringComparison.OrdinalIgnoreCase) == 0).SetValue(targetObject, objectValue);
        }
        public static Type[] GetObjectGenericArguments(this object srcObject, string jsonPropertyName)
        {
            return srcObject.GetType().GetProperties().FirstOrDefault(x => string.Compare((x.GetCustomAttributes(typeof(JsonPropertyAttribute), false).First() as JsonPropertyAttribute).PropertyName, jsonPropertyName, StringComparison.OrdinalIgnoreCase) == 0).PropertyType.GetGenericArguments();
        }
        public static object GetDataNamePropValue<T>(this T src, string columnName)
        {
            return src.GetType().GetProperties().Where(pi => Attribute.IsDefined(pi, typeof(DataNameAttribute))).FirstOrDefault(x => x.Name == columnName)?.GetValue(src, null);
        }
        public static object GetNestedObject<T>(this T src, int index)
        {
            object returnObject = null;
            if (src.IsList())
            {
                int pointer = 0;
                foreach (var currentObject in (IList)src)
                {
                    if (index == pointer)
                    {
                        return currentObject;
                    }
                    pointer += 1;
                }
            }
            return returnObject;
        }
        public static object GetNestedPropertyValue(this object obj, string propertyName)
        {
            if (obj != null)
            {
                foreach (var prop in propertyName.Split('.').Select(s => obj.GetType().GetProperty(s)))
                {
                    if (prop == null) { return null; }
                    obj = prop.GetValue(obj, null);
                    if (obj == null) return null;
                }
            }
            return obj;
        }
        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

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
        public static void AddToDictionary(this object DictionaryObject, object KeyObject, object ValueObject)
        {
            Type DictionaryType = DictionaryObject.GetType();

            if (!(DictionaryType.IsGenericType && DictionaryType.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                throw new Exception("sorry object is not a dictionary");

            Type[] TemplateTypes = DictionaryType.GetGenericArguments();
            var add = DictionaryType.GetMethod("Add", new[] { TemplateTypes[0], TemplateTypes[1] });
            add.Invoke(DictionaryObject, new object[] { KeyObject, ValueObject });
        }
        public static T CastTo<T>(this object o) => (T)o;
        public static object CastToReflected(this object o, Type type)
        {
            if (type == typeof(string))
            {
                return Convert.ToString(o);
            }
            else if (type == typeof(int))
            {
                return Convert.ToInt32(o);
            }
            else if (type == typeof(long))
            {
                return Convert.ToInt64(o);
            }
            else if (type == typeof(bool))
            {
                return Convert.ToBoolean(o);
            }
            else
            {
                return Activator.CreateInstance(type, new object[] { o });
            }
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
            else if (o is IList)
            {
                return o.GetType().IsGenericType && (o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)) || o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)));
            }
            return false;
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
            if (o is Type)
            {
                return (o as Type).IsGenericType && ((o as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>)) || (o as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>))) && (o as Type).GetGenericArguments()[0] == typeof(Dictionary<,>);
            }
            else if (o is PropertyInfo)
            {
                return (o as PropertyInfo).PropertyType.IsGenericType && ((o as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>)) || (o as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(IDictionary<,>)));
            }
            else
            {
                return o is IDictionary &&
                   o.GetType().IsGenericType &&
                   (o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(IDictionary<,>)) || o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(IDictionary<,>))) && o.GetType().GetGenericArguments()[0] == typeof(Dictionary<,>);
            }
        }
        public static OptionsSlimType RetrieveAttributeValue(this object o, string attributeName)
        {
            return o.RetrieveAttributeValues().FirstOrDefault(x => x.name == attributeName);
        }
        public static List<OptionsSlimType> RetrieveAttributeValues(this object o)
        {
            List<OptionsSlimType> tmpAttributes = new List<OptionsSlimType>();
            List<string> removeAttributes = new List<string>() { "Kind", "ApiVersion", "Status" };
            if (o != null)
            {
                if (o.GetType().IsGenericType)
                {
                    if (o.GetType().GetGenericTypeDefinition() == typeof(List<>))
                    {

                        o.GetType().GetGenericArguments()[0].GetProperties().ToList().ForEach(x =>
                        {
                            if (x.PropertyType.Name == typeof(IList<>).Name)
                            {
                                if (!removeAttributes.Contains(x.Name))
                                {
                                    var tmpObject = new OptionsSlimType() { name = x.Name.ToLower(), primaryType = x.PropertyType, propertyIsList = (x.PropertyType.Name == typeof(IList<>).Name) };
                                    if (x.GetType().Name != typeof(Nullable).Name || x.GetType().Name != typeof(IList).Name || x.GetType().Name != typeof(IDictionary).Name)
                                    {
                                        tmpObject.propertyIsRequired = true;
                                    }
                                    tmpAttributes.Add(tmpObject);
                                }
                            }
                            else
                            {
                                if (!removeAttributes.Contains(x.Name))
                                {
                                    tmpAttributes.Add(new OptionsSlimType() { name = x.Name.ToLower(), primaryType = x.PropertyType, propertyIsList = false, });
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
                                var newOptionObject = new OptionsSlimType()
                                {
                                    name = x.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName,
                                    primaryType = x.GetKubeType(),
                                    value = x.GetValue(o, null),
                                    propertyIsRequired = (x.GetCustomAttributes(typeof(KubernetesPropertyAttribute), false)[0] as KubernetesPropertyAttribute).IsRequired

                                };

                                if (x.IsStringArray())
                                {

                                    newOptionObject.displayType = $"Array<{x.PropertyType.GetGenericArguments()[0].Name.Replace("V1", "")}>";
                                    newOptionObject.primaryType = x.PropertyType.GetGenericArguments()[0];
                                    newOptionObject.propertyIsArray = true;
                                    newOptionObject.entryFormat = $"{newOptionObject.name} {x.PropertyType.GetGenericArguments()[0].Name}|{x.PropertyType.GetGenericArguments()[0].Name}|{x.PropertyType.GetGenericArguments()[0].Name}";
                                }
                                else if (x.IsList())
                                {
                                    newOptionObject.displayType = $"List<{x.PropertyType.GetGenericArguments()[0].Name.Replace("V1", "")}>";
                                    newOptionObject.primaryType = x.PropertyType.GetGenericArguments()[0];
                                    newOptionObject.propertyIsList = true;
                                    newOptionObject.entryFormat = $"{newOptionObject.name} <-";
                                }
                                else if (x.IsDictionary())
                                {
                                    newOptionObject.displayType = $"Dictionary<{x.PropertyType.GetGenericArguments()[0].Name.Replace("V1", "")},{x.PropertyType.GetGenericArguments()[1].Name.Replace("V1", "")}>";
                                    newOptionObject.primaryType = x.PropertyType.GetGenericArguments()[0];
                                    newOptionObject.secondaryType = x.PropertyType.GetGenericArguments()[1];
                                    newOptionObject.properyType = x.PropertyType;
                                    newOptionObject.propertyIsDictionary = true;
                                    newOptionObject.entryFormat = $"{newOptionObject.name} <{x.PropertyType.GetGenericArguments()[0].Name.Replace("V1", "")}>:<{x.PropertyType.GetGenericArguments()[1].Name.Replace("V1", "")}>|<{x.PropertyType.GetGenericArguments()[0].Name.Replace("V1", "")}>:<{x.PropertyType.GetGenericArguments()[1].Name.Replace("V1", "")}>";
                                }
                                else if (x.PropertyType.GetGenericArguments().Length > 0)
                                {
                                    newOptionObject.displayType = x.PropertyType.GetGenericArguments()[0].Name.Replace("V1", "");
                                    newOptionObject.primaryType = x.PropertyType.GetGenericArguments()[0];
                                    newOptionObject.entryFormat = $"{newOptionObject.name} <{x.PropertyType.GetGenericArguments()[0].Name.Replace("V1", "")}>";
                                }
                                else
                                {
                                    newOptionObject.displayType = x.PropertyType.Name.Replace("V1", "");
                                    if (x.PropertyType.IsPrimitive || x.PropertyType == typeof(string))
                                    {
                                        newOptionObject.entryFormat = $"{newOptionObject.name} <{x.PropertyType.Name.Replace("V1", "")}>";
                                    }
                                    else
                                    {
                                        newOptionObject.entryFormat = $"{newOptionObject.name}";
                                    }
                                    newOptionObject.propertyIsNamedType = true;
                                }
                                tmpAttributes.Add(newOptionObject);
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
