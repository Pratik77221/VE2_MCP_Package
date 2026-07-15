using Unity.AI.MCP.Editor.ToolRegistry;

namespace Imperial.VE2.MCP.Editor
{
    public enum VE2ProviderKind
    {
        player_spawner,
        platform_integration,
        instance_integration,
        file_system
    }

    public enum VE2TransmissionProtocolKind
    {
        UDP,
        TCP
    }

    public sealed class VE2SpawnPrefabParams
    {
        [McpDescription("Exact VE2 Resources prefab name, e.g. ToggleButton, CustomInfoPoint, PlayerSpawner, PlatformIntegration, InstanceIntegration.", Required = true)]
        public string ResourceName { get; set; }

        [McpDescription("Optional GameObject name to assign after spawning. The bridge will make it unique if needed.")]
        public string Name { get; set; }

        [McpDescription("Optional parent GameObject hierarchy path or object name.")]
        public string ParentPath { get; set; }

        [McpDescription("Optional world position [x,y,z].")]
        public float[] Position { get; set; }

        [McpDescription("Optional world rotation as Euler angles [x,y,z].")]
        public float[] RotationEuler { get; set; }

        [McpDescription("Optional local scale [x,y,z].")]
        public float[] Scale { get; set; }

        [McpDescription("If true, the tool fails when duplicate VE2 name-derived sync IDs are present after spawning.")]
        public bool EnsureUniqueSyncIds { get; set; } = true;
    }

    public sealed class VE2EnsureProviderParams
    {
        [McpDescription("VE2 provider to ensure in the current scene.", Required = true, EnumType = typeof(VE2ProviderKind))]
        public string Provider { get; set; }
    }

    public sealed class VE2FixNameClashesParams
    {
        [McpDescription("If true, report the changes that would be needed without renaming GameObjects.")]
        public bool DryRun { get; set; } = true;
    }

    public sealed class VE2QuickSetupParams
    {
        [McpDescription("Create or update the VE2 plugin asmdef in Assets/Scripts.")]
        public bool Asmdef { get; set; } = true;

        [McpDescription("Import TextMeshPro Essentials using VE2's setup utility.")]
        public bool Tmp { get; set; } = true;

        [McpDescription("Configure VE2 layers and tags. This can remove non-VE2 custom project layers/tags.")]
        public bool LayersAndTags { get; set; } = false;

        [McpDescription("Required when LayersAndTags is true because VE2's setup may remove non-VE2 custom layers/tags.")]
        public bool ConfirmDestructiveLayerTagChanges { get; set; } = false;

        [McpDescription("Run VE2 URP setup.")]
        public bool Urp { get; set; } = true;

        [McpDescription("Enable XR Plugin Management through VE2's setup utility.")]
        public bool Xr { get; set; } = true;

        [McpDescription("Enable OpenXR Oculus/Meta interaction features through VE2's setup utility.")]
        public bool OculusProfile { get; set; } = true;

        [McpDescription("Create VE2 Editor Toolbox settings.")]
        public bool EditorToolbox { get; set; } = true;

        [McpDescription("Copy VE2's AndroidManifest.xml from Resources.")]
        public bool AndroidManifest { get; set; } = true;

        [McpDescription("Create a VE2 quickstart scene using VE2SetupSceneHolder.")]
        public bool CreateScene { get; set; } = false;

        [McpDescription("Generate VE2's Unity .gitignore if one does not exist.")]
        public bool Gitignore { get; set; } = true;
    }

    public sealed class VE2QuickStartSceneParams
    {
        [McpDescription("Target alphanumeric scene name. If empty, the active scene name is used when available, otherwise VE2QuickStart is used.")]
        public string SceneName { get; set; }

        [McpDescription("If true, replace root GameObjects in an existing target scene with the VE2 quickstart template.")]
        public bool ResetExistingScene { get; set; } = true;

        [McpDescription("If true, save the target scene after applying the VE2 quickstart template.")]
        public bool SaveScene { get; set; } = true;
    }

    public sealed class VE2ConfigureSpawnManagerParams
    {
        [McpDescription("Hierarchy path or name of the GameObject containing V_GameObjectSpawnManager.", Required = true)]
        public string SpawnManagerPath { get; set; }

        [McpDescription("Scene GameObject path/name or prefab asset path to assign to _gameobjectToSpawn.")]
        public string ObjectToSpawnPathOrAsset { get; set; }

        [McpDescription("Scene Transform hierarchy path/name to assign to _spawnPosition.")]
        public string SpawnTransformPath { get; set; }
    }

    public sealed class VE2MakeGrabbableParams
    {
        [McpDescription("Hierarchy path or name of the GameObject that should be made VE2 grabbable.", Required = true)]
        public string GameObjectPath { get; set; }

        [McpDescription("If true, add a Rigidbody when the object does not already have one.")]
        public bool EnsureRigidbody { get; set; } = true;

        [McpDescription("If true, add a fallback BoxCollider when the object has no usable non-trigger collider.")]
        public bool EnsureCollider { get; set; } = true;

        [McpDescription("If true, add V_RigidbodySyncable when VE2 exposes that component and the object does not already have one.")]
        public bool EnsureRigidbodySyncable { get; set; } = true;

        [McpDescription("If true, save the active scene after applying grabbable changes.")]
        public bool SaveScene { get; set; } = false;
    }

    public sealed class VE2PrepareForDeploymentParams
    {
        [McpDescription("Optional alphanumeric scene/plugin name. VE2 deployment does not allow spaces or special characters.")]
        public string SceneName { get; set; }

        [McpDescription("If true, ensure an active V_PlatformIntegration exists before running deployment preflight.")]
        public bool EnsurePlatformIntegration { get; set; } = true;

        [McpDescription("If true, activate an existing quickstart NetworkIntegration object when found.")]
        public bool ActivateNetworkIntegration { get; set; } = true;

        [McpDescription("If true, save the active scene before running deployment preflight.")]
        public bool SaveScene { get; set; } = true;
    }

    public sealed class VE2ConfigureSyncParams
    {
        [McpDescription("Hierarchy path or name of the GameObject containing the VE2 component to configure.", Required = true)]
        public string GameObjectPath { get; set; }

        [McpDescription("Optional component type name when the GameObject has more than one sync-capable VE2 component.")]
        public string ComponentTypeName { get; set; }

        [McpDescription("If true, write the IsNetworked value to the component's serialized IsNetworked field.")]
        public bool ConfigureIsNetworked { get; set; } = false;

        [McpDescription("Networked flag value to write when ConfigureIsNetworked is true.")]
        public bool IsNetworked { get; set; } = true;

        [McpDescription("If true, write the TransmissionFrequency value to the component's serialized TransmissionFrequency field.")]
        public bool ConfigureTransmissionFrequency { get; set; } = false;

        [McpDescription("Transmission frequency in Hz to write when ConfigureTransmissionFrequency is true. VE2 supports 0.2 to 50 for WorldStateSyncConfig components.")]
        public float TransmissionFrequency { get; set; } = 1f;

        [McpDescription("Optional VE2 transmission protocol to write to TransmissionType.", EnumType = typeof(VE2TransmissionProtocolKind))]
        public string TransmissionProtocol { get; set; }
    }
}
