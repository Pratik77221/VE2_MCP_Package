# Changelog

## Unreleased

- Reworked MCP registration to match the Meta XR Unity MCP extension pattern: public per-tool wrapper classes with static `HandleCommand` methods.
- Replaced nullable primitive MCP parameters on `ve2_component_configure_sync` with explicit configure switches to avoid fragile Unity MCP schema generation.
- Removed broad `assembly.GetTypes()` reflection from VE2 type lookup so health checks and tool guards stay responsive in large Unity projects.
- Hardened provider tools so existing inactive VE2 provider components are activated before new provider prefabs are spawned.
- Added `ve2_scene_setup_quickstart` as the primary named-scene setup path. It creates/resets a scene from VE2's `VE2SetupSceneHolder` template, activates and enables the quickstart `NetworkIntegration` stack, saves, and runs deployment preflight.
- Added deployment preparation and docs-informed preflight validation for scene save state, scene naming, provider counts, compile state, sync-ID clashes, and activatable collider checks.
- Added serialized sync configuration for VE2 components through `SerializedObject` and reflected component lookup.
- Added `ve2_scene_make_grabbable` and `ve2_scene_validate_interactions` so AI agents use VE2-aware, idempotent grabbable/interaction tooling instead of generic AddComponent calls. This prevents false failures on prefabs such as `LaserPointer` that already include `V_FreeGrabbable`.
- Documented the package-only Unity MCP workflow for VE2 quick setup, quickstart scenes, multiplayer stack activation, sync configuration, and deployment preflight.
- Reworked `README.md` into a user-facing setup and usage guide covering installation, Project Settings/AI verification, natural-language prompts, tool selection, and troubleshooting.

## 0.1.0

- Initial UPM package version.
- Registers VE2 tools directly with Unity MCP through `[McpTool]`.
- Adds VE2 prefab spawning, setup, provider creation, sync-ID validation/fixing, spawn manager configuration, runtime instance inspection, build preflight, and context discovery.
