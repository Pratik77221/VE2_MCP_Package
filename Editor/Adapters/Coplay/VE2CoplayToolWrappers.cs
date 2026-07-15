using MCPForUnity.Editor.Tools;
using Newtonsoft.Json.Linq;

namespace Imperial.VE2.MCP.Editor.Adapters.Coplay
{
    internal static class CoplayParameterParser
    {
        public static T Parse<T>(JObject value) where T : class, new()
        {
            return value?.ToObject<T>() ?? new T();
        }
    }

    [McpForUnityTool(VE2ToolCatalog.HealthCheck.Name, Description = VE2ToolCatalog.HealthCheck.Description, Group = "core")]
    public static class VE2CoplayHealthCheckTool
    {
        public static object HandleCommand(JObject _)
        {
            return VE2McpTools.HealthCheck();
        }
    }

    [McpForUnityTool(VE2ToolCatalog.ProjectQuickSetup.Name, Description = VE2ToolCatalog.ProjectQuickSetup.Description, Group = "core")]
    public static class VE2CoplayProjectQuickSetupTool
    {
        public sealed class Parameters : CoplayQuickSetupParams { }

        public static object HandleCommand(JObject commandParams)
        {
            return VE2McpTools.QuickSetup(CoplayParameterParser.Parse<Parameters>(commandParams).ToCore());
        }
    }

    [McpForUnityTool(VE2ToolCatalog.SceneCreateQuickstart.Name, Description = VE2ToolCatalog.SceneCreateQuickstart.Description, Group = "core")]
    public static class VE2CoplaySceneCreateQuickstartTool
    {
        public static object HandleCommand(JObject _)
        {
            return VE2McpTools.CreateQuickStartScene();
        }
    }

    [McpForUnityTool(VE2ToolCatalog.SceneSetupQuickstart.Name, Description = VE2ToolCatalog.SceneSetupQuickstart.Description, Group = "core")]
    public static class VE2CoplaySceneSetupQuickstartTool
    {
        public sealed class Parameters : CoplayQuickStartSceneParams { }

        public static object HandleCommand(JObject commandParams)
        {
            return VE2McpTools.SetupQuickStartScene(CoplayParameterParser.Parse<Parameters>(commandParams).ToCore());
        }
    }

    [McpForUnityTool(VE2ToolCatalog.SceneSpawnPrefab.Name, Description = VE2ToolCatalog.SceneSpawnPrefab.Description, Group = "core")]
    public static class VE2CoplaySceneSpawnPrefabTool
    {
        public sealed class Parameters : CoplaySpawnPrefabParams { }

        public static object HandleCommand(JObject commandParams)
        {
            return VE2McpTools.SpawnPrefab(CoplayParameterParser.Parse<Parameters>(commandParams).ToCore());
        }
    }

    [McpForUnityTool(VE2ToolCatalog.SceneEnsureProvider.Name, Description = VE2ToolCatalog.SceneEnsureProvider.Description, Group = "core")]
    public static class VE2CoplaySceneEnsureProviderTool
    {
        public sealed class Parameters : CoplayEnsureProviderParams { }

        public static object HandleCommand(JObject commandParams)
        {
            return VE2McpTools.EnsureProvider(CoplayParameterParser.Parse<Parameters>(commandParams).ToCore());
        }
    }

    [McpForUnityTool(VE2ToolCatalog.SceneEnsureMultiplayerStack.Name, Description = VE2ToolCatalog.SceneEnsureMultiplayerStack.Description, Group = "core")]
    public static class VE2CoplaySceneEnsureMultiplayerStackTool
    {
        public static object HandleCommand(JObject _)
        {
            return VE2McpTools.EnsureMultiplayerStack();
        }
    }

    [McpForUnityTool(VE2ToolCatalog.SceneValidateSyncIds.Name, Description = VE2ToolCatalog.SceneValidateSyncIds.Description, Group = "core")]
    public static class VE2CoplaySceneValidateSyncIdsTool
    {
        public static object HandleCommand(JObject _)
        {
            return VE2McpTools.ValidateSyncIds();
        }
    }

    [McpForUnityTool(VE2ToolCatalog.SceneFixNameClashes.Name, Description = VE2ToolCatalog.SceneFixNameClashes.Description, Group = "core")]
    public static class VE2CoplaySceneFixNameClashesTool
    {
        public sealed class Parameters : CoplayFixNameClashesParams { }

        public static object HandleCommand(JObject commandParams)
        {
            return VE2McpTools.FixNameClashes(CoplayParameterParser.Parse<Parameters>(commandParams).ToCore());
        }
    }

    [McpForUnityTool(VE2ToolCatalog.SceneListPrefabs.Name, Description = VE2ToolCatalog.SceneListPrefabs.Description, Group = "core")]
    public static class VE2CoplaySceneListPrefabsTool
    {
        public static object HandleCommand(JObject _)
        {
            return VE2McpTools.ListPrefabs();
        }
    }

    [McpForUnityTool(VE2ToolCatalog.SceneConfigureSpawnManager.Name, Description = VE2ToolCatalog.SceneConfigureSpawnManager.Description, Group = "core")]
    public static class VE2CoplaySceneConfigureSpawnManagerTool
    {
        public sealed class Parameters : CoplayConfigureSpawnManagerParams { }

        public static object HandleCommand(JObject commandParams)
        {
            return VE2McpTools.ConfigureSpawnManager(CoplayParameterParser.Parse<Parameters>(commandParams).ToCore());
        }
    }

    [McpForUnityTool(VE2ToolCatalog.SceneMakeGrabbable.Name, Description = VE2ToolCatalog.SceneMakeGrabbable.Description, Group = "core")]
    public static class VE2CoplaySceneMakeGrabbableTool
    {
        public sealed class Parameters : CoplayMakeGrabbableParams { }

        public static object HandleCommand(JObject commandParams)
        {
            return VE2McpTools.MakeGrabbable(CoplayParameterParser.Parse<Parameters>(commandParams).ToCore());
        }
    }

    [McpForUnityTool(VE2ToolCatalog.SceneValidateInteractions.Name, Description = VE2ToolCatalog.SceneValidateInteractions.Description, Group = "core")]
    public static class VE2CoplaySceneValidateInteractionsTool
    {
        public static object HandleCommand(JObject _)
        {
            return VE2McpTools.ValidateInteractions();
        }
    }

    [McpForUnityTool(VE2ToolCatalog.ScenePrepareForDeployment.Name, Description = VE2ToolCatalog.ScenePrepareForDeployment.Description, Group = "core")]
    public static class VE2CoplayScenePrepareForDeploymentTool
    {
        public sealed class Parameters : CoplayPrepareForDeploymentParams { }

        public static object HandleCommand(JObject commandParams)
        {
            return VE2McpTools.PrepareForDeployment(CoplayParameterParser.Parse<Parameters>(commandParams).ToCore());
        }
    }

    [McpForUnityTool(VE2ToolCatalog.ComponentConfigureSync.Name, Description = VE2ToolCatalog.ComponentConfigureSync.Description, Group = "core")]
    public static class VE2CoplayComponentConfigureSyncTool
    {
        public sealed class Parameters : CoplayConfigureSyncParams { }

        public static object HandleCommand(JObject commandParams)
        {
            return VE2McpTools.ConfigureSync(CoplayParameterParser.Parse<Parameters>(commandParams).ToCore());
        }
    }

    [McpForUnityTool(VE2ToolCatalog.RuntimeInspectInstance.Name, Description = VE2ToolCatalog.RuntimeInspectInstance.Description, Group = "core")]
    public static class VE2CoplayRuntimeInspectInstanceTool
    {
        public static object HandleCommand(JObject _)
        {
            return VE2McpTools.InspectInstance();
        }
    }

    [McpForUnityTool(VE2ToolCatalog.BuildPreflightPlugin.Name, Description = VE2ToolCatalog.BuildPreflightPlugin.Description, Group = "core")]
    public static class VE2CoplayBuildPreflightPluginTool
    {
        public static object HandleCommand(JObject _)
        {
            return VE2McpTools.BuildPreflight();
        }
    }

    [McpForUnityTool(VE2ToolCatalog.GetContextInformation.Name, Description = VE2ToolCatalog.GetContextInformation.Description, Group = "core")]
    public static class VE2CoplayGetContextInformationTool
    {
        public static object HandleCommand(JObject _)
        {
            return VE2McpTools.GetContextInformation();
        }
    }
}
