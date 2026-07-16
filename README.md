# ViRSE 2 MCP Package

This Unity package gives AI assistants VE2-specific tools for project setup, multiplayer scenes, prefab spawning, interactions, sync configuration, runtime inspection, and deployment checks.

It supports two MCP hosts from one package:

- **Coplay MCP for Unity**: free/open-source MCP host; no Unity AI license is required.
- **Unity AI MCP**: registers the same tools in Unity's AI/MCP tool registry.

The package contains one host-neutral VE2 implementation and optional adapters for each MCP host. Install either host, or install both. There is no VE2 Node server, HTTP listener, or command-line process to start.

## Requirements

- Unity `6000.0` or newer.
- VE2 installed as `com.ic.ve2`.
- At least one supported MCP host:
  - Coplay `com.coplaydev.unity-mcp` version `10.1.0` or newer, or
  - Unity AI Assistant `com.unity.ai.assistant` version `2.6.0-pre.1` or newer.

The VE2 MCP package has no hard dependency on either host. A project containing VE2 and this package still compiles when neither MCP host is installed, but no external MCP tools are exposed until a host is added.

## Install From GitHub

In Unity, open `Window > Package Manager`, click `+`, choose `Add package from git URL...`, and enter:

```text
https://github.com/Pratik77221/VE2_MCP_Package.git#main
```

The repository is already a UPM package at its root, so no `?path=` suffix is required.

You can also add it directly to `Packages/manifest.json`:

```json
"com.imperial.ve2.mcp": "https://github.com/Pratik77221/VE2_MCP_Package.git#main"
```

For a private GitHub repository, Unity must be able to use Git credentials that already have access to the repository.

## Option A: Use With Coplay MCP for Unity

This path does not require a Unity AI license.

1. Add Coplay MCP for Unity from its official Git URL:

```text
https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main
```

2. Add this VE2 MCP package using the GitHub URL above.
3. Wait for Unity to finish compiling.
4. Open `Window > MCP for Unity` and choose `Configure All Detected Clients`.
5. Reconnect or restart the AI chat after the first compilation so it refreshes its MCP tool list.

The VE2 tools register automatically through Coplay's custom-tool API in its default-visible tool group. No terminal command is needed to enable them.

Test with a normal prompt:

```text
Check that VE2 is available, then tell me the active scene and whether Unity is compiling.
```

Then try a real workflow:

```text
Create a VE2 multiplayer scene named Mcptest using the VE2 quickstart template, save it, and check whether it is ready for deployment.
```

The assistant should select `ve2_scene_setup_quickstart` and `ve2_build_preflight_plugin` itself. You do not need to type tool names as commands.

## Option B: Use With Unity AI MCP

1. Install Unity AI Assistant / Unity MCP as `com.unity.ai.assistant` through Unity's supported package flow.
2. Add this VE2 MCP package using the GitHub URL above.
3. Wait for Unity to finish compiling.
4. Open `Edit > Project Settings > AI/MCP` and confirm tools beginning with `ve2_` are enabled.
5. Reconnect or restart the external AI chat if it connected before Unity finished compiling.

The Unity AI license requirement belongs to the Unity AI MCP host. It is not a requirement of this VE2 package when the Coplay host is used.

## Using Both Hosts

Both adapters can be installed at the same time. Each host discovers the same 54 tool names and routes them into the same VE2 core logic. An AI client normally connects through one Unity MCP host for a session; avoid connecting both hosts under the same client-facing server name.

Unity AI tools appear under `Project Settings > AI/MCP`. Coplay tools are managed from `Window > MCP for Unity` and the connected MCP client.

## Recommended Natural-Language Workflow

For a new VE2 project or scene, ask the assistant:

```text
Run the VE2 project setup, create a multiplayer scene named TrainingRoom from the official VE2 quickstart template, save it, and run deployment preflight.
```

For interactive content:

```text
List the available VE2 prefabs, spawn a LaserPointer named LaserPointer_Grabbable, make it VE2 grabbable, and validate all interactions.
```

For networking:

```text
Configure LaserPointer_Grabbable for network sync at 10 Hz over UDP, then check the scene for VE2 sync ID clashes.
```

For VE2-native interaction authoring:

```text
Create a VE2 toggle button named StartRound, configure it as networked, connect its activation event to the public StartGame method on GameManager, and validate the interaction setup.
```

For Play Mode diagnosis:

```text
Inspect the VE2 instance, local player, connected clients, and live interaction states, then run a non-destructive multiplayer smoke test.
```

For grounded script generation:

```text
Read the installed VE2 interface for a toggle activatable, scaffold a listener named StartRoundListener in Assets/Scripts, and validate the plugin scripts.
```

For deployment:

```text
Prepare this scene for VE2 deployment, run preflight, build the next Windows plugin version, wait for the build to finish, and show me its status. Do not upload until I explicitly confirm.
```

The AI should use VE2-specific tools whenever one exists. In particular:

- Use `ve2_scene_setup_quickstart` for a new multiplayer scene.
- Use `ve2_scene_spawn_prefab` for VE2 Resources prefabs.
- Use `ve2_scene_make_grabbable` instead of generic `AddComponent` for VE2 grab support.
- Use `ve2_component_configure_sync` for VE2 serialized networking fields.
- Use the dedicated activatable, adjustable, InfoPoint, player, teleport, and network-object tools instead of generic component edits.
- Run `ve2_scene_validate_interactions` after interaction changes.
- Run `ve2_build_preflight_plugin` before deployment.

## Tool Reference

### Project And Scene Foundation

| Tool | Purpose |
| --- | --- |
| `ve2_health_check` | Check VE2 installation and current editor/scene state. |
| `ve2_project_quick_setup` | Run VE2's own project setup utilities. |
| `ve2_scene_create_quickstart` | Create the default VE2QuickStart scene. |
| `ve2_scene_setup_quickstart` | Create or reset a named scene from the official VE2 quickstart template. |
| `ve2_scene_spawn_prefab` | Spawn a VE2 Resources prefab through VE2's instantiation path. |
| `ve2_scene_list_prefabs` | List VE2 prefab Resources available in the project. |
| `ve2_scene_ensure_provider` | Repair or activate one VE2 provider in an existing scene. |
| `ve2_scene_ensure_multiplayer_stack` | Repair or activate the platform and instance integrations. |
| `ve2_scene_inspect_object` | Inspect a GameObject's VE2 components, settings, dependencies, and sync IDs. |
| `ve2_scene_validate_sync_ids` | Find duplicate VE2 name-derived multiplayer IDs. |
| `ve2_scene_fix_name_clashes` | Safely rename duplicate syncable objects. |
| `ve2_scene_prepare_for_deployment` | Rename, save, repair, and preflight the active scene. |

### Interaction, Player, And Network Authoring

| Tool | Purpose |
| --- | --- |
| `ve2_scene_configure_spawn_manager` | Configure a VE2 game-object spawn manager. |
| `ve2_scene_make_grabbable` | Add or validate VE2 grab support idempotently. |
| `ve2_scene_validate_interactions` | Validate VE2 interaction objects and dependencies. |
| `ve2_scene_create_activatable` | Create an official VE2 toggle, hold button, or pressure plate. |
| `ve2_component_configure_activatable` | Configure a VE2 activatable's real serialized settings. |
| `ve2_scene_create_adjustable` | Create an official VE2 wheel, lever, joystick, or slider. |
| `ve2_component_configure_adjustable` | Configure one- or two-dimensional adjustable ranges and behavior. |
| `ve2_scene_create_infopoint` | Create a CustomInfoPoint and preserve its required hierarchy contract. |
| `ve2_scene_connect_interaction_event` | Connect an allowlisted interaction event to a compatible public method. |
| `ve2_scene_duplicate_interactable` | Duplicate an interaction hierarchy and repair name-derived sync IDs. |
| `ve2_scene_configure_player` | Configure V_PlayerSpawner modes, layers, movement, camera, and sync. |
| `ve2_scene_validate_player` | Validate player-spawner count and serialized player configuration. |
| `ve2_scene_create_teleport_anchor` | Create and configure the official VE2 TeleportAnchor. |
| `ve2_scene_validate_teleportation` | Validate teleport anchors and player layer settings. |
| `ve2_component_configure_sync` | Configure VE2 serialized sync settings. |
| `ve2_scene_make_networked` | Add and configure VE2 transform, rigidbody, or custom-object networking. |

### Play Mode And Multiplayer Debugging

| Tool | Purpose |
| --- | --- |
| `ve2_runtime_inspect_instance` | Inspect VE2 DarkRift instance state in Play Mode. |
| `ve2_runtime_inspect_player` | Inspect VE2's local player service and current player state. |
| `ve2_runtime_move_player` | Move or rotate the local player through VE2's public player service. |
| `ve2_runtime_inspect_clients` | Inspect local, host, and remote VE2 client state. |
| `ve2_runtime_inspect_interactions` | Inspect live activatable, grabbable, and adjustable state. |
| `ve2_runtime_set_activatable` | Activate or deactivate a VE2 activatable through its public API. |
| `ve2_runtime_set_adjustable` | Set or reset a VE2 adjustable through its public API. |
| `ve2_runtime_spawn_network_object` | Spawn through IV_GameObjectSpawnManager. |
| `ve2_runtime_despawn_network_object` | Despawn through IV_GameObjectSpawnManager. |
| `ve2_runtime_sync_snapshot` | Capture bounded VE2 sync state for diagnosis. |
| `ve2_runtime_multiplayer_smoke_test` | Run non-destructive service, player, interaction, and sync checks. |

### Context And Script Generation

| Tool | Purpose |
| --- | --- |
| `ve2_get_context_information` | Return selected installed VE2 API source for grounded script generation. |
| `ve2_context_search_api` | Search only installed VE2 public APIs and PluginInterfaces. |
| `ve2_context_get_interface` | Return the exact installed source for one public VE2 API type. |
| `ve2_context_get_prefab_contract` | Inspect an installed VE2 prefab hierarchy and component contract. |
| `ve2_scene_get_manifest` | Return a bounded VE2-aware manifest of the active scene. |
| `ve2_script_scaffold_interaction` | Scaffold a listener against an installed public VE2 interaction interface. |
| `ve2_script_scaffold_network_object` | Scaffold code against IV_NetworkObject without internal references. |
| `ve2_script_validate_plugin` | Check scripts, asmdefs, compile state, internal references, and unsafe sync patterns. |

### Build And Deployment

| Tool | Purpose |
| --- | --- |
| `ve2_build_preflight_plugin` | Run VE2 deployment preflight checks. |
| `ve2_build_export_plugin` | Start VE2's own plugin builder after explicit confirmation. |
| `ve2_build_get_status` | Inspect current VE2 builder and uploader state. |
| `ve2_build_get_version` | List local build versions and the next available version. |
| `ve2_build_rescan` | Run VE2's own local/remote version scan. |
| `ve2_deployment_upload` | Upload the newest eligible build after explicit confirmation. |
| `ve2_deployment_cancel` | Cancel MCP-owned queued or in-progress upload tasks. |

Build and upload tools use VE2's installed builder/uploader through reflection. A build requires `ConfirmBuild=true`; a remote upload requires `ConfirmUpload=true`. The upload tool continues its remote scan and upload through `EditorApplication.update`, so there is no VE2 window or terminal step to perform manually.

## MCP Resources

When Coplay is installed, the package also registers three read-only native MCP resources:

| Resource | Contents |
| --- | --- |
| `ve2_plugin_interfaces` | Installed VE2API, PluginInterfaces, and instancing public source. |
| `ve2_scene_manifest` | A bounded VE2-aware manifest of the active scene. |
| `ve2_prefab_catalog` | Installed VE2 Resources prefab names. |

Unity AI exposes the equivalent data through `ve2_get_context_information`, `ve2_scene_get_manifest`, and `ve2_scene_list_prefabs`. The package reads the installed VE2 source at request time and does not bundle private VE2 documentation.

## Important VE2 Behavior

The tools do not modify VE2 source and do not directly reference VE2 assemblies. They use Unity editor APIs, reflection, `SerializedObject`, and VE2's existing setup/instantiation paths.

VE2 multiplayer sync IDs are name-derived. Avoid casual renaming of activatables and syncable objects, and run sync-ID validation after generating scene hierarchies.

`ve2_scene_make_grabbable` is idempotent. If a prefab such as `LaserPointer` already includes `V_FreeGrabbable`, the tool validates and reuses it instead of adding a duplicate component.

## Troubleshooting

### The Package Installs But No VE2 Tools Appear

Confirm that at least one supported MCP host is installed. This package deliberately does not install Unity AI or Coplay automatically.

After Unity compiles, reconnect the MCP client or begin a new AI session. MCP clients commonly cache the tool list from the moment they connect.

### Unity AI Tools Are Missing

Confirm `com.unity.ai.assistant` is installed, then inspect `Edit > Project Settings > AI/MCP`. The Unity AI adapter only compiles while that package is present.

### Coplay Tools Are Missing

Confirm `com.coplaydev.unity-mcp` is installed, open `Window > MCP for Unity`, configure the detected client, and reconnect the AI session. The Coplay adapter uses its native custom-tool discovery and needs no separate VE2 command registration.

### Scene Setup Produces Only Provider Objects

Ask for a scene created from the VE2 quickstart template. The correct primary tool is `ve2_scene_setup_quickstart`; provider tools are repair tools for existing scenes.

### A Health Check Takes Too Long

The current reflection helper checks known VE2 types without scanning every type in every loaded Unity assembly. If an older package version hangs, update to `0.3.0` or newer and let Unity recompile.

## Architecture

```text
AI client
  -> Unity AI MCP adapter OR Coplay MCP for Unity adapter
  -> shared VE2 tool catalog and typed parameter mapping
  -> host-neutral VE2 editor core
  -> Unity Editor APIs + installed VE2 reflection/SerializedObject access
```

Adding another MCP host later requires only another adapter. VE2 setup, spawning, validation, and networking behavior remains in the shared core.
