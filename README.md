# ViRSE 2 Unity MCP Extension

This Unity package adds ViRSE 2 (VE2) tools to Unity MCP so an AI assistant can work inside a VE2 Unity project using natural language.

It follows the same model as Meta's Unity MCP extension: install the package in Unity, let Unity compile, and the tools are registered directly in the Unity Editor. There is no Node server to start and no HTTP listener to run.

## What It Does

The package lets an AI assistant:

- Check that VE2 and Unity MCP are loaded.
- Run VE2 project quick setup steps through VE2 editor utilities.
- Create or reset a named scene from VE2's quickstart template.
- Spawn VE2 prefabs through VE2's own resource-instantiation path.
- Make objects grabbable in a VE2-aware way.
- Validate VE2 interactions, colliders, sync IDs, and multiplayer provider setup.
- Configure serialized VE2 sync fields without direct VE2 source changes.
- Run deployment preflight checks before building or uploading.
- Read selected VE2 API context for better VE2 script generation.

## Requirements

Use this package in a Unity project that already has:

- Unity `6000.0` or newer.
- VE2 installed as `com.ic.ve2`.
- Unity AI Assistant / Unity MCP installed as `com.unity.ai.assistant`.

This package depends on:

```json
"com.unity.ai.assistant": "2.12.0-pre.2"
```

## Installation

### Option A: Add From Disk

Use this when the package is already on your machine.

1. Open the VE2 Unity project.
2. Open `Window > Package Manager`.
3. Click `+`.
4. Choose `Add package from disk...`.
5. Select:

```text
VE2_MCP_Package/package.json
```

6. Wait for Unity to finish compiling.

### Option B: Add From Git

Use this when the package is hosted in a Git repository.

In `Packages/manifest.json`, add the package with a path to `VE2_MCP_Package`:

```json
"com.imperial.ve2.mcp": "https://your-git-host/your-repo.git?path=/VE2_MCP_Package"
```

Then reopen Unity or let Package Manager resolve packages.

### Option C: Local Manifest Path

For local development, use a file dependency:

```json
"com.imperial.ve2.mcp": "file:D:/A_Projects/VIRSE2_MCP/VE2_MCP_Package"
```

Adjust the path for your machine.

## Confirm It Is Working

After Unity compiles:

1. Open `Edit > Project Settings`.
2. Go to the Unity AI / MCP settings page.
3. Look for tools whose names start with `ve2_`.
4. Confirm the VE2 tools are enabled.

In an AI assistant connected to Unity MCP, ask:

```text
Check whether VE2 MCP is installed and list the active scene state.
```

The assistant should use `ve2_health_check`.

You can also ask:

```text
List the VE2 prefab resources available in this project.
```

The assistant should use `ve2_scene_list_prefabs`.

## Recommended First Prompt

For a new VE2 multiplayer scene, ask the AI:

```text
Set up a VE2 multiplayer scene named Mcptest using the VE2 quickstart template, save it, and check whether it is ready for deployment.
```

The assistant should use:

- `ve2_scene_setup_quickstart`
- `ve2_build_preflight_plugin`

It should not manually add only `PlatformIntegration`, `InstanceIntegration`, or `PlayerSpawner`. The correct primary scene setup path is the quickstart template tool.

## Everyday Usage

### Create Or Reset A VE2 Scene

Ask:

```text
Create a VE2 multiplayer scene called TrainingRoom from the VE2 quickstart template.
```

Expected tool:

```text
ve2_scene_setup_quickstart
```

This creates or resets `Assets/Scenes/TrainingRoom.unity`, unpacks VE2's `VE2SetupSceneHolder`, activates the `NetworkIntegration` stack, saves, and runs preflight.

### Spawn A VE2 Prefab

Ask:

```text
Spawn the VE2 LaserPointer prefab at position 2, 1, 3 and name it LaserPointer_Grabbable.
```

Expected tool:

```text
ve2_scene_spawn_prefab
```

The spawn tool returns interaction metadata, so the assistant can see whether the object already has VE2 interaction components.

### Make An Object Grabbable

Ask:

```text
Make LaserPointer_Grabbable grabbable in VE2 and validate the result.
```

Expected tools:

```text
ve2_scene_make_grabbable
ve2_scene_validate_interactions
```

Do not ask the assistant to add `V_FreeGrabbable` with generic Unity component tools. Some VE2 prefabs, including `LaserPointer`, already include `V_FreeGrabbable`. The VE2 tool is idempotent: if the object is already grabbable, it reports success instead of trying to add a duplicate component.

### Check Interaction Problems

Ask:

```text
Validate all VE2 interactive objects in the scene and tell me what needs fixing.
```

Expected tool:

```text
ve2_scene_validate_interactions
```

This checks grabbables, laser pointers, activatables, adjustables, InfoPoints, colliders, attach points, rigidbodies, and multiplayer sync helpers.

### Configure Network Sync

Ask:

```text
Set LaserPointer_Grabbable sync to networked, 10 Hz, UDP.
```

Expected tool:

```text
ve2_component_configure_sync
```

The tool writes serialized VE2 fields such as `IsNetworked`, `TransmissionFrequency`, and `TransmissionType` when the target component exposes them.

### Deployment Preflight

Ask:

```text
Check whether this VE2 scene is ready for deployment.
```

Expected tool:

```text
ve2_build_preflight_plugin
```

The preflight checks:

- Scene is saved.
- Scene name is alphanumeric.
- Exactly one active `V_PlatformIntegration` exists.
- Multiplayer provider duplicates/inactive components are reported.
- Unity is not compiling and has no known compile failures.
- VE2 sync IDs do not clash.
- Common VE2 interaction setup issues are reported.

The tool cannot verify whether a scene name is globally unique on the VE2 platform, so it reports that as a warning.

## Tool Reference

### Setup And Health

| Tool | Use |
| --- | --- |
| `ve2_health_check` | Confirms VE2, Unity MCP, editor state, and active scene. |
| `ve2_project_quick_setup` | Runs selected VE2 setup utilities such as asmdef, TMP, URP, XR, Android manifest, and gitignore. |
| `ve2_scene_setup_quickstart` | Primary tool for creating or resetting named VE2 multiplayer scenes from the quickstart template. |
| `ve2_scene_create_quickstart` | Creates a default `VE2QuickStart` scene. Use the named setup tool for normal work. |

### Scene And Prefabs

| Tool | Use |
| --- | --- |
| `ve2_scene_list_prefabs` | Lists VE2 Resources prefabs. |
| `ve2_scene_spawn_prefab` | Spawns a VE2 Resources prefab through VE2's own instantiation helper. |
| `ve2_scene_ensure_provider` | Repair tool for individual VE2 providers. Not the primary scene setup path. |
| `ve2_scene_ensure_multiplayer_stack` | Repair tool for existing scenes. Prefer quickstart setup for new scenes. |
| `ve2_scene_configure_spawn_manager` | Configures `V_GameObjectSpawnManager` serialized references. |

### Interactions And Sync

| Tool | Use |
| --- | --- |
| `ve2_scene_make_grabbable` | Makes an object VE2 grabbable safely and idempotently. |
| `ve2_scene_validate_interactions` | Validates VE2 interactive objects and common setup problems. |
| `ve2_component_configure_sync` | Writes VE2 serialized sync settings. |
| `ve2_scene_validate_sync_ids` | Finds duplicate VE2 name-derived sync IDs. |
| `ve2_scene_fix_name_clashes` | Renames duplicate syncable GameObjects to avoid sync ID clashes. |

### Runtime And Deployment

| Tool | Use |
| --- | --- |
| `ve2_runtime_inspect_instance` | Inspects VE2 DarkRift instance state in Play Mode. |
| `ve2_scene_prepare_for_deployment` | Optionally renames/saves the active scene, ensures platform integration, and runs preflight. |
| `ve2_build_preflight_plugin` | Runs deployment preflight checks. |
| `ve2_get_context_information` | Returns selected VE2 source/API context for safer script generation. |

## Important Rules For AI Agents

When using this package, the assistant should follow these rules:

- Use `ve2_scene_setup_quickstart` for scene setup.
- Use `ve2_scene_spawn_prefab` for VE2 prefabs.
- Use `ve2_scene_make_grabbable` for grab support.
- Use `ve2_scene_validate_interactions` after editing interactive objects.
- Use `ve2_component_configure_sync` for VE2 sync fields.
- Do not use generic Unity `AddComponent` to add VE2 internals unless a VE2 MCP tool does not exist.
- Do not create duplicate provider objects when a quickstart `NetworkIntegration` already exists.
- Do not rename VE2 syncable objects casually; VE2 sync IDs are name-derived.

## Troubleshooting

### VE2 Tools Do Not Appear

Try these checks:

1. Confirm the package is installed in Package Manager.
2. Confirm `com.ic.ve2` is installed.
3. Wait until Unity finishes compiling.
4. Open `Edit > Project Settings > AI/MCP` and look for `ve2_` tools.
5. Reconnect the AI/MCP client if it connected before Unity finished compiling.

Unity MCP discovers tools with `[McpTool]` during editor/domain reload. If a client connected before the package compiled, the client may need to reconnect to see newly registered tools.

### Tools Appear In Unity But Not In The Current AI Chat

Unity may have the tools, but the external AI client may still have an old tool snapshot. Reconnect the AI client or start a fresh chat/session so it reloads the Unity MCP tool list.

### Scene Setup Looks Too Empty

If the AI only added `PlatformIntegration`, `InstanceIntegration`, and `PlayerSpawner`, it used the repair path instead of the quickstart setup path.

Ask it to use:

```text
ve2_scene_setup_quickstart
```

### Adding Grabbable To LaserPointer Fails

This usually means the real VE2 `LaserPointer` prefab already has `V_FreeGrabbable`. That is expected.

Use:

```text
ve2_scene_make_grabbable
```

The tool will treat an existing `V_FreeGrabbable` as success and validate the object instead of adding a duplicate component.

### Preflight Warns About Scene Name Uniqueness

That warning is expected. The local tool can check that the scene name format is valid, but it cannot verify global scene-name uniqueness on the VE2 platform.

## Package Design Notes

- This package does not modify VE2 source code.
- It does not add `InternalsVisibleTo` to VE2 assemblies.
- It uses reflection and serialized Unity APIs to interact with VE2 internals safely.
- It is a Unity package, not a standalone MCP server.
- It does not include private Notion exports or private VE2 documentation.
