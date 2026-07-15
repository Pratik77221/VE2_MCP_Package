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
        public string ResourceName { get; set; }

        public string Name { get; set; }

        public string ParentPath { get; set; }

        public float[] Position { get; set; }

        public float[] RotationEuler { get; set; }

        public float[] Scale { get; set; }

        public bool EnsureUniqueSyncIds { get; set; } = true;
    }

    public sealed class VE2EnsureProviderParams
    {
        public string Provider { get; set; }
    }

    public sealed class VE2FixNameClashesParams
    {
        public bool DryRun { get; set; } = true;
    }

    public sealed class VE2QuickSetupParams
    {
        public bool Asmdef { get; set; } = true;

        public bool Tmp { get; set; } = true;

        public bool LayersAndTags { get; set; } = false;

        public bool ConfirmDestructiveLayerTagChanges { get; set; } = false;

        public bool Urp { get; set; } = true;

        public bool Xr { get; set; } = true;

        public bool OculusProfile { get; set; } = true;

        public bool EditorToolbox { get; set; } = true;

        public bool AndroidManifest { get; set; } = true;

        public bool CreateScene { get; set; } = false;

        public bool Gitignore { get; set; } = true;
    }

    public sealed class VE2QuickStartSceneParams
    {
        public string SceneName { get; set; }

        public bool ResetExistingScene { get; set; } = true;

        public bool SaveScene { get; set; } = true;
    }

    public sealed class VE2ConfigureSpawnManagerParams
    {
        public string SpawnManagerPath { get; set; }

        public string ObjectToSpawnPathOrAsset { get; set; }

        public string SpawnTransformPath { get; set; }
    }

    public sealed class VE2MakeGrabbableParams
    {
        public string GameObjectPath { get; set; }

        public bool EnsureRigidbody { get; set; } = true;

        public bool EnsureCollider { get; set; } = true;

        public bool EnsureRigidbodySyncable { get; set; } = true;

        public bool SaveScene { get; set; } = false;
    }

    public sealed class VE2PrepareForDeploymentParams
    {
        public string SceneName { get; set; }

        public bool EnsurePlatformIntegration { get; set; } = true;

        public bool ActivateNetworkIntegration { get; set; } = true;

        public bool SaveScene { get; set; } = true;
    }

    public sealed class VE2ConfigureSyncParams
    {
        public string GameObjectPath { get; set; }

        public string ComponentTypeName { get; set; }

        public bool ConfigureIsNetworked { get; set; } = false;

        public bool IsNetworked { get; set; } = true;

        public bool ConfigureTransmissionFrequency { get; set; } = false;

        public float TransmissionFrequency { get; set; } = 1f;

        public string TransmissionProtocol { get; set; }
    }
}
