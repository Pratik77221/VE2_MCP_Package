using MCPForUnity.Editor.Resources;
using Newtonsoft.Json.Linq;

namespace Imperial.VE2.MCP.Editor.Adapters.Coplay
{
    [McpForUnityResource("ve2_plugin_interfaces", Description = "Read the installed VE2API, PluginInterfaces, and instancing public source used for grounded VE2 script generation.")]
    public static class VE2CoplayPluginInterfacesResource
    {
        public static object HandleCommand(JObject _) => VE2McpTools.GetContextInformation();
    }

    [McpForUnityResource("ve2_scene_manifest", Description = "Read a bounded VE2-aware manifest of the active Unity scene.")]
    public static class VE2CoplaySceneManifestResource
    {
        public static object HandleCommand(JObject _) => VE2McpTools.GetSceneManifest(new VE2SceneManifestParams());
    }

    [McpForUnityResource("ve2_prefab_catalog", Description = "Read the installed VE2 Resources prefab catalog.")]
    public static class VE2CoplayPrefabCatalogResource
    {
        public static object HandleCommand(JObject _) => VE2McpTools.ListPrefabs();
    }
}
