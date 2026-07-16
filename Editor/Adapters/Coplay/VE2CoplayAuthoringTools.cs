using MCPForUnity.Editor.Tools;
using Newtonsoft.Json.Linq;

namespace Imperial.VE2.MCP.Editor.Adapters.Coplay
{
    public class CoplayPlacementSchema
    {
        [ToolParameter("Optional unique GameObject name.", Required = false)] public string Name { get; set; }
        [ToolParameter("Optional parent hierarchy path or object name.", Required = false)] public string ParentPath { get; set; }
        [ToolParameter("Optional world position [x,y,z].", Required = false)] public float[] Position { get; set; }
        [ToolParameter("Optional world Euler rotation [x,y,z].", Required = false)] public float[] RotationEuler { get; set; }
        [ToolParameter("Optional local scale [x,y,z].", Required = false)] public float[] Scale { get; set; }
        [ToolParameter("Save the active scene after the change.", Required = false)] public bool SaveScene { get; set; }
    }

    [McpForUnityTool(VE2ToolCatalog.SceneInspectObject.Name, Description = VE2ToolCatalog.SceneInspectObject.Description, Group = "core")]
    public static class VE2CoplaySceneInspectObjectTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Hierarchy path or name of the GameObject to inspect.")] public string GameObjectPath { get; set; }
            [ToolParameter("Include bounded serialized component properties.", Required = false)] public bool IncludeSerializedProperties { get; set; } = true;
            [ToolParameter("Maximum serialized properties to return.", Required = false)] public int MaxSerializedProperties { get; set; } = 200;
        }
        public static object HandleCommand(JObject value) => VE2McpTools.InspectObject(CoplayParameterParser.Parse<VE2InspectObjectParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.SceneCreateActivatable.Name, Description = VE2ToolCatalog.SceneCreateActivatable.Description, Group = "core")]
    public static class VE2CoplaySceneCreateActivatableTool
    {
        public sealed class Parameters : CoplayPlacementSchema
        {
            [ToolParameter("Activatable kind: toggle_button, hold_button, or pressure_plate.")] public string Kind { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.CreateActivatable(CoplayParameterParser.Parse<VE2CreateActivatableParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.ComponentConfigureActivatable.Name, Description = VE2ToolCatalog.ComponentConfigureActivatable.Description, Group = "core")]
    public static class VE2CoplayComponentConfigureActivatableTool
    {
        public sealed class Parameters
        {
            [ToolParameter("GameObject path or name containing the activatable.")] public string GameObjectPath { get; set; }
            [ToolParameter("Optional exact activatable component type.", Required = false)] public string ComponentTypeName { get; set; }
            [ToolParameter("Write the admin-only setting.", Required = false)] public bool ConfigureAdminOnly { get; set; }
            [ToolParameter("Admin-only value.", Required = false)] public bool AdminOnly { get; set; }
            [ToolParameter("Write controller vibration behavior.", Required = false)] public bool ConfigureControllerVibrations { get; set; }
            [ToolParameter("Controller vibration value.", Required = false)] public bool EnableControllerVibrations { get; set; } = true;
            [ToolParameter("Write interaction enabled state.", Required = false)] public bool ConfigureIsInteractable { get; set; }
            [ToolParameter("Interaction enabled value.", Required = false)] public bool IsInteractable { get; set; } = true;
            [ToolParameter("Write interaction range.", Required = false)] public bool ConfigureInteractRange { get; set; }
            [ToolParameter("Interaction range value.", Required = false)] public float InteractRange { get; set; } = 50f;
            [ToolParameter("Write networked state.", Required = false)] public bool ConfigureIsNetworked { get; set; }
            [ToolParameter("Networked value.", Required = false)] public bool IsNetworked { get; set; } = true;
            [ToolParameter("Write activate-on-start state.", Required = false)] public bool ConfigureActivateOnStart { get; set; }
            [ToolParameter("Activate-on-start value.", Required = false)] public bool ActivateOnStart { get; set; }
            [ToolParameter("Optional VE2 activation group ID.", Required = false)] public string ActivationGroupId { get; set; }
            [ToolParameter("Save the scene.", Required = false)] public bool SaveScene { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.ConfigureActivatable(CoplayParameterParser.Parse<VE2ConfigureActivatableParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.SceneCreateAdjustable.Name, Description = VE2ToolCatalog.SceneCreateAdjustable.Description, Group = "core")]
    public static class VE2CoplaySceneCreateAdjustableTool
    {
        public sealed class Parameters : CoplayPlacementSchema
        {
            [ToolParameter("Adjustable kind: wheel, lever, joystick_2d, slider, or slider_2d.")] public string Kind { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.CreateAdjustable(CoplayParameterParser.Parse<VE2CreateAdjustableParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.ComponentConfigureAdjustable.Name, Description = VE2ToolCatalog.ComponentConfigureAdjustable.Description, Group = "core")]
    public static class VE2CoplayComponentConfigureAdjustableTool
    {
        public sealed class Parameters
        {
            [ToolParameter("GameObject path or name containing the adjustable.")] public string GameObjectPath { get; set; }
            [ToolParameter("Optional exact adjustable component type.", Required = false)] public string ComponentTypeName { get; set; }
            [ToolParameter("Optional one- or two-dimensional starting value.", Required = false)] public float[] StartingValue { get; set; }
            [ToolParameter("Optional minimum output value.", Required = false)] public float[] MinimumOutputValue { get; set; }
            [ToolParameter("Optional maximum output value.", Required = false)] public float[] MaximumOutputValue { get; set; }
            [ToolParameter("Optional minimum spatial value.", Required = false)] public float[] MinimumSpatialValue { get; set; }
            [ToolParameter("Optional maximum spatial value.", Required = false)] public float[] MaximumSpatialValue { get; set; }
            [ToolParameter("Optional number of discrete values.", Required = false)] public int NumberOfValues { get; set; }
            [ToolParameter("Write emit-on-start state.", Required = false)] public bool ConfigureEmitValueOnStart { get; set; }
            [ToolParameter("Emit-on-start value.", Required = false)] public bool EmitValueOnStart { get; set; } = true;
            [ToolParameter("Write admin-only state.", Required = false)] public bool ConfigureAdminOnly { get; set; }
            [ToolParameter("Admin-only value.", Required = false)] public bool AdminOnly { get; set; }
            [ToolParameter("Write controller vibration behavior.", Required = false)] public bool ConfigureControllerVibrations { get; set; }
            [ToolParameter("Controller vibration value.", Required = false)] public bool EnableControllerVibrations { get; set; } = true;
            [ToolParameter("Write interaction enabled state.", Required = false)] public bool ConfigureIsInteractable { get; set; }
            [ToolParameter("Interaction enabled value.", Required = false)] public bool IsInteractable { get; set; } = true;
            [ToolParameter("Write interaction range.", Required = false)] public bool ConfigureInteractRange { get; set; }
            [ToolParameter("Interaction range value.", Required = false)] public float InteractRange { get; set; } = 50f;
            [ToolParameter("Save the scene.", Required = false)] public bool SaveScene { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.ConfigureAdjustable(CoplayParameterParser.Parse<VE2ConfigureAdjustableParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.SceneCreateInfoPoint.Name, Description = VE2ToolCatalog.SceneCreateInfoPoint.Description, Group = "core")]
    public static class VE2CoplaySceneCreateInfoPointTool
    {
        public sealed class Parameters : CoplayPlacementSchema
        {
            [ToolParameter("Optional InfoPoint title.", Required = false)] public string Title { get; set; }
            [ToolParameter("Optional InfoPoint body content.", Required = false)] public string Content { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.CreateInfoPoint(CoplayParameterParser.Parse<VE2CreateInfoPointParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.SceneConnectInteractionEvent.Name, Description = VE2ToolCatalog.SceneConnectInteractionEvent.Description, Group = "core")]
    public static class VE2CoplaySceneConnectInteractionEventTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Source GameObject hierarchy path or name.")] public string SourceGameObjectPath { get; set; }
            [ToolParameter("Optional exact source component type.", Required = false)] public string SourceComponentTypeName { get; set; }
            [ToolParameter("Allowlisted VE2 event: OnActivate, OnDeactivate, OnGrab, OnDrop, or OnValueAdjusted.")] public string EventName { get; set; }
            [ToolParameter("Target GameObject hierarchy path or name.")] public string TargetGameObjectPath { get; set; }
            [ToolParameter("Exact target component type.")] public string TargetComponentTypeName { get; set; }
            [ToolParameter("Compatible public target method name.")] public string TargetMethodName { get; set; }
            [ToolParameter("Replace existing persistent listeners.", Required = false)] public bool ReplaceExisting { get; set; }
            [ToolParameter("Save the scene.", Required = false)] public bool SaveScene { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.ConnectInteractionEvent(CoplayParameterParser.Parse<VE2ConnectInteractionEventParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.SceneDuplicateInteractable.Name, Description = VE2ToolCatalog.SceneDuplicateInteractable.Description, Group = "core")]
    public static class VE2CoplaySceneDuplicateInteractableTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Source VE2 interactable hierarchy path or name.")] public string SourceGameObjectPath { get; set; }
            [ToolParameter("Optional unique name for the duplicate.", Required = false)] public string Name { get; set; }
            [ToolParameter("Optional destination parent path.", Required = false)] public string ParentPath { get; set; }
            [ToolParameter("Optional world position offset [x,y,z].", Required = false)] public float[] PositionOffset { get; set; }
            [ToolParameter("Save the scene.", Required = false)] public bool SaveScene { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.DuplicateInteractable(CoplayParameterParser.Parse<VE2DuplicateInteractableParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.SceneConfigurePlayer.Name, Description = VE2ToolCatalog.SceneConfigurePlayer.Description, Group = "core")]
    public static class VE2CoplaySceneConfigurePlayerTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Optional V_PlayerSpawner hierarchy path.", Required = false)] public string PlayerSpawnerPath { get; set; }
            [ToolParameter("Optional player mode: OnlyVR, Only2D, or Both.", Required = false)] public string SupportedPlayerModes { get; set; }
            [ToolParameter("Optional interactable layer names.", Required = false)] public string[] InteractableLayers { get; set; }
            [ToolParameter("Optional traversable layer names.", Required = false)] public string[] TraversableLayers { get; set; }
            [ToolParameter("Optional collision layer names.", Required = false)] public string[] CollisionLayers { get; set; }
            [ToolParameter("Write free-fly mode.", Required = false)] public bool ConfigureFreeFlyMode { get; set; }
            [ToolParameter("Free-fly mode value.", Required = false)] public bool FreeFlyMode { get; set; }
            [ToolParameter("Write teleport range multiplier.", Required = false)] public bool ConfigureTeleportRangeMultiplier { get; set; }
            [ToolParameter("Teleport range multiplier.", Required = false)] public float TeleportRangeMultiplier { get; set; } = 1f;
            [ToolParameter("Write maximum vertical drag height.", Required = false)] public bool ConfigureMaxVerticalDragHeight { get; set; }
            [ToolParameter("Maximum vertical drag height.", Required = false)] public float MaxVerticalDragHeight { get; set; } = 10f;
            [ToolParameter("Write 2D field of view.", Required = false)] public bool ConfigureFieldOfView2D { get; set; }
            [ToolParameter("2D field of view.", Required = false)] public float FieldOfView2D { get; set; } = 60f;
            [ToolParameter("Write near clipping plane.", Required = false)] public bool ConfigureNearClippingPlane { get; set; }
            [ToolParameter("Near clipping plane.", Required = false)] public float NearClippingPlane { get; set; } = 0.15f;
            [ToolParameter("Write far clipping plane.", Required = false)] public bool ConfigureFarClippingPlane { get; set; }
            [ToolParameter("Far clipping plane.", Required = false)] public float FarClippingPlane { get; set; } = 1000f;
            [ToolParameter("Write post-processing state.", Required = false)] public bool ConfigurePostProcessing { get; set; }
            [ToolParameter("Post-processing value.", Required = false)] public bool EnablePostProcessing { get; set; } = true;
            [ToolParameter("Write occlusion culling state.", Required = false)] public bool ConfigureOcclusionCulling { get; set; }
            [ToolParameter("Occlusion culling value.", Required = false)] public bool OcclusionCulling { get; set; } = true;
            [ToolParameter("Write prefer-VR state.", Required = false)] public bool ConfigurePreferVRMode { get; set; }
            [ToolParameter("Prefer-VR value.", Required = false)] public bool PreferVRMode { get; set; }
            [ToolParameter("Write player transmission frequency.", Required = false)] public bool ConfigureTransmissionFrequency { get; set; }
            [ToolParameter("Player transmission frequency in Hz.", Required = false)] public float TransmissionFrequency { get; set; } = 8f;
            [ToolParameter("Optional transmission protocol: UDP or TCP.", Required = false)] public string TransmissionProtocol { get; set; }
            [ToolParameter("Save the scene.", Required = false)] public bool SaveScene { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.ConfigurePlayer(CoplayParameterParser.Parse<VE2ConfigurePlayerParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.SceneValidatePlayer.Name, Description = VE2ToolCatalog.SceneValidatePlayer.Description, Group = "core")]
    public static class VE2CoplaySceneValidatePlayerTool
    {
        public static object HandleCommand(JObject _) => VE2McpTools.ValidatePlayer();
    }

    [McpForUnityTool(VE2ToolCatalog.SceneCreateTeleportAnchor.Name, Description = VE2ToolCatalog.SceneCreateTeleportAnchor.Description, Group = "core")]
    public static class VE2CoplaySceneCreateTeleportAnchorTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Optional unique anchor name.", Required = false)] public string Name { get; set; }
            [ToolParameter("Optional parent path.", Required = false)] public string ParentPath { get; set; }
            [ToolParameter("Optional world position [x,y,z].", Required = false)] public float[] Position { get; set; }
            [ToolParameter("Teleport anchor range.", Required = false)] public float Range { get; set; } = 0.75f;
            [ToolParameter("Save the scene.", Required = false)] public bool SaveScene { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.CreateTeleportAnchor(CoplayParameterParser.Parse<VE2CreateTeleportAnchorParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.SceneValidateTeleportation.Name, Description = VE2ToolCatalog.SceneValidateTeleportation.Description, Group = "core")]
    public static class VE2CoplaySceneValidateTeleportationTool
    {
        public static object HandleCommand(JObject _) => VE2McpTools.ValidateTeleportation();
    }

    [McpForUnityTool(VE2ToolCatalog.SceneMakeNetworked.Name, Description = VE2ToolCatalog.SceneMakeNetworked.Description, Group = "core")]
    public static class VE2CoplaySceneMakeNetworkedTool
    {
        public sealed class Parameters
        {
            [ToolParameter("GameObject hierarchy path or name.")] public string GameObjectPath { get; set; }
            [ToolParameter("Network component kind: transform, rigidbody, or network_object.")] public string Kind { get; set; }
            [ToolParameter("Ensure a Rigidbody for rigidbody sync.", Required = false)] public bool EnsureRigidbody { get; set; } = true;
            [ToolParameter("Write non-host transform modification state.", Required = false)] public bool ConfigureNonHostsCanModifyTransform { get; set; }
            [ToolParameter("Non-host transform modification value.", Required = false)] public bool NonHostsCanModifyTransform { get; set; } = true;
            [ToolParameter("VE2 networked state.", Required = false)] public bool IsNetworked { get; set; } = true;
            [ToolParameter("Transmission frequency in Hz.", Required = false)] public float TransmissionFrequency { get; set; } = 1f;
            [ToolParameter("Transmission protocol: UDP or TCP.", Required = false)] public string TransmissionProtocol { get; set; } = "UDP";
            [ToolParameter("Save the scene.", Required = false)] public bool SaveScene { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.MakeNetworked(CoplayParameterParser.Parse<VE2MakeNetworkedParams>(value));
    }
}
