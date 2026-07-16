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

        public static class SceneInspectObject
        {
            public const string Name = "ve2_scene_inspect_object";
            public const string Description = "Inspect one scene GameObject, including VE2 components, dependencies, serialized settings, colliders, rigidbodies, interaction state, and name-derived sync IDs.";
        }

        public static class SceneCreateActivatable
        {
            public const string Name = "ve2_scene_create_activatable";
            public const string Description = "Create an official VE2 ToggleButton, HoldButton, or PressurePlate prefab through CommonUtils.InstantiateResource.";
        }

        public static class ComponentConfigureActivatable
        {
            public const string Name = "ve2_component_configure_activatable";
            public const string Description = "Configure a VE2 activatable's real serialized interaction, activation, grouping, range, and network settings.";
        }

        public static class SceneCreateAdjustable
        {
            public const string Name = "ve2_scene_create_adjustable";
            public const string Description = "Create an official VE2 wheel, lever, slider, or two-dimensional adjustable prefab through VE2's Resources path.";
        }

        public static class ComponentConfigureAdjustable
        {
            public const string Name = "ve2_component_configure_adjustable";
            public const string Description = "Configure a VE2 adjustable's starting value, output and spatial ranges, discrete values, interaction range, and general interaction settings.";
        }

        public static class SceneCreateInfoPoint
        {
            public const string Name = "ve2_scene_create_infopoint";
            public const string Description = "Create an official VE2 CustomInfoPoint, preserve its required trigger/canvas naming, and optionally set its title and content text.";
        }

        public static class SceneConnectInteractionEvent
        {
            public const string Name = "ve2_scene_connect_interaction_event";
            public const string Description = "Connect an allowlisted VE2 interaction UnityEvent to a compatible public target method using a persistent serialized listener.";
        }

        public static class SceneDuplicateInteractable
        {
            public const string Name = "ve2_scene_duplicate_interactable";
            public const string Description = "Duplicate a VE2 interaction hierarchy and repair names on duplicated syncable objects so generated multiplayer IDs remain unique.";
        }

        public static class SceneConfigurePlayer
        {
            public const string Name = "ve2_scene_configure_player";
            public const string Description = "Configure the existing VE2 V_PlayerSpawner through its real nested serialized player, movement, camera, layer, and transmission settings.";
        }

        public static class SceneValidatePlayer
        {
            public const string Name = "ve2_scene_validate_player";
            public const string Description = "Validate VE2 player-spawner count, active state, player modes, layer masks, camera clipping, and transmission settings.";
        }

        public static class SceneCreateTeleportAnchor
        {
            public const string Name = "ve2_scene_create_teleport_anchor";
            public const string Description = "Create VE2's official TeleportAnchor prefab and configure its supported range.";
        }

        public static class SceneValidateTeleportation
        {
            public const string Name = "ve2_scene_validate_teleportation";
            public const string Description = "Validate VE2 teleport anchors and the player spawner's traversable and collision layer configuration.";
        }

        public static class RuntimeInspectPlayer
        {
            public const string Name = "ve2_runtime_inspect_player";
            public const string Description = "Inspect the local VE2 player service, mode, tracking, transform, camera, spawn point, and control overrides. Requires Play Mode.";
        }

        public static class RuntimeMovePlayer
        {
            public const string Name = "ve2_runtime_move_player";
            public const string Description = "Move or rotate the local player through VE2API.Player's public service methods. Requires Play Mode.";
        }

        public static class SceneMakeNetworked
        {
            public const string Name = "ve2_scene_make_networked";
            public const string Description = "Idempotently add a VE2 transform, rigidbody, or custom network-object component and configure its VE2 sync settings.";
        }

        public static class RuntimeInspectClients
        {
            public const string Name = "ve2_runtime_inspect_clients";
            public const string Description = "Inspect VE2 InstanceService client IDs, local client, host, ping, and known remote positions. Requires Play Mode.";
        }

        public static class RuntimeInspectInteractions
        {
            public const string Name = "ve2_runtime_inspect_interactions";
            public const string Description = "Inspect live VE2 activatable, grabbable, and adjustable states and their most recent interacting clients. Requires Play Mode.";
        }

        public static class RuntimeSetActivatable
        {
            public const string Name = "ve2_runtime_set_activatable";
            public const string Description = "Activate or deactivate one VE2 activatable through its public plugin-facing API. Requires Play Mode.";
        }

        public static class RuntimeSetAdjustable
        {
            public const string Name = "ve2_runtime_set_adjustable";
            public const string Description = "Set or reset a one- or two-dimensional VE2 adjustable through its public plugin-facing API. Requires Play Mode.";
        }

        public static class RuntimeSpawnNetworkObject
        {
            public const string Name = "ve2_runtime_spawn_network_object";
            public const string Description = "Spawn an object through VE2 IV_GameObjectSpawnManager.SpawnAndReturnGameObject. Requires Play Mode.";
        }

        public static class RuntimeDespawnNetworkObject
        {
            public const string Name = "ve2_runtime_despawn_network_object";
            public const string Description = "Despawn a scene object through a VE2 IV_GameObjectSpawnManager. Requires Play Mode.";
        }

        public static class RuntimeSyncSnapshot
        {
            public const string Name = "ve2_runtime_sync_snapshot";
            public const string Description = "Capture a bounded snapshot of VE2 sync components, IDs, sync configuration, transforms, rigidbodies, and network-object data.";
        }

        public static class RuntimeMultiplayerSmokeTest
        {
            public const string Name = "ve2_runtime_multiplayer_smoke_test";
            public const string Description = "Run a non-destructive Play Mode smoke test over VE2 services, providers, clients, player state, interactions, and sync-ID integrity.";
        }

        public static class ContextSearchApi
        {
            public const string Name = "ve2_context_search_api";
            public const string Description = "Search only the installed VE2 public API and PluginInterfaces source and return bounded, source-grounded matches.";
        }

        public static class ContextGetInterface
        {
            public const string Name = "ve2_context_get_interface";
            public const string Description = "Return the exact installed source for one VE2 public interface or API type.";
        }

        public static class ContextGetPrefabContract
        {
            public const string Name = "ve2_context_get_prefab_contract";
            public const string Description = "Inspect an installed VE2 Resources prefab and return its hierarchy, component contract, physics dependencies, and sync components.";
        }

        public static class SceneGetManifest
        {
            public const string Name = "ve2_scene_get_manifest";
            public const string Description = "Return a bounded VE2-aware manifest of the active scene hierarchy, providers, interactions, networking components, and sync-ID validation.";
        }

        public static class ScriptScaffoldInteraction
        {
            public const string Name = "ve2_script_scaffold_interaction";
            public const string Description = "Create a compile-ready C# listener scaffold against a selected installed VE2 public interaction interface.";
        }

        public static class ScriptScaffoldNetworkObject
        {
            public const string Name = "ve2_script_scaffold_network_object";
            public const string Description = "Create a compile-ready C# scaffold that consumes VE2 IV_NetworkObject without referencing internal VE2 classes.";
        }

        public static class ScriptValidatePlugin
        {
            public const string Name = "ve2_script_validate_plugin";
            public const string Description = "Validate plugin scripts for VE2 internal API references, missing assembly definitions, compile state, and unsafe name-derived synchronization patterns.";
        }

        public static class BuildExportPlugin
        {
            public const string Name = "ve2_build_export_plugin";
            public const string Description = "Start VE2's own reflected plugin build workflow for an explicit platform and version after deployment preflight and confirmation.";
        }

        public static class BuildGetStatus
        {
            public const string Name = "ve2_build_get_status";
            public const string Description = "Inspect the reflected VE2 plugin builder and uploader state without starting or changing a build.";
        }

        public static class BuildGetVersion
        {
            public const string Name = "ve2_build_get_version";
            public const string Description = "Inspect locally exported VE2 plugin versions for the active scene and requested platform.";
        }

        public static class BuildRescan
        {
            public const string Name = "ve2_build_rescan";
            public const string Description = "Start VE2's own local and remote deployment-version scan for an explicit platform. This performs network I/O.";
        }

        public static class DeploymentUpload
        {
            public const string Name = "ve2_deployment_upload";
            public const string Description = "Upload a previously exported VE2 plugin through VE2's own uploader after an explicit confirmation and completed version scan.";
        }

        public static class DeploymentCancel
        {
            public const string Name = "ve2_deployment_cancel";
            public const string Description = "Cancel queued or in-progress tasks owned by the VE2 MCP deployment uploader.";
        }
    }
}
