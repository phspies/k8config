using k8config.DataModels;
using k8config.Utilities;
using k8s.Models;
using System;
using System.Linq;

namespace k8config
{
    partial class Program
    {
        static void UpdateDescriptionView(string _nestedObject = null)
        {
            var currentProperties = KubeObject.GetCurrentObject().GetObjectProperties();
            Type currentKubeType = KubeObject.GetCurrentObject().GetKubeType();
            if (GlobalVariables.promptArray.Count == 1 && _nestedObject != null && GlobalVariables.availableKubeTypes.Exists(x => x.kind == _nestedObject))
            {
                descriptionView.Text = (Type.GetType(GlobalVariables.availableKubeTypes.FirstOrDefault(x => x.kind == _nestedObject)?.assemblyFullName).GetCustomAttributes(typeof(KubernetesPropertyAttribute), false).First() as KubernetesPropertyAttribute)?.Description;
            }
            else if (GlobalVariables.promptArray.Count >= 2 && currentKubeType != null && _nestedObject == null)
            {
                descriptionView.Text = (currentKubeType.GetCustomAttributes(typeof(KubernetesPropertyAttribute), false).First() as KubernetesPropertyAttribute)?.Description;
            }
            else if (GlobalVariables.promptArray.Count >= 2 && currentKubeType != null && _nestedObject != null)
            {
                if (currentProperties.Exists(x => x.Name.ToLower() == _nestedObject.ToLower()))
                {
                    descriptionView.Text = (currentKubeType.GetProperties().ToList().FirstOrDefault(x => x.Name.ToLower() == _nestedObject.ToLower()).GetCustomAttributes(typeof(KubernetesPropertyAttribute), false).First() as KubernetesPropertyAttribute)?.Description;
                }
            }
            else
            {
                if (GlobalVariables.sessionDefinedKinds.Count > 0)
                {
                    descriptionView.Text = $"No definition selected. Please select of the defined kinds with the 'select <index>' command.";
                }
                else
                {
                    descriptionView.Text = $"No definitions found. Please create a definition with the 'new <kind>' command.";
                }
            }
        }
    }
}
