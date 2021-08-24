using k8config.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace k8config.Utilities
{
    static class KubeObject
    {
        public static bool IsCurrentObjectRoot()
        {
            return GlobalVariables.promptArray.Count() == 1 ? true : false;
        }
        public static bool DoesRootIndexExist(int _index)
        {
            return GlobalVariables.sessionDefinedKinds.Exists(x => x.index == _index);
        }
        public static bool DoesNestedIndexExist(object _object, int _index)
        {
            if (_object.IsListType())
            {
                int index = 0;
                foreach (var currentObject in (IEnumerable)_object)
                {
                    if (index == _index) { return true; }
                    index += 1;
                }
                return false;
            }
            return false;
        }
        public static List<OptionsSlimType> GetNestedList(object _object)
        {
            List<OptionsSlimType> tmpList = new List<OptionsSlimType>();
            if (_object.IsListType())
            {
                int pointer = 0;
                foreach (var currentObject in (IEnumerable)_object)
                {
                    tmpList.Add(new OptionsSlimType() { index = pointer, name = currentObject.GetType().Name, value = currentObject, type = currentObject.GetType() });
                    pointer += 1;
                }
            }
            return tmpList;
        }
        public static object GetNestedObject(object _object, int index)
        {
            object returnObject = null;
            if (_object.IsListType())
            {
                int pointer = 0;
                foreach (var currentObject in (IEnumerable)_object)
                {  
                    if (index == pointer)
                    {
                        returnObject = currentObject;
                    }
                    pointer += 1;
                }
            }
            return returnObject;
        }
        public static object GetCurrentObject()
        {
            object privateObject = new object();
            if (GlobalVariables.promptArray.Count() > 1)
            {
                privateObject = GlobalVariables.sessionDefinedKinds.FirstOrDefault(x => x.index == int.Parse(GlobalVariables.promptArray[1])).KubeObject;
                for (int pointer = 1; GlobalVariables.promptArray.ToArray().Length > pointer; pointer++)
                {
                    int index = 0;
                    string pointerPromptValue = GlobalVariables.promptArray[pointer];
                    if (int.TryParse(pointerPromptValue, out index))
                    {
                        if (pointer == 1 && GlobalVariables.sessionDefinedKinds.Exists(x => x.index == index))
                        {
                            privateObject = GlobalVariables.sessionDefinedKinds.FirstOrDefault(x => x.index == index).KubeObject;
                        }
                        else
                        {
                            if (KubeObject.DoesNestedIndexExist(privateObject, index))
                            {
                                privateObject = KubeObject.GetNestedObject(privateObject, index);
                            }
                        }
                    }
                    else
                    {
                        var currentObject = privateObject.GetType().GetProperties().ToList().FirstOrDefault(x => x.Name.ToLower() == pointerPromptValue);
                        object tempvalue = privateObject.GetType().GetProperty(currentObject.Name).GetValue(privateObject);
                        if (tempvalue == null)
                        {
                            privateObject.GetType().GetProperty(currentObject.Name).SetValue(privateObject, Activator.CreateInstance(currentObject.PropertyType));
                        }
                        privateObject = privateObject.GetType().GetProperty(currentObject.Name).GetValue(privateObject);
                    }
                }
            }
            return privateObject;
        }
    }
}
