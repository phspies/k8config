using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config.Utilities
{
    public static class ArrayExtentions
    {
        public static string[] CommandToArray(this string thisObject)
        {
            string[] tmpArray = new string[2];
            tmpArray[0] = thisObject.Split("=")[0].Trim();
            tmpArray[1] = thisObject.Split("=")[1].Trim();
            return tmpArray;
        }
    }
}
