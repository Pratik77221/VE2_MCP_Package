using Unity.AI.MCP.Editor.ToolRegistry;

namespace Imperial.VE2.MCP.Editor.Adapters.UnityAI
{
    public sealed class UnityAiMovePlayerParams
    {
        [McpDescription("Optional player world position [x,y,z].", Required = false)] public float[] Position { get; set; }
        [McpDescription("Optional player world Euler rotation [x,y,z].", Required = false)] public float[] RotationEuler { get; set; }
        public VE2MovePlayerParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2MovePlayerParams>(this);
    }

    public sealed class UnityAiRuntimeObjectParams
    {
        [McpDescription("Optional GameObject path to limit inspection to one hierarchy.", Required = false)] public string GameObjectPath { get; set; }
        public VE2RuntimeObjectParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2RuntimeObjectParams>(this);
    }

    public sealed class UnityAiRuntimeSetActivatableParams
    {
        [McpDescription("GameObject path or name containing the VE2 activatable.", Required = true)] public string GameObjectPath { get; set; }
        [McpDescription("True to activate; false to deactivate.", Required = true)] public bool Activated { get; set; }
        public VE2RuntimeSetActivatableParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2RuntimeSetActivatableParams>(this);
    }

    public sealed class UnityAiRuntimeSetAdjustableParams
    {
        [McpDescription("GameObject path or name containing the VE2 adjustable.", Required = true)] public string GameObjectPath { get; set; }
        [McpDescription("One- or two-dimensional adjustable value.", Required = false)] public float[] Value { get; set; }
        [McpDescription("Reset to the configured starting value instead of using Value.", Required = false)] public bool ResetToStartingValue { get; set; }
        public VE2RuntimeSetAdjustableParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2RuntimeSetAdjustableParams>(this);
    }

    public sealed class UnityAiRuntimeSpawnParams
    {
        [McpDescription("GameObject path or name containing IV_GameObjectSpawnManager.", Required = true)] public string SpawnManagerPath { get; set; }
        public VE2RuntimeSpawnParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2RuntimeSpawnParams>(this);
    }

    public sealed class UnityAiRuntimeDespawnParams
    {
        [McpDescription("GameObject path or name containing IV_GameObjectSpawnManager.", Required = true)] public string SpawnManagerPath { get; set; }
        [McpDescription("Spawned target GameObject path or name.", Required = true)] public string TargetGameObjectPath { get; set; }
        public VE2RuntimeDespawnParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2RuntimeDespawnParams>(this);
    }

    public sealed class UnityAiRuntimeSyncSnapshotParams
    {
        [McpDescription("Optional GameObject path to limit the snapshot.", Required = false)] public string GameObjectPath { get; set; }
        [McpDescription("Maximum sync objects to return.", Required = false)] public int MaxObjects { get; set; } = 200;
        public VE2RuntimeSyncSnapshotParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2RuntimeSyncSnapshotParams>(this);
    }
}
