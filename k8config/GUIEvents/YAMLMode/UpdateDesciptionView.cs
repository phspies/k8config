using k8config.DataModels;
using k8config.GUIEvents;
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
            if (YAMLModePromptObject.CurrentPromptPositionIsRoot && _nestedObject != null && GlobalVariables.availableKubeTypes.Exists(x => x.classKind == _nestedObject))
            {
                YAMLModelControls.descriptionView.Text = HttpUtility.HtmlDecode((Type.GetType(GlobalVariables.availableKubeTypes.FirstOrDefault(x => x.classKind == _nestedObject)?.assemblyFullName).GetCustomAttributes(typeof(KubernetesPropertyAttribute), false).First() as KubernetesPropertyAttribute)?.Description);
            }
            else if (YAMLModePromptObject.CurrentPromptDefinitionSelected && currentKubeType != null && _nestedObject == null)
            {
                YAMLModelControls.descriptionView.Text = HttpUtility.HtmlDecode((currentKubeType.GetCustomAttributes(typeof(KubernetesPropertyAttribute), false).First() as KubernetesPropertyAttribute)?.Description);
            }
            else if (YAMLModePromptObject.CurrentPromptDefinitionSelected && currentKubeType != null && _nestedObject != null)
            {
                if (currentKubeObject.JsonPropertyExists(_nestedObject.ToLower()))
                {
                    YAMLModelControls.descriptionView.Text = HttpUtility.HtmlDecode(currentKubeObject.GetJsonKubernetesAttribute(_nestedObject)?.Description);
                    var test = currentKubeObject.RetrieveAttributeValues();
                    YAMLModelControls.descriptionView.Text += "\r\n\r\nEntry format = " + currentKubeObject.RetrieveAttributeValues().FirstOrDefault(x => string.Compare(x.name, _nestedObject, true) == 0)?.entryFormat + " <-";
                }
            }
            else
            {
                if (GlobalVariables.sessionDefinedKinds.Count > 0)
                {
                    YAMLModelControls.descriptionView.Text = $"No definition selected. Use 'select <index>' command to select a definition.";
                }
                else
                {
                    YAMLModelControls.descriptionView.Text = $"No definitions found. Create a definition with the 'new <kind>' command or import a YAML file with the 'import <file>' command.";
                }
            }
        }
    }
}
