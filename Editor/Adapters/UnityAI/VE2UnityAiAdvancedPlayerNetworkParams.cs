using Unity.AI.MCP.Editor.ToolRegistry;

namespace Imperial.VE2.MCP.Editor.Adapters.UnityAI
{
    public sealed class UnityAiConfigurePlayerParams
    {
        [McpDescription("Optional V_PlayerSpawner hierarchy path.", Required = false)] public string PlayerSpawnerPath { get; set; }
        [McpDescription("Optional supported player mode.", Required = false, EnumType = typeof(VE2PlayerModeKind))] public string SupportedPlayerModes { get; set; }
        [McpDescription("Optional interactable layer names.", Required = false)] public string[] InteractableLayers { get; set; }
        [McpDescription("Optional traversable layer names.", Required = false)] public string[] TraversableLayers { get; set; }
        [McpDescription("Optional collision layer names.", Required = false)] public string[] CollisionLayers { get; set; }
        [McpDescription("Write free-fly mode.", Required = false)] public bool ConfigureFreeFlyMode { get; set; }
        [McpDescription("Free-fly mode value.", Required = false)] public bool FreeFlyMode { get; set; }
        [McpDescription("Write teleport range multiplier.", Required = false)] public bool ConfigureTeleportRangeMultiplier { get; set; }
        [McpDescription("Teleport range multiplier.", Required = false)] public float TeleportRangeMultiplier { get; set; } = 1f;
        [McpDescription("Write maximum vertical drag height.", Required = false)] public bool ConfigureMaxVerticalDragHeight { get; set; }
        [McpDescription("Maximum vertical drag height.", Required = false)] public float MaxVerticalDragHeight { get; set; } = 10f;
        [McpDescription("Write 2D field of view.", Required = false)] public bool ConfigureFieldOfView2D { get; set; }
        [McpDescription("2D field of view.", Required = false)] public float FieldOfView2D { get; set; } = 60f;
        [McpDescription("Write near clipping plane.", Required = false)] public bool ConfigureNearClippingPlane { get; set; }
        [McpDescription("Near clipping plane.", Required = false)] public float NearClippingPlane { get; set; } = 0.15f;
        [McpDescription("Write far clipping plane.", Required = false)] public bool ConfigureFarClippingPlane { get; set; }
        [McpDescription("Far clipping plane.", Required = false)] public float FarClippingPlane { get; set; } = 1000f;
        [McpDescription("Write post-processing state.", Required = false)] public bool ConfigurePostProcessing { get; set; }
        [McpDescription("Post-processing value.", Required = false)] public bool EnablePostProcessing { get; set; } = true;
        [McpDescription("Write occlusion culling state.", Required = false)] public bool ConfigureOcclusionCulling { get; set; }
        [McpDescription("Occlusion culling value.", Required = false)] public bool OcclusionCulling { get; set; } = true;
        [McpDescription("Write prefer-VR state.", Required = false)] public bool ConfigurePreferVRMode { get; set; }
        [McpDescription("Prefer-VR value.", Required = false)] public bool PreferVRMode { get; set; }
        [McpDescription("Write player transmission frequency.", Required = false)] public bool ConfigureTransmissionFrequency { get; set; }
        [McpDescription("Player transmission frequency in Hz.", Required = false)] public float TransmissionFrequency { get; set; } = 8f;
        [McpDescription("Optional transmission protocol.", Required = false, EnumType = typeof(VE2TransmissionProtocolKind))] public string TransmissionProtocol { get; set; }
        [McpDescription("Save the scene.", Required = false)] public bool SaveScene { get; set; }
        public VE2ConfigurePlayerParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2ConfigurePlayerParams>(this);
    }

    public sealed class UnityAiCreateTeleportAnchorParams
    {
        [McpDescription("Optional unique anchor name.", Required = false)] public string Name { get; set; }
        [McpDescription("Optional parent path.", Required = false)] public string ParentPath { get; set; }
        [McpDescription("Optional world position [x,y,z].", Required = false)] public float[] Position { get; set; }
        [McpDescription("Teleport anchor range.", Required = false)] public float Range { get; set; } = 0.75f;
        [McpDescription("Save the scene.", Required = false)] public bool SaveScene { get; set; }
        public VE2CreateTeleportAnchorParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2CreateTeleportAnchorParams>(this);
    }

    public sealed class UnityAiMakeNetworkedParams
    {
        [McpDescription("GameObject hierarchy path or name.", Required = true)] public string GameObjectPath { get; set; }
        [McpDescription("Network component kind.", Required = true, EnumType = typeof(VE2NetworkComponentKind))] public string Kind { get; set; }
        [McpDescription("Ensure a Rigidbody for rigidbody sync.", Required = false)] public bool EnsureRigidbody { get; set; } = true;
        [McpDescription("Write non-host transform modification state.", Required = false)] public bool ConfigureNonHostsCanModifyTransform { get; set; }
        [McpDescription("Non-host transform modification value.", Required = false)] public bool NonHostsCanModifyTransform { get; set; } = true;
        [McpDescription("VE2 networked state.", Required = false)] public bool IsNetworked { get; set; } = true;
        [McpDescription("Transmission frequency in Hz.", Required = false)] public float TransmissionFrequency { get; set; } = 1f;
        [McpDescription("Transmission protocol.", Required = false, EnumType = typeof(VE2TransmissionProtocolKind))] public string TransmissionProtocol { get; set; } = "UDP";
        [McpDescription("Save the scene.", Required = false)] public bool SaveScene { get; set; }
        public VE2MakeNetworkedParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2MakeNetworkedParams>(this);
    }
}
