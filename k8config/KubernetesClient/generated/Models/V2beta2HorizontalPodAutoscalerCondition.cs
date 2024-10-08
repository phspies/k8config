// <auto-generated>
// Code generated by https://github.com/kubernetes-client/csharp/tree/master/gen/KubernetesGenerator
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace k8s.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Collections;
    using System.Linq;

    /// <summary>
    /// HorizontalPodAutoscalerCondition describes the state of a HorizontalPodAutoscaler at a certain point.
    /// </summary>
    [KubernetesProperty(Description: @"HorizontalPodAutoscalerCondition describes the state of a HorizontalPodAutoscaler at a certain point.")]
    public partial class V2beta2HorizontalPodAutoscalerCondition
    {
        /// <summary>
        /// Initializes a new instance of the V2beta2HorizontalPodAutoscalerCondition class.
        /// </summary>
        public V2beta2HorizontalPodAutoscalerCondition()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the V2beta2HorizontalPodAutoscalerCondition class.
        /// </summary>
        /// <param name="status">
        /// status is the status of the condition (True, False, Unknown)
        /// </param>
        /// <param name="type">
        /// type describes the current condition
        /// </param>
        /// <param name="lastTransitionTime">
        /// lastTransitionTime is the last time the condition transitioned from one status to another
        /// </param>
        /// <param name="message">
        /// message is a human-readable explanation containing details about the transition
        /// </param>
        /// <param name="reason">
        /// reason is the reason for the condition&apos;s last transition.
        /// </param>
        public V2beta2HorizontalPodAutoscalerCondition(string status, string type, System.DateTime? lastTransitionTime = null, string message = null, string reason = null)
        {
            LastTransitionTime = lastTransitionTime;
            Message = message;
            Reason = reason;
            Status = status;
            Type = type;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        [JsonProperty(PropertyName = "lastTransitionTime")]
        [KubernetesProperty(IsRequired: false, Description: @"lastTransitionTime is the last time the condition transitioned from one status to another")]
        public System.DateTime? LastTransitionTime { get; set; }

        [JsonProperty(PropertyName = "message")]
        [KubernetesProperty(IsRequired: false, Description: @"message is a human-readable explanation containing details about the transition")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "reason")]
        [KubernetesProperty(IsRequired: false, Description: @"reason is the reason for the condition&apos;s last transition.")]
        public string Reason { get; set; }

        [JsonProperty(PropertyName = "status")]
        [KubernetesProperty(IsRequired: true, Description: @"status is the status of the condition (True, False, Unknown)")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "type")]
        [KubernetesProperty(IsRequired: true, Description: @"type describes the current condition")]
        public string Type { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
        }
    }
}
