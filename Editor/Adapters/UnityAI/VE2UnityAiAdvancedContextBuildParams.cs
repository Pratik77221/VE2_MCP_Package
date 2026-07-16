using Unity.AI.MCP.Editor.ToolRegistry;

namespace Imperial.VE2.MCP.Editor.Adapters.UnityAI
{
    public sealed class UnityAiContextSearchParams
    {
        [McpDescription("Text or API symbol to search for in installed VE2 public source.", Required = true)] public string Query { get; set; }
        [McpDescription("Maximum matching source locations.", Required = false)] public int MaxResults { get; set; } = 20;
        [McpDescription("Context lines before and after each match.", Required = false)] public int ContextLines { get; set; } = 2;
        public VE2ContextSearchParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2ContextSearchParams>(this);
    }

    public sealed class UnityAiContextGetInterfaceParams
    {
        [McpDescription("Exact or short VE2 public interface/API type name.", Required = true)] public string TypeName { get; set; }
        public VE2ContextGetInterfaceParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2ContextGetInterfaceParams>(this);
    }

    public sealed class UnityAiPrefabContractParams
    {
        [McpDescription("Exact installed VE2 Resources prefab name.", Required = true)] public string ResourceName { get; set; }
        [McpDescription("Maximum prefab hierarchy depth.", Required = false)] public int MaxDepth { get; set; } = 8;
        public VE2PrefabContractParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2PrefabContractParams>(this);
    }

    public sealed class UnityAiSceneManifestParams
    {
        [McpDescription("Maximum scene hierarchy depth.", Required = false)] public int MaxDepth { get; set; } = 8;
        [McpDescription("Include inactive GameObjects and components.", Required = false)] public bool IncludeInactive { get; set; } = true;
        public VE2SceneManifestParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2SceneManifestParams>(this);
    }

    public sealed class UnityAiScaffoldInteractionParams
    {
        [McpDescription("Valid C# class name.", Required = true)] public string ClassName { get; set; }
        [McpDescription("Optional C# namespace.", Required = false)] public string Namespace { get; set; }
        [McpDescription("Installed VE2 interaction interface template.", Required = true, EnumType = typeof(VE2InteractionScaffoldKind))] public string InteractionKind { get; set; }
        [McpDescription("Project-relative output folder.", Required = false)] public string OutputFolder { get; set; } = "Assets/Scripts";
        [McpDescription("Allow replacement of an existing generated file.", Required = false)] public bool Overwrite { get; set; }
        public VE2ScaffoldInteractionParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2ScaffoldInteractionParams>(this);
    }

    public sealed class UnityAiScaffoldNetworkObjectParams
    {
        [McpDescription("Valid C# class name.", Required = true)] public string ClassName { get; set; }
        [McpDescription("Optional C# namespace.", Required = false)] public string Namespace { get; set; }
        [McpDescription("Project-relative output folder.", Required = false)] public string OutputFolder { get; set; } = "Assets/Scripts";
        [McpDescription("Allow replacement of an existing generated file.", Required = false)] public bool Overwrite { get; set; }
        public VE2ScaffoldNetworkObjectParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2ScaffoldNetworkObjectParams>(this);
    }

    public sealed class UnityAiValidatePluginScriptsParams
    {
        [McpDescription("Project-relative script root to validate.", Required = false)] public string RootPath { get; set; } = "Assets/Scripts";
        public VE2ValidatePluginScriptsParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2ValidatePluginScriptsParams>(this);
    }

    public sealed class UnityAiBuildExportParams
    {
        [McpDescription("Build platform.", Required = true, EnumType = typeof(VE2BuildPlatformKind))] public string Platform { get; set; }
        [McpDescription("Explicit version 1-999, or zero to choose the next local version.", Required = false)] public int Version { get; set; }
        [McpDescription("Enable VE2's ECS/Burst build option.", Required = false)] public bool BuildWithEcsBurst { get; set; }
        [McpDescription("Must be true to confirm the build and its build-settings changes.", Required = true)] public bool ConfirmBuild { get; set; }
        public VE2BuildExportParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2BuildExportParams>(this);
    }

    public sealed class UnityAiBuildPlatformParams
    {
        [McpDescription("VE2 build/deployment platform.", Required = true, EnumType = typeof(VE2BuildPlatformKind))] public string Platform { get; set; }
        public VE2BuildPlatformParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2BuildPlatformParams>(this);
    }

    public sealed class UnityAiDeploymentUploadParams
    {
        [McpDescription("Deployment platform.", Required = true, EnumType = typeof(VE2BuildPlatformKind))] public string Platform { get; set; }
        [McpDescription("Must be true to confirm publication to VE2's remote service.", Required = true)] public bool ConfirmUpload { get; set; }
        public VE2DeploymentUploadParams ToCore() => UnityAiAdvancedParameterMapper.ToCore<VE2DeploymentUploadParams>(this);
    }
}
