using k8config.Utilities;
using System;

namespace k8config.DataModels
{
    public class OptionsSlimType
    {
        public int index { get; set; }
        public string name { get; set;  }

        public object value { get; set; }
        public string displayType { get; set; }
        public string entryFormat { get; set; }
        public bool propertyIsRequired { get; set; }
        public bool propertyIsArray { get; set; }
        public bool propertyIsDictionary { get; set; }
        public bool propertyIsList { get; set; }
        public bool propertyIsCommand { get; set; }
        public bool propertyIsNamedType { get; set; }
        public Type primaryType { get; set; }
        public Type secondaryType { get; set; }
        public Type properyType { get; set; }

        public string TableView()
        {
            string returnString = name;
            if (primaryType != null) { 
                returnString += primaryType.IsList() ? $"  List<{displayType}>" : $" [{displayType}]"; 

            }
            if (propertyIsCommand)
            {
                returnString += $" ({displayType})";
            }
            returnString += propertyIsRequired ? $" (required)" : "" ;
            return returnString;
        }
        public string SelectView()
        {
            return $"[{index}] {name}";
        }
    }
}
