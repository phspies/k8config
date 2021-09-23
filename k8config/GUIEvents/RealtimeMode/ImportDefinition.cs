using k8config.DataModels;
using k8s;
using k8s.Models;
using System.Linq;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        static void ImportDefinition(object importObject)
        {
            Application.MainLoop.Invoke(() =>
            {
                ((IKubernetesObject<V1ObjectMeta>)importObject).Metadata.ManagedFields = null;
                ((IKubernetesObject<V1ObjectMeta>)importObject).Metadata.CreationTimestamp = null;
                ((IKubernetesObject<V1ObjectMeta>)importObject).Metadata.DeletionTimestamp = null;
                importObject.GetType().GetProperty("Status")?.SetValue(importObject, null);
                var newObject = new SessionDefinedKind() { kind = ((IKubernetesObject<V1ObjectMeta>)importObject).Kind, name = ((IKubernetesObject<V1ObjectMeta>)importObject).Name(), KubeObject = importObject, index = (GlobalVariables.sessionDefinedKinds.Count == 0 ? 1 : GlobalVariables.sessionDefinedKinds.Last().index + 1) };
                GlobalVariables.sessionDefinedKinds.Add(newObject);
                UpdateMessageBar($"{newObject.name} ({newObject.kind}) imported at index {newObject.index}");
                statusBar.SetNeedsDisplay();
            });
        }
    }
}
