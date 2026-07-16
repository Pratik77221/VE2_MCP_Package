using MCPForUnity.Editor.Tools;
using Newtonsoft.Json.Linq;

namespace Imperial.VE2.MCP.Editor.Adapters.Coplay
{
    [McpForUnityTool(VE2ToolCatalog.RuntimeInspectPlayer.Name, Description = VE2ToolCatalog.RuntimeInspectPlayer.Description, Group = "core")]
    public static class VE2CoplayRuntimeInspectPlayerTool
    {
        public static object HandleCommand(JObject _) => VE2McpTools.InspectPlayer();
    }

    [McpForUnityTool(VE2ToolCatalog.RuntimeMovePlayer.Name, Description = VE2ToolCatalog.RuntimeMovePlayer.Description, Group = "core")]
    public static class VE2CoplayRuntimeMovePlayerTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Optional player world position [x,y,z].", Required = false)] public float[] Position { get; set; }
            [ToolParameter("Optional player world Euler rotation [x,y,z].", Required = false)] public float[] RotationEuler { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.MovePlayer(CoplayParameterParser.Parse<VE2MovePlayerParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.RuntimeInspectClients.Name, Description = VE2ToolCatalog.RuntimeInspectClients.Description, Group = "core")]
    public static class VE2CoplayRuntimeInspectClientsTool
    {
        public static object HandleCommand(JObject _) => VE2McpTools.InspectClients();
    }

    [McpForUnityTool(VE2ToolCatalog.RuntimeInspectInteractions.Name, Description = VE2ToolCatalog.RuntimeInspectInteractions.Description, Group = "core")]
    public static class VE2CoplayRuntimeInspectInteractionsTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Optional GameObject path to limit inspection to one hierarchy.", Required = false)] public string GameObjectPath { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.InspectRuntimeInteractions(CoplayParameterParser.Parse<VE2RuntimeObjectParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.RuntimeSetActivatable.Name, Description = VE2ToolCatalog.RuntimeSetActivatable.Description, Group = "core")]
    public static class VE2CoplayRuntimeSetActivatableTool
    {
        public sealed class Parameters
        {
            [ToolParameter("GameObject path or name containing the VE2 activatable.")] public string GameObjectPath { get; set; }
            [ToolParameter("True to activate; false to deactivate.")] public bool Activated { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.SetRuntimeActivatable(CoplayParameterParser.Parse<VE2RuntimeSetActivatableParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.RuntimeSetAdjustable.Name, Description = VE2ToolCatalog.RuntimeSetAdjustable.Description, Group = "core")]
    public static class VE2CoplayRuntimeSetAdjustableTool
    {
        public sealed class Parameters
        {
            [ToolParameter("GameObject path or name containing the VE2 adjustable.")] public string GameObjectPath { get; set; }
            [ToolParameter("One- or two-dimensional adjustable value.", Required = false)] public float[] Value { get; set; }
            [ToolParameter("Reset to the configured starting value instead of using Value.", Required = false)] public bool ResetToStartingValue { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.SetRuntimeAdjustable(CoplayParameterParser.Parse<VE2RuntimeSetAdjustableParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.RuntimeSpawnNetworkObject.Name, Description = VE2ToolCatalog.RuntimeSpawnNetworkObject.Description, Group = "core")]
    public static class VE2CoplayRuntimeSpawnNetworkObjectTool
    {
        public sealed class Parameters
        {
            [ToolParameter("GameObject path or name containing IV_GameObjectSpawnManager.")] public string SpawnManagerPath { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.SpawnRuntimeNetworkObject(CoplayParameterParser.Parse<VE2RuntimeSpawnParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.RuntimeDespawnNetworkObject.Name, Description = VE2ToolCatalog.RuntimeDespawnNetworkObject.Description, Group = "core")]
    public static class VE2CoplayRuntimeDespawnNetworkObjectTool
    {
        public sealed class Parameters
        {
            [ToolParameter("GameObject path or name containing IV_GameObjectSpawnManager.")] public string SpawnManagerPath { get; set; }
            [ToolParameter("Spawned target GameObject path or name.")] public string TargetGameObjectPath { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.DespawnRuntimeNetworkObject(CoplayParameterParser.Parse<VE2RuntimeDespawnParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.RuntimeSyncSnapshot.Name, Description = VE2ToolCatalog.RuntimeSyncSnapshot.Description, Group = "core")]
    public static class VE2CoplayRuntimeSyncSnapshotTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Optional GameObject path to limit the snapshot.", Required = false)] public string GameObjectPath { get; set; }
            [ToolParameter("Maximum sync objects to return.", Required = false)] public int MaxObjects { get; set; } = 200;
        }
        public static object HandleCommand(JObject value) => VE2McpTools.CaptureSyncSnapshot(CoplayParameterParser.Parse<VE2RuntimeSyncSnapshotParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.RuntimeMultiplayerSmokeTest.Name, Description = VE2ToolCatalog.RuntimeMultiplayerSmokeTest.Description, Group = "core")]
    public static class VE2CoplayRuntimeMultiplayerSmokeTestTool
    {
        public static object HandleCommand(JObject _) => VE2McpTools.RunMultiplayerSmokeTest();
    }
}
