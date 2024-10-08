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
    /// ReplicaSetStatus represents the current status of a ReplicaSet.
    /// </summary>
    [KubernetesProperty(Description: @"ReplicaSetStatus represents the current status of a ReplicaSet.")]
    public partial class V1ReplicaSetStatus
    {
        /// <summary>
        /// Initializes a new instance of the V1ReplicaSetStatus class.
        /// </summary>
        public V1ReplicaSetStatus()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the V1ReplicaSetStatus class.
        /// </summary>
        /// <param name="replicas">
        /// Replicas is the most recently oberved number of replicas. More info: https://kubernetes.io/docs/concepts/workloads/controllers/replicationcontroller/#what-is-a-replicationcontroller
        /// </param>
        /// <param name="availableReplicas">
        /// The number of available replicas (ready for at least minReadySeconds) for this replica set.
        /// </param>
        /// <param name="conditions">
        /// Represents the latest available observations of a replica set&apos;s current state.
        /// </param>
        /// <param name="fullyLabeledReplicas">
        /// The number of pods that have labels matching the labels of the pod template of the replicaset.
        /// </param>
        /// <param name="observedGeneration">
        /// ObservedGeneration reflects the generation of the most recently observed ReplicaSet.
        /// </param>
        /// <param name="readyReplicas">
        /// The number of ready replicas for this replica set.
        /// </param>
        public V1ReplicaSetStatus(int replicas, int? availableReplicas = null, IList<V1ReplicaSetCondition> conditions = null, int? fullyLabeledReplicas = null, long? observedGeneration = null, int? readyReplicas = null)
        {
            AvailableReplicas = availableReplicas;
            Conditions = conditions;
            FullyLabeledReplicas = fullyLabeledReplicas;
            ObservedGeneration = observedGeneration;
            ReadyReplicas = readyReplicas;
            Replicas = replicas;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        [JsonProperty(PropertyName = "availableReplicas")]
        [KubernetesProperty(IsRequired: false, Description: @"The number of available replicas (ready for at least minReadySeconds) for this replica set.")]
        public int? AvailableReplicas { get; set; }

        [JsonProperty(PropertyName = "conditions")]
        [KubernetesProperty(IsRequired: false, Description: @"Represents the latest available observations of a replica set&apos;s current state.")]
        public IList<V1ReplicaSetCondition> Conditions { get; set; }

        [JsonProperty(PropertyName = "fullyLabeledReplicas")]
        [KubernetesProperty(IsRequired: false, Description: @"The number of pods that have labels matching the labels of the pod template of the replicaset.")]
        public int? FullyLabeledReplicas { get; set; }

        [JsonProperty(PropertyName = "observedGeneration")]
        [KubernetesProperty(IsRequired: false, Description: @"ObservedGeneration reflects the generation of the most recently observed ReplicaSet.")]
        public long? ObservedGeneration { get; set; }

        [JsonProperty(PropertyName = "readyReplicas")]
        [KubernetesProperty(IsRequired: false, Description: @"The number of ready replicas for this replica set.")]
        public int? ReadyReplicas { get; set; }

        [JsonProperty(PropertyName = "replicas")]
        [KubernetesProperty(IsRequired: true, Description: @"Replicas is the most recently oberved number of replicas. More info: https://kubernetes.io/docs/concepts/workloads/controllers/replicationcontroller/#what-is-a-replicationcontroller")]
        public int Replicas { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            foreach(var obj in Conditions)
            {
                obj.Validate();
            }
        }
    }
}
