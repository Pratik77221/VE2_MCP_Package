using Unity.AI.MCP.Editor.ToolRegistry;

namespace Imperial.VE2.MCP.Editor.Adapters.UnityAI
{
    public class VE2UnityAiBuildExportPluginTool
    {
        [McpTool(VE2ToolCatalog.BuildExportPlugin.Name, VE2ToolCatalog.BuildExportPlugin.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiBuildExportParams parameters) => VE2McpTools.ExportPlugin(parameters?.ToCore());
    }

    public class VE2UnityAiBuildGetStatusTool
    {
        [McpTool(VE2ToolCatalog.BuildGetStatus.Name, VE2ToolCatalog.BuildGetStatus.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand() => VE2McpTools.GetBuildStatus();
    }

    public class VE2UnityAiBuildGetVersionTool
    {
        [McpTool(VE2ToolCatalog.BuildGetVersion.Name, VE2ToolCatalog.BuildGetVersion.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiBuildPlatformParams parameters) => VE2McpTools.GetBuildVersion(parameters?.ToCore());
    }

    public class VE2UnityAiBuildRescanTool
    {
        [McpTool(VE2ToolCatalog.BuildRescan.Name, VE2ToolCatalog.BuildRescan.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiBuildPlatformParams parameters) => VE2McpTools.RescanBuildVersions(parameters?.ToCore());
    }

    public class VE2UnityAiDeploymentUploadTool
    {
        [McpTool(VE2ToolCatalog.DeploymentUpload.Name, VE2ToolCatalog.DeploymentUpload.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand(UnityAiDeploymentUploadParams parameters) => VE2McpTools.UploadDeployment(parameters?.ToCore());
    }

    public class VE2UnityAiDeploymentCancelTool
    {
        [McpTool(VE2ToolCatalog.DeploymentCancel.Name, VE2ToolCatalog.DeploymentCancel.Description, Groups = new[] { VE2ToolCatalog.Group, VE2ToolCatalog.LegacyGroup }, EnabledByDefault = true)]
        public static object HandleCommand() => VE2McpTools.CancelDeployment();
    }
}
