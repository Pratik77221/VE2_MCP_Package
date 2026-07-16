namespace Imperial.VE2.MCP.Editor
{
    public enum VE2ActivatableKind
    {
        toggle_button,
        hold_button,
        pressure_plate
    }

    public enum VE2AdjustableKind
    {
        wheel,
        lever,
        joystick_2d,
        slider,
        slider_2d
    }

    public enum VE2NetworkComponentKind
    {
        transform,
        rigidbody,
        network_object
    }

    public enum VE2PlayerModeKind
    {
        OnlyVR,
        Only2D,
        Both
    }

    public enum VE2BuildPlatformKind
    {
        Windows,
        Android
    }

    public enum VE2InteractionScaffoldKind
    {
        toggle_activatable,
        info_point,
        handheld_activatable,
        hold_activatable,
        pressure_plate,
        free_grabbable,
        sliding_adjustable,
        rotating_adjustable,
        handheld_adjustable,
        sliding_2d_adjustable,
        rotating_2d_adjustable
    }

    public sealed class VE2InspectObjectParams
    {
        public string GameObjectPath { get; set; }
        public bool IncludeSerializedProperties { get; set; } = true;
        public int MaxSerializedProperties { get; set; } = 200;
    }

    public sealed class VE2CreateActivatableParams
    {
        public string Kind { get; set; }
        public string Name { get; set; }
        public string ParentPath { get; set; }
        public float[] Position { get; set; }
        public float[] RotationEuler { get; set; }
        public float[] Scale { get; set; }
        public bool SaveScene { get; set; }
    }

    public sealed class VE2ConfigureActivatableParams
    {
        public string GameObjectPath { get; set; }
        public string ComponentTypeName { get; set; }
        public bool ConfigureAdminOnly { get; set; }
        public bool AdminOnly { get; set; }
        public bool ConfigureControllerVibrations { get; set; }
        public bool EnableControllerVibrations { get; set; } = true;
        public bool ConfigureIsInteractable { get; set; }
        public bool IsInteractable { get; set; } = true;
        public bool ConfigureInteractRange { get; set; }
        public float InteractRange { get; set; } = 50f;
        public bool ConfigureIsNetworked { get; set; }
        public bool IsNetworked { get; set; } = true;
        public bool ConfigureActivateOnStart { get; set; }
        public bool ActivateOnStart { get; set; }
        public string ActivationGroupId { get; set; }
        public bool SaveScene { get; set; }
    }

    public sealed class VE2CreateAdjustableParams
    {
        public string Kind { get; set; }
        public string Name { get; set; }
        public string ParentPath { get; set; }
        public float[] Position { get; set; }
        public float[] RotationEuler { get; set; }
        public float[] Scale { get; set; }
        public bool SaveScene { get; set; }
    }

    public sealed class VE2ConfigureAdjustableParams
    {
        public string GameObjectPath { get; set; }
        public string ComponentTypeName { get; set; }
        public float[] StartingValue { get; set; }
        public float[] MinimumOutputValue { get; set; }
        public float[] MaximumOutputValue { get; set; }
        public float[] MinimumSpatialValue { get; set; }
        public float[] MaximumSpatialValue { get; set; }
        public int NumberOfValues { get; set; }
        public bool ConfigureEmitValueOnStart { get; set; }
        public bool EmitValueOnStart { get; set; } = true;
        public bool ConfigureAdminOnly { get; set; }
        public bool AdminOnly { get; set; }
        public bool ConfigureControllerVibrations { get; set; }
        public bool EnableControllerVibrations { get; set; } = true;
        public bool ConfigureIsInteractable { get; set; }
        public bool IsInteractable { get; set; } = true;
        public bool ConfigureInteractRange { get; set; }
        public float InteractRange { get; set; } = 50f;
        public bool SaveScene { get; set; }
    }

    public sealed class VE2CreateInfoPointParams
    {
        public string Name { get; set; }
        public string ParentPath { get; set; }
        public float[] Position { get; set; }
        public float[] RotationEuler { get; set; }
        public float[] Scale { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool SaveScene { get; set; }
    }

    public sealed class VE2ConnectInteractionEventParams
    {
        public string SourceGameObjectPath { get; set; }
        public string SourceComponentTypeName { get; set; }
        public string EventName { get; set; }
        public string TargetGameObjectPath { get; set; }
        public string TargetComponentTypeName { get; set; }
        public string TargetMethodName { get; set; }
        public bool ReplaceExisting { get; set; }
        public bool SaveScene { get; set; }
    }

    public sealed class VE2DuplicateInteractableParams
    {
        public string SourceGameObjectPath { get; set; }
        public string Name { get; set; }
        public string ParentPath { get; set; }
        public float[] PositionOffset { get; set; }
        public bool SaveScene { get; set; }
    }

    public sealed class VE2ConfigurePlayerParams
    {
        public string PlayerSpawnerPath { get; set; }
        public string SupportedPlayerModes { get; set; }
        public string[] InteractableLayers { get; set; }
        public string[] TraversableLayers { get; set; }
        public string[] CollisionLayers { get; set; }
        public bool ConfigureFreeFlyMode { get; set; }
        public bool FreeFlyMode { get; set; }
        public bool ConfigureTeleportRangeMultiplier { get; set; }
        public float TeleportRangeMultiplier { get; set; } = 1f;
        public bool ConfigureMaxVerticalDragHeight { get; set; }
        public float MaxVerticalDragHeight { get; set; } = 10f;
        public bool ConfigureFieldOfView2D { get; set; }
        public float FieldOfView2D { get; set; } = 60f;
        public bool ConfigureNearClippingPlane { get; set; }
        public float NearClippingPlane { get; set; } = 0.15f;
        public bool ConfigureFarClippingPlane { get; set; }
        public float FarClippingPlane { get; set; } = 1000f;
        public bool ConfigurePostProcessing { get; set; }
        public bool EnablePostProcessing { get; set; } = true;
        public bool ConfigureOcclusionCulling { get; set; }
        public bool OcclusionCulling { get; set; } = true;
        public bool ConfigurePreferVRMode { get; set; }
        public bool PreferVRMode { get; set; }
        public bool ConfigureTransmissionFrequency { get; set; }
        public float TransmissionFrequency { get; set; } = 8f;
        public string TransmissionProtocol { get; set; }
        public bool SaveScene { get; set; }
    }

    public sealed class VE2CreateTeleportAnchorParams
    {
        public string Name { get; set; }
        public string ParentPath { get; set; }
        public float[] Position { get; set; }
        public float Range { get; set; } = 0.75f;
        public bool SaveScene { get; set; }
    }

    public sealed class VE2MovePlayerParams
    {
        public float[] Position { get; set; }
        public float[] RotationEuler { get; set; }
    }

    public sealed class VE2MakeNetworkedParams
    {
        public string GameObjectPath { get; set; }
        public string Kind { get; set; }
        public bool EnsureRigidbody { get; set; } = true;
        public bool ConfigureNonHostsCanModifyTransform { get; set; }
        public bool NonHostsCanModifyTransform { get; set; } = true;
        public bool IsNetworked { get; set; } = true;
        public float TransmissionFrequency { get; set; } = 1f;
        public string TransmissionProtocol { get; set; } = "UDP";
        public bool SaveScene { get; set; }
    }

    public sealed class VE2RuntimeObjectParams
    {
        public string GameObjectPath { get; set; }
    }

    public sealed class VE2RuntimeSetActivatableParams
    {
        public string GameObjectPath { get; set; }
        public bool Activated { get; set; }
    }

    public sealed class VE2RuntimeSetAdjustableParams
    {
        public string GameObjectPath { get; set; }
        public float[] Value { get; set; }
        public bool ResetToStartingValue { get; set; }
    }

    public sealed class VE2RuntimeSpawnParams
    {
        public string SpawnManagerPath { get; set; }
    }

    public sealed class VE2RuntimeDespawnParams
    {
        public string SpawnManagerPath { get; set; }
        public string TargetGameObjectPath { get; set; }
    }

    public sealed class VE2RuntimeSyncSnapshotParams
    {
        public string GameObjectPath { get; set; }
        public int MaxObjects { get; set; } = 200;
    }

    public sealed class VE2ContextSearchParams
    {
        public string Query { get; set; }
        public int MaxResults { get; set; } = 20;
        public int ContextLines { get; set; } = 2;
    }

    public sealed class VE2ContextGetInterfaceParams
    {
        public string TypeName { get; set; }
    }

    public sealed class VE2PrefabContractParams
    {
        public string ResourceName { get; set; }
        public int MaxDepth { get; set; } = 8;
    }

    public sealed class VE2SceneManifestParams
    {
        public int MaxDepth { get; set; } = 8;
        public bool IncludeInactive { get; set; } = true;
    }

    public sealed class VE2ScaffoldInteractionParams
    {
        public string ClassName { get; set; }
        public string Namespace { get; set; }
        public string InteractionKind { get; set; }
        public string OutputFolder { get; set; } = "Assets/Scripts";
        public bool Overwrite { get; set; }
    }

    public sealed class VE2ScaffoldNetworkObjectParams
    {
        public string ClassName { get; set; }
        public string Namespace { get; set; }
        public string OutputFolder { get; set; } = "Assets/Scripts";
        public bool Overwrite { get; set; }
    }

    public sealed class VE2ValidatePluginScriptsParams
    {
        public string RootPath { get; set; } = "Assets/Scripts";
    }

    public sealed class VE2BuildExportParams
    {
        public string Platform { get; set; }
        public int Version { get; set; }
        public bool BuildWithEcsBurst { get; set; }
        public bool ConfirmBuild { get; set; }
    }

    public sealed class VE2BuildPlatformParams
    {
        public string Platform { get; set; }
    }

    public sealed class VE2DeploymentUploadParams
    {
        public string Platform { get; set; }
        public bool ConfirmUpload { get; set; }
    }
}
