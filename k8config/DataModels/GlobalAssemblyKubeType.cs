﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config.DataModels
{
    public class GlobalAssemblyKubeType
    {
        public string classKind { get; set; }
        public string kind { get; set; }
        public string assemblyFullName { get; set; }
        public string group { get; set; }
        public string version { get; set; }

    }
}
