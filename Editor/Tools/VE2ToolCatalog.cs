namespace Imperial.VE2.MCP.Editor
{
    public static class VE2ToolCatalog
    {
        public const string Group = "ve2";
        public const string LegacyGroup = "virse";

        public static class HealthCheck
        {
            public const string Name = "ve2_health_check";
            public const string Description = "Check whether VE2 is present in the current Unity project and return project/editor state.";
        }

        public static class ProjectQuickSetup
        {
            public const string Name = "ve2_project_quick_setup";
            public const string Description = "Run selected VE2 project setup steps via VE2's own editor utilities. Layer/tag changes require explicit confirmation. For scene creation/setup, prefer ve2_scene_setup_quickstart.";
        }

        public static class SceneCreateQuickstart
        {
            public const string Name = "ve2_scene_create_quickstart";
            public const string Description = "Create and open a default VE2QuickStart scene from VE2SetupSceneHolder without modal dialogs. For a named scene, use ve2_scene_setup_quickstart.";
        }

        public static class SceneSetupQuickstart
        {
            public const string Name = "ve2_scene_setup_quickstart";
            public const string Description = "Primary VE2 scene setup tool. Create or reset a named scene from VE2SetupSceneHolder, activate the NetworkIntegration multiplayer stack, save, and run deployment preflight. Use this when asked to scaffold a VE2 multiplayer scene or project.";
        }

        public static class SceneSpawnPrefab
        {
            public const string Name = "ve2_scene_spawn_prefab";
            public const string Description = "Spawn a VE2 prefab by exact Resources name through VE2 CommonUtils.InstantiateResource and preserve VE2-specific post-processing.";
        }

        public static class SceneEnsureProvider
        {
            public const string Name = "ve2_scene_ensure_provider";
            public const string Description = "Repair an existing scene by ensuring one requested VE2 provider exists or activating an inactive provider. Use ve2_scene_setup_quickstart for primary scene setup.";
        }

        public static class SceneEnsureMultiplayerStack
        {
            public const string Name = "ve2_scene_ensure_multiplayer_stack";
            public const string Description = "Repair an existing scene by activating or ensuring V_PlatformIntegration and V_InstanceIntegration. Use ve2_scene_setup_quickstart for primary scene setup.";
        }

        public static class SceneValidateSyncIds
        {
            public const string Name = "ve2_scene_validate_sync_ids";
            public const string Description = "Detect duplicate VE2 name-derived sync IDs such as Activatable-*, NetObj-*, TS-*, and RBS-*.";
        }

        public static class SceneFixNameClashes
        {
            public const string Name = "ve2_scene_fix_name_clashes";
            public const string Description = "Rename duplicate VE2 syncable GameObjects to prevent name-derived multiplayer sync ID clashes.";
        }

        public static class SceneListPrefabs
        {
            public const string Name = "ve2_scene_list_prefabs";
            public const string Description = "List VE2 prefab Resources names discovered under VE2 FRAMEWORK Resources folders.";
        }

        public static class SceneConfigureSpawnManager
        {
            public const string Name = "ve2_scene_configure_spawn_manager";
            public const string Description = "Configure V_GameObjectSpawnManager serialized object-to-spawn and spawn-position references.";
        }

        public static class SceneMakeGrabbable
        {
            public const string Name = "ve2_scene_make_grabbable";
            public const string Description = "Make a scene object VE2 grabbable with V_FreeGrabbable in an idempotent VE2-aware way. Avoids duplicate interaction components and validates colliders, Rigidbody, and multiplayer sync.";
        }

        public static class SceneValidateInteractions
        {
            public const string Name = "ve2_scene_validate_interactions";
            public const string Description = "Validate VE2 interaction objects including grabbables, laser pointers, activatables, adjustables, colliders, rigidbodies, attach points, and network sync helpers.";
        }

        public static class ScenePrepareForDeployment
        {
            public const string Name = "ve2_scene_prepare_for_deployment";
            public const string Description = "Prepare the active scene for VE2 deployment with an optional alphanumeric rename, active platform integration, save, and deployment preflight.";
        }

        public static class ComponentConfigureSync
        {
            public const string Name = "ve2_component_configure_sync";
            public const string Description = "Configure VE2 serialized sync settings on a component: IsNetworked, TransmissionFrequency, and TransmissionType.";
        }

        public static class RuntimeInspectInstance
        {
            public const string Name = "ve2_runtime_inspect_instance";
            public const string Description = "Inspect VE2's custom DarkRift instance state through VE2API.InstanceService. Requires Play Mode.";
        }

        public static class BuildPreflightPlugin
        {
            public const string Name = "ve2_build_preflight_plugin";
            public const string Description = "Run VE2 deployment preflight for scene naming/save state, provider setup, compilation, script assemblies, interactions, and name-derived sync IDs.";
        }

        public static class GetContextInformation
        {
            public const string Name = "ve2_get_context_information";
            public const string Description = "Return actual VE2 source locations and selected public API source text for grounded VE2 script generation.";
        }
    }
}
