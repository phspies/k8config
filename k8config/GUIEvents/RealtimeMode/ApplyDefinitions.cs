using k8config.DataModels;
using k8s;
using k8s.Models;
using Microsoft.Rest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Terminal.Gui;

namespace k8config
{
    partial class Program
    {
        public static void ApplyDefinitions()
        {
            RealtimeModeControls.applyStatusList.Clear();
            RealtimeModeControls.ApplyStatusWindow.Visible = true;
            RealtimeModeControls.closeApplyWindowButton.SetFocus();
            GlobalVariables.sessionDefinedKinds.ForEach(definition =>
                {

                    try
                    {
                        string currentNamespace = "default";
                        Application.MainLoop.Invoke(() =>
                        {
                            RealtimeModeControls.applyStatusList.Insert(0, $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff",CultureInfo.InvariantCulture)}: Checking if {definition.metaData.Name()} exists in namespace {currentNamespace}");
                            RealtimeModeControls.applyListView.SetNeedsDisplay();
                        });
                        if (CheckIfExist(definition.metaData, currentNamespace))
                        {
                            Application.MainLoop.Invoke(() =>
                            {
                                RealtimeModeControls.applyStatusList.Insert(0, $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}: {definition.metaData.Name()} exists in namespace {currentNamespace}, running a patch operation");
                                RealtimeModeControls.applyListView.SetNeedsDisplay();
                            });

                            HttpOperationResponse patchResponse = PatchIfExist(definition.metaData, currentNamespace);
                            Application.MainLoop.Invoke(() =>
                            {
                                RealtimeModeControls.applyStatusList.Insert(0, $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}: Patch operation for {definition.metaData.Name()} returned {patchResponse.Response.StatusCode} : {patchResponse.Response.ReasonPhrase}");
                                RealtimeModeControls.applyListView.SetNeedsDisplay();
                            });
                        }
                        else
                        {
                            Application.MainLoop.Invoke(() =>
                            {
                                RealtimeModeControls.applyStatusList.Insert(0, $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}: {definition.metaData.Name()} does not exist in namespace {currentNamespace}, running a create operation");
                                RealtimeModeControls.applyListView.SetNeedsDisplay();
                            });
                            HttpOperationResponse createResponse = Create(definition.metaData, currentNamespace);
                            Application.MainLoop.Invoke(() =>
                            {
                                RealtimeModeControls.applyStatusList.Insert(0, $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}: Patch operation for {definition.metaData.Name()} returned {createResponse.Response.StatusCode} : {createResponse.Response.ReasonPhrase}");
                                RealtimeModeControls.applyListView.SetNeedsDisplay();
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobalVariables.Log.Error($"Cannot apply definition to context: {ex.Message}");
                        Application.MainLoop.Invoke(() =>
                        {
                            RealtimeModeControls.applyStatusList.Insert(0, $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)}: Operation for {definition.metaData.Name()} threw an exception {ex.Message}");
                            RealtimeModeControls.applyListView.SetNeedsDisplay();
                        });
                    }
                });
        }
        static public HttpOperationResponse Create(IKubernetesObject<V1ObjectMeta> kubeObject, string currentNamespace)
        {
            MethodInfo retrieveMethod = k8Client.GetType().GetMethods().Where(x => x.Name.Contains($"CreateNamespaced{kubeObject.Kind}WithHttpMessagesAsync"))?.First();
            Dictionary<string, object> namedParameters = new Dictionary<string, object>();
            namedParameters["namespaceParameter"] = currentNamespace;
            namedParameters["body"] = kubeObject;

            Task result = (Task)retrieveMethod.Invoke(k8Client, injectParameters(retrieveMethod, namedParameters));
            result.Wait();
            return (HttpOperationResponse)result.GetType().GetProperties().FirstOrDefault(x => x.Name == "Result").GetValue(result);
        }
        static public bool CheckIfExist(IKubernetesObject<V1ObjectMeta> kubeObject, string currentNamespace)
        {
            var returnList = k8Client.ListNamespacedService(currentNamespace);
            MethodInfo retrieveMethod = k8Client.GetType().GetMethods().Where(x => x.Name == $"ListNamespaced{kubeObject.Kind}WithHttpMessagesAsync")?.First();
            Dictionary<string, object> namedParameters = new Dictionary<string, object>();
            namedParameters["namespaceParameter"] = currentNamespace;

            Task result = (Task)retrieveMethod.Invoke(k8Client, injectParameters(retrieveMethod, namedParameters));
            result.Wait();
            var returnListValue = (HttpOperationResponse)result.GetType().GetProperty("Result").GetValue(result);
            var body = returnListValue.GetType().GetProperty("Body").GetValue(returnListValue);
            foreach (var record in body.GetType().GetProperty("Items").GetValue(body) as IEnumerable)
            {
                if (((IKubernetesObject<V1ObjectMeta>)record).Name() == kubeObject.Name())
                {
                    return true;
                }
            }
            return false;
        }
        static public HttpOperationResponse PatchIfExist(IKubernetesObject<V1ObjectMeta> kubeObject, string currentNamespace)
        {
            V1Patch patchObject = new V1Patch(kubeObject, V1Patch.PatchType.StrategicMergePatch);

            MethodInfo patchMethod = k8Client.GetType().GetMethods().Where(x => x.Name.Contains($"PatchNamespaced{kubeObject.Kind}WithHttpMessagesAsync"))?.First();
            Dictionary<string, object> namedParameters = new Dictionary<string, object>();
            namedParameters["body"] = patchObject;
            namedParameters["namespaceParameter"] = currentNamespace;
            namedParameters["name"] = kubeObject.Name();

            Task result = (Task)patchMethod.Invoke(k8Client, injectParameters(patchMethod, namedParameters));
            result.Wait();
            return (HttpOperationResponse)result.GetType().GetProperties().FirstOrDefault(x => x.Name == "Result").GetValue(result);
        }
        static public object[] injectParameters(MethodInfo methodObject, Dictionary<string, object> namedParameters)
        {
            string[] paramNames = methodObject.GetParameters().Select(p => p.Name).ToArray();
            object[] parameters = new object[paramNames.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                parameters[i] = Type.Missing;
            }
            foreach (var item in namedParameters)
            {
                var paramName = item.Key;
                var paramIndex = Array.IndexOf(paramNames, paramName);
                if (paramIndex >= 0)
                {
                    parameters[paramIndex] = item.Value;
                }
            }
            return parameters;
        }
    }
}
