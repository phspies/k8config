using k8config.DataModels;
using k8config.Utilities;
using k8s.Models;
using System;
using System.Linq;
using System.Web;

namespace k8config
{
    partial class Program
    {
        static void UpdateDescriptionView(string _nestedObject = null)
        {
            var currentKubeObject = KubeObject.GetCurrentObject();
            Type currentKubeType = currentKubeObject.GetKubeType();
            if (GlobalVariables.promptArray.Count == 1 && _nestedObject != null && GlobalVariables.availableKubeTypes.Exists(x => x.kind == _nestedObject))
            {
                descriptionView.Text = HttpUtility.HtmlDecode((Type.GetType(GlobalVariables.availableKubeTypes.FirstOrDefault(x => x.kind == _nestedObject)?.assemblyFullName).GetCustomAttributes(typeof(KubernetesPropertyAttribute), false).First() as KubernetesPropertyAttribute)?.Description);
                descriptionView.Text += "\r\n\r\nEntry format = " + currentKubeObject.RetrieveAttributeValues().FirstOrDefault(x => string.Compare(x.name, _nestedObject, true) == 0)?.entryFormat + " <-";

            }
            else if (GlobalVariables.promptArray.Count >= 2 && currentKubeType != null && _nestedObject == null)
            {
                descriptionView.Text = HttpUtility.HtmlDecode((currentKubeType.GetCustomAttributes(typeof(KubernetesPropertyAttribute), false).First() as KubernetesPropertyAttribute)?.Description);
                //descriptionView.Text += "\r\n" + currentKubeObject.RetrieveAttributeValues().FirstOrDefault(x => string.Compare(x.name, _nestedObject, true) == 0).entryFormat;

            }
            else if (GlobalVariables.promptArray.Count >= 2 && currentKubeType != null && _nestedObject != null)
            {
                if (currentKubeObject.JsonPropertyExists(_nestedObject.ToLower()))
                {
                    descriptionView.Text = HttpUtility.HtmlDecode(currentKubeObject.GetJsonKubernetesAttribute(_nestedObject)?.Description);
                    var test = currentKubeObject.RetrieveAttributeValues();
                    descriptionView.Text += "\r\n\r\nEntry format = " + currentKubeObject.RetrieveAttributeValues().FirstOrDefault(x => string.Compare(x.name, _nestedObject, true) == 0)?.entryFormat + " <-";
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
