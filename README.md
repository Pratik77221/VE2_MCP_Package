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
  - Unity AI Assistant `com.unity.ai.assistant` version `2.12.0-pre.2` or newer.

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

Both adapters can be installed at the same time. Each host discovers the same 18 tool names and routes them into the same VE2 core logic. An AI client normally connects through one Unity MCP host for a session; avoid connecting both hosts under the same client-facing server name.

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

The AI should use VE2-specific tools whenever one exists. In particular:

- Use `ve2_scene_setup_quickstart` for a new multiplayer scene.
- Use `ve2_scene_spawn_prefab` for VE2 Resources prefabs.
- Use `ve2_scene_make_grabbable` instead of generic `AddComponent` for VE2 grab support.
- Use `ve2_component_configure_sync` for VE2 serialized networking fields.
- Run `ve2_scene_validate_interactions` after interaction changes.
- Run `ve2_build_preflight_plugin` before deployment.

## Tool Reference

| Tool | Purpose |
| --- | --- |
| `ve2_health_check` | Check VE2 installation and current editor/scene state. |
| `ve2_project_quick_setup` | Run VE2's own project setup utilities. |
| `ve2_scene_create_quickstart` | Create the default VE2QuickStart scene. |
| `ve2_scene_setup_quickstart` | Create or reset a named scene from the official VE2 quickstart template. |
| `ve2_scene_spawn_prefab` | Spawn a VE2 Resources prefab through VE2's instantiation path. |
| `ve2_scene_ensure_provider` | Repair or activate one VE2 provider in an existing scene. |
| `ve2_scene_ensure_multiplayer_stack` | Repair or activate the platform and instance integrations. |
| `ve2_scene_validate_sync_ids` | Find duplicate VE2 name-derived multiplayer IDs. |
| `ve2_scene_fix_name_clashes` | Safely rename duplicate syncable objects. |
| `ve2_scene_list_prefabs` | List VE2 prefab Resources available in the project. |
| `ve2_scene_configure_spawn_manager` | Configure a VE2 game-object spawn manager. |
| `ve2_scene_make_grabbable` | Add or validate VE2 grab support idempotently. |
| `ve2_scene_validate_interactions` | Validate VE2 interaction objects and dependencies. |
| `ve2_scene_prepare_for_deployment` | Rename/save/repair a scene and run deployment checks. |
| `ve2_component_configure_sync` | Configure VE2 serialized sync settings. |
| `ve2_runtime_inspect_instance` | Inspect VE2 DarkRift instance state in Play Mode. |
| `ve2_build_preflight_plugin` | Run VE2 deployment preflight checks. |
| `ve2_get_context_information` | Return selected installed VE2 API source for grounded script generation. |

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

The current reflection helper checks known VE2 types without scanning every type in every loaded Unity assembly. If an older package version hangs, update to `0.2.0` or newer and let Unity recompile.

## Architecture

```text
AI client
  -> Unity AI MCP adapter OR Coplay MCP for Unity adapter
  -> shared VE2 tool catalog and typed parameter mapping
  -> host-neutral VE2 editor core
  -> Unity Editor APIs + installed VE2 reflection/SerializedObject access
```

Adding another MCP host later requires only another adapter. VE2 setup, spawning, validation, and networking behavior remains in the shared core.
