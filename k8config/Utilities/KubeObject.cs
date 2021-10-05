using k8config.DataModels;
using k8config.GUIEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace k8config.Utilities
{
    static class KubeObject
    {

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

        public static object GetCurrentObject()
        {
            object returnObject = new object();
            if (YAMLModePromptObject.CurrentPromptPositionIsNotRoot)
            {
                returnObject = GlobalVariables.sessionDefinedKinds.FirstOrDefault(x => x.index == int.Parse(YAMLModePromptObject.GetFolderAt(1))).KubeObject;
                for (int pointer = 1; YAMLModePromptObject.Count > pointer; pointer++)
                {
                    int index = 0;
                    string pointerPromptValue = YAMLModePromptObject.GetFolderAt(pointer);
                    if (int.TryParse(pointerPromptValue, out index))
                    {
                        if (pointer == 1 && GlobalVariables.sessionDefinedKinds.Exists(x => x.index == index))
                        {
                            returnObject = GlobalVariables.sessionDefinedKinds.FirstOrDefault(x => x.index == index).KubeObject;
                        }
                        else
                        {
                            if (DoesNestedIndexExist(returnObject, index))
                            {
                                returnObject = returnObject.GetNestedObject(index);
                            }
                        }
                    }
                    else
                    {
                        var currentObject = returnObject.GetJsonObjectPropertyValue(pointerPromptValue);
                        if (returnObject.GetJsonObjectPropertyValueIsNotNull(pointerPromptValue))
                        {
                            Type[] selectListTypes = returnObject.GetObjectGenericArguments(pointerPromptValue);
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
                                returnObject.SetObjectPropertyValue(pointerPromptValue, ObjectExtensions.ConstructDictionary(selectListTypes[0], selectListTypes[1]));
                            }
                        }
                        returnObject = returnObject.GetJsonObjectPropertyValue(pointerPromptValue);
                    }
                }
            }
            return returnObject;
        }
    }
}
