using Unity.AI.MCP.Editor.ToolRegistry;

namespace Imperial.VE2.MCP.Editor.Adapters.UnityAI
{
    public class VE2UnityAiHealthCheckTool
    {
        [McpTool(VE2ToolCatalog.HealthCheck.Name, VE2ToolCatalog.HealthCheck.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.HealthCheck();
        }
    }

    public class VE2UnityAiProjectQuickSetupTool
    {
        [McpTool(VE2ToolCatalog.ProjectQuickSetup.Name, VE2ToolCatalog.ProjectQuickSetup.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiQuickSetupParams parameters)
        {
            return VE2McpTools.QuickSetup(parameters?.ToCore());
        }
    }

    public class VE2UnityAiSceneCreateQuickstartTool
    {
        [McpTool(VE2ToolCatalog.SceneCreateQuickstart.Name, VE2ToolCatalog.SceneCreateQuickstart.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.CreateQuickStartScene();
        }
    }

    public class VE2UnityAiSceneSetupQuickstartTool
    {
        [McpTool(VE2ToolCatalog.SceneSetupQuickstart.Name, VE2ToolCatalog.SceneSetupQuickstart.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiQuickStartSceneParams parameters)
        {
            return VE2McpTools.SetupQuickStartScene(parameters?.ToCore());
        }
    }

    public class VE2UnityAiSceneSpawnPrefabTool
    {
        [McpTool(VE2ToolCatalog.SceneSpawnPrefab.Name, VE2ToolCatalog.SceneSpawnPrefab.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiSpawnPrefabParams parameters)
        {
            return VE2McpTools.SpawnPrefab(parameters?.ToCore());
        }
    }

    public class VE2UnityAiSceneEnsureProviderTool
    {
        [McpTool(VE2ToolCatalog.SceneEnsureProvider.Name, VE2ToolCatalog.SceneEnsureProvider.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiEnsureProviderParams parameters)
        {
            return VE2McpTools.EnsureProvider(parameters?.ToCore());
        }
    }

    public class VE2UnityAiSceneEnsureMultiplayerStackTool
    {
        [McpTool(VE2ToolCatalog.SceneEnsureMultiplayerStack.Name, VE2ToolCatalog.SceneEnsureMultiplayerStack.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.EnsureMultiplayerStack();
        }
    }

    public class VE2UnityAiSceneValidateSyncIdsTool
    {
        [McpTool(VE2ToolCatalog.SceneValidateSyncIds.Name, VE2ToolCatalog.SceneValidateSyncIds.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.ValidateSyncIds();
        }
    }

    public class VE2UnityAiSceneFixNameClashesTool
    {
        [McpTool(VE2ToolCatalog.SceneFixNameClashes.Name, VE2ToolCatalog.SceneFixNameClashes.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiFixNameClashesParams parameters)
        {
            return VE2McpTools.FixNameClashes(parameters?.ToCore());
        }
    }

    public class VE2UnityAiSceneListPrefabsTool
    {
        [McpTool(VE2ToolCatalog.SceneListPrefabs.Name, VE2ToolCatalog.SceneListPrefabs.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.ListPrefabs();
        }
    }

    public class VE2UnityAiSceneConfigureSpawnManagerTool
    {
        [McpTool(VE2ToolCatalog.SceneConfigureSpawnManager.Name, VE2ToolCatalog.SceneConfigureSpawnManager.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiConfigureSpawnManagerParams parameters)
        {
            return VE2McpTools.ConfigureSpawnManager(parameters?.ToCore());
        }
    }

    public class VE2UnityAiSceneMakeGrabbableTool
    {
        [McpTool(VE2ToolCatalog.SceneMakeGrabbable.Name, VE2ToolCatalog.SceneMakeGrabbable.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiMakeGrabbableParams parameters)
        {
            return VE2McpTools.MakeGrabbable(parameters?.ToCore());
        }
    }

    public class VE2UnityAiSceneValidateInteractionsTool
    {
        [McpTool(VE2ToolCatalog.SceneValidateInteractions.Name, VE2ToolCatalog.SceneValidateInteractions.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.ValidateInteractions();
        }
    }

    public class VE2UnityAiScenePrepareForDeploymentTool
    {
        [McpTool(VE2ToolCatalog.ScenePrepareForDeployment.Name, VE2ToolCatalog.ScenePrepareForDeployment.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiPrepareForDeploymentParams parameters)
        {
            return VE2McpTools.PrepareForDeployment(parameters?.ToCore());
        }
    }

    public class VE2UnityAiComponentConfigureSyncTool
    {
        [McpTool(VE2ToolCatalog.ComponentConfigureSync.Name, VE2ToolCatalog.ComponentConfigureSync.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiConfigureSyncParams parameters)
        {
            return VE2McpTools.ConfigureSync(parameters?.ToCore());
        }
    }

    public class VE2UnityAiRuntimeInspectInstanceTool
    {
        [McpTool(VE2ToolCatalog.RuntimeInspectInstance.Name, VE2ToolCatalog.RuntimeInspectInstance.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.InspectInstance();
        }
    }

    public class VE2UnityAiBuildPreflightPluginTool
    {
        [McpTool(VE2ToolCatalog.BuildPreflightPlugin.Name, VE2ToolCatalog.BuildPreflightPlugin.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.BuildPreflight();
        }
    }

    public class VE2UnityAiGetContextInformationTool
    {
        [McpTool(VE2ToolCatalog.GetContextInformation.Name, VE2ToolCatalog.GetContextInformation.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.GetContextInformation();
        }
    }
}
