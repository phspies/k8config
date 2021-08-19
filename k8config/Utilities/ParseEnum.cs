using k8config.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config.Utilities
{
    public static class ParseEnum
    {
        public static FieldFormat ParseFormat(string _input)
        {
            object _format;
            FieldFormat format = FieldFormat.Object;
            Enum.TryParse(typeof(FieldFormat), _input, true, out _format);
            if (_format != null) { format = (FieldFormat)_format; }
            return format;
        }
        public static FieldType ParseType(string _input)
        {
            object _type;
            FieldType type = FieldType.Object;
            Enum.TryParse(typeof(FieldType), _input, true, out _type);
            if (_type != null) { type = (FieldType)_type; }
            return type;
        }
    }
}
