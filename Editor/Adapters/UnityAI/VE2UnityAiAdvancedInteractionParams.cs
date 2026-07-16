using System;
using System.Linq;
using System.Reflection;
using Unity.AI.MCP.Editor.ToolRegistry;

namespace Imperial.VE2.MCP.Editor.Adapters.UnityAI
{
    internal static class UnityAiAdvancedParameterMapper
    {
        public static T ToCore<T>(object source) where T : class, new()
        {
            var target = new T();
            if (source == null) return target;

            var targetProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanWrite)
                .ToDictionary(property => property.Name, StringComparer.Ordinal);
            foreach (var sourceProperty in source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!sourceProperty.CanRead || !targetProperties.TryGetValue(sourceProperty.Name, out var targetProperty))
                    continue;
                var value = sourceProperty.GetValue(source);
                if (value == null || targetProperty.PropertyType.IsInstanceOfType(value))
                    targetProperty.SetValue(target, value);
            }
            return target;
        }
    }

    public class UnityAiPlacementParams
    {
        [McpDescription("Optional unique GameObject name.", Required = false)] public string Name { get; set; }
        [McpDescription("Optional parent hierarchy path or object name.", Required = false)] public string ParentPath { get; set; }
        [McpDescription("Optional world position [x,y,z].", Required = false)] public float[] Position { get; set; }
        [McpDescription("Optional world Euler rotation [x,y,z].", Required = false)] public float[] RotationEuler { get; set; }
        [McpDescription("Optional local scale [x,y,z].", Required = false)] public float[] Scale { get; set; }
        [McpDescription("Save the active scene after the change.", Required = false)] public bool SaveScene { get; set; }
    }

    public sealed class UnityAiInspectObjectParams
    {
        [McpDescription("Hierarchy path or name of the GameObject to inspect.", Required = true)] public string GameObjectPath { get; set; }
        [McpDescription("Include bounded serialized component properties.", Required = false)] public bool IncludeSerializedProperties { get; set; } = true;
        [McpDescription("Maximum serialized properties to return.", Required = false)] public int MaxSerializedProperties { get; set; } = 200;
        public VE2InspectObjectParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2InspectObjectParams>(this);
    }

    public sealed class UnityAiCreateActivatableParams : UnityAiPlacementParams
    {
        [McpDescription("Activatable kind.", Required = true, EnumType = typeof(VE2ActivatableKind))] public string Kind { get; set; }
        public VE2CreateActivatableParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2CreateActivatableParams>(this);
    }

    public sealed class UnityAiConfigureActivatableParams
    {
        [McpDescription("GameObject path or name containing the activatable.", Required = true)] public string GameObjectPath { get; set; }
        [McpDescription("Optional exact activatable component type.", Required = false)] public string ComponentTypeName { get; set; }
        [McpDescription("Write the admin-only setting.", Required = false)] public bool ConfigureAdminOnly { get; set; }
        [McpDescription("Admin-only value.", Required = false)] public bool AdminOnly { get; set; }
        [McpDescription("Write controller vibration behavior.", Required = false)] public bool ConfigureControllerVibrations { get; set; }
        [McpDescription("Controller vibration value.", Required = false)] public bool EnableControllerVibrations { get; set; } = true;
        [McpDescription("Write interaction enabled state.", Required = false)] public bool ConfigureIsInteractable { get; set; }
        [McpDescription("Interaction enabled value.", Required = false)] public bool IsInteractable { get; set; } = true;
        [McpDescription("Write interaction range.", Required = false)] public bool ConfigureInteractRange { get; set; }
        [McpDescription("Interaction range value.", Required = false)] public float InteractRange { get; set; } = 50f;
        [McpDescription("Write networked state.", Required = false)] public bool ConfigureIsNetworked { get; set; }
        [McpDescription("Networked value.", Required = false)] public bool IsNetworked { get; set; } = true;
        [McpDescription("Write activate-on-start state.", Required = false)] public bool ConfigureActivateOnStart { get; set; }
        [McpDescription("Activate-on-start value.", Required = false)] public bool ActivateOnStart { get; set; }
        [McpDescription("Optional VE2 activation group ID.", Required = false)] public string ActivationGroupId { get; set; }
        [McpDescription("Save the scene.", Required = false)] public bool SaveScene { get; set; }
        public VE2ConfigureActivatableParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2ConfigureActivatableParams>(this);
    }

    public sealed class UnityAiCreateAdjustableParams : UnityAiPlacementParams
    {
        [McpDescription("Adjustable kind.", Required = true, EnumType = typeof(VE2AdjustableKind))] public string Kind { get; set; }
        public VE2CreateAdjustableParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2CreateAdjustableParams>(this);
    }

    public sealed class UnityAiConfigureAdjustableParams
    {
        [McpDescription("GameObject path or name containing the adjustable.", Required = true)] public string GameObjectPath { get; set; }
        [McpDescription("Optional exact adjustable component type.", Required = false)] public string ComponentTypeName { get; set; }
        [McpDescription("Optional one- or two-dimensional starting value.", Required = false)] public float[] StartingValue { get; set; }
        [McpDescription("Optional minimum output value.", Required = false)] public float[] MinimumOutputValue { get; set; }
        [McpDescription("Optional maximum output value.", Required = false)] public float[] MaximumOutputValue { get; set; }
        [McpDescription("Optional minimum spatial value.", Required = false)] public float[] MinimumSpatialValue { get; set; }
        [McpDescription("Optional maximum spatial value.", Required = false)] public float[] MaximumSpatialValue { get; set; }
        [McpDescription("Optional number of discrete values.", Required = false)] public int NumberOfValues { get; set; }
        [McpDescription("Write emit-on-start state.", Required = false)] public bool ConfigureEmitValueOnStart { get; set; }
        [McpDescription("Emit-on-start value.", Required = false)] public bool EmitValueOnStart { get; set; } = true;
        [McpDescription("Write admin-only state.", Required = false)] public bool ConfigureAdminOnly { get; set; }
        [McpDescription("Admin-only value.", Required = false)] public bool AdminOnly { get; set; }
        [McpDescription("Write controller vibration behavior.", Required = false)] public bool ConfigureControllerVibrations { get; set; }
        [McpDescription("Controller vibration value.", Required = false)] public bool EnableControllerVibrations { get; set; } = true;
        [McpDescription("Write interaction enabled state.", Required = false)] public bool ConfigureIsInteractable { get; set; }
        [McpDescription("Interaction enabled value.", Required = false)] public bool IsInteractable { get; set; } = true;
        [McpDescription("Write interaction range.", Required = false)] public bool ConfigureInteractRange { get; set; }
        [McpDescription("Interaction range value.", Required = false)] public float InteractRange { get; set; } = 50f;
        [McpDescription("Save the scene.", Required = false)] public bool SaveScene { get; set; }
        public VE2ConfigureAdjustableParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2ConfigureAdjustableParams>(this);
    }

    public sealed class UnityAiCreateInfoPointParams : UnityAiPlacementParams
    {
        [McpDescription("Optional InfoPoint title.", Required = false)] public string Title { get; set; }
        [McpDescription("Optional InfoPoint body content.", Required = false)] public string Content { get; set; }
        public VE2CreateInfoPointParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2CreateInfoPointParams>(this);
    }

    public sealed class UnityAiConnectInteractionEventParams
    {
        [McpDescription("Source GameObject hierarchy path or name.", Required = true)] public string SourceGameObjectPath { get; set; }
        [McpDescription("Optional exact source component type.", Required = false)] public string SourceComponentTypeName { get; set; }
        [McpDescription("Allowlisted event: OnActivate, OnDeactivate, OnGrab, OnDrop, or OnValueAdjusted.", Required = true)] public string EventName { get; set; }
        [McpDescription("Target GameObject hierarchy path or name.", Required = true)] public string TargetGameObjectPath { get; set; }
        [McpDescription("Exact target component type.", Required = true)] public string TargetComponentTypeName { get; set; }
        [McpDescription("Compatible public target method name.", Required = true)] public string TargetMethodName { get; set; }
        [McpDescription("Replace existing persistent listeners.", Required = false)] public bool ReplaceExisting { get; set; }
        [McpDescription("Save the scene.", Required = false)] public bool SaveScene { get; set; }
        public VE2ConnectInteractionEventParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2ConnectInteractionEventParams>(this);
    }

    public sealed class UnityAiDuplicateInteractableParams
    {
        [McpDescription("Source VE2 interactable hierarchy path or name.", Required = true)] public string SourceGameObjectPath { get; set; }
        [McpDescription("Optional unique name for the duplicate.", Required = false)] public string Name { get; set; }
        [McpDescription("Optional destination parent path.", Required = false)] public string ParentPath { get; set; }
        [McpDescription("Optional world position offset [x,y,z].", Required = false)] public float[] PositionOffset { get; set; }
        [McpDescription("Save the scene.", Required = false)] public bool SaveScene { get; set; }
        public VE2DuplicateInteractableParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2DuplicateInteractableParams>(this);
    }
}
