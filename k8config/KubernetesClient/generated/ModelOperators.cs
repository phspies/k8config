// <auto-generated>
// Code generated by https://github.com/kubernetes-client/csharp/tree/master/gen/KubernetesGenerator
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
using k8s.Versioning;

namespace k8s.Models
{
    public partial class V1AggregationRule
    {
        public static explicit operator V1AggregationRule(V1alpha1AggregationRule s) => VersionConverter.Mapper.Map<V1AggregationRule>(s);
    }
    public partial class V1alpha1AggregationRule
    {
        public static explicit operator V1alpha1AggregationRule(V1AggregationRule s) => VersionConverter.Mapper.Map<V1alpha1AggregationRule>(s);
    }
    public partial class V1alpha1ClusterRole
    {
        public static explicit operator V1alpha1ClusterRole(V1ClusterRole s) => VersionConverter.Mapper.Map<V1alpha1ClusterRole>(s);
    }
    public partial class V1ClusterRole
    {
        public static explicit operator V1ClusterRole(V1alpha1ClusterRole s) => VersionConverter.Mapper.Map<V1ClusterRole>(s);
    }
    public partial class V1alpha1ClusterRoleBinding
    {
        public static explicit operator V1alpha1ClusterRoleBinding(V1ClusterRoleBinding s) => VersionConverter.Mapper.Map<V1alpha1ClusterRoleBinding>(s);
    }
    public partial class V1ClusterRoleBinding
    {
        public static explicit operator V1ClusterRoleBinding(V1alpha1ClusterRoleBinding s) => VersionConverter.Mapper.Map<V1ClusterRoleBinding>(s);
    }
    public partial class V1alpha1ClusterRoleBindingList
    {
        public static explicit operator V1alpha1ClusterRoleBindingList(V1ClusterRoleBindingList s) => VersionConverter.Mapper.Map<V1alpha1ClusterRoleBindingList>(s);
    }
    public partial class V1ClusterRoleBindingList
    {
        public static explicit operator V1ClusterRoleBindingList(V1alpha1ClusterRoleBindingList s) => VersionConverter.Mapper.Map<V1ClusterRoleBindingList>(s);
    }
    public partial class V1alpha1ClusterRoleList
    {
        public static explicit operator V1alpha1ClusterRoleList(V1ClusterRoleList s) => VersionConverter.Mapper.Map<V1alpha1ClusterRoleList>(s);
    }
    public partial class V1ClusterRoleList
    {
        public static explicit operator V1ClusterRoleList(V1alpha1ClusterRoleList s) => VersionConverter.Mapper.Map<V1ClusterRoleList>(s);
    }
    public partial class V2beta1ContainerResourceMetricSource
    {
        public static explicit operator V2beta1ContainerResourceMetricSource(V2beta2ContainerResourceMetricSource s) => VersionConverter.Mapper.Map<V2beta1ContainerResourceMetricSource>(s);
    }
    public partial class V2beta2ContainerResourceMetricSource
    {
        public static explicit operator V2beta2ContainerResourceMetricSource(V2beta1ContainerResourceMetricSource s) => VersionConverter.Mapper.Map<V2beta2ContainerResourceMetricSource>(s);
    }
    public partial class V2beta1ContainerResourceMetricStatus
    {
        public static explicit operator V2beta1ContainerResourceMetricStatus(V2beta2ContainerResourceMetricStatus s) => VersionConverter.Mapper.Map<V2beta1ContainerResourceMetricStatus>(s);
    }
    public partial class V2beta2ContainerResourceMetricStatus
    {
        public static explicit operator V2beta2ContainerResourceMetricStatus(V2beta1ContainerResourceMetricStatus s) => VersionConverter.Mapper.Map<V2beta2ContainerResourceMetricStatus>(s);
    }
    public partial class V1beta1CronJob
    {
        public static explicit operator V1beta1CronJob(V1CronJob s) => VersionConverter.Mapper.Map<V1beta1CronJob>(s);
    }
    public partial class V1CronJob
    {
        public static explicit operator V1CronJob(V1beta1CronJob s) => VersionConverter.Mapper.Map<V1CronJob>(s);
    }
    public partial class V1beta1CronJobList
    {
        public static explicit operator V1beta1CronJobList(V1CronJobList s) => VersionConverter.Mapper.Map<V1beta1CronJobList>(s);
    }
    public partial class V1CronJobList
    {
        public static explicit operator V1CronJobList(V1beta1CronJobList s) => VersionConverter.Mapper.Map<V1CronJobList>(s);
    }
    public partial class V1beta1CronJobSpec
    {
        public static explicit operator V1beta1CronJobSpec(V1CronJobSpec s) => VersionConverter.Mapper.Map<V1beta1CronJobSpec>(s);
    }
    public partial class V1CronJobSpec
    {
        public static explicit operator V1CronJobSpec(V1beta1CronJobSpec s) => VersionConverter.Mapper.Map<V1CronJobSpec>(s);
    }
    public partial class V1beta1CronJobStatus
    {
        public static explicit operator V1beta1CronJobStatus(V1CronJobStatus s) => VersionConverter.Mapper.Map<V1beta1CronJobStatus>(s);
    }
    public partial class V1CronJobStatus
    {
        public static explicit operator V1CronJobStatus(V1beta1CronJobStatus s) => VersionConverter.Mapper.Map<V1CronJobStatus>(s);
    }
    public partial class V1CrossVersionObjectReference
    {
        public static explicit operator V1CrossVersionObjectReference(V2beta1CrossVersionObjectReference s) => VersionConverter.Mapper.Map<V1CrossVersionObjectReference>(s);
    }
    public partial class V2beta1CrossVersionObjectReference
    {
        public static explicit operator V2beta1CrossVersionObjectReference(V1CrossVersionObjectReference s) => VersionConverter.Mapper.Map<V2beta1CrossVersionObjectReference>(s);
    }
    public partial class V1CrossVersionObjectReference
    {
        public static explicit operator V1CrossVersionObjectReference(V2beta2CrossVersionObjectReference s) => VersionConverter.Mapper.Map<V1CrossVersionObjectReference>(s);
    }
    public partial class V2beta2CrossVersionObjectReference
    {
        public static explicit operator V2beta2CrossVersionObjectReference(V1CrossVersionObjectReference s) => VersionConverter.Mapper.Map<V2beta2CrossVersionObjectReference>(s);
    }
    public partial class V2beta1CrossVersionObjectReference
    {
        public static explicit operator V2beta1CrossVersionObjectReference(V2beta2CrossVersionObjectReference s) => VersionConverter.Mapper.Map<V2beta1CrossVersionObjectReference>(s);
    }
    public partial class V2beta2CrossVersionObjectReference
    {
        public static explicit operator V2beta2CrossVersionObjectReference(V2beta1CrossVersionObjectReference s) => VersionConverter.Mapper.Map<V2beta2CrossVersionObjectReference>(s);
    }
    public partial class V1alpha1CSIStorageCapacity
    {
        public static explicit operator V1alpha1CSIStorageCapacity(V1beta1CSIStorageCapacity s) => VersionConverter.Mapper.Map<V1alpha1CSIStorageCapacity>(s);
    }
    public partial class V1beta1CSIStorageCapacity
    {
        public static explicit operator V1beta1CSIStorageCapacity(V1alpha1CSIStorageCapacity s) => VersionConverter.Mapper.Map<V1beta1CSIStorageCapacity>(s);
    }
    public partial class V1alpha1CSIStorageCapacityList
    {
        public static explicit operator V1alpha1CSIStorageCapacityList(V1beta1CSIStorageCapacityList s) => VersionConverter.Mapper.Map<V1alpha1CSIStorageCapacityList>(s);
    }
    public partial class V1beta1CSIStorageCapacityList
    {
        public static explicit operator V1beta1CSIStorageCapacityList(V1alpha1CSIStorageCapacityList s) => VersionConverter.Mapper.Map<V1beta1CSIStorageCapacityList>(s);
    }
    public partial class V1beta1Endpoint
    {
        public static explicit operator V1beta1Endpoint(V1Endpoint s) => VersionConverter.Mapper.Map<V1beta1Endpoint>(s);
    }
    public partial class V1Endpoint
    {
        public static explicit operator V1Endpoint(V1beta1Endpoint s) => VersionConverter.Mapper.Map<V1Endpoint>(s);
    }
    public partial class V1beta1EndpointConditions
    {
        public static explicit operator V1beta1EndpointConditions(V1EndpointConditions s) => VersionConverter.Mapper.Map<V1beta1EndpointConditions>(s);
    }
    public partial class V1EndpointConditions
    {
        public static explicit operator V1EndpointConditions(V1beta1EndpointConditions s) => VersionConverter.Mapper.Map<V1EndpointConditions>(s);
    }
    public partial class V1beta1EndpointHints
    {
        public static explicit operator V1beta1EndpointHints(V1EndpointHints s) => VersionConverter.Mapper.Map<V1beta1EndpointHints>(s);
    }
    public partial class V1EndpointHints
    {
        public static explicit operator V1EndpointHints(V1beta1EndpointHints s) => VersionConverter.Mapper.Map<V1EndpointHints>(s);
    }
    public partial class V1beta1EndpointSlice
    {
        public static explicit operator V1beta1EndpointSlice(V1EndpointSlice s) => VersionConverter.Mapper.Map<V1beta1EndpointSlice>(s);
    }
    public partial class V1EndpointSlice
    {
        public static explicit operator V1EndpointSlice(V1beta1EndpointSlice s) => VersionConverter.Mapper.Map<V1EndpointSlice>(s);
    }
    public partial class V1beta1EndpointSliceList
    {
        public static explicit operator V1beta1EndpointSliceList(V1EndpointSliceList s) => VersionConverter.Mapper.Map<V1beta1EndpointSliceList>(s);
    }
    public partial class V1EndpointSliceList
    {
        public static explicit operator V1EndpointSliceList(V1beta1EndpointSliceList s) => VersionConverter.Mapper.Map<V1EndpointSliceList>(s);
    }
    public partial class V2beta1ExternalMetricSource
    {
        public static explicit operator V2beta1ExternalMetricSource(V2beta2ExternalMetricSource s) => VersionConverter.Mapper.Map<V2beta1ExternalMetricSource>(s);
    }
    public partial class V2beta2ExternalMetricSource
    {
        public static explicit operator V2beta2ExternalMetricSource(V2beta1ExternalMetricSource s) => VersionConverter.Mapper.Map<V2beta2ExternalMetricSource>(s);
    }
    public partial class V2beta1ExternalMetricStatus
    {
        public static explicit operator V2beta1ExternalMetricStatus(V2beta2ExternalMetricStatus s) => VersionConverter.Mapper.Map<V2beta1ExternalMetricStatus>(s);
    }
    public partial class V2beta2ExternalMetricStatus
    {
        public static explicit operator V2beta2ExternalMetricStatus(V2beta1ExternalMetricStatus s) => VersionConverter.Mapper.Map<V2beta2ExternalMetricStatus>(s);
    }
    public partial class V1beta1ForZone
    {
        public static explicit operator V1beta1ForZone(V1ForZone s) => VersionConverter.Mapper.Map<V1beta1ForZone>(s);
    }
    public partial class V1ForZone
    {
        public static explicit operator V1ForZone(V1beta1ForZone s) => VersionConverter.Mapper.Map<V1ForZone>(s);
    }
    public partial class V1HorizontalPodAutoscaler
    {
        public static explicit operator V1HorizontalPodAutoscaler(V2beta1HorizontalPodAutoscaler s) => VersionConverter.Mapper.Map<V1HorizontalPodAutoscaler>(s);
    }
    public partial class V2beta1HorizontalPodAutoscaler
    {
        public static explicit operator V2beta1HorizontalPodAutoscaler(V1HorizontalPodAutoscaler s) => VersionConverter.Mapper.Map<V2beta1HorizontalPodAutoscaler>(s);
    }
    public partial class V1HorizontalPodAutoscaler
    {
        public static explicit operator V1HorizontalPodAutoscaler(V2beta2HorizontalPodAutoscaler s) => VersionConverter.Mapper.Map<V1HorizontalPodAutoscaler>(s);
    }
    public partial class V2beta2HorizontalPodAutoscaler
    {
        public static explicit operator V2beta2HorizontalPodAutoscaler(V1HorizontalPodAutoscaler s) => VersionConverter.Mapper.Map<V2beta2HorizontalPodAutoscaler>(s);
    }
    public partial class V2beta1HorizontalPodAutoscaler
    {
        public static explicit operator V2beta1HorizontalPodAutoscaler(V2beta2HorizontalPodAutoscaler s) => VersionConverter.Mapper.Map<V2beta1HorizontalPodAutoscaler>(s);
    }
    public partial class V2beta2HorizontalPodAutoscaler
    {
        public static explicit operator V2beta2HorizontalPodAutoscaler(V2beta1HorizontalPodAutoscaler s) => VersionConverter.Mapper.Map<V2beta2HorizontalPodAutoscaler>(s);
    }
    public partial class V2beta1HorizontalPodAutoscalerCondition
    {
        public static explicit operator V2beta1HorizontalPodAutoscalerCondition(V2beta2HorizontalPodAutoscalerCondition s) => VersionConverter.Mapper.Map<V2beta1HorizontalPodAutoscalerCondition>(s);
    }
    public partial class V2beta2HorizontalPodAutoscalerCondition
    {
        public static explicit operator V2beta2HorizontalPodAutoscalerCondition(V2beta1HorizontalPodAutoscalerCondition s) => VersionConverter.Mapper.Map<V2beta2HorizontalPodAutoscalerCondition>(s);
    }
    public partial class V1HorizontalPodAutoscalerList
    {
        public static explicit operator V1HorizontalPodAutoscalerList(V2beta1HorizontalPodAutoscalerList s) => VersionConverter.Mapper.Map<V1HorizontalPodAutoscalerList>(s);
    }
    public partial class V2beta1HorizontalPodAutoscalerList
    {
        public static explicit operator V2beta1HorizontalPodAutoscalerList(V1HorizontalPodAutoscalerList s) => VersionConverter.Mapper.Map<V2beta1HorizontalPodAutoscalerList>(s);
    }
    public partial class V1HorizontalPodAutoscalerList
    {
        public static explicit operator V1HorizontalPodAutoscalerList(V2beta2HorizontalPodAutoscalerList s) => VersionConverter.Mapper.Map<V1HorizontalPodAutoscalerList>(s);
    }
    public partial class V2beta2HorizontalPodAutoscalerList
    {
        public static explicit operator V2beta2HorizontalPodAutoscalerList(V1HorizontalPodAutoscalerList s) => VersionConverter.Mapper.Map<V2beta2HorizontalPodAutoscalerList>(s);
    }
    public partial class V2beta1HorizontalPodAutoscalerList
    {
        public static explicit operator V2beta1HorizontalPodAutoscalerList(V2beta2HorizontalPodAutoscalerList s) => VersionConverter.Mapper.Map<V2beta1HorizontalPodAutoscalerList>(s);
    }
    public partial class V2beta2HorizontalPodAutoscalerList
    {
        public static explicit operator V2beta2HorizontalPodAutoscalerList(V2beta1HorizontalPodAutoscalerList s) => VersionConverter.Mapper.Map<V2beta2HorizontalPodAutoscalerList>(s);
    }
    public partial class V1HorizontalPodAutoscalerSpec
    {
        public static explicit operator V1HorizontalPodAutoscalerSpec(V2beta1HorizontalPodAutoscalerSpec s) => VersionConverter.Mapper.Map<V1HorizontalPodAutoscalerSpec>(s);
    }
    public partial class V2beta1HorizontalPodAutoscalerSpec
    {
        public static explicit operator V2beta1HorizontalPodAutoscalerSpec(V1HorizontalPodAutoscalerSpec s) => VersionConverter.Mapper.Map<V2beta1HorizontalPodAutoscalerSpec>(s);
    }
    public partial class V1HorizontalPodAutoscalerSpec
    {
        public static explicit operator V1HorizontalPodAutoscalerSpec(V2beta2HorizontalPodAutoscalerSpec s) => VersionConverter.Mapper.Map<V1HorizontalPodAutoscalerSpec>(s);
    }
    public partial class V2beta2HorizontalPodAutoscalerSpec
    {
        public static explicit operator V2beta2HorizontalPodAutoscalerSpec(V1HorizontalPodAutoscalerSpec s) => VersionConverter.Mapper.Map<V2beta2HorizontalPodAutoscalerSpec>(s);
    }
    public partial class V2beta1HorizontalPodAutoscalerSpec
    {
        public static explicit operator V2beta1HorizontalPodAutoscalerSpec(V2beta2HorizontalPodAutoscalerSpec s) => VersionConverter.Mapper.Map<V2beta1HorizontalPodAutoscalerSpec>(s);
    }
    public partial class V2beta2HorizontalPodAutoscalerSpec
    {
        public static explicit operator V2beta2HorizontalPodAutoscalerSpec(V2beta1HorizontalPodAutoscalerSpec s) => VersionConverter.Mapper.Map<V2beta2HorizontalPodAutoscalerSpec>(s);
    }
    public partial class V1HorizontalPodAutoscalerStatus
    {
        public static explicit operator V1HorizontalPodAutoscalerStatus(V2beta1HorizontalPodAutoscalerStatus s) => VersionConverter.Mapper.Map<V1HorizontalPodAutoscalerStatus>(s);
    }
    public partial class V2beta1HorizontalPodAutoscalerStatus
    {
        public static explicit operator V2beta1HorizontalPodAutoscalerStatus(V1HorizontalPodAutoscalerStatus s) => VersionConverter.Mapper.Map<V2beta1HorizontalPodAutoscalerStatus>(s);
    }
    public partial class V1HorizontalPodAutoscalerStatus
    {
        public static explicit operator V1HorizontalPodAutoscalerStatus(V2beta2HorizontalPodAutoscalerStatus s) => VersionConverter.Mapper.Map<V1HorizontalPodAutoscalerStatus>(s);
    }
    public partial class V2beta2HorizontalPodAutoscalerStatus
    {
        public static explicit operator V2beta2HorizontalPodAutoscalerStatus(V1HorizontalPodAutoscalerStatus s) => VersionConverter.Mapper.Map<V2beta2HorizontalPodAutoscalerStatus>(s);
    }
    public partial class V2beta1HorizontalPodAutoscalerStatus
    {
        public static explicit operator V2beta1HorizontalPodAutoscalerStatus(V2beta2HorizontalPodAutoscalerStatus s) => VersionConverter.Mapper.Map<V2beta1HorizontalPodAutoscalerStatus>(s);
    }
    public partial class V2beta2HorizontalPodAutoscalerStatus
    {
        public static explicit operator V2beta2HorizontalPodAutoscalerStatus(V2beta1HorizontalPodAutoscalerStatus s) => VersionConverter.Mapper.Map<V2beta2HorizontalPodAutoscalerStatus>(s);
    }
    public partial class V1beta1JobTemplateSpec
    {
        public static explicit operator V1beta1JobTemplateSpec(V1JobTemplateSpec s) => VersionConverter.Mapper.Map<V1beta1JobTemplateSpec>(s);
    }
    public partial class V1JobTemplateSpec
    {
        public static explicit operator V1JobTemplateSpec(V1beta1JobTemplateSpec s) => VersionConverter.Mapper.Map<V1JobTemplateSpec>(s);
    }
    public partial class V2beta1MetricSpec
    {
        public static explicit operator V2beta1MetricSpec(V2beta2MetricSpec s) => VersionConverter.Mapper.Map<V2beta1MetricSpec>(s);
    }
    public partial class V2beta2MetricSpec
    {
        public static explicit operator V2beta2MetricSpec(V2beta1MetricSpec s) => VersionConverter.Mapper.Map<V2beta2MetricSpec>(s);
    }
    public partial class V2beta1MetricStatus
    {
        public static explicit operator V2beta1MetricStatus(V2beta2MetricStatus s) => VersionConverter.Mapper.Map<V2beta1MetricStatus>(s);
    }
    public partial class V2beta2MetricStatus
    {
        public static explicit operator V2beta2MetricStatus(V2beta1MetricStatus s) => VersionConverter.Mapper.Map<V2beta2MetricStatus>(s);
    }
    public partial class V2beta1ObjectMetricSource
    {
        public static explicit operator V2beta1ObjectMetricSource(V2beta2ObjectMetricSource s) => VersionConverter.Mapper.Map<V2beta1ObjectMetricSource>(s);
    }
    public partial class V2beta2ObjectMetricSource
    {
        public static explicit operator V2beta2ObjectMetricSource(V2beta1ObjectMetricSource s) => VersionConverter.Mapper.Map<V2beta2ObjectMetricSource>(s);
    }
    public partial class V2beta1ObjectMetricStatus
    {
        public static explicit operator V2beta1ObjectMetricStatus(V2beta2ObjectMetricStatus s) => VersionConverter.Mapper.Map<V2beta1ObjectMetricStatus>(s);
    }
    public partial class V2beta2ObjectMetricStatus
    {
        public static explicit operator V2beta2ObjectMetricStatus(V2beta1ObjectMetricStatus s) => VersionConverter.Mapper.Map<V2beta2ObjectMetricStatus>(s);
    }
    public partial class V1alpha1Overhead
    {
        public static explicit operator V1alpha1Overhead(V1beta1Overhead s) => VersionConverter.Mapper.Map<V1alpha1Overhead>(s);
    }
    public partial class V1beta1Overhead
    {
        public static explicit operator V1beta1Overhead(V1alpha1Overhead s) => VersionConverter.Mapper.Map<V1beta1Overhead>(s);
    }
    public partial class V1alpha1Overhead
    {
        public static explicit operator V1alpha1Overhead(V1Overhead s) => VersionConverter.Mapper.Map<V1alpha1Overhead>(s);
    }
    public partial class V1Overhead
    {
        public static explicit operator V1Overhead(V1alpha1Overhead s) => VersionConverter.Mapper.Map<V1Overhead>(s);
    }
    public partial class V1beta1Overhead
    {
        public static explicit operator V1beta1Overhead(V1Overhead s) => VersionConverter.Mapper.Map<V1beta1Overhead>(s);
    }
    public partial class V1Overhead
    {
        public static explicit operator V1Overhead(V1beta1Overhead s) => VersionConverter.Mapper.Map<V1Overhead>(s);
    }
    public partial class V1beta1PodDisruptionBudget
    {
        public static explicit operator V1beta1PodDisruptionBudget(V1PodDisruptionBudget s) => VersionConverter.Mapper.Map<V1beta1PodDisruptionBudget>(s);
    }
    public partial class V1PodDisruptionBudget
    {
        public static explicit operator V1PodDisruptionBudget(V1beta1PodDisruptionBudget s) => VersionConverter.Mapper.Map<V1PodDisruptionBudget>(s);
    }
    public partial class V1beta1PodDisruptionBudgetList
    {
        public static explicit operator V1beta1PodDisruptionBudgetList(V1PodDisruptionBudgetList s) => VersionConverter.Mapper.Map<V1beta1PodDisruptionBudgetList>(s);
    }
    public partial class V1PodDisruptionBudgetList
    {
        public static explicit operator V1PodDisruptionBudgetList(V1beta1PodDisruptionBudgetList s) => VersionConverter.Mapper.Map<V1PodDisruptionBudgetList>(s);
    }
    public partial class V1beta1PodDisruptionBudgetSpec
    {
        public static explicit operator V1beta1PodDisruptionBudgetSpec(V1PodDisruptionBudgetSpec s) => VersionConverter.Mapper.Map<V1beta1PodDisruptionBudgetSpec>(s);
    }
    public partial class V1PodDisruptionBudgetSpec
    {
        public static explicit operator V1PodDisruptionBudgetSpec(V1beta1PodDisruptionBudgetSpec s) => VersionConverter.Mapper.Map<V1PodDisruptionBudgetSpec>(s);
    }
    public partial class V1beta1PodDisruptionBudgetStatus
    {
        public static explicit operator V1beta1PodDisruptionBudgetStatus(V1PodDisruptionBudgetStatus s) => VersionConverter.Mapper.Map<V1beta1PodDisruptionBudgetStatus>(s);
    }
    public partial class V1PodDisruptionBudgetStatus
    {
        public static explicit operator V1PodDisruptionBudgetStatus(V1beta1PodDisruptionBudgetStatus s) => VersionConverter.Mapper.Map<V1PodDisruptionBudgetStatus>(s);
    }
    public partial class V2beta1PodsMetricSource
    {
        public static explicit operator V2beta1PodsMetricSource(V2beta2PodsMetricSource s) => VersionConverter.Mapper.Map<V2beta1PodsMetricSource>(s);
    }
    public partial class V2beta2PodsMetricSource
    {
        public static explicit operator V2beta2PodsMetricSource(V2beta1PodsMetricSource s) => VersionConverter.Mapper.Map<V2beta2PodsMetricSource>(s);
    }
    public partial class V2beta1PodsMetricStatus
    {
        public static explicit operator V2beta1PodsMetricStatus(V2beta2PodsMetricStatus s) => VersionConverter.Mapper.Map<V2beta1PodsMetricStatus>(s);
    }
    public partial class V2beta2PodsMetricStatus
    {
        public static explicit operator V2beta2PodsMetricStatus(V2beta1PodsMetricStatus s) => VersionConverter.Mapper.Map<V2beta2PodsMetricStatus>(s);
    }
    public partial class V1alpha1PolicyRule
    {
        public static explicit operator V1alpha1PolicyRule(V1PolicyRule s) => VersionConverter.Mapper.Map<V1alpha1PolicyRule>(s);
    }
    public partial class V1PolicyRule
    {
        public static explicit operator V1PolicyRule(V1alpha1PolicyRule s) => VersionConverter.Mapper.Map<V1PolicyRule>(s);
    }
    public partial class V1alpha1PriorityClass
    {
        public static explicit operator V1alpha1PriorityClass(V1PriorityClass s) => VersionConverter.Mapper.Map<V1alpha1PriorityClass>(s);
    }
    public partial class V1PriorityClass
    {
        public static explicit operator V1PriorityClass(V1alpha1PriorityClass s) => VersionConverter.Mapper.Map<V1PriorityClass>(s);
    }
    public partial class V1alpha1PriorityClassList
    {
        public static explicit operator V1alpha1PriorityClassList(V1PriorityClassList s) => VersionConverter.Mapper.Map<V1alpha1PriorityClassList>(s);
    }
    public partial class V1PriorityClassList
    {
        public static explicit operator V1PriorityClassList(V1alpha1PriorityClassList s) => VersionConverter.Mapper.Map<V1PriorityClassList>(s);
    }
    public partial class V2beta1ResourceMetricSource
    {
        public static explicit operator V2beta1ResourceMetricSource(V2beta2ResourceMetricSource s) => VersionConverter.Mapper.Map<V2beta1ResourceMetricSource>(s);
    }
    public partial class V2beta2ResourceMetricSource
    {
        public static explicit operator V2beta2ResourceMetricSource(V2beta1ResourceMetricSource s) => VersionConverter.Mapper.Map<V2beta2ResourceMetricSource>(s);
    }
    public partial class V2beta1ResourceMetricStatus
    {
        public static explicit operator V2beta1ResourceMetricStatus(V2beta2ResourceMetricStatus s) => VersionConverter.Mapper.Map<V2beta1ResourceMetricStatus>(s);
    }
    public partial class V2beta2ResourceMetricStatus
    {
        public static explicit operator V2beta2ResourceMetricStatus(V2beta1ResourceMetricStatus s) => VersionConverter.Mapper.Map<V2beta2ResourceMetricStatus>(s);
    }
    public partial class V1alpha1Role
    {
        public static explicit operator V1alpha1Role(V1Role s) => VersionConverter.Mapper.Map<V1alpha1Role>(s);
    }
    public partial class V1Role
    {
        public static explicit operator V1Role(V1alpha1Role s) => VersionConverter.Mapper.Map<V1Role>(s);
    }
    public partial class V1alpha1RoleBinding
    {
        public static explicit operator V1alpha1RoleBinding(V1RoleBinding s) => VersionConverter.Mapper.Map<V1alpha1RoleBinding>(s);
    }
    public partial class V1RoleBinding
    {
        public static explicit operator V1RoleBinding(V1alpha1RoleBinding s) => VersionConverter.Mapper.Map<V1RoleBinding>(s);
    }
    public partial class V1alpha1RoleBindingList
    {
        public static explicit operator V1alpha1RoleBindingList(V1RoleBindingList s) => VersionConverter.Mapper.Map<V1alpha1RoleBindingList>(s);
    }
    public partial class V1RoleBindingList
    {
        public static explicit operator V1RoleBindingList(V1alpha1RoleBindingList s) => VersionConverter.Mapper.Map<V1RoleBindingList>(s);
    }
    public partial class V1alpha1RoleList
    {
        public static explicit operator V1alpha1RoleList(V1RoleList s) => VersionConverter.Mapper.Map<V1alpha1RoleList>(s);
    }
    public partial class V1RoleList
    {
        public static explicit operator V1RoleList(V1alpha1RoleList s) => VersionConverter.Mapper.Map<V1RoleList>(s);
    }
    public partial class V1alpha1RoleRef
    {
        public static explicit operator V1alpha1RoleRef(V1RoleRef s) => VersionConverter.Mapper.Map<V1alpha1RoleRef>(s);
    }
    public partial class V1RoleRef
    {
        public static explicit operator V1RoleRef(V1alpha1RoleRef s) => VersionConverter.Mapper.Map<V1RoleRef>(s);
    }
    public partial class V1alpha1RuntimeClass
    {
        public static explicit operator V1alpha1RuntimeClass(V1beta1RuntimeClass s) => VersionConverter.Mapper.Map<V1alpha1RuntimeClass>(s);
    }
    public partial class V1beta1RuntimeClass
    {
        public static explicit operator V1beta1RuntimeClass(V1alpha1RuntimeClass s) => VersionConverter.Mapper.Map<V1beta1RuntimeClass>(s);
    }
    public partial class V1alpha1RuntimeClass
    {
        public static explicit operator V1alpha1RuntimeClass(V1RuntimeClass s) => VersionConverter.Mapper.Map<V1alpha1RuntimeClass>(s);
    }
    public partial class V1RuntimeClass
    {
        public static explicit operator V1RuntimeClass(V1alpha1RuntimeClass s) => VersionConverter.Mapper.Map<V1RuntimeClass>(s);
    }
    public partial class V1beta1RuntimeClass
    {
        public static explicit operator V1beta1RuntimeClass(V1RuntimeClass s) => VersionConverter.Mapper.Map<V1beta1RuntimeClass>(s);
    }
    public partial class V1RuntimeClass
    {
        public static explicit operator V1RuntimeClass(V1beta1RuntimeClass s) => VersionConverter.Mapper.Map<V1RuntimeClass>(s);
    }
    public partial class V1alpha1RuntimeClassList
    {
        public static explicit operator V1alpha1RuntimeClassList(V1beta1RuntimeClassList s) => VersionConverter.Mapper.Map<V1alpha1RuntimeClassList>(s);
    }
    public partial class V1beta1RuntimeClassList
    {
        public static explicit operator V1beta1RuntimeClassList(V1alpha1RuntimeClassList s) => VersionConverter.Mapper.Map<V1beta1RuntimeClassList>(s);
    }
    public partial class V1alpha1RuntimeClassList
    {
        public static explicit operator V1alpha1RuntimeClassList(V1RuntimeClassList s) => VersionConverter.Mapper.Map<V1alpha1RuntimeClassList>(s);
    }
    public partial class V1RuntimeClassList
    {
        public static explicit operator V1RuntimeClassList(V1alpha1RuntimeClassList s) => VersionConverter.Mapper.Map<V1RuntimeClassList>(s);
    }
    public partial class V1beta1RuntimeClassList
    {
        public static explicit operator V1beta1RuntimeClassList(V1RuntimeClassList s) => VersionConverter.Mapper.Map<V1beta1RuntimeClassList>(s);
    }
    public partial class V1RuntimeClassList
    {
        public static explicit operator V1RuntimeClassList(V1beta1RuntimeClassList s) => VersionConverter.Mapper.Map<V1RuntimeClassList>(s);
    }
    public partial class V1alpha1Scheduling
    {
        public static explicit operator V1alpha1Scheduling(V1beta1Scheduling s) => VersionConverter.Mapper.Map<V1alpha1Scheduling>(s);
    }
    public partial class V1beta1Scheduling
    {
        public static explicit operator V1beta1Scheduling(V1alpha1Scheduling s) => VersionConverter.Mapper.Map<V1beta1Scheduling>(s);
    }
    public partial class V1alpha1Scheduling
    {
        public static explicit operator V1alpha1Scheduling(V1Scheduling s) => VersionConverter.Mapper.Map<V1alpha1Scheduling>(s);
    }
    public partial class V1Scheduling
    {
        public static explicit operator V1Scheduling(V1alpha1Scheduling s) => VersionConverter.Mapper.Map<V1Scheduling>(s);
    }
    public partial class V1beta1Scheduling
    {
        public static explicit operator V1beta1Scheduling(V1Scheduling s) => VersionConverter.Mapper.Map<V1beta1Scheduling>(s);
    }
    public partial class V1Scheduling
    {
        public static explicit operator V1Scheduling(V1beta1Scheduling s) => VersionConverter.Mapper.Map<V1Scheduling>(s);
    }
    public partial class V1alpha1Subject
    {
        public static explicit operator V1alpha1Subject(V1beta1Subject s) => VersionConverter.Mapper.Map<V1alpha1Subject>(s);
    }
    public partial class V1beta1Subject
    {
        public static explicit operator V1beta1Subject(V1alpha1Subject s) => VersionConverter.Mapper.Map<V1beta1Subject>(s);
    }
    public partial class V1alpha1Subject
    {
        public static explicit operator V1alpha1Subject(V1Subject s) => VersionConverter.Mapper.Map<V1alpha1Subject>(s);
    }
    public partial class V1Subject
    {
        public static explicit operator V1Subject(V1alpha1Subject s) => VersionConverter.Mapper.Map<V1Subject>(s);
    }
    public partial class V1beta1Subject
    {
        public static explicit operator V1beta1Subject(V1Subject s) => VersionConverter.Mapper.Map<V1beta1Subject>(s);
    }
    public partial class V1Subject
    {
        public static explicit operator V1Subject(V1beta1Subject s) => VersionConverter.Mapper.Map<V1Subject>(s);
    }
    public partial class V1alpha1VolumeAttachment
    {
        public static explicit operator V1alpha1VolumeAttachment(V1VolumeAttachment s) => VersionConverter.Mapper.Map<V1alpha1VolumeAttachment>(s);
    }
    public partial class V1VolumeAttachment
    {
        public static explicit operator V1VolumeAttachment(V1alpha1VolumeAttachment s) => VersionConverter.Mapper.Map<V1VolumeAttachment>(s);
    }
    public partial class V1alpha1VolumeAttachmentList
    {
        public static explicit operator V1alpha1VolumeAttachmentList(V1VolumeAttachmentList s) => VersionConverter.Mapper.Map<V1alpha1VolumeAttachmentList>(s);
    }
    public partial class V1VolumeAttachmentList
    {
        public static explicit operator V1VolumeAttachmentList(V1alpha1VolumeAttachmentList s) => VersionConverter.Mapper.Map<V1VolumeAttachmentList>(s);
    }
    public partial class V1alpha1VolumeAttachmentSource
    {
        public static explicit operator V1alpha1VolumeAttachmentSource(V1VolumeAttachmentSource s) => VersionConverter.Mapper.Map<V1alpha1VolumeAttachmentSource>(s);
    }
    public partial class V1VolumeAttachmentSource
    {
        public static explicit operator V1VolumeAttachmentSource(V1alpha1VolumeAttachmentSource s) => VersionConverter.Mapper.Map<V1VolumeAttachmentSource>(s);
    }
    public partial class V1alpha1VolumeAttachmentSpec
    {
        public static explicit operator V1alpha1VolumeAttachmentSpec(V1VolumeAttachmentSpec s) => VersionConverter.Mapper.Map<V1alpha1VolumeAttachmentSpec>(s);
    }
    public partial class V1VolumeAttachmentSpec
    {
        public static explicit operator V1VolumeAttachmentSpec(V1alpha1VolumeAttachmentSpec s) => VersionConverter.Mapper.Map<V1VolumeAttachmentSpec>(s);
    }
    public partial class V1alpha1VolumeAttachmentStatus
    {
        public static explicit operator V1alpha1VolumeAttachmentStatus(V1VolumeAttachmentStatus s) => VersionConverter.Mapper.Map<V1alpha1VolumeAttachmentStatus>(s);
    }
    public partial class V1VolumeAttachmentStatus
    {
        public static explicit operator V1VolumeAttachmentStatus(V1alpha1VolumeAttachmentStatus s) => VersionConverter.Mapper.Map<V1VolumeAttachmentStatus>(s);
    }
    public partial class V1alpha1VolumeError
    {
        public static explicit operator V1alpha1VolumeError(V1VolumeError s) => VersionConverter.Mapper.Map<V1alpha1VolumeError>(s);
    }
    public partial class V1VolumeError
    {
        public static explicit operator V1VolumeError(V1alpha1VolumeError s) => VersionConverter.Mapper.Map<V1VolumeError>(s);
    }
}
