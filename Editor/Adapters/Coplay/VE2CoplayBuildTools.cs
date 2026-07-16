using MCPForUnity.Editor.Tools;
using Newtonsoft.Json.Linq;

namespace Imperial.VE2.MCP.Editor.Adapters.Coplay
{
    [McpForUnityTool(VE2ToolCatalog.BuildExportPlugin.Name, Description = VE2ToolCatalog.BuildExportPlugin.Description, Group = "core")]
    public static class VE2CoplayBuildExportPluginTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Build platform: Windows or Android.")] public string Platform { get; set; }
            [ToolParameter("Explicit version 1-999, or zero to choose the next local version.", Required = false)] public int Version { get; set; }
            [ToolParameter("Enable VE2's ECS/Burst build option.", Required = false)] public bool BuildWithEcsBurst { get; set; }
            [ToolParameter("Must be true to confirm the build and its build-settings changes.")] public bool ConfirmBuild { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.ExportPlugin(CoplayParameterParser.Parse<VE2BuildExportParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.BuildGetStatus.Name, Description = VE2ToolCatalog.BuildGetStatus.Description, Group = "core")]
    public static class VE2CoplayBuildGetStatusTool
    {
        public static object HandleCommand(JObject _) => VE2McpTools.GetBuildStatus();
    }

    [McpForUnityTool(VE2ToolCatalog.BuildGetVersion.Name, Description = VE2ToolCatalog.BuildGetVersion.Description, Group = "core")]
    public static class VE2CoplayBuildGetVersionTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Build platform: Windows or Android.")] public string Platform { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.GetBuildVersion(CoplayParameterParser.Parse<VE2BuildPlatformParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.BuildRescan.Name, Description = VE2ToolCatalog.BuildRescan.Description, Group = "core")]
    public static class VE2CoplayBuildRescanTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Deployment platform to scan: Windows or Android.")] public string Platform { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.RescanBuildVersions(CoplayParameterParser.Parse<VE2BuildPlatformParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.DeploymentUpload.Name, Description = VE2ToolCatalog.DeploymentUpload.Description, Group = "core")]
    public static class VE2CoplayDeploymentUploadTool
    {
        public sealed class Parameters
        {
            [ToolParameter("Deployment platform: Windows or Android.")] public string Platform { get; set; }
            [ToolParameter("Must be true to confirm publication to VE2's remote service.")] public bool ConfirmUpload { get; set; }
        }
        public static object HandleCommand(JObject value) => VE2McpTools.UploadDeployment(CoplayParameterParser.Parse<VE2DeploymentUploadParams>(value));
    }

    [McpForUnityTool(VE2ToolCatalog.DeploymentCancel.Name, Description = VE2ToolCatalog.DeploymentCancel.Description, Group = "core")]
    public static class VE2CoplayDeploymentCancelTool
    {
        public static object HandleCommand(JObject _) => VE2McpTools.CancelDeployment();
    }
}
