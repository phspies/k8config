using k8config.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace k8config.Utilities
{
    public static class ObjectExtensions
    {
        public static T CastTo<T>(this object o) => (T)o;
        public static dynamic CastToReflected(this object o, Type type)
        {
            var methodInfo = typeof(ObjectExtensions).GetMethod(nameof(CastTo), BindingFlags.Static | BindingFlags.Public);
            var genericArguments = new[] { type };
            var genericMethodInfo = methodInfo?.MakeGenericMethod(genericArguments);
            return genericMethodInfo?.Invoke(null, new[] { o });
        }
        public static List<ObjectPropertyType> RetrieveAttributeValues(this object o)
        {
            List<ObjectPropertyType> tmpAttributes = new List<ObjectPropertyType>();
            List<string> removeAttributes = new List<string>() {"Kind", "ApiVersion", "Status" };
            o.GetType().GetProperties().ToList().ForEach(x =>
            {
                if (!removeAttributes.Contains(x.Name)) { tmpAttributes.Add(new ObjectPropertyType() { name = x.Name.ToLower(), kubeType = x.PropertyType, kubeObject = x.GetValue(o, null) }); }
            });
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
