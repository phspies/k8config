using k8config.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        public static void DeleteNestedAtIndex(object _object, int _index)
        {
            if (_object.IsList())
            {
                ((IList)_object).RemoveAt(_index);
            }
        }
        public static bool DoesNestedIndexExist(object _object, int _index)
        {
            return (_object.IsList() && ((IList)_object).Count > _index);
        }
        public static List<OptionsSlimType> GetNestedList(object _object)
        {
            List<OptionsSlimType> tmpList = new List<OptionsSlimType>();
            if (_object.IsList())
            {
                int pointer = 0;
                foreach (var currentObject in (IList)_object)
                {
                    tmpList.Add(new OptionsSlimType() { 
                        index = pointer, 
                        name = currentObject.GetNestedPropertyValue("Name") == null ? currentObject.GetNestedPropertyValue("Metadata.Name")?.ToString() : currentObject.GetNestedPropertyValue("Name")?.ToString(), 
                        displayType = currentObject.GetType().Name, value = currentObject, 
                        primaryType = currentObject.GetType() 
                    });
                    pointer += 1;
                }
            }
            return tmpList;
        }
        public static object GetNestedObject(object _object, int index)
        {
            object returnObject = null;
            if (_object.IsList())
            {
                int pointer = 0;
                foreach (var currentObject in (IList)_object)
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
                            Type[] selectListTypes = privateObject.GetType().GetProperty(currentObject.Name).PropertyType.GetGenericArguments();
                            if (selectListTypes.Length == 1)
                            {
                                if (selectListTypes[0].Name == "String")
                                {
                                    currentObject.GetType().GetMethod("Add").Invoke(currentObject, new[] { new String("") });
                                }
                                else
                                {
                                    currentObject.GetType().GetMethod("Add").Invoke(currentObject, new[] { Activator.CreateInstance(selectListTypes[0]) });
                                }
                            }
                            else if (selectListTypes.Length == 2)
                            {
                                var temp = ObjectExtensions.ConstructDictionary(selectListTypes[0], selectListTypes[1]);
                                privateObject.GetType().GetProperty(currentObject.Name).SetValue(privateObject, temp);
             
                            }
                        }
                        privateObject = privateObject.GetType().GetProperty(currentObject.Name).GetValue(privateObject);
                    }
                }
            }
            return privateObject;
        }

    }
}
