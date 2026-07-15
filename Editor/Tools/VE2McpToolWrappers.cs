using Unity.AI.MCP.Editor.ToolRegistry;

namespace Imperial.VE2.MCP.Editor
{
    public class VE2HealthCheckTool
    {
        private const string ToolName = "ve2_health_check";
        private const string Description = "Check whether VE2 is present in the current Unity project and return project/editor state.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.HealthCheck();
        }
    }

    public class VE2ProjectQuickSetupTool
    {
        private const string ToolName = "ve2_project_quick_setup";
        private const string Description = "Run selected VE2 project setup steps via VE2's own editor utilities. Layer/tag changes require explicit confirmation. For scene creation/setup, prefer ve2_scene_setup_quickstart.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand(VE2QuickSetupParams parameters)
        {
            return VE2McpTools.QuickSetup(parameters);
        }
    }

    public class VE2SceneCreateQuickstartTool
    {
        private const string ToolName = "ve2_scene_create_quickstart";
        private const string Description = "Create and open a default VE2QuickStart scene from VE2SetupSceneHolder without modal dialogs. For a named scene like Mcptest, use ve2_scene_setup_quickstart.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.CreateQuickStartScene();
        }
    }

    public class VE2SceneSetupQuickstartTool
    {
        private const string ToolName = "ve2_scene_setup_quickstart";
        private const string Description = "Primary scene setup tool for VE2. Create or reset a named scene from VE2's quickstart template VE2SetupSceneHolder, activate and enable the NetworkIntegration multiplayer stack, save, and run deployment preflight. Use this when the user asks to set up or scaffold a VE2 multiplayer scene/project.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand(VE2QuickStartSceneParams parameters)
        {
            return VE2McpTools.SetupQuickStartScene(parameters);
        }
    }

    public class VE2SceneSpawnPrefabTool
    {
        private const string ToolName = "ve2_scene_spawn_prefab";
        private const string Description = "Spawn a VE2 prefab by exact Resources name. Uses VE2's CommonUtils.InstantiateResource by reflection and preserves InfoPoint post-processing.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand(VE2SpawnPrefabParams parameters)
        {
            return VE2McpTools.SpawnPrefab(parameters);
        }
    }

    public class VE2SceneEnsureProviderTool
    {
        private const string ToolName = "ve2_scene_ensure_provider";
        private const string Description = "Repair/advanced tool only: ensure one VE2 provider exists or activate an existing inactive provider. Do not use this as primary scene setup; use ve2_scene_setup_quickstart for new or reset VE2 scenes.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand(VE2EnsureProviderParams parameters)
        {
            return VE2McpTools.EnsureProvider(parameters);
        }
    }

    public class VE2SceneEnsureMultiplayerStackTool
    {
        private const string ToolName = "ve2_scene_ensure_multiplayer_stack";
        private const string Description = "Repair/advanced tool only: activate or ensure V_PlatformIntegration and V_InstanceIntegration in an existing scene. For setting up a VE2 scene, use ve2_scene_setup_quickstart so the official quickstart template is applied.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.EnsureMultiplayerStack();
        }
    }

    public class VE2SceneValidateSyncIdsTool
    {
        private const string ToolName = "ve2_scene_validate_sync_ids";
        private const string Description = "Detect duplicate VE2 name-derived sync IDs such as Activatable-*, NetObj-*, TS-*, and RBS-*.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.ValidateSyncIds();
        }
    }

    public class VE2SceneFixNameClashesTool
    {
        private const string ToolName = "ve2_scene_fix_name_clashes";
        private const string Description = "Rename duplicate VE2 syncable GameObjects to prevent name-derived multiplayer sync ID clashes.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand(VE2FixNameClashesParams parameters)
        {
            return VE2McpTools.FixNameClashes(parameters);
        }
    }

    public class VE2SceneListPrefabsTool
    {
        private const string ToolName = "ve2_scene_list_prefabs";
        private const string Description = "List VE2 prefab Resources names discovered under VE2 FRAMEWORK Resources folders.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.ListPrefabs();
        }
    }

    public class VE2SceneConfigureSpawnManagerTool
    {
        private const string ToolName = "ve2_scene_configure_spawn_manager";
        private const string Description = "Configure V_GameObjectSpawnManager by setting its private serialized object-to-spawn and spawn-position fields.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand(VE2ConfigureSpawnManagerParams parameters)
        {
            return VE2McpTools.ConfigureSpawnManager(parameters);
        }
    }

    public class VE2SceneMakeGrabbableTool
    {
        private const string ToolName = "ve2_scene_make_grabbable";
        private const string Description = "Make a scene object VE2 grabbable using V_FreeGrabbable in an idempotent, VE2-aware way. Use this instead of generic Unity AddComponent for grab support; it treats existing grabbables such as the VE2 LaserPointer prefab as success, avoids duplicate components, checks colliders/rigidbody/sync, and reports warnings.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand(VE2MakeGrabbableParams parameters)
        {
            return VE2McpTools.MakeGrabbable(parameters);
        }
    }

    public class VE2SceneValidateInteractionsTool
    {
        private const string ToolName = "ve2_scene_validate_interactions";
        private const string Description = "Validate VE2 interaction objects in the active scene, including V_FreeGrabbable, V_LaserPointer, activatables, adjustables, colliders, rigidbodies, attach points, and network sync helpers. Use after spawning or editing interactive VE2 objects.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.ValidateInteractions();
        }
    }

    public class VE2ScenePrepareForDeploymentTool
    {
        private const string ToolName = "ve2_scene_prepare_for_deployment";
        private const string Description = "Prepare the active scene for VE2 deployment: optional alphanumeric scene rename, active platform integration, save, and preflight.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand(VE2PrepareForDeploymentParams parameters)
        {
            return VE2McpTools.PrepareForDeployment(parameters);
        }
    }

    public class VE2ComponentConfigureSyncTool
    {
        private const string ToolName = "ve2_component_configure_sync";
        private const string Description = "Configure VE2 serialized sync settings on a component: IsNetworked, TransmissionFrequency, and TransmissionType.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand(VE2ConfigureSyncParams parameters)
        {
            return VE2McpTools.ConfigureSync(parameters);
        }
    }

    public class VE2RuntimeInspectInstanceTool
    {
        private const string ToolName = "ve2_runtime_inspect_instance";
        private const string Description = "Inspect VE2's custom DarkRift instance state through VE2API.InstanceService. Requires Play Mode.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.InspectInstance();
        }
    }

    public class VE2BuildPreflightPluginTool
    {
        private const string ToolName = "ve2_build_preflight_plugin";
        private const string Description = "Run safe VE2 plugin preflight checks: platform integration, scene naming, Assembly-CSharp scripts, and sync ID clashes.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.BuildPreflight();
        }
    }

    public class VE2GetContextInformationTool
    {
        private const string ToolName = "ve2_get_context_information";
        private const string Description = "Return actual VE2 source locations and selected public API source text for hallucination-free script generation.";

        [McpTool(ToolName, Description, Groups = new[] { "ve2", "virse" }, EnabledByDefault = true)]
        public static object HandleCommand()
        {
            return VE2McpTools.GetContextInformation();
        }
    }
}
