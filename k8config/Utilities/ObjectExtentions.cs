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
    /// <summary>
    /// Defines the <see cref="ObjectExtensions" />.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// The GetObjectProperties.
        /// </summary>
        /// <param name="srcObject">The srcObject<see cref="object"/>.</param>
        /// <returns>The <see cref="List{PropertyInfo}"/>.</returns>
        public static List<PropertyInfo> GetObjectProperties(this object srcObject)
        {
            return srcObject.GetPublicProperties();
        }

        /// <summary>
        /// The GetObjectProperty.
        /// </summary>
        /// <param name="srcObject">The srcObject<see cref="object"/>.</param>
        /// <param name="property">The property<see cref="string"/>.</param>
        /// <returns>The <see cref="PropertyInfo"/>.</returns>
        public static PropertyInfo GetObjectProperty(this object srcObject, string property)
        {
            return srcObject.GetPublicProperties()?.FirstOrDefault(x => x.Name.ToLower() == property.ToLower());
        }

        /// <summary>
        /// The GetJsonKubernetesAttribute.
        /// </summary>
        /// <param name="sourceObject">The sourceObject<see cref="object"/>.</param>
        /// <param name="jsonPropertyName">The jsonPropertyName<see cref="string"/>.</param>
        /// <returns>The <see cref="KubernetesPropertyAttribute"/>.</returns>
        public static KubernetesPropertyAttribute GetJsonKubernetesAttribute(this object sourceObject, string jsonPropertyName)
        {
            return sourceObject.GetJsonPropertyInfo(jsonPropertyName).GetCustomAttributes(typeof(KubernetesPropertyAttribute), false).First() as KubernetesPropertyAttribute;
        }

        /// <summary>
        /// The GetJsonPropertyInfo.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="obj">The obj<see cref="T"/>.</param>
        /// <param name="jsonPropertyName">The jsonPropertyName<see cref="string"/>.</param>
        /// <returns>The <see cref="PropertyInfo"/>.</returns>
        public static PropertyInfo GetJsonPropertyInfo<T>(this T obj, string jsonPropertyName)
        {
            return obj.GetPublicProperties()?.FirstOrDefault(x => string.Compare((x.GetCustomAttributes(typeof(JsonPropertyAttribute), false).First() as JsonPropertyAttribute).PropertyName, jsonPropertyName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// The JsonPropertyExists.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="obj">The obj<see cref="T"/>.</param>
        /// <param name="jsonPropertyName">The jsonPropertyName<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool JsonPropertyExists<T>(this T obj, string jsonPropertyName)
        {
            return obj.IsList() ? false : (obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)?.Any(x => string.Compare((x.GetCustomAttributes(typeof(JsonPropertyAttribute), false)?.First() as JsonPropertyAttribute)?.PropertyName, jsonPropertyName, StringComparison.OrdinalIgnoreCase) == 0) ?? false);
        }

        /// <summary>
        /// The GetJsonObjectPropertyValueIsNotNull.
        /// </summary>
        /// <param name="srcObject">The srcObject<see cref="object"/>.</param>
        /// <param name="jsonPropertyName">The jsonPropertyName<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool GetJsonObjectPropertyValueIsNotNull(this object srcObject, string jsonPropertyName)
        {
            return srcObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(x => string.Compare((x.GetCustomAttributes(typeof(JsonPropertyAttribute), false).First() as JsonPropertyAttribute).PropertyName, jsonPropertyName, StringComparison.OrdinalIgnoreCase) == 0)?.GetValue(srcObject, null) == null ? true : false;
        }


        public static bool GetJsonObjectPropertyValueExists(this object srcObject, string jsonPropertyName)
        {
            return srcObject.GetJsonObjectPropertyValues().Any(x => x == jsonPropertyName);
        }
        public static List<string> GetJsonObjectPropertyValues(this object srcObject)
        {
            return srcObject.GetPublicProperties()?.Select(x => (x.GetCustomAttribute(typeof(JsonPropertyAttribute), false) as JsonPropertyAttribute).PropertyName).ToList();
        }


        /// <summary>
        /// The GetJsonObjectPropertyValue.
        /// </summary>
        /// <param name="srcObject">The srcObject<see cref="object"/>.</param>
        /// <param name="jsonPropertyName">The jsonPropertyName<see cref="string"/>.</param>
        /// <returns>The <see cref="object"/>.</returns>
        public static object GetJsonObjectPropertyValue(this object srcObject, string jsonPropertyName)
        {
            return srcObject.GetPublicProperties()?.FirstOrDefault(x => string.Compare((x.GetCustomAttributes(typeof(JsonPropertyAttribute), false).First() as JsonPropertyAttribute).PropertyName, jsonPropertyName, StringComparison.OrdinalIgnoreCase) == 0)?.GetValue(srcObject, null);
        }

        /// <summary>
        /// The GetObjectPropertyValue.
        /// </summary>
        /// <param name="srcObject">The srcObject<see cref="object"/>.</param>
        /// <param name="property">The property<see cref="string"/>.</param>
        /// <returns>The <see cref="object"/>.</returns>
        public static object GetObjectPropertyValue(this object srcObject, string property)
        {
            return srcObject.GetPublicProperties()?.FirstOrDefault(x => x.Name.ToLower() == property.ToLower())?.GetValue(srcObject);
        }

        /// <summary>
        /// The ObjectPropertyValueExist.
        /// </summary>
        /// <param name="srcObject">The srcObject<see cref="object"/>.</param>
        /// <param name="property">The property<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool ObjectPropertyValueExist(this object srcObject, string property)
        {
            return srcObject.GetPublicProperties()?.ToList()?.Exists(x => x.Name.ToLower() == property.ToLower()) ?? false;
        }

        /// <summary>
        /// The SetObjectPropertyValue.
        /// </summary>
        /// <param name="targetObject">The targetObject<see cref="object"/>.</param>
        /// <param name="jsonPropertyName">The jsonPropertyName<see cref="string"/>.</param>
        /// <param name="objectValue">The objectValue<see cref="object"/>.</param>
        public static void SetObjectPropertyValue(this object targetObject, string jsonPropertyName, object objectValue)
        {
            targetObject.GetPublicProperties()?.FirstOrDefault(x => string.Compare((x.GetCustomAttributes(typeof(JsonPropertyAttribute), false).First() as JsonPropertyAttribute).PropertyName, jsonPropertyName, StringComparison.OrdinalIgnoreCase) == 0).SetValue(targetObject, objectValue);
        }

        /// <summary>
        /// The GetObjectGenericArguments.
        /// </summary>
        /// <param name="srcObject">The srcObject<see cref="object"/>.</param>
        /// <param name="jsonPropertyName">The jsonPropertyName<see cref="string"/>.</param>
        /// <returns>The <see cref="Type[]"/>.</returns>
        public static Type[] GetObjectGenericArguments(this object srcObject, string jsonPropertyName)
        {
            return srcObject.GetPublicProperties()?.FirstOrDefault(x => string.Compare((x.GetCustomAttributes(typeof(JsonPropertyAttribute), false).First() as JsonPropertyAttribute).PropertyName, jsonPropertyName, StringComparison.OrdinalIgnoreCase) == 0)?.PropertyType?.GetGenericArguments();
        }

        /// <summary>
        /// The GetDataNamePropValue.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="src">The src<see cref="T"/>.</param>
        /// <param name="columnName">The columnName<see cref="string"/>.</param>
        /// <returns>The <see cref="object"/>.</returns>
        public static object GetDataNamePropValue<T>(this T src, string columnName)
        {
            return src.GetPublicProperties()?.Where(pi => Attribute.IsDefined(pi, typeof(DataNameAttribute))).FirstOrDefault(x => x.Name == columnName)?.GetValue(src, null);
        }

        /// <summary>
        /// The GetPublicProperties.
        /// </summary>
        /// <param name="srcObject">The srcObject<see cref="object"/>.</param>
        /// <returns>The <see cref="List{PropertyInfo}"/>.</returns>
        private static List<PropertyInfo> GetPublicProperties(this object srcObject)
        {
            return srcObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
        }

        /// <summary>
        /// The GetNestedObject.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="src">The src<see cref="T"/>.</param>
        /// <param name="index">The index<see cref="int"/>.</param>
        /// <returns>The <see cref="object"/>.</returns>
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

        /// <summary>
        /// The GetNestedPropertyValue.
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/>.</param>
        /// <param name="propertyName">The propertyName<see cref="string"/>.</param>
        /// <returns>The <see cref="object"/>.</returns>
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

        /// <summary>
        /// The Clone.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="source">The source<see cref="T"/>.</param>
        /// <returns>The <see cref="T"/>.</returns>
        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        /// <summary>
        /// The ConstructDictionary.
        /// </summary>
        /// <param name="KeyType">The KeyType<see cref="Type"/>.</param>
        /// <param name="ValueType">The ValueType<see cref="Type"/>.</param>
        /// <returns>The <see cref="object"/>.</returns>
        public static object ConstructDictionary(Type KeyType, Type ValueType)
        {
            return Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(new Type[] { KeyType, ValueType }));
        }

        /// <summary>
        /// The ForEach.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="list">The list<see cref="IList{T}"/>.</param>
        /// <param name="action">The action<see cref="Action{T}"/>.</param>
        public static void ForEach<T>(this IList<T> list, Action<T> action)
        {
            foreach (T t in list)
                action(t);
        }
        /// <summary>
        /// The AddToDictionary.
        /// </summary>
        /// <param name="DictionaryObject">The DictionaryObject<see cref="object"/>.</param>
        /// <param name="KeyObject">The KeyObject<see cref="object"/>.</param>
        /// <param name="ValueObject">The ValueObject<see cref="object"/>.</param>
        public static void AddToDictionary(this object DictionaryObject, object KeyObject, object ValueObject)
        {
            Type DictionaryType = DictionaryObject.GetType();

            if (!(DictionaryType.IsGenericType && DictionaryType.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                throw new Exception("sorry object is not a dictionary");

            Type[] TemplateTypes = DictionaryType.GetGenericArguments();
            var add = DictionaryType.GetMethod("Add", new[] { TemplateTypes[0], TemplateTypes[1] });
            add.Invoke(DictionaryObject, new object[] { KeyObject, ValueObject });
        }

        /// <summary>
        /// The CastTo.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="o">The o<see cref="object"/>.</param>
        /// <returns>The <see cref="T"/>.</returns>
        public static T CastTo<T>(this object o) => (T)o;

        /// <summary>
        /// The CastToReflected.
        /// </summary>
        /// <param name="o">The o<see cref="object"/>.</param>
        /// <param name="type">The type<see cref="Type"/>.</param>
        /// <returns>The <see cref="object"/>.</returns>
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

        /// <summary>
        /// The IsList.
        /// </summary>
        /// <param name="srcObject">The o<see cref="object"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsList(this object srcObject)
        {
            if (srcObject is Type)
            {
                return (srcObject as Type).IsGenericType && ((srcObject as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)) || (srcObject as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)));
            }
            else if (srcObject is PropertyInfo)
            {
                return (srcObject as PropertyInfo).PropertyType.IsGenericType && ((srcObject as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)) || (srcObject as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)));
            }
            else if (srcObject is IList)
            {
                return srcObject.GetType().IsGenericType && (srcObject.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)) || srcObject.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)));
            }
            return false;
        }

        /// <summary>
        /// The GetKubeType.
        /// </summary>
        /// <param name="secObject">The o<see cref="object"/>.</param>
        /// <returns>The <see cref="Type"/>.</returns>
        public static Type GetKubeType(this object secObject)
        {
            Type returnType = null;
            if (secObject is Type)
            {
                if ((secObject as Type).IsGenericType && ((secObject as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)) || (secObject as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>))))
                {
                    returnType = (secObject as Type).GetGenericArguments()[0];
                }
                else
                {
                    returnType = (secObject as Type);
                }
            }
            else if (secObject is PropertyInfo)
            {
                if ((secObject as PropertyInfo).PropertyType.IsGenericType && ((secObject as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)) || (secObject as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>))))
                {
                    returnType = (secObject as PropertyInfo).PropertyType.GetGenericArguments()[0];
                }
                else
                {
                    returnType = (secObject as PropertyInfo).PropertyType;
                }
            }
            else if (secObject is IList)
            {
                if (secObject.GetType().IsGenericType && (secObject.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)) || secObject.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>))))
                {
                    returnType = secObject.GetType().GetGenericArguments()[0];
                }
            }
            else
            {
                returnType = secObject.GetType();
            }
            return returnType;
        }

        /// <summary>
        /// The IsStringArray.
        /// </summary>
        /// <param name="srcObject">The o<see cref="object"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsStringArray(this object srcObject)
        {
            if (srcObject is Type)
            {
                return (srcObject as Type).IsGenericType && ((srcObject as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)) || (srcObject as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>))) && (srcObject as Type).GetGenericArguments()[0] == typeof(String);
            }
            else if (srcObject is PropertyInfo)
            {
                return (srcObject as PropertyInfo).PropertyType.IsGenericType && ((srcObject as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)) || (srcObject as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>))) && (srcObject as PropertyInfo).PropertyType.GetGenericArguments()[0] == typeof(String);
            }
            else
            {
                return srcObject is IList &&
                   srcObject.GetType().IsGenericType &&
                   (srcObject.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)) || srcObject.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>))) && srcObject.GetType().GetGenericArguments()[0] == typeof(String);
            }
        }

        /// <summary>
        /// The IsDictionary.
        /// </summary>
        /// <param name="secObject">The o<see cref="object"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsDictionary(this object secObject)
        {
            if (secObject is Type)
            {
                return (secObject as Type).IsGenericType && ((secObject as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>)) || (secObject as Type).GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>))) && (secObject as Type).GetGenericArguments()[0] == typeof(Dictionary<,>);
            }
            else if (secObject is PropertyInfo)
            {
                return (secObject as PropertyInfo).PropertyType.IsGenericType && ((secObject as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>)) || (secObject as PropertyInfo).PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(IDictionary<,>)));
            }
            else
            {
                return secObject is IDictionary &&
                   secObject.GetType().IsGenericType &&
                   (secObject.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(IDictionary<,>)) || secObject.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(IDictionary<,>))) && secObject.GetType().GetGenericArguments()[0] == typeof(Dictionary<,>);
            }
        }

        /// <summary>
        /// The RetrieveAttributeValue.
        /// </summary>
        /// <param name="srcObject">The o<see cref="object"/>.</param>
        /// <param name="attributeName">The attributeName<see cref="string"/>.</param>
        /// <returns>The <see cref="OptionsSlimType"/>.</returns>
        public static OptionsSlimType RetrieveAttributeValue(this object srcObject, string attributeName)
        {
            return srcObject.RetrieveAttributeValues().FirstOrDefault(x => string.Compare(x.name, attributeName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// The RetrieveAttributeValues.
        /// </summary>
        /// <param name="srcObject">The o<see cref="object"/>.</param>
        /// <returns>The <see cref="List{OptionsSlimType}"/>.</returns>
        public static List<OptionsSlimType> RetrieveAttributeValues(this object srcObject)
        {
            List<OptionsSlimType> returnAttributes = new List<OptionsSlimType>();
            List<string> ignoreAttributes = new List<string>() { "Kind", "ApiVersion", "Status" };
            if (srcObject == null)
            {
                return returnAttributes;
            }

            if (srcObject.GetType().IsGenericType)
            {
                if (srcObject.GetType().GetGenericTypeDefinition() == typeof(List<>))
                {

                    srcObject.GetType().GetGenericArguments()[0].GetProperties().ToList().ForEach(x =>
                    {
                        if (x.PropertyType.Name == typeof(IList<>).Name)
                        {
                            if (!ignoreAttributes.Contains(x.Name))
                            {
                                var tmpObject = new OptionsSlimType() { name = x.Name.ToLower(), primaryType = x.PropertyType, propertyIsList = (x.PropertyType.Name == typeof(IList<>).Name) };
                                if (x.GetType().Name != typeof(Nullable).Name || x.GetType().Name != typeof(IList).Name || x.GetType().Name != typeof(IDictionary).Name)
                                {
                                    tmpObject.propertyIsRequired = true;
                                }
                                returnAttributes.Add(tmpObject);
                            }
                        }
                        else
                        {
                            if (!ignoreAttributes.Contains(x.Name))
                            {
                                returnAttributes.Add(new OptionsSlimType() { name = x.Name.ToLower(), primaryType = x.PropertyType, propertyIsList = false, });
                            }

                        }
                    });
                }
            }
            else
            {
                srcObject.GetType().GetProperties().ToList().ForEach(x =>
                    {
                        if (!ignoreAttributes.Contains(x.Name))
                        {
                            var newOptionObject = new OptionsSlimType()
                            {
                                name = x.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName,
                                primaryType = x.GetKubeType(),
                                value = x.GetValue(srcObject, null),
                                propertyIsRequired = (x.GetCustomAttributes(typeof(KubernetesPropertyAttribute), false)[0] as KubernetesPropertyAttribute).IsRequired
                            };
                            if (x.PropertyType.Name == "IntstrIntOrString")
                            {
                                newOptionObject.displayType = $"String";
                                newOptionObject.primaryType = typeof(string);
                                newOptionObject.entryFormat = $"{newOptionObject.name} String";
                            }
                            else if (x.IsStringArray())
                            {
                                newOptionObject.displayType = $"Array<{x.PropertyType.GetGenericArguments()[0].Name.Replace("V1", "")}>";
                                newOptionObject.primaryType = x.PropertyType.GetGenericArguments()[0];
                                newOptionObject.propertyIsArray = true;
                                newOptionObject.entryFormat = $"{newOptionObject.name} |{x.PropertyType.GetGenericArguments()[0].Name}|{x.PropertyType.GetGenericArguments()[0].Name}|{x.PropertyType.GetGenericArguments()[0].Name}|";
                            }
                            else if (x.IsList())
                            {
                                newOptionObject.displayType = $"List<{x.PropertyType.GetGenericArguments()[0].Name.Replace("V1", "")}>";
                                newOptionObject.primaryType = x.PropertyType.GetGenericArguments()[0];
                                newOptionObject.propertyIsList = true;
                                newOptionObject.entryFormat = $"{newOptionObject.name}";
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
                            returnAttributes.Add(newOptionObject);
                        }
                    });
            }
            return returnAttributes;
        }
    }
}
