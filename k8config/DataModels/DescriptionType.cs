﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config.DataModels
{

    public class DescriptionType
    {
        public DescriptionType()
        {
            properties = new List<DescriptionType>();
        }
        public bool rootObject { get; set; }
        public bool required { get; set; }
        public List<KubeObjectType> kubedetails { get; set; }
        public string name { get; set; }
        public string reference { get; set; }
        public string fullpath { get; set; }
        public string description { get; set; }
        public string format { get; set; }
        public string type { get; set; }
        public List<DescriptionType> properties { get; set; }
    }
}
