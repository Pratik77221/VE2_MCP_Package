using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Unity.AI.MCP.Editor.Helpers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Imperial.VE2.MCP.Editor
{
    public static class VE2McpTools
    {
        private static readonly Dictionary<string, string> ProviderResources = new()
        {
            ["player_spawner"] = "PlayerSpawner",
            ["platform_integration"] = "PlatformIntegration",
            ["instance_integration"] = "InstanceIntegration",
            ["file_system"] = "FileSystem"
        };

        private static readonly Dictionary<string, string> ProviderTypes = new()
        {
            ["player_spawner"] = "V_PlayerSpawner",
            ["platform_integration"] = "V_PlatformIntegration",
            ["instance_integration"] = "V_InstanceIntegration",
            ["file_system"] = "V_PluginFileSystem"
        };

        public static object HealthCheck()
        {
            return Response.Success("VE2 MCP package is loaded.",
                new
                {
                    package = "com.imperial.ve2.mcp",
                    unityVersion = Application.unityVersion,
                    projectPath = Directory.GetParent(Application.dataPath)?.FullName,
                    dataPath = Application.dataPath,
                    activeScene = SceneManager.GetActiveScene().path,
                    isPlaying = EditorApplication.isPlaying,
                    isCompiling = EditorApplication.isCompiling,
                    ve2Installed = VE2Reflection.IsVE2Installed,
                    ve2ApiTypeFound = VE2Reflection.FindType("VE2.Common.API.VE2API") != null
                });
        }

        public static object QuickSetup(VE2QuickSetupParams parameters)
        {
            return GuardVE2(() =>
            {
                parameters ??= new VE2QuickSetupParams();

                if (parameters.LayersAndTags && !parameters.ConfirmDestructiveLayerTagChanges)
                {
                    return Response.Error("Refusing to configure layers/tags without ConfirmDestructiveLayerTagChanges=true because VE2's LayerAutoConfig can remove non-VE2 custom layers/tags.");
                }

                var completed = new List<string>();

                if (parameters.Asmdef)
                {
                    VE2Reflection.InvokeStatic("VE2.Common.Shared.VE2AutoAsmDef", "CreateOrUpdateAsmdef");
                    completed.Add("asmdef");
                }

                if (parameters.Tmp)
                {
                    VE2Reflection.InvokeStatic("VE2.Common.Shared.VE2TMPSetup", "ImportTextMeshProEssentials");
                    completed.Add("tmp");
                }

                if (parameters.LayersAndTags)
                {
                    VE2Reflection.InvokeStatic("VE2.Common.Shared.VE2LayerAutoConfig", "ConfigureLayersAndTags", true);
                    completed.Add("layersAndTags");
                }

                if (parameters.Urp)
                {
                    completed.Add("urp:" + SetupUrp());
                }

                if (parameters.Xr)
                {
                    VE2Reflection.InvokeStatic("VE2.Common.Shared.VE2SetupXR", "EnableXRPlugInManagement", true, false);
                    completed.Add("xr");
                }

                if (parameters.OculusProfile)
                {
                    VE2Reflection.InvokeStatic("VE2.Common.Shared.VE2SetupXR", "EnableOpenXRFeatures");
                    completed.Add("oculusProfile");
                }

                if (parameters.EditorToolbox)
                {
                    VE2Reflection.InvokeStatic("VE2.Common.Shared.VE2AutoEditorToolboxSetup", "CreateToolboxEditorSettingsAsset");
                    completed.Add("editorToolbox");
                }

                if (parameters.AndroidManifest)
                {
                    VE2Reflection.InvokeStatic("VE2.Common.Shared.VE2SetupAndroidManifest", "CopyManifestFromResources");
                    completed.Add("androidManifest");
                }

                if (parameters.CreateScene)
                {
                    completed.Add("createScene:" + CreateQuickStartSceneInternal());
                }

                if (parameters.LayersAndTags)
                {
                    VE2Reflection.InvokeStatic("VE2.Common.Shared.VE2LayerAutoConfig", "ConfigureLayersAndTags", false);
                    completed.Add("layersAndTagsRetry");
                }

                if (parameters.Xr)
                {
                    VE2Reflection.InvokeStatic("VE2.Common.Shared.VE2SetupXR", "EnableXRPlugInManagement", false, false);
                    completed.Add("xrRetry");
                }

                if (parameters.OculusProfile)
                {
                    VE2Reflection.InvokeStatic("VE2.Common.Shared.VE2SetupXR", "EnableOpenXRFeatures", false);
                    completed.Add("oculusProfileRetry");
                }

                if (parameters.Gitignore)
                {
                    VE2Reflection.InvokeStatic("VE2.Common.Shared.VE2GitignoreGenerator", "GenerateGitignore");
                    completed.Add("gitignore");
                }

                AssetDatabase.Refresh();
                return Response.Success("VE2 setup steps completed.", new { completed });
            });
        }

        public static object CreateQuickStartScene()
        {
            return GuardVE2(() =>
            {
                var scenePath = CreateQuickStartSceneInternal();
                return Response.Success("Created VE2 quickstart scene.", new { scenePath });
            });
        }

        public static object SetupQuickStartScene(VE2QuickStartSceneParams parameters)
        {
            return GuardVE2(() =>
            {
                parameters ??= new VE2QuickStartSceneParams();

                var actions = new List<string>();
                var scenePath = SetupQuickStartSceneInternal(parameters, actions);
                var rootObjects = SceneManager.GetActiveScene()
                    .GetRootGameObjects()
                    .Select(VE2Reflection.GameObjectData)
                    .ToArray();
                var preflight = RunDeploymentPreflight(true);

                return Response.Success(preflight.valid
                        ? "Set up VE2 quickstart multiplayer scene."
                        : "Set up VE2 quickstart multiplayer scene with deployment preflight issues.",
                    new
                    {
                        scenePath,
                        rootObjects,
                        actions,
                        preflight
                    });
            });
        }

        public static object SpawnPrefab(VE2SpawnPrefabParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.ResourceName))
                {
                    return Response.Error("ResourceName is required.");
                }

                var gameObject = VE2Reflection.InstantiateVE2Resource(parameters.ResourceName);
                if (gameObject == null)
                {
                    return Response.Error($"Could not instantiate VE2 Resources prefab '{parameters.ResourceName}'.");
                }

                if (!string.IsNullOrWhiteSpace(parameters.Name))
                {
                    gameObject.name = VE2Reflection.MakeUniqueSceneName(parameters.Name, gameObject);
                }

                var parent = VE2Reflection.FindGameObjectByPath(parameters.ParentPath)?.transform;
                if (parent != null)
                {
                    gameObject.transform.SetParent(parent, true);
                }

                ApplyTransform(gameObject.transform, parameters);
                PostProcessSpecialResource(parameters.ResourceName, gameObject);

                Selection.activeGameObject = gameObject;
                Undo.RegisterCompleteObjectUndo(gameObject, "VE2 MCP Configure Spawned Prefab");
                VE2Reflection.MarkActiveSceneDirty();

                var syncReport = VE2SyncIdUtility.Scan();
                if (parameters.EnsureUniqueSyncIds && syncReport.clashCount > 0)
                {
                    return Response.Success("Spawned prefab, but duplicate VE2 sync IDs are present. Run ve2_scene_fix_name_clashes or rename clashing GameObjects.",
                        new
                        {
                            valid = false,
                            spawned = VE2Reflection.GameObjectData(gameObject),
                            interaction = BuildInteractionObjectReport(gameObject),
                            syncValidation = syncReport
                        });
                }

                return Response.Success("Spawned VE2 prefab.",
                    new
                    {
                        spawned = VE2Reflection.GameObjectData(gameObject),
                        interaction = BuildInteractionObjectReport(gameObject),
                        syncValidation = syncReport
                    });
            });
        }

        public static object EnsureProvider(VE2EnsureProviderParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.Provider))
                {
                    return Response.Error("Provider is required.");
                }

                var provider = EnsureProviderInternal(parameters.Provider.Trim().ToLowerInvariant());
                return Response.Success("Ensured VE2 provider.", provider);
            });
        }

        public static object EnsureMultiplayerStack()
        {
            return GuardVE2(() =>
            {
                var platform = EnsureProviderInternal("platform_integration");
                var instance = EnsureProviderInternal("instance_integration");
                return Response.Success("Ensured VE2 multiplayer stack.", new { platform, instance });
            });
        }

        public static object ValidateSyncIds()
        {
            return GuardVE2(() =>
            {
                var report = VE2SyncIdUtility.Scan();
                return Response.Success(report.clashCount == 0 ? "No VE2 sync ID clashes found." : "VE2 sync ID clashes found.",
                    new
                    {
                        valid = report.clashCount == 0,
                        syncValidation = report
                    });
            });
        }

        public static object FixNameClashes(VE2FixNameClashesParams parameters)
        {
            return GuardVE2(() =>
            {
                parameters ??= new VE2FixNameClashesParams();
                var result = VE2SyncIdUtility.Fix(parameters.DryRun);
                return Response.Success(parameters.DryRun ? "Dry run completed; no GameObjects were renamed." : "VE2 GameObject name clashes fixed.", result);
            });
        }

        public static object ListPrefabs()
        {
            return GuardVE2(() =>
            {
                var prefabs = new List<object>();
                foreach (var guid in AssetDatabase.FindAssets("t:Prefab"))
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid).Replace('\\', '/');
                    if (!assetPath.Contains("/FRAMEWORK/") || !assetPath.Contains("/Resources/"))
                    {
                        continue;
                    }

                    prefabs.Add(new
                    {
                        resourceName = VE2Reflection.ResourceNameFromAssetPath(assetPath),
                        assetPath
                    });
                }

                return Response.Success("Listed VE2 prefab resources.", new { prefabs });
            });
        }

        public static object ConfigureSpawnManager(VE2ConfigureSpawnManagerParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.SpawnManagerPath))
                {
                    return Response.Error("SpawnManagerPath is required.");
                }

                var managerObject = VE2Reflection.FindGameObjectByPath(parameters.SpawnManagerPath);
                if (managerObject == null)
                {
                    return Response.Error($"Could not find GameObject '{parameters.SpawnManagerPath}'.");
                }

                var spawnManager = managerObject.GetComponents<Component>().FirstOrDefault(c => c != null && c.GetType().Name == "V_GameObjectSpawnManager");
                if (spawnManager == null)
                {
                    return Response.Error($"{managerObject.name} does not contain V_GameObjectSpawnManager.");
                }

                var serialized = new SerializedObject(spawnManager);

                if (!string.IsNullOrWhiteSpace(parameters.ObjectToSpawnPathOrAsset))
                {
                    var objectToSpawn = VE2Reflection.FindGameObjectByPath(parameters.ObjectToSpawnPathOrAsset) as UnityEngine.Object
                                        ?? AssetDatabase.LoadAssetAtPath<GameObject>(parameters.ObjectToSpawnPathOrAsset);
                    if (objectToSpawn == null)
                    {
                        return Response.Error("ObjectToSpawnPathOrAsset did not resolve to a scene GameObject or prefab asset.");
                    }

                    var prop = serialized.FindProperty("_gameobjectToSpawn");
                    if (prop == null)
                    {
                        return Response.Error("Could not find _gameobjectToSpawn on V_GameObjectSpawnManager.");
                    }

                    prop.objectReferenceValue = objectToSpawn;
                }

                if (!string.IsNullOrWhiteSpace(parameters.SpawnTransformPath))
                {
                    var spawnTransform = VE2Reflection.FindGameObjectByPath(parameters.SpawnTransformPath)?.transform;
                    if (spawnTransform == null)
                    {
                        return Response.Error("SpawnTransformPath did not resolve to a scene Transform.");
                    }

                    var prop = serialized.FindProperty("_spawnPosition");
                    if (prop == null)
                    {
                        return Response.Error("Could not find _spawnPosition on V_GameObjectSpawnManager.");
                    }

                    prop.objectReferenceValue = spawnTransform;
                }

                serialized.ApplyModifiedProperties();
                EditorUtility.SetDirty(spawnManager);
                VE2Reflection.MarkActiveSceneDirty();

                return Response.Success("Configured V_GameObjectSpawnManager.", VE2Reflection.GameObjectData(managerObject));
            });
        }

        public static object MakeGrabbable(VE2MakeGrabbableParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.GameObjectPath))
                {
                    return Response.Error("GameObjectPath is required.");
                }

                var gameObject = VE2Reflection.FindGameObjectByPath(parameters.GameObjectPath);
                if (gameObject == null)
                {
                    return Response.Error($"Could not find GameObject '{parameters.GameObjectPath}'.");
                }

                var freeGrabbableType = VE2Reflection.FindType("V_FreeGrabbable");
                if (freeGrabbableType == null)
                {
                    return Response.Error("Could not find VE2 V_FreeGrabbable component type.");
                }

                var actions = new List<string>();
                var warnings = new List<string>();
                var changed = false;

                var freeGrabbable = GetComponentByTypeName(gameObject, "V_FreeGrabbable");
                if (freeGrabbable == null)
                {
                    freeGrabbable = Undo.AddComponent(gameObject, freeGrabbableType) as Component;
                    actions.Add("added:V_FreeGrabbable");
                    changed = true;
                }
                else
                {
                    actions.Add("alreadyHad:V_FreeGrabbable");
                }

                if (parameters.EnsureRigidbody && gameObject.GetComponent<Rigidbody>() == null)
                {
                    var rigidbody = Undo.AddComponent<Rigidbody>(gameObject);
                    rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                    actions.Add("added:Rigidbody");
                    changed = true;
                }

                if (parameters.EnsureCollider && !HasUsableNonTriggerColliderInSelfOrChildren(gameObject))
                {
                    var colliderWarning = AddFallbackBoxCollider(gameObject);
                    actions.Add("added:BoxCollider");
                    if (!string.IsNullOrWhiteSpace(colliderWarning))
                    {
                        warnings.Add(colliderWarning);
                    }

                    changed = true;
                }

                if (parameters.EnsureRigidbodySyncable &&
                    GetComponentByTypeName(gameObject, "V_RigidbodySyncable") == null)
                {
                    var syncableType = VE2Reflection.FindType("V_RigidbodySyncable");
                    if (syncableType == null)
                    {
                        warnings.Add("Could not find V_RigidbodySyncable. The object can be grabbable locally, but rigidbody state may not sync in multiplayer.");
                    }
                    else
                    {
                        Undo.AddComponent(gameObject, syncableType);
                        actions.Add("added:V_RigidbodySyncable");
                        changed = true;
                    }
                }

                if (changed)
                {
                    EditorUtility.SetDirty(gameObject);
                    if (freeGrabbable != null)
                    {
                        EditorUtility.SetDirty(freeGrabbable);
                    }

                    VE2Reflection.MarkActiveSceneDirty();
                }

                if (parameters.SaveScene)
                {
                    actions.Add("saved:" + SaveActiveScene());
                }

                var report = BuildInteractionObjectReport(gameObject);
                warnings.AddRange(report.warnings);
                var syncReport = VE2SyncIdUtility.Scan();

                return Response.Success(report.errors.Count == 0
                        ? "VE2 grabbable configuration completed."
                        : "VE2 grabbable configuration completed with interaction issues.",
                    new
                    {
                        target = report,
                        actions = actions.Distinct().ToArray(),
                        warnings = warnings.Distinct().ToArray(),
                        valid = report.errors.Count == 0,
                        syncValidation = syncReport
                    });
            });
        }

        public static object ValidateInteractions()
        {
            return GuardVE2(() =>
            {
                var interactionTypeNames = new[]
                {
                    "V_FreeGrabbable",
                    "V_LaserPointer",
                    "V_ToggleActivatable",
                    "V_HoldActivatable",
                    "V_PressurePlateActivatable",
                    "V_HandheldActivatable",
                    "V_HandheldAdjustable",
                    "V_SlidingAdjustable",
                    "V_Sliding2DAdjustable",
                    "V_RotatingAdjustable",
                    "V_Rotating2DAdjustable",
                    "V_CustomInfoPoint"
                };

                var visited = new HashSet<int>();
                var reports = new List<InteractionObjectReport>();
                foreach (var typeName in interactionTypeNames)
                {
                    foreach (var component in VE2Reflection.FindComponentsByTypeName(typeName))
                    {
                        if (component == null || !visited.Add(component.gameObject.GetInstanceID()))
                        {
                            continue;
                        }

                        reports.Add(BuildInteractionObjectReport(component.gameObject));
                    }
                }

                var errors = reports.SelectMany(report => report.errors).Distinct().ToArray();
                var warnings = reports.SelectMany(report => report.warnings).Distinct().ToArray();
                var syncReport = VE2SyncIdUtility.Scan();

                return Response.Success(errors.Length == 0
                        ? "VE2 interaction validation completed."
                        : "VE2 interaction validation found issues.",
                    new
                    {
                        valid = errors.Length == 0 && syncReport.clashCount == 0,
                        interactionCount = reports.Count,
                        errors,
                        warnings,
                        interactions = reports.OrderBy(report => report.path).ToArray(),
                        syncValidation = syncReport
                    });
            });
        }

        public static object PrepareForDeployment(VE2PrepareForDeploymentParams parameters)
        {
            return GuardVE2(() =>
            {
                parameters ??= new VE2PrepareForDeploymentParams();
                var actions = new List<string>();

                if (!string.IsNullOrWhiteSpace(parameters.SceneName))
                {
                    var sceneName = parameters.SceneName.Trim();
                    if (!Regex.IsMatch(sceneName, "^[a-zA-Z0-9]+$"))
                    {
                        return Response.Error($"SceneName '{sceneName}' is invalid for VE2 deployment. Use only letters and numbers.");
                    }

                    actions.Add("scenePath:" + SaveActiveSceneAs(sceneName));
                }

                if (parameters.ActivateNetworkIntegration)
                {
                    var networkIntegration = FindQuickStartNetworkIntegrationObject();
                    if (networkIntegration != null)
                    {
                        var activated = ActivateGameObjectAndParents(networkIntegration);
                        actions.Add(activated ? "activated:NetworkIntegration" : "alreadyActive:NetworkIntegration");
                    }
                }

                if (parameters.EnsurePlatformIntegration)
                {
                    EnsureProviderInternal("platform_integration");
                    actions.Add("ensured:platform_integration");
                }

                if (parameters.SaveScene)
                {
                    actions.Add("saved:" + SaveActiveScene());
                }

                var preflight = RunDeploymentPreflight(true);
                return Response.Success(preflight.valid ? "VE2 scene prepared for deployment." : "VE2 scene preparation completed with preflight issues.",
                    new
                    {
                        actions,
                        preflight
                    });
            });
        }

        public static object ConfigureSync(VE2ConfigureSyncParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.GameObjectPath))
                {
                    return Response.Error("GameObjectPath is required.");
                }

                var gameObject = VE2Reflection.FindGameObjectByPath(parameters.GameObjectPath);
                if (gameObject == null)
                {
                    return Response.Error($"Could not find GameObject '{parameters.GameObjectPath}'.");
                }

                var component = ResolveSyncConfigComponent(gameObject, parameters.ComponentTypeName, out var resolveError);
                if (component == null)
                {
                    return Response.Error(resolveError);
                }

                if (parameters.ConfigureTransmissionFrequency &&
                    (parameters.TransmissionFrequency < 0.2f || parameters.TransmissionFrequency > 50f))
                {
                    return Response.Error("TransmissionFrequency must be between 0.2 and 50 Hz.");
                }

                var serialized = new SerializedObject(component);
                var changed = new List<string>();
                var warnings = new List<string>();

                if (parameters.ConfigureIsNetworked)
                {
                    var property = FindSerializedPropertyByLeafName(serialized, "IsNetworked");
                    if (property == null || property.propertyType != SerializedPropertyType.Boolean)
                    {
                        warnings.Add("Could not find boolean IsNetworked on the selected component.");
                    }
                    else
                    {
                        property.boolValue = parameters.IsNetworked;
                        changed.Add("IsNetworked");
                    }
                }

                if (parameters.ConfigureTransmissionFrequency)
                {
                    var property = FindSerializedPropertyByLeafName(serialized, "TransmissionFrequency");
                    if (property == null || property.propertyType != SerializedPropertyType.Float)
                    {
                        warnings.Add("Could not find float TransmissionFrequency on the selected component. Hold activatables use player sync settings and may not expose this field.");
                    }
                    else
                    {
                        property.floatValue = parameters.TransmissionFrequency;
                        changed.Add("TransmissionFrequency");
                    }
                }

                if (!string.IsNullOrWhiteSpace(parameters.TransmissionProtocol))
                {
                    var protocol = parameters.TransmissionProtocol.Trim().ToUpperInvariant();
                    if (protocol != "UDP" && protocol != "TCP")
                    {
                        return Response.Error("TransmissionProtocol must be UDP or TCP.");
                    }

                    var property = FindSerializedPropertyByLeafName(serialized, "TransmissionType");
                    if (property == null || property.propertyType != SerializedPropertyType.Enum)
                    {
                        warnings.Add("Could not find enum TransmissionType on the selected component. Hold activatables use player sync settings and may not expose this field.");
                    }
                    else
                    {
                        var index = Array.FindIndex(property.enumNames, name => string.Equals(name, protocol, StringComparison.OrdinalIgnoreCase));
                        if (index < 0)
                        {
                            warnings.Add("TransmissionType enum does not contain the requested protocol.");
                        }
                        else
                        {
                            property.enumValueIndex = index;
                            changed.Add("TransmissionType");
                        }
                    }
                }

                serialized.ApplyModifiedProperties();
                if (changed.Count > 0)
                {
                    EditorUtility.SetDirty(component);
                    VE2Reflection.MarkActiveSceneDirty();
                }

                return Response.Success("Configured VE2 sync settings.",
                    new
                    {
                        target = VE2Reflection.GameObjectData(gameObject),
                        componentType = component.GetType().Name,
                        changed,
                        warnings
                    });
            });
        }

        public static object InspectInstance()
        {
            return GuardVE2(() =>
            {
                if (!EditorApplication.isPlaying)
                {
                    return Response.Error("Runtime instance inspection requires Play Mode.");
                }

                var instance = VE2Reflection.GetVE2ApiProperty("InstanceService");
                if (instance == null)
                {
                    return Response.Error("VE2API.InstanceService is null. Ensure V_PlatformIntegration and V_InstanceIntegration are present.");
                }

                return Response.Success("Inspected VE2 InstanceService.",
                    new
                    {
                        isConnectedToServer = VE2Reflection.ReadProperty(instance, "IsConnectedToServer"),
                        isHost = VE2Reflection.ReadProperty(instance, "IsHost"),
                        localClientID = VE2Reflection.ReadProperty(instance, "LocalClientID"),
                        hostID = VE2Reflection.ReadProperty(instance, "HostID"),
                        numberOfClientsInCurrentInstance = VE2Reflection.ReadProperty(instance, "NumberOfClientsInCurrentInstance"),
                        ping = VE2Reflection.ReadProperty(instance, "Ping"),
                        smoothPing = VE2Reflection.ReadProperty(instance, "SmoothPing")
                    });
            });
        }

        public static object BuildPreflight()
        {
            return GuardVE2(() =>
            {
                var data = RunDeploymentPreflight(true);
                return Response.Success(data.valid ? "VE2 plugin preflight passed." : "VE2 plugin preflight failed.", data);
            });
        }

        public static object GetContextInformation()
        {
            return GuardVE2(() =>
            {
                var frameworkRoots = FindFrameworkAssetRoots().ToArray();
                var pluginInterfaces = FindAssetPaths("/Core/VComponents/Scripts/API/PluginInterfaces/", ".cs")
                    .Select(path => new
                    {
                        path,
                        text = ReadAssetText(path)
                    })
                    .ToArray();

                var ve2Api = FindAssetPaths("/Common/Scripts/API/", "VE2API.cs").FirstOrDefault();
                var instancingApi = FindAssetPaths("/Non-Core/Instancing/Scripts/API/", ".cs").ToArray();

                return Response.Success("VE2 context information.",
                    new
                    {
                        frameworkRoots,
                        ve2Api = ve2Api == null ? null : new { path = ve2Api, text = ReadAssetText(ve2Api) },
                        pluginInterfaces,
                        instancingApi
                    });
            });
        }

        private static object GuardVE2(Func<object> action)
        {
            try
            {
                if (!VE2Reflection.IsVE2Installed)
                {
                    return Response.Error("VE2 does not appear to be installed in this Unity project. Could not find VE2.Common.API.VE2API.");
                }

                return action();
            }
            catch (TargetInvocationException ex)
            {
                return Response.Error(ex.InnerException?.Message ?? ex.Message);
            }
            catch (Exception ex)
            {
                return Response.Error(ex.Message);
            }
        }

        private static PreflightResult RunDeploymentPreflight(bool includeNameUniquenessWarning)
        {
            var scene = SceneManager.GetActiveScene();
            var errors = new List<string>();
            var warnings = new List<string>();

            if (string.IsNullOrWhiteSpace(scene.path))
            {
                errors.Add("Scene must be saved before building or uploading a VE2 plugin.");
            }

            if (scene.isDirty)
            {
                errors.Add("Scene has unsaved changes. Save the scene before building or uploading a VE2 plugin.");
            }

            if (string.IsNullOrWhiteSpace(scene.name) || !Regex.IsMatch(scene.name, "^[a-zA-Z0-9]+$"))
            {
                errors.Add($"Scene name '{scene.name}' is invalid for VE2 deployment. Use only letters and numbers.");
            }
            else if (includeNameUniquenessWarning)
            {
                warnings.Add("Scene name format is valid, but global uniqueness on the VE2 platform cannot be verified locally.");
            }

            var compileState = GetCompileState();
            if (compileState.isCompiling)
            {
                errors.Add("Unity scripts are still compiling. Wait for compilation to finish before building or uploading.");
            }

            if (compileState.scriptCompilationFailed == true)
            {
                errors.Add("Unity reports script compilation failures. Fix compile errors before building or uploading.");
            }

            var providerSummary = BuildProviderSummary();
            if (providerSummary.activePlatformIntegrationCount != 1)
            {
                errors.Add($"Expected exactly one active V_PlatformIntegration; found {providerSummary.activePlatformIntegrationCount}.");
            }

            if (providerSummary.inactivePlatformIntegrationCount > 0)
            {
                warnings.Add($"Found {providerSummary.inactivePlatformIntegrationCount} inactive V_PlatformIntegration component(s).");
            }

            if (providerSummary.totalPlatformIntegrationCount > 1)
            {
                warnings.Add($"Found {providerSummary.totalPlatformIntegrationCount} total V_PlatformIntegration component(s). VE2 providers should not be duplicated.");
            }

            if (providerSummary.activeInstanceIntegrationCount == 0)
            {
                warnings.Add("No active V_InstanceIntegration found. VE2 multiplayer and networked component state will be disabled.");
            }
            else if (providerSummary.activeInstanceIntegrationCount > 1)
            {
                errors.Add($"Expected at most one active V_InstanceIntegration; found {providerSummary.activeInstanceIntegrationCount}.");
            }

            if (providerSummary.inactiveInstanceIntegrationCount > 0)
            {
                warnings.Add($"Found {providerSummary.inactiveInstanceIntegrationCount} inactive V_InstanceIntegration component(s).");
            }

            if (providerSummary.totalInstanceIntegrationCount > 1)
            {
                warnings.Add($"Found {providerSummary.totalInstanceIntegrationCount} total V_InstanceIntegration component(s). VE2 providers should not be duplicated.");
            }

            var scriptsWithoutAsmdef = FindScriptsWithoutAsmdef(scene);
            if (scriptsWithoutAsmdef.Count > 0)
            {
                warnings.Add("Some scene scripts are in Assembly-CSharp and may not be packaged correctly by VE2 PluginBuilder.");
            }

            var activatableValidation = FindActivatableValidationWarnings();
            foreach (var warning in activatableValidation)
            {
                warnings.Add(warning);
            }

            var syncReport = VE2SyncIdUtility.Scan();
            if (syncReport.clashCount > 0)
            {
                errors.Add("VE2 sync ID clashes were found.");
            }

            return new PreflightResult
            {
                valid = errors.Count == 0,
                sceneName = scene.name,
                scenePath = scene.path,
                sceneIsDirty = scene.isDirty,
                errors = errors,
                warnings = warnings,
                compileState = compileState,
                providerSummary = providerSummary,
                scriptsWithoutAsmdef = scriptsWithoutAsmdef,
                activatableValidation = activatableValidation,
                syncValidation = syncReport
            };
        }

        private static CompileState GetCompileState()
        {
            return new CompileState
            {
                isCompiling = EditorApplication.isCompiling,
                scriptCompilationFailed = TryGetScriptCompilationFailed()
            };
        }

        private static bool? TryGetScriptCompilationFailed()
        {
            try
            {
                var property = typeof(EditorUtility).GetProperty("scriptCompilationFailed", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (property == null || property.PropertyType != typeof(bool))
                {
                    return null;
                }

                return (bool)property.GetValue(null);
            }
            catch
            {
                return null;
            }
        }

        private static ProviderSummary BuildProviderSummary()
        {
            var platform = GetComponentStatuses("V_PlatformIntegration").ToArray();
            var instance = GetComponentStatuses("V_InstanceIntegration").ToArray();

            return new ProviderSummary
            {
                platformIntegration = platform,
                instanceIntegration = instance,
                activePlatformIntegrationCount = platform.Count(status => status.activeInHierarchy && status.enabled),
                inactivePlatformIntegrationCount = platform.Count(status => !status.activeInHierarchy || !status.enabled),
                totalPlatformIntegrationCount = platform.Length,
                activeInstanceIntegrationCount = instance.Count(status => status.activeInHierarchy && status.enabled),
                inactiveInstanceIntegrationCount = instance.Count(status => !status.activeInHierarchy || !status.enabled),
                totalInstanceIntegrationCount = instance.Length
            };
        }

        private static IEnumerable<ComponentStatus> GetComponentStatuses(string typeName)
        {
            return VE2Reflection.FindComponentsByTypeName(typeName)
                .Select(component => new ComponentStatus
                {
                    componentType = component.GetType().Name,
                    path = VE2Reflection.GetHierarchyPath(component.transform),
                    gameObject = VE2Reflection.GameObjectData(component.gameObject),
                    enabled = !(component is Behaviour behaviour) || behaviour.enabled,
                    activeSelf = component.gameObject.activeSelf,
                    activeInHierarchy = component.gameObject.activeInHierarchy
                })
                .OrderBy(status => status.path);
        }

        private static List<string> FindActivatableValidationWarnings()
        {
            var warnings = new List<string>();
            var activatableTypes = new[]
            {
                "V_ToggleActivatable",
                "V_HoldActivatable",
                "V_PressurePlateActivatable",
                "V_HandheldActivatable",
                "V_HandheldAdjustable"
            };

            foreach (var typeName in activatableTypes)
            {
                foreach (var component in VE2Reflection.FindComponentsByTypeName(typeName))
                {
                    if (!HasNonTriggerColliderInSelfOrChildren(component.gameObject))
                    {
                        warnings.Add($"{VE2Reflection.GetHierarchyPath(component.transform)} has {component.GetType().Name} but no non-trigger Collider on the same GameObject or a child GameObject.");
                    }
                }
            }

            foreach (var infoPoint in VE2Reflection.FindComponentsByTypeName("V_CustomInfoPoint"))
            {
                var trigger = VE2Reflection.GetComponentInChildrenByTypeName(infoPoint.gameObject, "InfoPointTriggerAnimationHandler");
                if (trigger == null)
                {
                    warnings.Add($"{VE2Reflection.GetHierarchyPath(infoPoint.transform)} has V_CustomInfoPoint but no InfoPointTriggerAnimationHandler child.");
                    continue;
                }

                if (!HasAnyCollider(trigger.gameObject))
                {
                    warnings.Add($"{VE2Reflection.GetHierarchyPath(trigger.transform)} is the V_CustomInfoPoint trigger but has no Collider.");
                }
            }

            return warnings;
        }

        private static bool HasNonTriggerColliderInSelfOrChildren(GameObject gameObject)
        {
            return gameObject != null && gameObject.GetComponentsInChildren<Collider>(true).Any(collider => collider != null && !collider.isTrigger);
        }

        private static bool HasAnyCollider(GameObject gameObject)
        {
            return gameObject != null && gameObject.GetComponents<Collider>().Any(collider => collider != null);
        }

        private static Component GetComponentByTypeName(GameObject gameObject, string typeName)
        {
            var type = VE2Reflection.FindType(typeName);
            return gameObject != null && type != null ? gameObject.GetComponent(type) as Component : null;
        }

        private static bool HasUsableNonTriggerColliderInSelfOrChildren(GameObject gameObject)
        {
            return gameObject != null && gameObject.GetComponentsInChildren<Collider>(true).Any(IsUsableNonTriggerCollider);
        }

        private static bool IsUsableNonTriggerCollider(Collider collider)
        {
            if (collider == null || !collider.enabled || collider.isTrigger)
            {
                return false;
            }

            if (collider is MeshCollider meshCollider && meshCollider.sharedMesh == null)
            {
                return false;
            }

            return collider.bounds.size.sqrMagnitude > 0.0001f;
        }

        private static string AddFallbackBoxCollider(GameObject gameObject)
        {
            var boxCollider = Undo.AddComponent<BoxCollider>(gameObject);
            boxCollider.isTrigger = false;

            var renderers = gameObject.GetComponentsInChildren<Renderer>(true)
                .Where(renderer => renderer != null)
                .ToArray();

            if (renderers.Length == 0)
            {
                boxCollider.center = Vector3.zero;
                boxCollider.size = Vector3.one * 0.2f;
                return $"{VE2Reflection.GetHierarchyPath(gameObject.transform)} had no Renderer or usable Collider, so a fallback 0.2m BoxCollider was added. Prefer spawning the real VE2 prefab when one exists, e.g. LaserPointer instead of an empty placeholder.";
            }

            var bounds = renderers[0].bounds;
            foreach (var renderer in renderers.Skip(1))
            {
                bounds.Encapsulate(renderer.bounds);
            }

            boxCollider.center = gameObject.transform.InverseTransformPoint(bounds.center);
            boxCollider.size = WorldSizeToLocalSize(gameObject.transform, bounds.size);
            return null;
        }

        private static Vector3 WorldSizeToLocalSize(Transform transform, Vector3 worldSize)
        {
            var scale = transform.lossyScale;
            return new Vector3(
                Mathf.Max(0.02f, worldSize.x / Mathf.Max(0.0001f, Mathf.Abs(scale.x))),
                Mathf.Max(0.02f, worldSize.y / Mathf.Max(0.0001f, Mathf.Abs(scale.y))),
                Mathf.Max(0.02f, worldSize.z / Mathf.Max(0.0001f, Mathf.Abs(scale.z))));
        }

        private static InteractionObjectReport BuildInteractionObjectReport(GameObject gameObject)
        {
            var report = new InteractionObjectReport
            {
                path = gameObject != null ? VE2Reflection.GetHierarchyPath(gameObject.transform) : null,
                gameObject = VE2Reflection.GameObjectData(gameObject),
                componentTypes = gameObject == null
                    ? Array.Empty<string>()
                    : gameObject.GetComponents<Component>()
                        .Where(component => component != null)
                        .Select(component => component.GetType().Name)
                        .OrderBy(name => name)
                        .ToArray(),
                warnings = new List<string>(),
                errors = new List<string>()
            };

            if (gameObject == null)
            {
                report.errors.Add("GameObject is null.");
                return report;
            }

            var freeGrabbable = GetComponentByTypeName(gameObject, "V_FreeGrabbable");
            var laserPointer = GetComponentByTypeName(gameObject, "V_LaserPointer");
            var rigidbodySyncable = GetComponentByTypeName(gameObject, "V_RigidbodySyncable");

            report.hasFreeGrabbable = freeGrabbable != null;
            report.hasLaserPointer = laserPointer != null;
            report.hasRigidbody = gameObject.GetComponent<Rigidbody>() != null;
            report.hasRigidbodySyncable = rigidbodySyncable != null;
            report.hasRenderer = gameObject.GetComponentsInChildren<Renderer>(true).Any(renderer => renderer != null);
            report.hasUsableNonTriggerCollider = HasUsableNonTriggerColliderInSelfOrChildren(gameObject);
            report.nonTriggerColliderCount = gameObject.GetComponentsInChildren<Collider>(true)
                .Count(collider => collider != null && !collider.isTrigger);

            AddColliderWarnings(gameObject, report);

            if (report.hasFreeGrabbable)
            {
                if (!report.hasRigidbody)
                {
                    report.errors.Add($"{report.path} has V_FreeGrabbable but no Rigidbody.");
                }

                if (!report.hasUsableNonTriggerCollider)
                {
                    report.errors.Add($"{report.path} has V_FreeGrabbable but no usable non-trigger Collider.");
                }

                report.freeGrabbableAttachPointPath = GetObjectReferencePath(freeGrabbable, "_attachPoint");
                if (string.IsNullOrWhiteSpace(report.freeGrabbableAttachPointPath))
                {
                    report.warnings.Add($"{report.path} has V_FreeGrabbable but no serialized _attachPoint was found. Some prefabs may fall back to the root transform, but handheld objects should usually have an explicit grab point.");
                }

                if (!report.hasRigidbodySyncable)
                {
                    report.warnings.Add($"{report.path} has V_FreeGrabbable but no V_RigidbodySyncable. Multiplayer rigidbody state may not sync.");
                }
            }

            if (report.hasLaserPointer)
            {
                if (!report.hasFreeGrabbable)
                {
                    report.warnings.Add($"{report.path} has V_LaserPointer but no V_FreeGrabbable. VE2's built-in LaserPointer prefab is already grabbable; prefer spawning that prefab instead of adding components manually.");
                }

                foreach (var referenceName in new[] { "raycastOrigin", "beam", "pointLight", "downLaser" })
                {
                    if (string.IsNullOrWhiteSpace(GetObjectReferencePath(laserPointer, referenceName)))
                    {
                        report.warnings.Add($"{report.path} has V_LaserPointer but serialized reference '{referenceName}' is missing.");
                    }
                }
            }

            AddActivatableAndAdjustableWarnings(gameObject, report);
            return report;
        }

        private static void AddColliderWarnings(GameObject gameObject, InteractionObjectReport report)
        {
            foreach (var collider in gameObject.GetComponentsInChildren<Collider>(true))
            {
                if (collider == null)
                {
                    continue;
                }

                var colliderPath = VE2Reflection.GetHierarchyPath(collider.transform);
                if (collider is MeshCollider meshCollider && meshCollider.sharedMesh == null)
                {
                    report.warnings.Add($"{colliderPath} has a MeshCollider with no mesh. This often happens when V_FreeGrabbable is added to an empty placeholder.");
                }
                else if (!collider.isTrigger && collider.bounds.size.sqrMagnitude <= 0.0001f)
                {
                    report.warnings.Add($"{colliderPath} has a non-trigger Collider with near-zero bounds.");
                }
            }
        }

        private static void AddActivatableAndAdjustableWarnings(GameObject gameObject, InteractionObjectReport report)
        {
            var activatableTypes = new[]
            {
                "V_ToggleActivatable",
                "V_HoldActivatable",
                "V_PressurePlateActivatable",
                "V_HandheldActivatable",
                "V_HandheldAdjustable"
            };

            if (activatableTypes.Any(typeName => GetComponentByTypeName(gameObject, typeName) != null) &&
                !HasNonTriggerColliderInSelfOrChildren(gameObject))
            {
                report.warnings.Add($"{report.path} has a VE2 activatable component but no non-trigger Collider on the same GameObject or a child GameObject.");
            }

            var adjustableTypes = new[]
            {
                "V_SlidingAdjustable",
                "V_Sliding2DAdjustable",
                "V_RotatingAdjustable",
                "V_Rotating2DAdjustable"
            };

            if (adjustableTypes.Any(typeName => GetComponentByTypeName(gameObject, typeName) != null) &&
                !HasAnyCollider(gameObject))
            {
                report.warnings.Add($"{report.path} has a VE2 adjustable component but no Collider on the same GameObject.");
            }

            var infoPoint = GetComponentByTypeName(gameObject, "V_CustomInfoPoint");
            if (infoPoint == null)
            {
                return;
            }

            var trigger = VE2Reflection.GetComponentInChildrenByTypeName(gameObject, "InfoPointTriggerAnimationHandler");
            if (trigger == null)
            {
                report.warnings.Add($"{report.path} has V_CustomInfoPoint but no InfoPointTriggerAnimationHandler child.");
            }
            else if (!HasAnyCollider(trigger.gameObject))
            {
                report.warnings.Add($"{VE2Reflection.GetHierarchyPath(trigger.transform)} is the V_CustomInfoPoint trigger but has no Collider.");
            }
        }

        private static string GetObjectReferencePath(Component component, string leafName)
        {
            if (component == null)
            {
                return null;
            }

            try
            {
                var property = FindSerializedPropertyByLeafName(new SerializedObject(component), leafName);
                if (property == null || property.propertyType != SerializedPropertyType.ObjectReference || property.objectReferenceValue == null)
                {
                    return null;
                }

                if (property.objectReferenceValue is Component referencedComponent)
                {
                    return VE2Reflection.GetHierarchyPath(referencedComponent.transform);
                }

                if (property.objectReferenceValue is GameObject referencedGameObject)
                {
                    return VE2Reflection.GetHierarchyPath(referencedGameObject.transform);
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        private static string SaveActiveScene()
        {
            var scene = SceneManager.GetActiveScene();
            if (string.IsNullOrWhiteSpace(scene.path))
            {
                if (string.IsNullOrWhiteSpace(scene.name))
                {
                    throw new InvalidOperationException("Cannot save an unnamed scene. Provide SceneName.");
                }

                return SaveActiveSceneAs(scene.name);
            }

            if (!EditorSceneManager.SaveScene(scene))
            {
                throw new InvalidOperationException($"Failed to save scene '{scene.path}'.");
            }

            return scene.path;
        }

        private static string SaveActiveSceneAs(string sceneName)
        {
            EnsureScenesFolder();

            var scene = SceneManager.GetActiveScene();
            var folder = string.IsNullOrWhiteSpace(scene.path)
                ? "Assets/Scenes"
                : Path.GetDirectoryName(scene.path)?.Replace('\\', '/') ?? "Assets/Scenes";

            var targetPath = $"{folder}/{sceneName}.unity";
            if (!string.Equals(scene.path, targetPath, StringComparison.OrdinalIgnoreCase) &&
                AssetDatabase.LoadAssetAtPath<SceneAsset>(targetPath) != null)
            {
                throw new InvalidOperationException($"A scene already exists at '{targetPath}'. Choose a different SceneName.");
            }

            if (!EditorSceneManager.SaveScene(scene, targetPath))
            {
                throw new InvalidOperationException($"Failed to save scene as '{targetPath}'.");
            }

            AssetDatabase.SaveAssets();
            return targetPath;
        }

        private static void EnsureScenesFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }
        }

        private static Component ResolveSyncConfigComponent(GameObject gameObject, string componentTypeName, out string error)
        {
            error = null;
            var components = gameObject.GetComponents<Component>().Where(component => component != null).ToArray();

            if (!string.IsNullOrWhiteSpace(componentTypeName))
            {
                var component = components.FirstOrDefault(c =>
                    string.Equals(c.GetType().Name, componentTypeName.Trim(), StringComparison.Ordinal) ||
                    string.Equals(c.GetType().FullName, componentTypeName.Trim(), StringComparison.Ordinal));

                if (component == null)
                {
                    error = $"{gameObject.name} does not contain component '{componentTypeName}'.";
                }
                else if (!HasSerializedLeaf(component, "IsNetworked"))
                {
                    error = $"{component.GetType().Name} does not expose a serialized IsNetworked field.";
                }

                return error == null ? component : null;
            }

            var candidates = components.Where(component => HasSerializedLeaf(component, "IsNetworked")).ToArray();
            if (candidates.Length == 0)
            {
                error = $"{gameObject.name} does not contain a sync-configurable VE2 component.";
                return null;
            }

            if (candidates.Length > 1)
            {
                error = $"{gameObject.name} has multiple sync-configurable components: {string.Join(", ", candidates.Select(c => c.GetType().Name))}. Provide ComponentTypeName.";
                return null;
            }

            return candidates[0];
        }

        private static bool HasSerializedLeaf(UnityEngine.Object target, string leafName)
        {
            try
            {
                return FindSerializedPropertyByLeafName(new SerializedObject(target), leafName) != null;
            }
            catch
            {
                return false;
            }
        }

        private static SerializedProperty FindSerializedPropertyByLeafName(SerializedObject serialized, string leafName)
        {
            var iterator = serialized.GetIterator();
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = true;
                if (iterator.name == leafName)
                {
                    return iterator.Copy();
                }
            }

            return null;
        }

        private static GameObject FindQuickStartNetworkIntegrationObject()
        {
            return VE2Reflection.GetAllSceneTransforms()
                .Select(transform => transform.gameObject)
                .FirstOrDefault(gameObject =>
                    gameObject.name == "NetworkIntegration" &&
                    VE2Reflection.GetComponentInChildrenByTypeName(gameObject, "V_PlatformIntegration") != null &&
                    VE2Reflection.GetComponentInChildrenByTypeName(gameObject, "V_InstanceIntegration") != null);
        }

        private static bool ActivateGameObjectAndParents(GameObject gameObject)
        {
            var changed = false;
            var current = gameObject != null ? gameObject.transform : null;
            while (current != null)
            {
                if (!current.gameObject.activeSelf)
                {
                    Undo.RecordObject(current.gameObject, "VE2 MCP Activate GameObject");
                    current.gameObject.SetActive(true);
                    changed = true;
                }

                current = current.parent;
            }

            if (changed)
            {
                VE2Reflection.MarkActiveSceneDirty();
            }

            return changed;
        }

        private static bool ActivateProviderComponent(Component component)
        {
            var changed = ActivateGameObjectAndParents(component.gameObject);
            if (component is Behaviour behaviour && !behaviour.enabled)
            {
                Undo.RecordObject(behaviour, "VE2 MCP Enable Provider Component");
                behaviour.enabled = true;
                changed = true;
            }

            if (changed)
            {
                EditorUtility.SetDirty(component);
                VE2Reflection.MarkActiveSceneDirty();
            }

            return changed;
        }

        private static bool IsComponentActive(Component component)
        {
            return component != null &&
                   component.gameObject.activeInHierarchy &&
                   (!(component is Behaviour behaviour) || behaviour.enabled);
        }

        private static object EnsureProviderInternal(string provider)
        {
            if (!ProviderResources.TryGetValue(provider, out var resourceName) || !ProviderTypes.TryGetValue(provider, out var typeName))
            {
                throw new InvalidOperationException($"Unsupported provider '{provider}'.");
            }

            Component existing = null;
            var quickStartNetworkIntegration = FindQuickStartNetworkIntegrationObject();
            if (quickStartNetworkIntegration != null && (provider == "platform_integration" || provider == "instance_integration"))
            {
                existing = VE2Reflection.GetComponentInChildrenByTypeName(quickStartNetworkIntegration, typeName);
            }

            existing ??= VE2Reflection.FindComponentsByTypeName(typeName)
                .OrderByDescending(IsComponentActive)
                .ThenBy(component => VE2Reflection.GetHierarchyPath(component.transform))
                .FirstOrDefault();

            if (existing != null)
            {
                var activated = ActivateProviderComponent(existing);
                Selection.activeGameObject = existing.gameObject;
                return new
                {
                    provider,
                    alreadyExisted = true,
                    activated,
                    reusedQuickstartNetworkIntegration = quickStartNetworkIntegration != null && existing.transform.IsChildOf(quickStartNetworkIntegration.transform),
                    componentActive = IsComponentActive(existing),
                    gameObject = VE2Reflection.GameObjectData(existing.gameObject)
                };
            }

            var created = VE2Reflection.InstantiateVE2Resource(resourceName);
            if (created == null)
            {
                throw new InvalidOperationException($"Failed to instantiate VE2 provider resource '{resourceName}'.");
            }

            Selection.activeGameObject = created;
            VE2Reflection.MarkActiveSceneDirty();

            return new
            {
                provider,
                alreadyExisted = false,
                activated = true,
                reusedQuickstartNetworkIntegration = false,
                componentActive = true,
                gameObject = VE2Reflection.GameObjectData(created),
                warning = provider == "instance_integration" && VE2Reflection.FindFirstComponentByTypeName("V_PlatformIntegration") == null
                    ? "V_InstanceIntegration requires V_PlatformIntegration in the scene."
                    : null
            };
        }

        private static string SetupUrp()
        {
            var pipelineBeforeVE2Setup = GraphicsSettings.defaultRenderPipeline;
            VE2Reflection.InvokeStatic("VE2.Common.Shared.VE2URPSetup", "SetupURP");

            var urpAsset = FindVE2UrpAsset();
            if (urpAsset == null && GraphicsSettings.defaultRenderPipeline != pipelineBeforeVE2Setup)
            {
                urpAsset = GraphicsSettings.defaultRenderPipeline;
            }

            if (urpAsset == null)
            {
                throw new InvalidOperationException("Could not find VE2 URP-HighFidelity.asset.");
            }

            GraphicsSettings.defaultRenderPipeline = urpAsset;

            var originalQualityLevel = QualitySettings.GetQualityLevel();
            for (var i = 0; i < QualitySettings.names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = urpAsset;
            }

            QualitySettings.SetQualityLevel(originalQualityLevel, false);
            AssetDatabase.SaveAssets();

            return AssetDatabase.GetAssetPath(urpAsset);
        }

        private static RenderPipelineAsset FindVE2UrpAsset()
        {
            foreach (var guid in AssetDatabase.FindAssets("URP-HighFidelity"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid).Replace('\\', '/');
                if (!path.Contains("/FRAMEWORK/") || Path.GetFileNameWithoutExtension(path) != "URP-HighFidelity")
                {
                    continue;
                }

                var asset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(path);
                if (asset != null)
                {
                    return asset;
                }
            }

            return null;
        }

        private static string CreateQuickStartSceneInternal()
        {
            const string scenesFolder = "Assets/Scenes";
            if (!AssetDatabase.IsValidFolder(scenesFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            var scenePath = AssetDatabase.GenerateUniqueAssetPath($"{scenesFolder}/VE2QuickStart.unity");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, scenePath);
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            var sceneSetupPrefab = Resources.Load<GameObject>("VE2SetupSceneHolder");
            if (sceneSetupPrefab == null)
            {
                throw new InvalidOperationException("Could not find VE2SetupSceneHolder in Resources.");
            }

            var holder = UnityEngine.Object.Instantiate(sceneSetupPrefab);
            for (var i = holder.transform.childCount - 1; i >= 0; i--)
            {
                holder.transform.GetChild(i).SetParent(null);
            }

            UnityEngine.Object.DestroyImmediate(holder);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            return scenePath;
        }

        private static string SetupQuickStartSceneInternal(VE2QuickStartSceneParams parameters, List<string> actions)
        {
            EnsureScenesFolder();

            var sceneName = ResolveQuickStartSceneName(parameters.SceneName);
            if (!Regex.IsMatch(sceneName, "^[a-zA-Z0-9]+$"))
            {
                throw new InvalidOperationException($"SceneName '{sceneName}' is invalid for VE2 deployment. Use only letters and numbers.");
            }

            var scenePath = $"Assets/Scenes/{sceneName}.unity";
            var currentScene = SceneManager.GetActiveScene();
            var targetExists = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) != null;

            if (!targetExists)
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                if (!EditorSceneManager.SaveScene(scene, scenePath))
                {
                    throw new InvalidOperationException($"Failed to create scene '{scenePath}'.");
                }

                actions.Add("createdScene:" + scenePath);
            }
            else if (!string.Equals(currentScene.path, scenePath, StringComparison.OrdinalIgnoreCase))
            {
                if (currentScene.isDirty && !string.IsNullOrWhiteSpace(currentScene.path))
                {
                    if (!EditorSceneManager.SaveScene(currentScene))
                    {
                        throw new InvalidOperationException($"Active scene '{currentScene.path}' has unsaved changes and could not be saved before opening '{scenePath}'.");
                    }

                    actions.Add("savedPreviousScene:" + currentScene.path);
                }

                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                actions.Add("openedScene:" + scenePath);
            }

            var activeScene = SceneManager.GetActiveScene();
            var existingRoots = activeScene.GetRootGameObjects();
            if (existingRoots.Length > 0)
            {
                if (!parameters.ResetExistingScene)
                {
                    throw new InvalidOperationException($"Scene '{scenePath}' already contains root GameObjects. Set ResetExistingScene=true to replace it with the VE2 quickstart template.");
                }

                foreach (var root in existingRoots)
                {
                    Undo.DestroyObjectImmediate(root);
                }

                actions.Add("clearedRootObjects:" + existingRoots.Length);
            }

            var sceneSetupPrefab = Resources.Load<GameObject>("VE2SetupSceneHolder");
            if (sceneSetupPrefab == null)
            {
                throw new InvalidOperationException("Could not find VE2SetupSceneHolder in Resources. Ensure VE2 is installed and imported correctly.");
            }

            var holder = UnityEngine.Object.Instantiate(sceneSetupPrefab);
            holder.name = "VE2SetupSceneHolder";
            var unpackedRoots = new List<string>();
            for (var i = holder.transform.childCount - 1; i >= 0; i--)
            {
                var child = holder.transform.GetChild(i);
                child.SetParent(null);
                unpackedRoots.Add(child.name);
            }

            UnityEngine.Object.DestroyImmediate(holder);
            actions.Add("unpackedVE2SetupSceneHolder:" + string.Join(",", unpackedRoots.OrderBy(name => name)));

            var networkIntegration = FindQuickStartNetworkIntegrationObject();
            if (networkIntegration != null)
            {
                var activated = ActivateGameObjectAndParents(networkIntegration);
                actions.Add(activated ? "activated:NetworkIntegration" : "alreadyActive:NetworkIntegration");
            }
            else
            {
                actions.Add("warning:NetworkIntegrationNotFoundInQuickstartTemplate");
            }

            EnsureProviderInternal("platform_integration");
            actions.Add("ensuredActive:platform_integration");
            EnsureProviderInternal("instance_integration");
            actions.Add("ensuredActive:instance_integration");

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            if (parameters.SaveScene)
            {
                if (!EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), scenePath))
                {
                    throw new InvalidOperationException($"Failed to save scene '{scenePath}'.");
                }

                actions.Add("saved:" + scenePath);
            }

            AssetDatabase.SaveAssets();
            if (parameters.SaveScene)
            {
                if (!EditorSceneManager.SaveOpenScenes())
                {
                    throw new InvalidOperationException("Failed to save open scenes after VE2 quickstart setup.");
                }

                actions.Add("savedOpenScenes");
            }

            return scenePath;
        }

        private static string ResolveQuickStartSceneName(string requestedSceneName)
        {
            if (!string.IsNullOrWhiteSpace(requestedSceneName))
            {
                return requestedSceneName.Trim();
            }

            var activeScene = SceneManager.GetActiveScene();
            if (!string.IsNullOrWhiteSpace(activeScene.name) && activeScene.name != "Untitled")
            {
                return activeScene.name.Trim();
            }

            return "VE2QuickStart";
        }

        private static void ApplyTransform(Transform transform, VE2SpawnPrefabParams parameters)
        {
            if (parameters.Position != null && parameters.Position.Length >= 3)
            {
                transform.position = new Vector3(parameters.Position[0], parameters.Position[1], parameters.Position[2]);
            }

            if (parameters.RotationEuler != null && parameters.RotationEuler.Length >= 3)
            {
                transform.rotation = Quaternion.Euler(parameters.RotationEuler[0], parameters.RotationEuler[1], parameters.RotationEuler[2]);
            }

            if (parameters.Scale != null && parameters.Scale.Length >= 3)
            {
                transform.localScale = new Vector3(parameters.Scale[0], parameters.Scale[1], parameters.Scale[2]);
            }
        }

        private static void PostProcessSpecialResource(string resourceName, GameObject gameObject)
        {
            if (gameObject == null || resourceName != "CustomInfoPoint")
            {
                return;
            }

            var trigger = VE2Reflection.GetComponentInChildrenByTypeName(gameObject, "InfoPointTriggerAnimationHandler");
            if (trigger != null)
            {
                trigger.gameObject.name = $"{gameObject.name}_Trigger";
            }

            var canvas = VE2Reflection.GetComponentInChildrenByTypeName(gameObject, "InfoPointCanvasAnimationHandler");
            if (canvas != null)
            {
                canvas.gameObject.name = $"{gameObject.name}_Canvas";
            }
        }

        private static List<string> FindScriptsWithoutAsmdef(Scene scene)
        {
            var result = new List<string>();
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var child in root.GetComponentsInChildren<Transform>(true))
                {
                    foreach (var component in child.GetComponents<MonoBehaviour>())
                    {
                        if (component == null)
                        {
                            continue;
                        }

                        var assembly = component.GetType().Assembly;
                        if (assembly == null || assembly.GetName().Name != "Assembly-CSharp")
                        {
                            continue;
                        }

                        var script = MonoScript.FromMonoBehaviour(component);
                        var assetPath = script != null ? AssetDatabase.GetAssetPath(script) : string.Empty;
                        if (!string.IsNullOrEmpty(assetPath) && assetPath.StartsWith("Assets/"))
                        {
                            result.Add($"{VE2Reflection.GetHierarchyPath(child)} -> {component.GetType().Name} ({assetPath})");
                        }
                    }
                }
            }

            return result;
        }

        private static IEnumerable<string> FindFrameworkAssetRoots()
        {
            return AssetDatabase.FindAssets("VE2API t:MonoScript")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => path.Replace('\\', '/'))
                .Where(path => path.Contains("/FRAMEWORK/"))
                .Select(path =>
                {
                    var index = path.IndexOf("/FRAMEWORK/", StringComparison.Ordinal);
                    return index >= 0 ? path.Substring(0, index + "/FRAMEWORK".Length) : path;
                })
                .Distinct()
                .OrderBy(path => path);
        }

        private static IEnumerable<string> FindAssetPaths(string requiredFolderSegment, string fileNameOrExtension)
        {
            return AssetDatabase.FindAssets("t:MonoScript")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => path.Replace('\\', '/'))
                .Where(path => path.Contains("/FRAMEWORK/"))
                .Where(path => path.Contains(requiredFolderSegment))
                .Where(path => fileNameOrExtension.StartsWith(".", StringComparison.Ordinal)
                    ? path.EndsWith(fileNameOrExtension, StringComparison.OrdinalIgnoreCase)
                    : string.Equals(Path.GetFileName(path), fileNameOrExtension, StringComparison.OrdinalIgnoreCase))
                .OrderBy(path => path);
        }

        private static string ReadAssetText(string assetPath)
        {
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
            if (script != null)
            {
                return script.text;
            }

            var text = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            if (text != null)
            {
                return text.text;
            }

            return File.Exists(assetPath) ? File.ReadAllText(assetPath) : string.Empty;
        }

        private sealed class PreflightResult
        {
            public bool valid;
            public string sceneName;
            public string scenePath;
            public bool sceneIsDirty;
            public List<string> errors;
            public List<string> warnings;
            public CompileState compileState;
            public ProviderSummary providerSummary;
            public List<string> scriptsWithoutAsmdef;
            public List<string> activatableValidation;
            public SyncValidationReport syncValidation;
        }

        private sealed class CompileState
        {
            public bool isCompiling;
            public bool? scriptCompilationFailed;
        }

        private sealed class ProviderSummary
        {
            public ComponentStatus[] platformIntegration;
            public ComponentStatus[] instanceIntegration;
            public int activePlatformIntegrationCount;
            public int inactivePlatformIntegrationCount;
            public int totalPlatformIntegrationCount;
            public int activeInstanceIntegrationCount;
            public int inactiveInstanceIntegrationCount;
            public int totalInstanceIntegrationCount;
        }

        private sealed class ComponentStatus
        {
            public string componentType;
            public string path;
            public object gameObject;
            public bool enabled;
            public bool activeSelf;
            public bool activeInHierarchy;
        }

        private sealed class InteractionObjectReport
        {
            public string path;
            public object gameObject;
            public string[] componentTypes;
            public bool hasFreeGrabbable;
            public bool hasLaserPointer;
            public bool hasRigidbody;
            public bool hasRigidbodySyncable;
            public bool hasRenderer;
            public bool hasUsableNonTriggerCollider;
            public int nonTriggerColliderCount;
            public string freeGrabbableAttachPointPath;
            public List<string> warnings;
            public List<string> errors;
        }
    }
}
