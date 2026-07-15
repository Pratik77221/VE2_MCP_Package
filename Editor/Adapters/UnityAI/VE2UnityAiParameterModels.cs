using Unity.AI.MCP.Editor.ToolRegistry;

namespace Imperial.VE2.MCP.Editor.Adapters.UnityAI
{
    public sealed class UnityAiSpawnPrefabParams
    {
        [McpDescription("Exact VE2 Resources prefab name, for example ToggleButton, CustomInfoPoint, PlayerSpawner, PlatformIntegration, or InstanceIntegration.", Required = true)]
        public string ResourceName { get; set; }

        [McpDescription("Optional GameObject name to assign after spawning. The tool makes it unique when needed.")]
        public string Name { get; set; }

        [McpDescription("Optional parent GameObject hierarchy path or object name.")]
        public string ParentPath { get; set; }

        [McpDescription("Optional world position as [x,y,z].")]
        public float[] Position { get; set; }

        [McpDescription("Optional world rotation as Euler angles [x,y,z].")]
        public float[] RotationEuler { get; set; }

        [McpDescription("Optional local scale as [x,y,z].")]
        public float[] Scale { get; set; }

        [McpDescription("When true, report duplicate VE2 name-derived sync IDs after spawning.")]
        public bool EnsureUniqueSyncIds { get; set; } = true;

        public VE2SpawnPrefabParams ToCore()
        {
            return new VE2SpawnPrefabParams
            {
                ResourceName = ResourceName,
                Name = Name,
                ParentPath = ParentPath,
                Position = Position,
                RotationEuler = RotationEuler,
                Scale = Scale,
                EnsureUniqueSyncIds = EnsureUniqueSyncIds
            };
        }
    }

    public sealed class UnityAiEnsureProviderParams
    {
        [McpDescription("VE2 provider to ensure in the current scene.", Required = true, EnumType = typeof(VE2ProviderKind))]
        public string Provider { get; set; }

        public VE2EnsureProviderParams ToCore()
        {
            return new VE2EnsureProviderParams { Provider = Provider };
        }
    }

    public sealed class UnityAiFixNameClashesParams
    {
        [McpDescription("When true, report required renames without changing GameObjects.")]
        public bool DryRun { get; set; } = true;

        public VE2FixNameClashesParams ToCore()
        {
            return new VE2FixNameClashesParams { DryRun = DryRun };
        }
    }

    public sealed class UnityAiQuickSetupParams
    {
        [McpDescription("Create or update the VE2 plugin asmdef in Assets/Scripts.")]
        public bool Asmdef { get; set; } = true;

        [McpDescription("Import TextMeshPro Essentials using VE2's setup utility.")]
        public bool Tmp { get; set; } = true;

        [McpDescription("Configure VE2 layers and tags. This can remove non-VE2 custom layers and tags.")]
        public bool LayersAndTags { get; set; }

        [McpDescription("Required when LayersAndTags is true because VE2 setup can remove non-VE2 custom layers and tags.")]
        public bool ConfirmDestructiveLayerTagChanges { get; set; }

        [McpDescription("Run VE2 URP setup.")]
        public bool Urp { get; set; } = true;

        [McpDescription("Enable XR Plugin Management through VE2's setup utility.")]
        public bool Xr { get; set; } = true;

        [McpDescription("Enable OpenXR Oculus and Meta interaction features through VE2's setup utility.")]
        public bool OculusProfile { get; set; } = true;

        [McpDescription("Create VE2 Editor Toolbox settings.")]
        public bool EditorToolbox { get; set; } = true;

        [McpDescription("Copy VE2's AndroidManifest.xml from Resources.")]
        public bool AndroidManifest { get; set; } = true;

        [McpDescription("Create a VE2 quickstart scene using VE2SetupSceneHolder.")]
        public bool CreateScene { get; set; }

        [McpDescription("Generate VE2's Unity .gitignore if one does not exist.")]
        public bool Gitignore { get; set; } = true;

        public VE2QuickSetupParams ToCore()
        {
            return new VE2QuickSetupParams
            {
                Asmdef = Asmdef,
                Tmp = Tmp,
                LayersAndTags = LayersAndTags,
                ConfirmDestructiveLayerTagChanges = ConfirmDestructiveLayerTagChanges,
                Urp = Urp,
                Xr = Xr,
                OculusProfile = OculusProfile,
                EditorToolbox = EditorToolbox,
                AndroidManifest = AndroidManifest,
                CreateScene = CreateScene,
                Gitignore = Gitignore
            };
        }
    }

    public sealed class UnityAiQuickStartSceneParams
    {
        [McpDescription("Target alphanumeric scene name. When empty, use the active scene name or VE2QuickStart.")]
        public string SceneName { get; set; }

        [McpDescription("When true, replace root GameObjects in an existing target scene with the VE2 quickstart template.")]
        public bool ResetExistingScene { get; set; } = true;

        [McpDescription("When true, save the target scene after applying the VE2 quickstart template.")]
        public bool SaveScene { get; set; } = true;

        public VE2QuickStartSceneParams ToCore()
        {
            return new VE2QuickStartSceneParams
            {
                SceneName = SceneName,
                ResetExistingScene = ResetExistingScene,
                SaveScene = SaveScene
            };
        }
    }

    public sealed class UnityAiConfigureSpawnManagerParams
    {
        [McpDescription("Hierarchy path or name of the GameObject containing V_GameObjectSpawnManager.", Required = true)]
        public string SpawnManagerPath { get; set; }

        [McpDescription("Scene GameObject path, object name, or prefab asset path to assign to _gameobjectToSpawn.")]
        public string ObjectToSpawnPathOrAsset { get; set; }

        [McpDescription("Scene Transform hierarchy path or name to assign to _spawnPosition.")]
        public string SpawnTransformPath { get; set; }

        public VE2ConfigureSpawnManagerParams ToCore()
        {
            return new VE2ConfigureSpawnManagerParams
            {
                SpawnManagerPath = SpawnManagerPath,
                ObjectToSpawnPathOrAsset = ObjectToSpawnPathOrAsset,
                SpawnTransformPath = SpawnTransformPath
            };
        }
    }

    public sealed class UnityAiMakeGrabbableParams
    {
        [McpDescription("Hierarchy path or name of the GameObject that should be made VE2 grabbable.", Required = true)]
        public string GameObjectPath { get; set; }

        [McpDescription("When true, add a Rigidbody if the object does not have one.")]
        public bool EnsureRigidbody { get; set; } = true;

        [McpDescription("When true, add a fallback BoxCollider if the object has no usable non-trigger collider.")]
        public bool EnsureCollider { get; set; } = true;

        [McpDescription("When true, add V_RigidbodySyncable when VE2 exposes it and the object does not have one.")]
        public bool EnsureRigidbodySyncable { get; set; } = true;

        [McpDescription("When true, save the active scene after applying changes.")]
        public bool SaveScene { get; set; }

        public VE2MakeGrabbableParams ToCore()
        {
            return new VE2MakeGrabbableParams
            {
                GameObjectPath = GameObjectPath,
                EnsureRigidbody = EnsureRigidbody,
                EnsureCollider = EnsureCollider,
                EnsureRigidbodySyncable = EnsureRigidbodySyncable,
                SaveScene = SaveScene
            };
        }
    }

    public sealed class UnityAiPrepareForDeploymentParams
    {
        [McpDescription("Optional alphanumeric scene and plugin name. VE2 deployment does not allow spaces or special characters.")]
        public string SceneName { get; set; }

        [McpDescription("When true, ensure an active V_PlatformIntegration exists before preflight.")]
        public bool EnsurePlatformIntegration { get; set; } = true;

        [McpDescription("When true, activate an existing quickstart NetworkIntegration object when found.")]
        public bool ActivateNetworkIntegration { get; set; } = true;

        [McpDescription("When true, save the active scene before preflight.")]
        public bool SaveScene { get; set; } = true;

        public VE2PrepareForDeploymentParams ToCore()
        {
            return new VE2PrepareForDeploymentParams
            {
                SceneName = SceneName,
                EnsurePlatformIntegration = EnsurePlatformIntegration,
                ActivateNetworkIntegration = ActivateNetworkIntegration,
                SaveScene = SaveScene
            };
        }
    }

    public sealed class UnityAiConfigureSyncParams
    {
        [McpDescription("Hierarchy path or name of the GameObject containing the VE2 component to configure.", Required = true)]
        public string GameObjectPath { get; set; }

        [McpDescription("Optional component type name when the GameObject has multiple sync-capable VE2 components.")]
        public string ComponentTypeName { get; set; }

        [McpDescription("When true, write IsNetworked to the component's serialized field.")]
        public bool ConfigureIsNetworked { get; set; }

        [McpDescription("Value to write when ConfigureIsNetworked is true.")]
        public bool IsNetworked { get; set; } = true;

        [McpDescription("When true, write TransmissionFrequency to the component's serialized field.")]
        public bool ConfigureTransmissionFrequency { get; set; }

        [McpDescription("Frequency in Hz when ConfigureTransmissionFrequency is true. VE2 supports 0.2 to 50 for WorldStateSyncConfig components.")]
        public float TransmissionFrequency { get; set; } = 1f;

        [McpDescription("Optional VE2 transmission protocol to write to TransmissionType.", EnumType = typeof(VE2TransmissionProtocolKind))]
        public string TransmissionProtocol { get; set; }

        public VE2ConfigureSyncParams ToCore()
        {
            return new VE2ConfigureSyncParams
            {
                GameObjectPath = GameObjectPath,
                ComponentTypeName = ComponentTypeName,
                ConfigureIsNetworked = ConfigureIsNetworked,
                IsNetworked = IsNetworked,
                ConfigureTransmissionFrequency = ConfigureTransmissionFrequency,
                TransmissionFrequency = TransmissionFrequency,
                TransmissionProtocol = TransmissionProtocol
            };
        }
    }
}
