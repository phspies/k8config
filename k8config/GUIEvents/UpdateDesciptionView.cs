using k8config.DataModels;
using k8config.Utilities;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config
{
    partial class Program
    {
        static void UpdateDescriptionView(string _nestedObject = null)
        {
            object currentKubeObject = KubeObject.GetCurrentObject();
            if (GlobalVariables.promptArray.Count == 1 && _nestedObject != null && GlobalVariables.availableKubeTypes.Exists(x => x.kind == _nestedObject))
            {
                descriptionView.Text = (Type.GetType(GlobalVariables.availableKubeTypes.FirstOrDefault(x => x.kind == _nestedObject)?.assemblyFullName).GetCustomAttributes(typeof(KubernetesPropertyAttribute), false).First() as KubernetesPropertyAttribute).Description;
            }
            else if (GlobalVariables.promptArray.Count >= 2 && currentKubeObject != null && _nestedObject == null)
            {
                descriptionView.Text = (currentKubeObject.GetType().GetCustomAttributes(typeof(KubernetesPropertyAttribute), false).First() as KubernetesPropertyAttribute).Description;
            }
            else if (GlobalVariables.promptArray.Count >= 2 && currentKubeObject != null && _nestedObject != null)
            {
                descriptionView.Text = (currentKubeObject.GetType().GetProperties().ToList().FirstOrDefault(x => x.Name.ToLower() == _nestedObject.ToLower()).GetCustomAttributes(typeof(KubernetesPropertyAttribute), false).First() as KubernetesPropertyAttribute).Description;
            }
        }
    }
}
