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
        public bool isRequired { get; set; }
        public bool isArray { get; set; }
        public bool isList { get; set; }
        public bool isCommand { get; set; }
        public Type type { get; set; }

        public string TableView()
        {
            string returnString = name;
            if (type != null) { 
                returnString += type.IsList() ? $"  List<{displayType}>" : $" [{displayType}]"; 

            }
            if (isCommand)
            {
                returnString += $" ({displayType})";
            }
            returnString += isRequired ? $" (required)" : "" ;
            return returnString;
        }
    }
}
