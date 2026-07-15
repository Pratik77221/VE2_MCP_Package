using MCPForUnity.Editor.Tools;

namespace Imperial.VE2.MCP.Editor.Adapters.Coplay
{
    public class CoplaySpawnPrefabParams
    {
        [ToolParameter("Exact VE2 Resources prefab name, for example ToggleButton, CustomInfoPoint, PlayerSpawner, PlatformIntegration, or InstanceIntegration.")]
        public string ResourceName { get; set; }

        [ToolParameter("Optional GameObject name to assign after spawning. The tool makes it unique when needed.", Required = false)]
        public string Name { get; set; }

        [ToolParameter("Optional parent GameObject hierarchy path or object name.", Required = false)]
        public string ParentPath { get; set; }

        [ToolParameter("Optional world position as [x,y,z].", Required = false)]
        public float[] Position { get; set; }

        [ToolParameter("Optional world rotation as Euler angles [x,y,z].", Required = false)]
        public float[] RotationEuler { get; set; }

        [ToolParameter("Optional local scale as [x,y,z].", Required = false)]
        public float[] Scale { get; set; }

        [ToolParameter("When true, report duplicate VE2 name-derived sync IDs after spawning.", Required = false)]
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

    public class CoplayEnsureProviderParams
    {
        [ToolParameter("VE2 provider to ensure: player_spawner, platform_integration, instance_integration, or file_system.")]
        public string Provider { get; set; }

        public VE2EnsureProviderParams ToCore()
        {
            return new VE2EnsureProviderParams { Provider = Provider };
        }
    }

    public class CoplayFixNameClashesParams
    {
        [ToolParameter("When true, report required renames without changing GameObjects.", Required = false)]
        public bool DryRun { get; set; } = true;

        public VE2FixNameClashesParams ToCore()
        {
            return new VE2FixNameClashesParams { DryRun = DryRun };
        }
    }

    public class CoplayQuickSetupParams
    {
        [ToolParameter("Create or update the VE2 plugin asmdef in Assets/Scripts.", Required = false)]
        public bool Asmdef { get; set; } = true;

        [ToolParameter("Import TextMeshPro Essentials using VE2's setup utility.", Required = false)]
        public bool Tmp { get; set; } = true;

        [ToolParameter("Configure VE2 layers and tags. This can remove non-VE2 custom layers and tags.", Required = false)]
        public bool LayersAndTags { get; set; }

        [ToolParameter("Required when LayersAndTags is true because VE2 setup can remove non-VE2 custom layers and tags.", Required = false)]
        public bool ConfirmDestructiveLayerTagChanges { get; set; }

        [ToolParameter("Run VE2 URP setup.", Required = false)]
        public bool Urp { get; set; } = true;

        [ToolParameter("Enable XR Plugin Management through VE2's setup utility.", Required = false)]
        public bool Xr { get; set; } = true;

        [ToolParameter("Enable OpenXR Oculus and Meta interaction features through VE2's setup utility.", Required = false)]
        public bool OculusProfile { get; set; } = true;

        [ToolParameter("Create VE2 Editor Toolbox settings.", Required = false)]
        public bool EditorToolbox { get; set; } = true;

        [ToolParameter("Copy VE2's AndroidManifest.xml from Resources.", Required = false)]
        public bool AndroidManifest { get; set; } = true;

        [ToolParameter("Create a VE2 quickstart scene using VE2SetupSceneHolder.", Required = false)]
        public bool CreateScene { get; set; }

        [ToolParameter("Generate VE2's Unity .gitignore if one does not exist.", Required = false)]
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

    public class CoplayQuickStartSceneParams
    {
        [ToolParameter("Target alphanumeric scene name. When empty, use the active scene name or VE2QuickStart.", Required = false)]
        public string SceneName { get; set; }

        [ToolParameter("When true, replace root GameObjects in an existing target scene with the VE2 quickstart template.", Required = false)]
        public bool ResetExistingScene { get; set; } = true;

        [ToolParameter("When true, save the target scene after applying the VE2 quickstart template.", Required = false)]
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

    public class CoplayConfigureSpawnManagerParams
    {
        [ToolParameter("Hierarchy path or name of the GameObject containing V_GameObjectSpawnManager.")]
        public string SpawnManagerPath { get; set; }

        [ToolParameter("Scene GameObject path, object name, or prefab asset path to assign to _gameobjectToSpawn.", Required = false)]
        public string ObjectToSpawnPathOrAsset { get; set; }

        [ToolParameter("Scene Transform hierarchy path or name to assign to _spawnPosition.", Required = false)]
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

    public class CoplayMakeGrabbableParams
    {
        [ToolParameter("Hierarchy path or name of the GameObject that should be made VE2 grabbable.")]
        public string GameObjectPath { get; set; }

        [ToolParameter("When true, add a Rigidbody if the object does not have one.", Required = false)]
        public bool EnsureRigidbody { get; set; } = true;

        [ToolParameter("When true, add a fallback BoxCollider if the object has no usable non-trigger collider.", Required = false)]
        public bool EnsureCollider { get; set; } = true;

        [ToolParameter("When true, add V_RigidbodySyncable when VE2 exposes it and the object does not have one.", Required = false)]
        public bool EnsureRigidbodySyncable { get; set; } = true;

        [ToolParameter("When true, save the active scene after applying changes.", Required = false)]
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

    public class CoplayPrepareForDeploymentParams
    {
        [ToolParameter("Optional alphanumeric scene and plugin name. VE2 deployment does not allow spaces or special characters.", Required = false)]
        public string SceneName { get; set; }

        [ToolParameter("When true, ensure an active V_PlatformIntegration exists before preflight.", Required = false)]
        public bool EnsurePlatformIntegration { get; set; } = true;

        [ToolParameter("When true, activate an existing quickstart NetworkIntegration object when found.", Required = false)]
        public bool ActivateNetworkIntegration { get; set; } = true;

        [ToolParameter("When true, save the active scene before preflight.", Required = false)]
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

    public class CoplayConfigureSyncParams
    {
        [ToolParameter("Hierarchy path or name of the GameObject containing the VE2 component to configure.")]
        public string GameObjectPath { get; set; }

        [ToolParameter("Optional component type name when the GameObject has multiple sync-capable VE2 components.", Required = false)]
        public string ComponentTypeName { get; set; }

        [ToolParameter("When true, write IsNetworked to the component's serialized field.", Required = false)]
        public bool ConfigureIsNetworked { get; set; }

        [ToolParameter("Value to write when ConfigureIsNetworked is true.", Required = false)]
        public bool IsNetworked { get; set; } = true;

        [ToolParameter("When true, write TransmissionFrequency to the component's serialized field.", Required = false)]
        public bool ConfigureTransmissionFrequency { get; set; }

        [ToolParameter("Frequency in Hz when ConfigureTransmissionFrequency is true. VE2 supports 0.2 to 50 for WorldStateSyncConfig components.", Required = false)]
        public float TransmissionFrequency { get; set; } = 1f;

        [ToolParameter("Optional VE2 transmission protocol: UDP or TCP.", Required = false)]
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
