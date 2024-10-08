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
    /// Describe a container image
    /// </summary>
    [KubernetesProperty(Description: @"Describe a container image")]
    public partial class V1ContainerImage
    {
        /// <summary>
        /// Initializes a new instance of the V1ContainerImage class.
        /// </summary>
        public V1ContainerImage()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the V1ContainerImage class.
        /// </summary>
        /// <param name="names">
        /// Names by which this image is known. e.g. [&quot;k8s.gcr.io/hyperkube:v1.0.7&quot;, &quot;dockerhub.io/google_containers/hyperkube:v1.0.7&quot;]
        /// </param>
        /// <param name="sizeBytes">
        /// The size of the image in bytes.
        /// </param>
        public V1ContainerImage(IList<string> names = null, long? sizeBytes = null)
        {
            Names = names;
            SizeBytes = sizeBytes;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        [JsonProperty(PropertyName = "names")]
        [KubernetesProperty(IsRequired: false, Description: @"Names by which this image is known. e.g. [&quot;k8s.gcr.io/hyperkube:v1.0.7&quot;, &quot;dockerhub.io/google_containers/hyperkube:v1.0.7&quot;]")]
        public IList<string> Names { get; set; }

        [JsonProperty(PropertyName = "sizeBytes")]
        [KubernetesProperty(IsRequired: false, Description: @"The size of the image in bytes.")]
        public long? SizeBytes { get; set; }

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
