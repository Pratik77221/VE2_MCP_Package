using MCPForUnity.Editor.Tools;
using Newtonsoft.Json.Linq;

namespace Imperial.VE2.MCP.Editor.Adapters.Coplay
{
    [McpForUnityTool(VE2ToolCatalog.ContextSearchApi.Name, Description = VE2ToolCatalog.ContextSearchApi.Description, Group = "core")]
    public static class VE2CoplayContextSearchApiTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Text or API symbol to search for in installed VE2 public source.")] public string Query { get; set; }
            [ToolParameter("Maximum matching source locations.", Required = false)] public int MaxResults { get; set; } = 20;
            [ToolParameter("Context lines before and after each match.", Required = false)] public int ContextLines { get; set; } = 2;
        }
        public static object HandleCommand(JObject value) => VE2McpTools.SearchApi(CoplayParameterParser.Parse<VE2ContextSearchParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.ContextGetInterface.Name, Description = VE2ToolCatalog.ContextGetInterface.Description, Group = "core")]
    public static class VE2CoplayContextGetInterfaceTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Exact or short VE2 public interface/API type name.")] public string TypeName { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.GetInterfaceSource(CoplayParameterParser.Parse<VE2ContextGetInterfaceParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.ContextGetPrefabContract.Name, Description = VE2ToolCatalog.ContextGetPrefabContract.Description, Group = "core")]
    public static class VE2CoplayContextGetPrefabContractTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Exact installed VE2 Resources prefab name.")] public string ResourceName { get; set; }
            [ToolParameter("Maximum prefab hierarchy depth.", Required = false)] public int MaxDepth { get; set; } = 8;
        }
        public static object HandleCommand(JObject value) => VE2McpTools.GetPrefabContract(CoplayParameterParser.Parse<VE2PrefabContractParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.SceneGetManifest.Name, Description = VE2ToolCatalog.SceneGetManifest.Description, Group = "core")]
    public static class VE2CoplaySceneGetManifestTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Maximum scene hierarchy depth.", Required = false)] public int MaxDepth { get; set; } = 8;
            [ToolParameter("Include inactive GameObjects and components.", Required = false)] public bool IncludeInactive { get; set; } = true;
        }
        public static object HandleCommand(JObject value) => VE2McpTools.GetSceneManifest(CoplayParameterParser.Parse<VE2SceneManifestParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.ScriptScaffoldInteraction.Name, Description = VE2ToolCatalog.ScriptScaffoldInteraction.Description, Group = "core")]
    public static class VE2CoplayScriptScaffoldInteractionTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Valid C# class name.")] public string ClassName { get; set; }
            [ToolParameter("Optional C# namespace.", Required = false)] public string Namespace { get; set; }
            [ToolParameter("Interaction kind: toggle_activatable, info_point, handheld_activatable, hold_activatable, pressure_plate, free_grabbable, sliding_adjustable, rotating_adjustable, handheld_adjustable, sliding_2d_adjustable, or rotating_2d_adjustable.")] public string InteractionKind { get; set; }
            [ToolParameter("Project-relative output folder.", Required = false)] public string OutputFolder { get; set; } = "Assets/Scripts";
            [ToolParameter("Allow replacement of an existing generated file.", Required = false)] public bool Overwrite { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.ScaffoldInteractionScript(CoplayParameterParser.Parse<VE2ScaffoldInteractionParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.ScriptScaffoldNetworkObject.Name, Description = VE2ToolCatalog.ScriptScaffoldNetworkObject.Description, Group = "core")]
    public static class VE2CoplayScriptScaffoldNetworkObjectTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Valid C# class name.")] public string ClassName { get; set; }
            [ToolParameter("Optional C# namespace.", Required = false)] public string Namespace { get; set; }
            [ToolParameter("Project-relative output folder.", Required = false)] public string OutputFolder { get; set; } = "Assets/Scripts";
            [ToolParameter("Allow replacement of an existing generated file.", Required = false)] public bool Overwrite { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.ScaffoldNetworkObjectScript(CoplayParameterParser.Parse<VE2ScaffoldNetworkObjectParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.ScriptValidatePlugin.Name, Description = VE2ToolCatalog.ScriptValidatePlugin.Description, Group = "core")]
    public static class VE2CoplayScriptValidatePluginTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Project-relative script root to validate.", Required = false)] public string RootPath { get; set; } = "Assets/Scripts";
        }
        public static object HandleCommand(JObject value) => VE2McpTools.ValidatePluginScripts(CoplayParameterParser.Parse<VE2ValidatePluginScriptsParams>(value));
    }
}
