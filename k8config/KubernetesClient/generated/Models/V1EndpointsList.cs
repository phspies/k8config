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
    /// EndpointsList is a list of endpoints.
    /// </summary>
    [KubernetesProperty(Description: @"EndpointsList is a list of endpoints.")]
    public partial class V1EndpointsList
    {
        /// <summary>
        /// Initializes a new instance of the V1EndpointsList class.
        /// </summary>
        public V1EndpointsList()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the V1EndpointsList class.
        /// </summary>
        /// <param name="items">
        /// List of endpoints.
        /// </param>
        /// <param name="apiVersion">
        /// APIVersion defines the versioned schema of this representation of an object. Servers should convert recognized schemas to the latest internal value, and may reject unrecognized values. More info: https://git.k8s.io/community/contributors/devel/sig-architecture/api-conventions.md#resources
        /// </param>
        /// <param name="kind">
        /// Kind is a string value representing the REST resource this object represents. Servers may infer this from the endpoint the client submits requests to. Cannot be updated. In CamelCase. More info: https://git.k8s.io/community/contributors/devel/sig-architecture/api-conventions.md#types-kinds
        /// </param>
        /// <param name="metadata">
        /// Standard list metadata. More info: https://git.k8s.io/community/contributors/devel/sig-architecture/api-conventions.md#types-kinds
        /// </param>
        public V1EndpointsList(IList<V1Endpoints> items, string apiVersion = null, string kind = null, V1ListMeta metadata = null)
        {
            ApiVersion = apiVersion;
            Items = items;
            Kind = kind;
            Metadata = metadata;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        [JsonProperty(PropertyName = "apiVersion")]
        [KubernetesProperty(IsRequired: false, Description: @"APIVersion defines the versioned schema of this representation of an object. Servers should convert recognized schemas to the latest internal value, and may reject unrecognized values. More info: https://git.k8s.io/community/contributors/devel/sig-architecture/api-conventions.md#resources")]
        public string ApiVersion { get; set; }

        [JsonProperty(PropertyName = "items")]
        [KubernetesProperty(IsRequired: true, Description: @"List of endpoints.")]
        public IList<V1Endpoints> Items { get; set; }

        [JsonProperty(PropertyName = "kind")]
        [KubernetesProperty(IsRequired: false, Description: @"Kind is a string value representing the REST resource this object represents. Servers may infer this from the endpoint the client submits requests to. Cannot be updated. In CamelCase. More info: https://git.k8s.io/community/contributors/devel/sig-architecture/api-conventions.md#types-kinds")]
        public string Kind { get; set; }

        [JsonProperty(PropertyName = "metadata")]
        [KubernetesProperty(IsRequired: false, Description: @"Standard list metadata. More info: https://git.k8s.io/community/contributors/devel/sig-architecture/api-conventions.md#types-kinds")]
        public V1ListMeta Metadata { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            foreach(var obj in Items)
            {
                obj.Validate();
            }
            Metadata?.Validate();
        }
    }
}
