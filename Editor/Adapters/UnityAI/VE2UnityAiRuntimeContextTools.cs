using Unity.AI.MCP.Editor.ToolRegistry;

namespace Imperial.VE2.MCP.Editor.Adapters.UnityAI
{
    public class VE2UnityAiRuntimeInspectPlayerTool
    {
        [McpTool(VE2ToolCatalog.RuntimeInspectPlayer.Name, VE2ToolCatalog.RuntimeInspectPlayer.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand() => VE2McpTools.InspectPlayer();
    }

    public class VE2UnityAiRuntimeMovePlayerTool
    {
        [McpTool(VE2ToolCatalog.RuntimeMovePlayer.Name, VE2ToolCatalog.RuntimeMovePlayer.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiMovePlayerParams parameters) => VE2McpTools.MovePlayer(parameters?.ToCore());
    }

    public class VE2UnityAiRuntimeInspectClientsTool
    {
        [McpTool(VE2ToolCatalog.RuntimeInspectClients.Name, VE2ToolCatalog.RuntimeInspectClients.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand() => VE2McpTools.InspectClients();
    }

    public class VE2UnityAiRuntimeInspectInteractionsTool
    {
        [McpTool(VE2ToolCatalog.RuntimeInspectInteractions.Name, VE2ToolCatalog.RuntimeInspectInteractions.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiRuntimeObjectParams parameters) => VE2McpTools.InspectRuntimeInteractions(parameters?.ToCore());
    }

    public class VE2UnityAiRuntimeSetActivatableTool
    {
        [McpTool(VE2ToolCatalog.RuntimeSetActivatable.Name, VE2ToolCatalog.RuntimeSetActivatable.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiRuntimeSetActivatableParams parameters) => VE2McpTools.SetRuntimeActivatable(parameters?.ToCore());
    }

    public class VE2UnityAiRuntimeSetAdjustableTool
    {
        [McpTool(VE2ToolCatalog.RuntimeSetAdjustable.Name, VE2ToolCatalog.RuntimeSetAdjustable.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiRuntimeSetAdjustableParams parameters) => VE2McpTools.SetRuntimeAdjustable(parameters?.ToCore());
    }

    public class VE2UnityAiRuntimeSpawnNetworkObjectTool
    {
        [McpTool(VE2ToolCatalog.RuntimeSpawnNetworkObject.Name, VE2ToolCatalog.RuntimeSpawnNetworkObject.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiRuntimeSpawnParams parameters) => VE2McpTools.SpawnRuntimeNetworkObject(parameters?.ToCore());
    }

    public class VE2UnityAiRuntimeDespawnNetworkObjectTool
    {
        [McpTool(VE2ToolCatalog.RuntimeDespawnNetworkObject.Name, VE2ToolCatalog.RuntimeDespawnNetworkObject.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiRuntimeDespawnParams parameters) => VE2McpTools.DespawnRuntimeNetworkObject(parameters?.ToCore());
    }

    public class VE2UnityAiRuntimeSyncSnapshotTool
    {
        [McpTool(VE2ToolCatalog.RuntimeSyncSnapshot.Name, VE2ToolCatalog.RuntimeSyncSnapshot.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiRuntimeSyncSnapshotParams parameters) => VE2McpTools.CaptureSyncSnapshot(parameters?.ToCore());
    }

    public class VE2UnityAiRuntimeMultiplayerSmokeTestTool
    {
        [McpTool(VE2ToolCatalog.RuntimeMultiplayerSmokeTest.Name, VE2ToolCatalog.RuntimeMultiplayerSmokeTest.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand() => VE2McpTools.RunMultiplayerSmokeTest();
    }

    public class VE2UnityAiContextSearchApiTool
    {
        [McpTool(VE2ToolCatalog.ContextSearchApi.Name, VE2ToolCatalog.ContextSearchApi.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiContextSearchParams parameters) => VE2McpTools.SearchApi(parameters?.ToCore());
    }

    public class VE2UnityAiContextGetInterfaceTool
    {
        [McpTool(VE2ToolCatalog.ContextGetInterface.Name, VE2ToolCatalog.ContextGetInterface.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiContextGetInterfaceParams parameters) => VE2McpTools.GetInterfaceSource(parameters?.ToCore());
    }

    public class VE2UnityAiContextGetPrefabContractTool
    {
        [McpTool(VE2ToolCatalog.ContextGetPrefabContract.Name, VE2ToolCatalog.ContextGetPrefabContract.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiPrefabContractParams parameters) => VE2McpTools.GetPrefabContract(parameters?.ToCore());
    }

    public class VE2UnityAiSceneGetManifestTool
    {
        [McpTool(VE2ToolCatalog.SceneGetManifest.Name, VE2ToolCatalog.SceneGetManifest.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiSceneManifestParams parameters) => VE2McpTools.GetSceneManifest(parameters?.ToCore());
    }

    public class VE2UnityAiScriptScaffoldInteractionTool
    {
        [McpTool(VE2ToolCatalog.ScriptScaffoldInteraction.Name, VE2ToolCatalog.ScriptScaffoldInteraction.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiScaffoldInteractionParams parameters) => VE2McpTools.ScaffoldInteractionScript(parameters?.ToCore());
    }

    public class VE2UnityAiScriptScaffoldNetworkObjectTool
    {
        [McpTool(VE2ToolCatalog.ScriptScaffoldNetworkObject.Name, VE2ToolCatalog.ScriptScaffoldNetworkObject.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiScaffoldNetworkObjectParams parameters) => VE2McpTools.ScaffoldNetworkObjectScript(parameters?.ToCore());
    }

    public class VE2UnityAiScriptValidatePluginTool
    {
        [McpTool(VE2ToolCatalog.ScriptValidatePlugin.Name, VE2ToolCatalog.ScriptValidatePlugin.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiValidatePluginScriptsParams parameters) => VE2McpTools.ValidatePluginScripts(parameters?.ToCore());
    }
}
