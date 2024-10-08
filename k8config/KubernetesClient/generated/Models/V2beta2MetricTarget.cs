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
    /// MetricTarget defines the target value, average value, or average utilization of a specific metric
    /// </summary>
    [KubernetesProperty(Description: @"MetricTarget defines the target value, average value, or average utilization of a specific metric")]
    public partial class V2beta2MetricTarget
    {
        /// <summary>
        /// Initializes a new instance of the V2beta2MetricTarget class.
        /// </summary>
        public V2beta2MetricTarget()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the V2beta2MetricTarget class.
        /// </summary>
        /// <param name="type">
        /// type represents whether the metric type is Utilization, Value, or AverageValue
        /// </param>
        /// <param name="averageUtilization">
        /// averageUtilization is the target value of the average of the resource metric across all relevant pods, represented as a percentage of the requested value of the resource for the pods. Currently only valid for Resource metric source type
        /// </param>
        /// <param name="averageValue">
        /// averageValue is the target value of the average of the metric across all relevant pods (as a quantity)
        /// </param>
        /// <param name="value">
        /// value is the target value of the metric (as a quantity).
        /// </param>
        public V2beta2MetricTarget(string type, int? averageUtilization = null, ResourceQuantity averageValue = null, ResourceQuantity value = null)
        {
            AverageUtilization = averageUtilization;
            AverageValue = averageValue;
            Type = type;
            Value = value;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        [JsonProperty(PropertyName = "averageUtilization")]
        [KubernetesProperty(IsRequired: false, Description: @"averageUtilization is the target value of the average of the resource metric across all relevant pods, represented as a percentage of the requested value of the resource for the pods. Currently only valid for Resource metric source type")]
        public int? AverageUtilization { get; set; }

        [JsonProperty(PropertyName = "averageValue")]
        [KubernetesProperty(IsRequired: false, Description: @"averageValue is the target value of the average of the metric across all relevant pods (as a quantity)")]
        public ResourceQuantity AverageValue { get; set; }

        [JsonProperty(PropertyName = "type")]
        [KubernetesProperty(IsRequired: true, Description: @"type represents whether the metric type is Utilization, Value, or AverageValue")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "value")]
        [KubernetesProperty(IsRequired: false, Description: @"value is the target value of the metric (as a quantity).")]
        public ResourceQuantity Value { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            AverageValue?.Validate();
            Value?.Validate();
        }
    }
}
