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
    /// SubjectAccessReviewSpec is a description of the access request.  Exactly one of ResourceAuthorizationAttributes and NonResourceAuthorizationAttributes must be set
    /// </summary>
    [KubernetesProperty(Description: @"SubjectAccessReviewSpec is a description of the access request.  Exactly one of ResourceAuthorizationAttributes and NonResourceAuthorizationAttributes must be set")]
    public partial class V1SubjectAccessReviewSpec
    {
        /// <summary>
        /// Initializes a new instance of the V1SubjectAccessReviewSpec class.
        /// </summary>
        public V1SubjectAccessReviewSpec()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the V1SubjectAccessReviewSpec class.
        /// </summary>
        /// <param name="extra">
        /// Extra corresponds to the user.Info.GetExtra() method from the authenticator.  Since that is input to the authorizer it needs a reflection here.
        /// </param>
        /// <param name="groups">
        /// Groups is the groups you&apos;re testing for.
        /// </param>
        /// <param name="nonResourceAttributes">
        /// NonResourceAttributes describes information for a non-resource access request
        /// </param>
        /// <param name="resourceAttributes">
        /// ResourceAuthorizationAttributes describes information for a resource access request
        /// </param>
        /// <param name="uid">
        /// UID information about the requesting user.
        /// </param>
        /// <param name="user">
        /// User is the user you&apos;re testing for. If you specify &quot;User&quot; but not &quot;Groups&quot;, then is it interpreted as &quot;What if User were not a member of any groups
        /// </param>
        public V1SubjectAccessReviewSpec(IDictionary<string, IList<string>> extra = null, IList<string> groups = null, V1NonResourceAttributes nonResourceAttributes = null, V1ResourceAttributes resourceAttributes = null, string uid = null, string user = null)
        {
            Extra = extra;
            Groups = groups;
            NonResourceAttributes = nonResourceAttributes;
            ResourceAttributes = resourceAttributes;
            Uid = uid;
            User = user;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        [JsonProperty(PropertyName = "extra")]
        [KubernetesProperty(IsRequired: false, Description: @"Extra corresponds to the user.Info.GetExtra() method from the authenticator.  Since that is input to the authorizer it needs a reflection here.")]
        public IDictionary<string, IList<string>> Extra { get; set; }

        [JsonProperty(PropertyName = "groups")]
        [KubernetesProperty(IsRequired: false, Description: @"Groups is the groups you&apos;re testing for.")]
        public IList<string> Groups { get; set; }

        [JsonProperty(PropertyName = "nonResourceAttributes")]
        [KubernetesProperty(IsRequired: false, Description: @"NonResourceAttributes describes information for a non-resource access request")]
        public V1NonResourceAttributes NonResourceAttributes { get; set; }

        [JsonProperty(PropertyName = "resourceAttributes")]
        [KubernetesProperty(IsRequired: false, Description: @"ResourceAuthorizationAttributes describes information for a resource access request")]
        public V1ResourceAttributes ResourceAttributes { get; set; }

        [JsonProperty(PropertyName = "uid")]
        [KubernetesProperty(IsRequired: false, Description: @"UID information about the requesting user.")]
        public string Uid { get; set; }

        [JsonProperty(PropertyName = "user")]
        [KubernetesProperty(IsRequired: false, Description: @"User is the user you&apos;re testing for. If you specify &quot;User&quot; but not &quot;Groups&quot;, then is it interpreted as &quot;What if User were not a member of any groups")]
        public string User { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            NonResourceAttributes?.Validate();
            ResourceAttributes?.Validate();
        }
    }
}
