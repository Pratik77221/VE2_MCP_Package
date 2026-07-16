using Unity.AI.MCP.Editor.ToolRegistry;

namespace Imperial.VE2.MCP.Editor.Adapters.UnityAI
{
    public class VE2UnityAiSceneInspectObjectTool
    {
        [McpTool(VE2ToolCatalog.SceneInspectObject.Name, VE2ToolCatalog.SceneInspectObject.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiInspectObjectParams parameters) => VE2McpTools.InspectObject(parameters?.ToCore());
    }

    public class VE2UnityAiSceneCreateActivatableTool
    {
        [McpTool(VE2ToolCatalog.SceneCreateActivatable.Name, VE2ToolCatalog.SceneCreateActivatable.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiCreateActivatableParams parameters) => VE2McpTools.CreateActivatable(parameters?.ToCore());
    }

    public class VE2UnityAiComponentConfigureActivatableTool
    {
        [McpTool(VE2ToolCatalog.ComponentConfigureActivatable.Name, VE2ToolCatalog.ComponentConfigureActivatable.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiConfigureActivatableParams parameters) => VE2McpTools.ConfigureActivatable(parameters?.ToCore());
    }

    public class VE2UnityAiSceneCreateAdjustableTool
    {
        [McpTool(VE2ToolCatalog.SceneCreateAdjustable.Name, VE2ToolCatalog.SceneCreateAdjustable.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiCreateAdjustableParams parameters) => VE2McpTools.CreateAdjustable(parameters?.ToCore());
    }

    public class VE2UnityAiComponentConfigureAdjustableTool
    {
        [McpTool(VE2ToolCatalog.ComponentConfigureAdjustable.Name, VE2ToolCatalog.ComponentConfigureAdjustable.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiConfigureAdjustableParams parameters) => VE2McpTools.ConfigureAdjustable(parameters?.ToCore());
    }

    public class VE2UnityAiSceneCreateInfoPointTool
    {
        [McpTool(VE2ToolCatalog.SceneCreateInfoPoint.Name, VE2ToolCatalog.SceneCreateInfoPoint.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiCreateInfoPointParams parameters) => VE2McpTools.CreateInfoPoint(parameters?.ToCore());
    }

    public class VE2UnityAiSceneConnectInteractionEventTool
    {
        [McpTool(VE2ToolCatalog.SceneConnectInteractionEvent.Name, VE2ToolCatalog.SceneConnectInteractionEvent.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiConnectInteractionEventParams parameters) => VE2McpTools.ConnectInteractionEvent(parameters?.ToCore());
    }

    public class VE2UnityAiSceneDuplicateInteractableTool
    {
        [McpTool(VE2ToolCatalog.SceneDuplicateInteractable.Name, VE2ToolCatalog.SceneDuplicateInteractable.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiDuplicateInteractableParams parameters) => VE2McpTools.DuplicateInteractable(parameters?.ToCore());
    }

    public class VE2UnityAiSceneConfigurePlayerTool
    {
        [McpTool(VE2ToolCatalog.SceneConfigurePlayer.Name, VE2ToolCatalog.SceneConfigurePlayer.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiConfigurePlayerParams parameters) => VE2McpTools.ConfigurePlayer(parameters?.ToCore());
    }

    public class VE2UnityAiSceneValidatePlayerTool
    {
        [McpTool(VE2ToolCatalog.SceneValidatePlayer.Name, VE2ToolCatalog.SceneValidatePlayer.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand() => VE2McpTools.ValidatePlayer();
    }

    public class VE2UnityAiSceneCreateTeleportAnchorTool
    {
        [McpTool(VE2ToolCatalog.SceneCreateTeleportAnchor.Name, VE2ToolCatalog.SceneCreateTeleportAnchor.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiCreateTeleportAnchorParams parameters) => VE2McpTools.CreateTeleportAnchor(parameters?.ToCore());
    }

    public class VE2UnityAiSceneValidateTeleportationTool
    {
        [McpTool(VE2ToolCatalog.SceneValidateTeleportation.Name, VE2ToolCatalog.SceneValidateTeleportation.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand() => VE2McpTools.ValidateTeleportation();
    }

    public class VE2UnityAiSceneMakeNetworkedTool
    {
        [McpTool(VE2ToolCatalog.SceneMakeNetworked.Name, VE2ToolCatalog.SceneMakeNetworked.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiMakeNetworkedParams parameters) => VE2McpTools.MakeNetworked(parameters?.ToCore());
    }
}
