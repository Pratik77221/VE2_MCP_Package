using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Imperial.VE2.MCP.Editor
{
    public static partial class VE2McpTools
    {
        private static readonly Dictionary<string, string> ActivatableResources = new(StringComparer.OrdinalIgnoreCase)
        {
            ["toggle_button"] = "ToggleButton",
            ["toggle"] = "ToggleButton",
            ["hold_button"] = "HoldButton",
            ["hold"] = "HoldButton",
            ["pressure_plate"] = "PressurePlate",
            ["pressure"] = "PressurePlate"
        };

        private static readonly Dictionary<string, string> AdjustableResources = new(StringComparer.OrdinalIgnoreCase)
        {
            ["wheel"] = "AdjustableWheel",
            ["lever"] = "AdjustableLever",
            ["joystick_2d"] = "AdjustableJoystick2D",
            ["joystick2d"] = "AdjustableJoystick2D",
            ["slider"] = "AdjustableSlider",
            ["slider_2d"] = "AdjustableSlider2D",
            ["slider2d"] = "AdjustableSlider2D"
        };

        private static readonly string[] ActivatableComponentTypes =
        {
            "V_ToggleActivatable",
            "V_HoldActivatable",
            "V_PressurePlateActivatable",
            "V_HandheldActivatable",
            "V_CustomInfoPoint"
        };

        private static readonly string[] AdjustableComponentTypes =
        {
            "V_HandheldAdjustable",
            "V_SlidingAdjustable",
            "V_Sliding2DAdjustable",
            "V_RotatingAdjustable",
            "V_Rotating2DAdjustable"
        };

        private static readonly string[] SyncComponentTypes =
        {
            "V_ToggleActivatable",
            "V_HoldActivatable",
            "V_PressurePlateActivatable",
            "V_HandheldActivatable",
            "V_CustomInfoPoint",
            "V_FreeGrabbable",
            "V_HandheldAdjustable",
            "V_SlidingAdjustable",
            "V_Sliding2DAdjustable",
            "V_RotatingAdjustable",
            "V_Rotating2DAdjustable",
            "V_TransformSyncable",
            "V_RigidbodySyncable",
            "V_NetworkObject"
        };

        private static readonly HashSet<string> AllowedInteractionEvents = new(StringComparer.Ordinal)
        {
            "OnActivate",
            "OnDeactivate",
            "OnGrab",
            "OnDrop",
            "OnValueAdjusted"
        };

        public static object InspectObject(VE2InspectObjectParams parameters)
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

                var maxProperties = Mathf.Clamp(parameters.MaxSerializedProperties, 1, 500);
                var components = gameObject.GetComponents<Component>()
                    .Select(component => BuildComponentInspection(component, parameters.IncludeSerializedProperties, maxProperties))
                    .ToArray();

                var colliders = gameObject.GetComponentsInChildren<Collider>(true)
                    .Select(collider => new
                    {
                        path = VE2Reflection.GetHierarchyPath(collider.transform),
                        type = collider.GetType().Name,
                        enabled = collider.enabled,
                        isTrigger = collider.isTrigger
                    })
                    .ToArray();

                var rigidbody = gameObject.GetComponent<Rigidbody>();
                var syncEntries = BuildSyncEntries(gameObject).ToArray();
                var interaction = BuildInteractionObjectReport(gameObject);

                return Response.Success("Inspected VE2 scene object.", new
                {
                    target = VE2Reflection.GameObjectData(gameObject),
                    layer = LayerMask.LayerToName(gameObject.layer),
                    tag = gameObject.tag,
                    components,
                    colliders,
                    rigidbody = rigidbody == null ? null : new
                    {
                        rigidbody.isKinematic,
                        rigidbody.useGravity,
                        rigidbody.mass,
                        interpolation = rigidbody.interpolation.ToString(),
                        collisionDetection = rigidbody.collisionDetectionMode.ToString()
                    },
                    syncEntries,
                    interaction
                });
            });
        }

        public static object CreateActivatable(VE2CreateActivatableParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.Kind))
                {
                    return Response.Error("Kind is required: toggle_button, hold_button, or pressure_plate.");
                }

                if (!ActivatableResources.TryGetValue(parameters.Kind.Trim(), out var resourceName))
                {
                    return Response.Error("Kind must be toggle_button, hold_button, or pressure_plate.");
                }

                var gameObject = SpawnOfficialResource(resourceName, parameters.Name, parameters.ParentPath,
                    parameters.Position, parameters.RotationEuler, parameters.Scale);
                if (gameObject == null)
                {
                    return Response.Error($"Could not instantiate VE2 resource '{resourceName}'.");
                }

                var actions = new List<string> { "spawned:" + resourceName };
                if (parameters.SaveScene)
                {
                    actions.Add("saved:" + SaveActiveScene());
                }

                return Response.Success("Created VE2 activatable.", new
                {
                    target = VE2Reflection.GameObjectData(gameObject),
                    interaction = BuildInteractionObjectReport(gameObject),
                    actions,
                    syncValidation = VE2SyncIdUtility.Scan()
                });
            });
        }

        public static object ConfigureActivatable(VE2ConfigureActivatableParams parameters)
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

                var component = ResolveKnownComponent(gameObject, parameters.ComponentTypeName, ActivatableComponentTypes, out var resolveError);
                if (component == null)
                {
                    return Response.Error(resolveError);
                }

                if (parameters.ConfigureInteractRange && parameters.InteractRange <= 0f)
                {
                    return Response.Error("InteractRange must be greater than zero.");
                }

                Undo.RecordObject(component, "Configure VE2 Activatable");
                var serialized = new SerializedObject(component);
                serialized.Update();
                var changed = new List<string>();
                var warnings = new List<string>();

                TrySetBoolean(serialized, "AdminOnly", parameters.ConfigureAdminOnly, parameters.AdminOnly, changed, warnings);
                TrySetBoolean(serialized, "EnableControllerVibrations", parameters.ConfigureControllerVibrations,
                    parameters.EnableControllerVibrations, changed, warnings);
                TrySetBoolean(serialized, "IsInteractable", parameters.ConfigureIsInteractable,
                    parameters.IsInteractable, changed, warnings);
                TrySetFloat(serialized, "InteractionRange", parameters.ConfigureInteractRange,
                    parameters.InteractRange, changed, warnings);
                TrySetBoolean(serialized, "IsNetworked", parameters.ConfigureIsNetworked,
                    parameters.IsNetworked, changed, warnings);
                TrySetBoolean(serialized, "ActivateOnStart", parameters.ConfigureActivateOnStart,
                    parameters.ActivateOnStart, changed, warnings);

                if (parameters.ActivationGroupId != null)
                {
                    var groupId = parameters.ActivationGroupId.Trim();
                    var useGroup = !string.IsNullOrWhiteSpace(groupId) && !string.Equals(groupId, "None", StringComparison.OrdinalIgnoreCase);
                    TrySetBoolean(serialized, "UseActivationGroup", true, useGroup, changed, warnings);
                    TrySetString(serialized, "ActivationGroupID", true, useGroup ? groupId : "None", changed, warnings);
                }

                serialized.ApplyModifiedProperties();
                if (changed.Count > 0)
                {
                    EditorUtility.SetDirty(component);
                    VE2Reflection.MarkActiveSceneDirty();
                }

                if (parameters.SaveScene)
                {
                    changed.Add("saved:" + SaveActiveScene());
                }

                return Response.Success("Configured VE2 activatable.", new
                {
                    target = VE2Reflection.GameObjectData(gameObject),
                    componentType = component.GetType().Name,
                    changed = changed.Distinct().ToArray(),
                    warnings = warnings.Distinct().ToArray(),
                    interaction = BuildInteractionObjectReport(gameObject),
                    syncValidation = VE2SyncIdUtility.Scan()
                });
            });
        }

        public static object CreateAdjustable(VE2CreateAdjustableParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.Kind))
                {
                    return Response.Error("Kind is required: wheel, lever, joystick_2d, slider, or slider_2d.");
                }

                if (!AdjustableResources.TryGetValue(parameters.Kind.Trim(), out var resourceName))
                {
                    return Response.Error("Kind must be wheel, lever, joystick_2d, slider, or slider_2d.");
                }

                var gameObject = SpawnOfficialResource(resourceName, parameters.Name, parameters.ParentPath,
                    parameters.Position, parameters.RotationEuler, parameters.Scale);
                if (gameObject == null)
                {
                    return Response.Error($"Could not instantiate VE2 resource '{resourceName}'.");
                }

                var actions = new List<string> { "spawned:" + resourceName };
                if (parameters.SaveScene)
                {
                    actions.Add("saved:" + SaveActiveScene());
                }

                return Response.Success("Created VE2 adjustable.", new
                {
                    target = VE2Reflection.GameObjectData(gameObject),
                    interaction = BuildInteractionObjectReport(gameObject),
                    actions,
                    syncValidation = VE2SyncIdUtility.Scan()
                });
            });
        }

        public static object ConfigureAdjustable(VE2ConfigureAdjustableParams parameters)
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

                var component = ResolveKnownComponent(gameObject, parameters.ComponentTypeName, AdjustableComponentTypes, out var resolveError);
                if (component == null)
                {
                    return Response.Error(resolveError);
                }

                if (parameters.NumberOfValues < 0 || parameters.NumberOfValues == 1)
                {
                    return Response.Error("NumberOfValues must be zero to leave unchanged, or at least 2.");
                }

                if (!IsRangeValid(parameters.MinimumOutputValue, parameters.MaximumOutputValue) ||
                    !IsRangeValid(parameters.MinimumSpatialValue, parameters.MaximumSpatialValue))
                {
                    return Response.Error("Each minimum adjustable value must be less than or equal to its matching maximum value.");
                }

                Undo.RecordObject(component, "Configure VE2 Adjustable");
                var serialized = new SerializedObject(component);
                serialized.Update();
                var changed = new List<string>();
                var warnings = new List<string>();

                TrySetNumericArray(serialized, "StartingOutputValue", parameters.StartingValue, changed, warnings);
                TrySetNumericArray(serialized, "MinimumOutputValue", parameters.MinimumOutputValue, changed, warnings);
                TrySetNumericArray(serialized, "MaximumOutputValue", parameters.MaximumOutputValue, changed, warnings);
                TrySetNumericArray(serialized, "MinimumSpatialValue", parameters.MinimumSpatialValue, changed, warnings);
                TrySetNumericArray(serialized, "MaximumSpatialValue", parameters.MaximumSpatialValue, changed, warnings);
                TrySetInteger(serialized, "NumberOfDiscreteValues", parameters.NumberOfValues > 0,
                    parameters.NumberOfValues, changed, warnings);
                TrySetBoolean(serialized, "EmitValueOnStart", parameters.ConfigureEmitValueOnStart,
                    parameters.EmitValueOnStart, changed, warnings);
                TrySetBoolean(serialized, "AdminOnly", parameters.ConfigureAdminOnly, parameters.AdminOnly, changed, warnings);
                TrySetBoolean(serialized, "EnableControllerVibrations", parameters.ConfigureControllerVibrations,
                    parameters.EnableControllerVibrations, changed, warnings);
                TrySetBoolean(serialized, "IsInteractable", parameters.ConfigureIsInteractable,
                    parameters.IsInteractable, changed, warnings);
                TrySetFloat(serialized, "InteractionRange", parameters.ConfigureInteractRange,
                    parameters.InteractRange, changed, warnings);

                serialized.ApplyModifiedProperties();
                if (changed.Count > 0)
                {
                    EditorUtility.SetDirty(component);
                    VE2Reflection.MarkActiveSceneDirty();
                }

                if (parameters.SaveScene)
                {
                    changed.Add("saved:" + SaveActiveScene());
                }

                return Response.Success("Configured VE2 adjustable.", new
                {
                    target = VE2Reflection.GameObjectData(gameObject),
                    componentType = component.GetType().Name,
                    changed = changed.Distinct().ToArray(),
                    warnings = warnings.Distinct().ToArray(),
                    interaction = BuildInteractionObjectReport(gameObject)
                });
            });
        }

        public static object CreateInfoPoint(VE2CreateInfoPointParams parameters)
        {
            return GuardVE2(() =>
            {
                parameters ??= new VE2CreateInfoPointParams();
                var gameObject = SpawnOfficialResource("CustomInfoPoint", parameters.Name, parameters.ParentPath,
                    parameters.Position, parameters.RotationEuler, parameters.Scale);
                if (gameObject == null)
                {
                    return Response.Error("Could not instantiate VE2 resource 'CustomInfoPoint'.");
                }

                var changed = new List<string> { "spawned:CustomInfoPoint" };
                var canvasHandler = VE2Reflection.GetComponentInChildrenByTypeName(gameObject, "InfoPointCanvasAnimationHandler");
                var textRoot = canvasHandler != null ? canvasHandler.transform : gameObject.transform;

                if (parameters.Title != null)
                {
                    changed.AddRange(SetTextOnNamedChildren(textRoot, "TitleText", parameters.Title));
                }

                if (parameters.Content != null)
                {
                    changed.AddRange(SetTextOnNamedChildren(textRoot, "ContentText", parameters.Content, true));
                }

                PostProcessSpecialResource("CustomInfoPoint", gameObject);
                VE2Reflection.MarkActiveSceneDirty();
                if (parameters.SaveScene)
                {
                    changed.Add("saved:" + SaveActiveScene());
                }

                return Response.Success("Created VE2 CustomInfoPoint.", new
                {
                    target = VE2Reflection.GameObjectData(gameObject),
                    changed = changed.Distinct().ToArray(),
                    interaction = BuildInteractionObjectReport(gameObject),
                    syncValidation = VE2SyncIdUtility.Scan()
                });
            });
        }

        public static object ConnectInteractionEvent(VE2ConnectInteractionEventParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.SourceGameObjectPath) ||
                    string.IsNullOrWhiteSpace(parameters.EventName) ||
                    string.IsNullOrWhiteSpace(parameters.TargetGameObjectPath) ||
                    string.IsNullOrWhiteSpace(parameters.TargetMethodName))
                {
                    return Response.Error("SourceGameObjectPath, EventName, TargetGameObjectPath, and TargetMethodName are required.");
                }

                if (!AllowedInteractionEvents.Contains(parameters.EventName.Trim()))
                {
                    return Response.Error("EventName must be OnActivate, OnDeactivate, OnGrab, OnDrop, or OnValueAdjusted.");
                }

                var sourceObject = VE2Reflection.FindGameObjectByPath(parameters.SourceGameObjectPath);
                var targetObject = VE2Reflection.FindGameObjectByPath(parameters.TargetGameObjectPath);
                if (sourceObject == null || targetObject == null)
                {
                    return Response.Error("Could not resolve the source or target GameObject.");
                }

                var source = ResolveEventSourceComponent(sourceObject, parameters.SourceComponentTypeName,
                    parameters.EventName.Trim(), out var sourceError);
                if (source == null)
                {
                    return Response.Error(sourceError);
                }

                var serialized = new SerializedObject(source);
                var eventProperty = FindSerializedPropertyByLeafName(serialized, parameters.EventName.Trim());
                if (eventProperty == null)
                {
                    return Response.Error($"Could not find serialized event '{parameters.EventName}' on {source.GetType().Name}.");
                }

                var eventFieldType = ResolveSerializedFieldType(source.GetType(), eventProperty.propertyPath);
                var eventArgument = GetUnityEventArgumentType(eventFieldType);
                var target = ResolveEventTargetComponent(targetObject, parameters.TargetComponentTypeName,
                    parameters.TargetMethodName.Trim(), eventArgument, out var method, out var targetError);
                if (target == null || method == null)
                {
                    return Response.Error(targetError);
                }

                Undo.RecordObject(source, "Connect VE2 Interaction Event");
                var calls = eventProperty.FindPropertyRelative("m_PersistentCalls")?.FindPropertyRelative("m_Calls");
                if (calls == null || !calls.isArray)
                {
                    return Response.Error("UnityEvent persistent-call serialization was not available for this event.");
                }

                if (parameters.ReplaceExisting)
                {
                    calls.ClearArray();
                }

                for (var index = 0; index < calls.arraySize; index++)
                {
                    var existing = calls.GetArrayElementAtIndex(index);
                    var existingTarget = existing.FindPropertyRelative("m_Target")?.objectReferenceValue;
                    var existingMethod = existing.FindPropertyRelative("m_MethodName")?.stringValue;
                    if (existingTarget == target && existingMethod == method.Name)
                    {
                        return Response.Success("The requested VE2 interaction listener already exists.", new
                        {
                            source = VE2Reflection.GameObjectData(sourceObject),
                            sourceComponent = source.GetType().Name,
                            eventName = parameters.EventName,
                            target = VE2Reflection.GameObjectData(targetObject),
                            targetComponent = target.GetType().Name,
                            targetMethod = method.Name,
                            alreadyConfigured = true
                        });
                    }
                }

                calls.InsertArrayElementAtIndex(calls.arraySize);
                var call = calls.GetArrayElementAtIndex(calls.arraySize - 1);
                ConfigurePersistentCall(call, target, method, eventArgument);
                serialized.ApplyModifiedProperties();
                EditorUtility.SetDirty(source);
                VE2Reflection.MarkActiveSceneDirty();

                var actions = new List<string> { "listenerAdded" };
                if (parameters.SaveScene)
                {
                    actions.Add("saved:" + SaveActiveScene());
                }

                return Response.Success("Connected VE2 interaction event.", new
                {
                    source = VE2Reflection.GameObjectData(sourceObject),
                    sourceComponent = source.GetType().Name,
                    eventName = parameters.EventName,
                    target = VE2Reflection.GameObjectData(targetObject),
                    targetComponent = target.GetType().Name,
                    targetMethod = method.Name,
                    eventArgumentType = eventArgument?.FullName,
                    actions
                });
            });
        }

        public static object DuplicateInteractable(VE2DuplicateInteractableParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.SourceGameObjectPath))
                {
                    return Response.Error("SourceGameObjectPath is required.");
                }

                var source = VE2Reflection.FindGameObjectByPath(parameters.SourceGameObjectPath);
                if (source == null)
                {
                    return Response.Error($"Could not find GameObject '{parameters.SourceGameObjectPath}'.");
                }

                if (!ContainsKnownComponent(source, ActivatableComponentTypes.Concat(AdjustableComponentTypes).Append("V_FreeGrabbable")))
                {
                    return Response.Error("The source hierarchy does not contain a recognized VE2 interaction component.");
                }

                var parent = string.IsNullOrWhiteSpace(parameters.ParentPath)
                    ? source.transform.parent
                    : VE2Reflection.FindGameObjectByPath(parameters.ParentPath)?.transform;
                if (!string.IsNullOrWhiteSpace(parameters.ParentPath) && parent == null)
                {
                    return Response.Error($"Could not find parent GameObject '{parameters.ParentPath}'.");
                }

                var duplicate = UnityEngine.Object.Instantiate(source, parent);
                Undo.RegisterCreatedObjectUndo(duplicate, "Duplicate VE2 Interactable");
                duplicate.name = VE2Reflection.MakeUniqueSceneName(
                    string.IsNullOrWhiteSpace(parameters.Name) ? source.name : parameters.Name.Trim(), duplicate);

                if (parameters.PositionOffset != null && parameters.PositionOffset.Length >= 3)
                {
                    duplicate.transform.position += new Vector3(parameters.PositionOffset[0], parameters.PositionOffset[1], parameters.PositionOffset[2]);
                }

                var renamed = EnsureUniqueSyncableNames(duplicate).ToArray();
                if (VE2Reflection.GetComponentInChildrenByTypeName(duplicate, "V_CustomInfoPoint") != null)
                {
                    PostProcessSpecialResource("CustomInfoPoint", duplicate);
                }

                Selection.activeGameObject = duplicate;
                VE2Reflection.MarkActiveSceneDirty();
                var actions = new List<string> { "duplicated" };
                if (parameters.SaveScene)
                {
                    actions.Add("saved:" + SaveActiveScene());
                }

                return Response.Success("Duplicated VE2 interactable with unique syncable names.", new
                {
                    source = VE2Reflection.GameObjectData(source),
                    duplicate = VE2Reflection.GameObjectData(duplicate),
                    renamed,
                    actions,
                    syncValidation = VE2SyncIdUtility.Scan()
                });
            });
        }

        public static object ConfigurePlayer(VE2ConfigurePlayerParams parameters)
        {
            return GuardVE2(() =>
            {
                parameters ??= new VE2ConfigurePlayerParams();
                var spawner = ResolvePlayerSpawner(parameters.PlayerSpawnerPath, out var resolveError);
                if (spawner == null)
                {
                    return Response.Error(resolveError);
                }

                if (parameters.ConfigureTeleportRangeMultiplier && parameters.TeleportRangeMultiplier <= 0f)
                {
                    return Response.Error("TeleportRangeMultiplier must be greater than zero.");
                }

                if (parameters.ConfigureNearClippingPlane && parameters.NearClippingPlane <= 0f)
                {
                    return Response.Error("NearClippingPlane must be greater than zero.");
                }

                if (parameters.ConfigureFarClippingPlane && parameters.FarClippingPlane <= 0f)
                {
                    return Response.Error("FarClippingPlane must be greater than zero.");
                }

                if (parameters.ConfigureTransmissionFrequency &&
                    (parameters.TransmissionFrequency < 3f || parameters.TransmissionFrequency > 15f))
                {
                    return Response.Error("Player TransmissionFrequency must be between 3 and 15 Hz.");
                }

                Undo.RecordObject(spawner, "Configure VE2 Player Spawner");
                var serialized = new SerializedObject(spawner);
                serialized.Update();
                var changed = new List<string>();
                var warnings = new List<string>();

                if (!string.IsNullOrWhiteSpace(parameters.SupportedPlayerModes))
                {
                    TrySetEnum(serialized, "SupportedPlayerModes", parameters.SupportedPlayerModes.Trim(), changed, warnings);
                }

                TrySetLayerMask(serialized, "InteractableLayers", parameters.InteractableLayers, changed, warnings);
                TrySetLayerMask(serialized, "TraversableLayers", parameters.TraversableLayers, changed, warnings);
                TrySetLayerMask(serialized, "CollisionLayers", parameters.CollisionLayers, changed, warnings);
                TrySetBoolean(serialized, "FreeFlyMode", parameters.ConfigureFreeFlyMode, parameters.FreeFlyMode, changed, warnings);
                TrySetFloat(serialized, "TeleportRangeMultiplier", parameters.ConfigureTeleportRangeMultiplier,
                    parameters.TeleportRangeMultiplier, changed, warnings);
                TrySetFloat(serialized, "MaxVerticalDragHeight", parameters.ConfigureMaxVerticalDragHeight,
                    parameters.MaxVerticalDragHeight, changed, warnings);
                TrySetFloat(serialized, "FieldOfView2D", parameters.ConfigureFieldOfView2D,
                    parameters.FieldOfView2D, changed, warnings);
                TrySetFloat(serialized, "NearClippingPlane", parameters.ConfigureNearClippingPlane,
                    parameters.NearClippingPlane, changed, warnings);
                TrySetFloat(serialized, "FarClippingPlane", parameters.ConfigureFarClippingPlane,
                    parameters.FarClippingPlane, changed, warnings);
                TrySetBoolean(serialized, "EnablePostProcessing", parameters.ConfigurePostProcessing,
                    parameters.EnablePostProcessing, changed, warnings);
                TrySetBoolean(serialized, "OcclusionCulling", parameters.ConfigureOcclusionCulling,
                    parameters.OcclusionCulling, changed, warnings);
                TrySetBoolean(serialized, "PreferVRMode", parameters.ConfigurePreferVRMode,
                    parameters.PreferVRMode, changed, warnings);
                TrySetFloat(serialized, "TransmissionFrequency", parameters.ConfigureTransmissionFrequency,
                    parameters.TransmissionFrequency, changed, warnings);

                if (!string.IsNullOrWhiteSpace(parameters.TransmissionProtocol))
                {
                    TrySetEnum(serialized, "TransmissionType", parameters.TransmissionProtocol.Trim().ToUpperInvariant(), changed, warnings);
                }

                serialized.ApplyModifiedProperties();
                if (changed.Count > 0)
                {
                    EditorUtility.SetDirty(spawner);
                    VE2Reflection.MarkActiveSceneDirty();
                }

                if (parameters.SaveScene)
                {
                    changed.Add("saved:" + SaveActiveScene());
                }

                return Response.Success("Configured VE2 player spawner.", new
                {
                    target = VE2Reflection.GameObjectData(spawner.gameObject),
                    changed = changed.Distinct().ToArray(),
                    warnings = warnings.Distinct().ToArray(),
                    validation = BuildPlayerValidation()
                });
            });
        }

        public static object ValidatePlayer()
        {
            return GuardVE2(() =>
            {
                var validation = BuildPlayerValidation();
                return Response.Success(validation.valid
                    ? "VE2 player configuration is valid."
                    : "VE2 player configuration has issues.", validation);
            });
        }

        public static object CreateTeleportAnchor(VE2CreateTeleportAnchorParams parameters)
        {
            return GuardVE2(() =>
            {
                parameters ??= new VE2CreateTeleportAnchorParams();
                if (parameters.Range < 0.75f || parameters.Range > 2.5f)
                {
                    return Response.Error("Teleport anchor Range must be between 0.75 and 2.5 metres.");
                }

                var gameObject = SpawnOfficialResource("TeleportAnchor", parameters.Name, parameters.ParentPath,
                    parameters.Position, null, null);
                if (gameObject == null)
                {
                    return Response.Error("Could not instantiate VE2 resource 'TeleportAnchor'.");
                }

                var anchor = VE2Reflection.GetComponentInChildrenByTypeName(gameObject, "V_TeleportAnchor");
                if (anchor == null)
                {
                    return Response.Error("Spawned TeleportAnchor does not contain V_TeleportAnchor.");
                }

                Undo.RecordObject(anchor, "Configure VE2 Teleport Anchor");
                var serialized = new SerializedObject(anchor);
                var range = FindSerializedPropertyByLeafName(serialized, "Range");
                if (range == null || range.propertyType != SerializedPropertyType.Float)
                {
                    return Response.Error("Could not find the VE2 TeleportAnchor Range property.");
                }

                range.floatValue = parameters.Range;
                serialized.ApplyModifiedProperties();
                EditorUtility.SetDirty(anchor);
                VE2Reflection.MarkActiveSceneDirty();
                var actions = new List<string> { "range:" + parameters.Range };
                if (parameters.SaveScene)
                {
                    actions.Add("saved:" + SaveActiveScene());
                }

                return Response.Success("Created VE2 teleport anchor.", new
                {
                    target = VE2Reflection.GameObjectData(gameObject),
                    range = parameters.Range,
                    actions,
                    validation = BuildTeleportValidation()
                });
            });
        }

        public static object ValidateTeleportation()
        {
            return GuardVE2(() =>
            {
                var validation = BuildTeleportValidation();
                return Response.Success(validation.valid
                    ? "VE2 teleportation configuration is valid."
                    : "VE2 teleportation configuration has issues.", validation);
            });
        }

        public static object MakeNetworked(VE2MakeNetworkedParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.GameObjectPath) ||
                    string.IsNullOrWhiteSpace(parameters.Kind))
                {
                    return Response.Error("GameObjectPath and Kind are required.");
                }

                if (parameters.TransmissionFrequency < 0.2f || parameters.TransmissionFrequency > 50f)
                {
                    return Response.Error("TransmissionFrequency must be between 0.2 and 50 Hz.");
                }

                var protocol = (parameters.TransmissionProtocol ?? "UDP").Trim().ToUpperInvariant();
                if (protocol != "UDP" && protocol != "TCP")
                {
                    return Response.Error("TransmissionProtocol must be UDP or TCP.");
                }

                var typeName = parameters.Kind.Trim().ToLowerInvariant() switch
                {
                    "transform" => "V_TransformSyncable",
                    "rigidbody" => "V_RigidbodySyncable",
                    "network_object" => "V_NetworkObject",
                    "networkobject" => "V_NetworkObject",
                    _ => null
                };
                if (typeName == null)
                {
                    return Response.Error("Kind must be transform, rigidbody, or network_object.");
                }

                var gameObject = VE2Reflection.FindGameObjectByPath(parameters.GameObjectPath);
                if (gameObject == null)
                {
                    return Response.Error($"Could not find GameObject '{parameters.GameObjectPath}'.");
                }

                var actions = new List<string>();
                if (typeName == "V_RigidbodySyncable" && parameters.EnsureRigidbody && gameObject.GetComponent<Rigidbody>() == null)
                {
                    var body = Undo.AddComponent<Rigidbody>(gameObject);
                    body.interpolation = RigidbodyInterpolation.Interpolate;
                    actions.Add("added:Rigidbody");
                }

                var component = GetComponentByTypeName(gameObject, typeName);
                if (component == null)
                {
                    var type = VE2Reflection.FindType(typeName);
                    if (type == null)
                    {
                        return Response.Error($"Could not find VE2 component type '{typeName}'.");
                    }

                    component = Undo.AddComponent(gameObject, type) as Component;
                    actions.Add("added:" + typeName);
                }
                else
                {
                    actions.Add("alreadyHad:" + typeName);
                }

                if (component == null)
                {
                    return Response.Error($"Could not add VE2 component '{typeName}'.");
                }

                gameObject.name = VE2Reflection.MakeUniqueSceneName(gameObject.name, gameObject);
                Undo.RecordObject(component, "Configure VE2 Network Component");
                var serialized = new SerializedObject(component);
                serialized.Update();
                var warnings = new List<string>();
                TrySetBoolean(serialized, "IsNetworked", true, parameters.IsNetworked, actions, warnings);
                TrySetFloat(serialized, "TransmissionFrequency", true, parameters.TransmissionFrequency, actions, warnings);
                TrySetEnum(serialized, "TransmissionType", protocol, actions, warnings);
                TrySetBoolean(serialized, "NonHostsCanModifyTransform",
                    typeName == "V_TransformSyncable" && parameters.ConfigureNonHostsCanModifyTransform,
                    parameters.NonHostsCanModifyTransform, actions, warnings);
                serialized.ApplyModifiedProperties();

                EditorUtility.SetDirty(component);
                EditorUtility.SetDirty(gameObject);
                VE2Reflection.MarkActiveSceneDirty();
                if (parameters.SaveScene)
                {
                    actions.Add("saved:" + SaveActiveScene());
                }

                return Response.Success("Configured VE2 network component.", new
                {
                    target = VE2Reflection.GameObjectData(gameObject),
                    componentType = typeName,
                    actions = actions.Distinct().ToArray(),
                    warnings = warnings.Distinct().ToArray(),
                    syncValidation = VE2SyncIdUtility.Scan()
                });
            });
        }

        private static GameObject SpawnOfficialResource(string resourceName, string requestedName, string parentPath,
            float[] position, float[] rotationEuler, float[] scale)
        {
            var gameObject = VE2Reflection.InstantiateVE2Resource(resourceName);
            if (gameObject == null)
            {
                return null;
            }

            Undo.RegisterCreatedObjectUndo(gameObject, "Create VE2 " + resourceName);
            if (!string.IsNullOrWhiteSpace(requestedName))
            {
                gameObject.name = VE2Reflection.MakeUniqueSceneName(requestedName.Trim(), gameObject);
            }

            if (!string.IsNullOrWhiteSpace(parentPath))
            {
                var parent = VE2Reflection.FindGameObjectByPath(parentPath);
                if (parent == null)
                {
                    Undo.DestroyObjectImmediate(gameObject);
                    return null;
                }

                Undo.SetTransformParent(gameObject.transform, parent.transform, "Parent VE2 Object");
            }

            ApplyTransform(gameObject.transform, new VE2SpawnPrefabParams
            {
                Position = position,
                RotationEuler = rotationEuler,
                Scale = scale
            });
            PostProcessSpecialResource(resourceName, gameObject);
            Selection.activeGameObject = gameObject;
            EditorUtility.SetDirty(gameObject);
            VE2Reflection.MarkActiveSceneDirty();
            return gameObject;
        }

        private static object BuildComponentInspection(Component component, bool includeProperties, int maxProperties)
        {
            if (component == null)
            {
                return new { missingScript = true };
            }

            var behaviour = component as Behaviour;
            return new
            {
                type = component.GetType().Name,
                fullType = component.GetType().FullName,
                assembly = component.GetType().Assembly.GetName().Name,
                enabled = behaviour == null ? (bool?)null : behaviour.enabled,
                serializedProperties = includeProperties
                    ? ReadSerializedProperties(component, maxProperties).ToArray()
                    : Array.Empty<object>()
            };
        }

        private static IEnumerable<object> ReadSerializedProperties(UnityEngine.Object target, int maxProperties)
        {
            SerializedObject serialized;
            try
            {
                serialized = new SerializedObject(target);
            }
            catch
            {
                yield break;
            }

            var iterator = serialized.GetIterator();
            var enterChildren = true;
            var count = 0;
            while (iterator.NextVisible(enterChildren) && count < maxProperties)
            {
                enterChildren = false;
                if (iterator.propertyPath == "m_Script")
                {
                    continue;
                }

                count++;
                yield return new
                {
                    path = iterator.propertyPath,
                    type = iterator.propertyType.ToString(),
                    value = SerializedPropertyValue(iterator)
                };
            }
        }

        private static object SerializedPropertyValue(SerializedProperty property)
        {
            try
            {
                return property.propertyType switch
                {
                    SerializedPropertyType.Boolean => property.boolValue,
                    SerializedPropertyType.Integer => property.longValue,
                    SerializedPropertyType.Float => property.doubleValue,
                    SerializedPropertyType.String => property.stringValue,
                    SerializedPropertyType.Enum => property.enumDisplayNames.Length > property.enumValueIndex && property.enumValueIndex >= 0
                        ? property.enumDisplayNames[property.enumValueIndex]
                        : property.enumValueIndex,
                    SerializedPropertyType.Vector2 => new[] { property.vector2Value.x, property.vector2Value.y },
                    SerializedPropertyType.Vector3 => new[] { property.vector3Value.x, property.vector3Value.y, property.vector3Value.z },
                    SerializedPropertyType.Vector4 => new[] { property.vector4Value.x, property.vector4Value.y, property.vector4Value.z, property.vector4Value.w },
                    SerializedPropertyType.Quaternion => new[] { property.quaternionValue.x, property.quaternionValue.y, property.quaternionValue.z, property.quaternionValue.w },
                    SerializedPropertyType.Color => new[] { property.colorValue.r, property.colorValue.g, property.colorValue.b, property.colorValue.a },
                    SerializedPropertyType.ObjectReference => ObjectReferenceValue(property.objectReferenceValue),
                    _ => null
                };
            }
            catch
            {
                return null;
            }
        }

        private static object ObjectReferenceValue(UnityEngine.Object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is Component component)
            {
                return new { type = value.GetType().Name, path = VE2Reflection.GetHierarchyPath(component.transform) };
            }

            if (value is GameObject gameObject && gameObject.scene.IsValid())
            {
                return new { type = "GameObject", path = VE2Reflection.GetHierarchyPath(gameObject.transform) };
            }

            return new { type = value.GetType().Name, name = value.name, assetPath = AssetDatabase.GetAssetPath(value) };
        }

        private static IEnumerable<object> BuildSyncEntries(GameObject root)
        {
            var prefixes = new Dictionary<string, string>
            {
                ["V_ToggleActivatable"] = "Activatable-",
                ["V_CustomInfoPoint"] = "Activatable-",
                ["V_HoldActivatable"] = "HoldActivatable-",
                ["V_PressurePlateActivatable"] = "HoldActivatable-",
                ["V_HandheldActivatable"] = "Activatable-",
                ["V_FreeGrabbable"] = "Grabbable-",
                ["V_TransformSyncable"] = "TS-",
                ["V_RigidbodySyncable"] = "RBS-",
                ["V_NetworkObject"] = "NetObj-"
            };

            foreach (var component in root.GetComponentsInChildren<Component>(true).Where(c => c != null))
            {
                if (!prefixes.TryGetValue(component.GetType().Name, out var prefix))
                {
                    continue;
                }

                yield return new
                {
                    id = prefix + component.gameObject.name,
                    componentType = component.GetType().Name,
                    path = VE2Reflection.GetHierarchyPath(component.transform)
                };
            }
        }

        private static Component ResolveKnownComponent(GameObject gameObject, string requestedType,
            IEnumerable<string> allowedTypes, out string error)
        {
            error = null;
            var allowed = allowedTypes.ToArray();
            if (!string.IsNullOrWhiteSpace(requestedType))
            {
                if (!allowed.Contains(requestedType.Trim(), StringComparer.Ordinal))
                {
                    error = $"ComponentTypeName must be one of: {string.Join(", ", allowed)}.";
                    return null;
                }

                var selected = VE2Reflection.GetComponentInChildrenByTypeName(gameObject, requestedType.Trim());
                if (selected == null)
                {
                    error = $"Could not find {requestedType.Trim()} on '{gameObject.name}' or its children.";
                }

                return selected;
            }

            var matches = allowed
                .Select(typeName => VE2Reflection.GetComponentInChildrenByTypeName(gameObject, typeName))
                .Where(component => component != null)
                .Distinct()
                .ToArray();
            if (matches.Length == 0)
            {
                error = $"Could not find a supported VE2 component on '{gameObject.name}'.";
                return null;
            }

            if (matches.Length > 1)
            {
                error = "Multiple supported VE2 components were found. Provide ComponentTypeName explicitly: " +
                        string.Join(", ", matches.Select(match => match.GetType().Name));
                return null;
            }

            return matches[0];
        }

        private static void TrySetBoolean(SerializedObject serialized, string leafName, bool configure, bool value,
            ICollection<string> changed, ICollection<string> warnings)
        {
            if (!configure)
            {
                return;
            }

            var property = FindSerializedPropertyByLeafName(serialized, leafName);
            if (property == null || property.propertyType != SerializedPropertyType.Boolean)
            {
                warnings.Add($"Could not find boolean {leafName} on {serialized.targetObject.GetType().Name}.");
                return;
            }

            property.boolValue = value;
            changed.Add(leafName);
        }

        private static void TrySetFloat(SerializedObject serialized, string leafName, bool configure, float value,
            ICollection<string> changed, ICollection<string> warnings)
        {
            if (!configure)
            {
                return;
            }

            var property = FindSerializedPropertyByLeafName(serialized, leafName);
            if (property == null || property.propertyType != SerializedPropertyType.Float)
            {
                warnings.Add($"Could not find float {leafName} on {serialized.targetObject.GetType().Name}.");
                return;
            }

            property.floatValue = value;
            changed.Add(leafName);
        }

        private static void TrySetInteger(SerializedObject serialized, string leafName, bool configure, int value,
            ICollection<string> changed, ICollection<string> warnings)
        {
            if (!configure)
            {
                return;
            }

            var property = FindSerializedPropertyByLeafName(serialized, leafName);
            if (property == null || property.propertyType != SerializedPropertyType.Integer)
            {
                warnings.Add($"Could not find integer {leafName} on {serialized.targetObject.GetType().Name}.");
                return;
            }

            property.intValue = value;
            changed.Add(leafName);
        }

        private static void TrySetString(SerializedObject serialized, string leafName, bool configure, string value,
            ICollection<string> changed, ICollection<string> warnings)
        {
            if (!configure)
            {
                return;
            }

            var property = FindSerializedPropertyByLeafName(serialized, leafName);
            if (property == null || property.propertyType != SerializedPropertyType.String)
            {
                warnings.Add($"Could not find string {leafName} on {serialized.targetObject.GetType().Name}.");
                return;
            }

            property.stringValue = value ?? string.Empty;
            changed.Add(leafName);
        }

        private static void TrySetEnum(SerializedObject serialized, string leafName, string enumName,
            ICollection<string> changed, ICollection<string> warnings)
        {
            var property = FindSerializedPropertyByLeafName(serialized, leafName);
            if (property == null || property.propertyType != SerializedPropertyType.Enum)
            {
                warnings.Add($"Could not find enum {leafName} on {serialized.targetObject.GetType().Name}.");
                return;
            }

            var index = Array.FindIndex(property.enumNames,
                name => string.Equals(name, enumName, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                warnings.Add($"{leafName} does not support '{enumName}'. Allowed values: {string.Join(", ", property.enumNames)}.");
                return;
            }

            property.enumValueIndex = index;
            changed.Add(leafName);
        }

        private static void TrySetNumericArray(SerializedObject serialized, string leafName, float[] value,
            ICollection<string> changed, ICollection<string> warnings)
        {
            if (value == null)
            {
                return;
            }

            var property = FindSerializedPropertyByLeafName(serialized, leafName);
            if (property == null)
            {
                warnings.Add($"Could not find {leafName} on {serialized.targetObject.GetType().Name}.");
                return;
            }

            if (property.propertyType == SerializedPropertyType.Float && value.Length >= 1)
            {
                property.floatValue = value[0];
                changed.Add(leafName);
                return;
            }

            if (property.propertyType == SerializedPropertyType.Vector2 && value.Length >= 2)
            {
                property.vector2Value = new Vector2(value[0], value[1]);
                changed.Add(leafName);
                return;
            }

            warnings.Add($"{leafName} expects {(property.propertyType == SerializedPropertyType.Vector2 ? "[x,y]" : "[value]")}.");
        }

        private static bool IsRangeValid(float[] minimum, float[] maximum)
        {
            if (minimum == null || maximum == null)
            {
                return true;
            }

            if (minimum.Length == 0 || maximum.Length == 0 || minimum.Length != maximum.Length)
            {
                return false;
            }

            for (var index = 0; index < minimum.Length; index++)
            {
                if (minimum[index] > maximum[index])
                {
                    return false;
                }
            }

            return true;
        }

        private static IEnumerable<string> SetTextOnNamedChildren(Transform root, string name, string text,
            bool includeNumberedCopies = false)
        {
            if (root == null)
            {
                yield break;
            }

            foreach (var child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name != name && (!includeNumberedCopies || !child.name.StartsWith(name + " (", StringComparison.Ordinal)))
                {
                    continue;
                }

                foreach (var component in child.GetComponents<Component>().Where(component => component != null))
                {
                    var serialized = new SerializedObject(component);
                    var textProperty = serialized.FindProperty("m_text");
                    if (textProperty == null || textProperty.propertyType != SerializedPropertyType.String)
                    {
                        continue;
                    }

                    Undo.RecordObject(component, "Set VE2 InfoPoint Text");
                    textProperty.stringValue = text ?? string.Empty;
                    serialized.ApplyModifiedProperties();
                    EditorUtility.SetDirty(component);
                    yield return "text:" + VE2Reflection.GetHierarchyPath(child);
                }
            }
        }

        private static Component ResolveEventSourceComponent(GameObject sourceObject, string requestedType,
            string eventName, out string error)
        {
            error = null;
            var candidates = sourceObject.GetComponentsInChildren<Component>(true)
                .Where(component => component != null &&
                                    ActivatableComponentTypes.Concat(AdjustableComponentTypes).Append("V_FreeGrabbable")
                                        .Contains(component.GetType().Name))
                .Where(component => string.IsNullOrWhiteSpace(requestedType) || component.GetType().Name == requestedType.Trim())
                .Where(component => FindSerializedPropertyByLeafName(new SerializedObject(component), eventName) != null)
                .ToArray();
            if (candidates.Length == 0)
            {
                error = $"No supported VE2 interaction component with serialized event '{eventName}' was found.";
                return null;
            }

            if (candidates.Length > 1)
            {
                error = "Multiple event sources were found. Provide SourceComponentTypeName explicitly: " +
                        string.Join(", ", candidates.Select(candidate => candidate.GetType().Name));
                return null;
            }

            return candidates[0];
        }

        private static Type ResolveSerializedFieldType(Type rootType, string propertyPath)
        {
            var current = rootType;
            foreach (var segment in propertyPath.Split('.'))
            {
                if (segment == "Array" || segment.StartsWith("data[", StringComparison.Ordinal))
                {
                    return null;
                }

                var field = FindFieldInHierarchy(current, segment);
                if (field == null)
                {
                    return null;
                }

                current = field.FieldType;
            }

            return current;
        }

        private static FieldInfo FindFieldInHierarchy(Type type, string name)
        {
            while (type != null)
            {
                var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (field != null)
                {
                    return field;
                }

                type = type.BaseType;
            }

            return null;
        }

        private static Type GetUnityEventArgumentType(Type eventType)
        {
            var current = eventType;
            while (current != null)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition().Name.StartsWith("UnityEvent`", StringComparison.Ordinal))
                {
                    var arguments = current.GetGenericArguments();
                    return arguments.Length == 1 ? arguments[0] : null;
                }

                current = current.BaseType;
            }

            return null;
        }

        private static Component ResolveEventTargetComponent(GameObject targetObject, string requestedType,
            string methodName, Type eventArgument, out MethodInfo selectedMethod, out string error)
        {
            selectedMethod = null;
            error = null;
            var candidates = new List<(Component component, MethodInfo method)>();
            foreach (var component in targetObject.GetComponents<Component>().Where(component => component != null))
            {
                if (!string.IsNullOrWhiteSpace(requestedType) &&
                    component.GetType().Name != requestedType.Trim() && component.GetType().FullName != requestedType.Trim())
                {
                    continue;
                }

                foreach (var method in component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (method.Name != methodName || method.IsSpecialName || method.IsGenericMethod || method.ReturnType != typeof(void))
                    {
                        continue;
                    }

                    var methodParameters = method.GetParameters();
                    if (methodParameters.Length == 0 ||
                        (eventArgument != null && methodParameters.Length == 1 && methodParameters[0].ParameterType == eventArgument))
                    {
                        candidates.Add((component, method));
                    }
                }
            }

            if (candidates.Count == 0)
            {
                error = eventArgument == null
                    ? $"No public void {methodName}() method was found on the target."
                    : $"No public void {methodName}() or {methodName}({eventArgument.Name}) method was found on the target.";
                return null;
            }

            if (candidates.Count > 1)
            {
                error = "Multiple compatible target methods were found. Provide TargetComponentTypeName explicitly: " +
                        string.Join(", ", candidates.Select(candidate => candidate.component.GetType().Name).Distinct());
                return null;
            }

            selectedMethod = candidates[0].method;
            return candidates[0].component;
        }

        private static void ConfigurePersistentCall(SerializedProperty call, Component target, MethodInfo method,
            Type eventArgument)
        {
            call.FindPropertyRelative("m_Target").objectReferenceValue = target;
            call.FindPropertyRelative("m_TargetAssemblyTypeName").stringValue = target.GetType().AssemblyQualifiedName;
            call.FindPropertyRelative("m_MethodName").stringValue = method.Name;
            call.FindPropertyRelative("m_Mode").enumValueIndex = method.GetParameters().Length == 0 ? 1 : 0;
            call.FindPropertyRelative("m_CallState").enumValueIndex = 2;

            var arguments = call.FindPropertyRelative("m_Arguments");
            if (arguments == null)
            {
                return;
            }

            var objectType = arguments.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
            if (objectType != null)
            {
                objectType.stringValue = eventArgument?.AssemblyQualifiedName ?? typeof(UnityEngine.Object).AssemblyQualifiedName;
            }
        }

        private static bool ContainsKnownComponent(GameObject root, IEnumerable<string> typeNames)
        {
            var names = new HashSet<string>(typeNames, StringComparer.Ordinal);
            return root.GetComponentsInChildren<Component>(true)
                .Any(component => component != null && names.Contains(component.GetType().Name));
        }

        private static IEnumerable<object> EnsureUniqueSyncableNames(GameObject root)
        {
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (!transform.GetComponents<Component>().Any(component =>
                        component != null && SyncComponentTypes.Contains(component.GetType().Name)))
                {
                    continue;
                }

                var oldName = transform.name;
                var newName = VE2Reflection.MakeUniqueSceneName(oldName, transform.gameObject);
                if (oldName == newName)
                {
                    continue;
                }

                Undo.RecordObject(transform.gameObject, "Repair VE2 Syncable Name");
                transform.name = newName;
                yield return new
                {
                    oldName,
                    newName,
                    path = VE2Reflection.GetHierarchyPath(transform)
                };
            }
        }

        private static Component ResolvePlayerSpawner(string requestedPath, out string error)
        {
            error = null;
            if (!string.IsNullOrWhiteSpace(requestedPath))
            {
                var gameObject = VE2Reflection.FindGameObjectByPath(requestedPath);
                if (gameObject == null)
                {
                    error = $"Could not find PlayerSpawner GameObject '{requestedPath}'.";
                    return null;
                }

                var component = VE2Reflection.GetComponentInChildrenByTypeName(gameObject, "V_PlayerSpawner");
                if (component == null)
                {
                    error = $"'{requestedPath}' does not contain V_PlayerSpawner.";
                }

                return component;
            }

            var spawners = VE2Reflection.FindComponentsByTypeName("V_PlayerSpawner").ToArray();
            if (spawners.Length == 0)
            {
                error = "No V_PlayerSpawner was found. Use ve2_scene_setup_quickstart or ve2_scene_ensure_provider.";
                return null;
            }

            if (spawners.Length > 1)
            {
                error = "Multiple V_PlayerSpawner components were found. Provide PlayerSpawnerPath explicitly.";
                return null;
            }

            return spawners[0];
        }

        private static void TrySetLayerMask(SerializedObject serialized, string leafName, string[] layerNames,
            ICollection<string> changed, ICollection<string> warnings)
        {
            if (layerNames == null)
            {
                return;
            }

            var mask = 0;
            foreach (var layerName in layerNames.Where(name => !string.IsNullOrWhiteSpace(name)))
            {
                var layer = LayerMask.NameToLayer(layerName.Trim());
                if (layer < 0)
                {
                    warnings.Add($"Unity layer '{layerName}' does not exist and was not included in {leafName}.");
                    continue;
                }

                mask |= 1 << layer;
            }

            var property = FindSerializedPropertyByLeafName(serialized, leafName);
            if (property == null || property.propertyType != SerializedPropertyType.Integer)
            {
                warnings.Add($"Could not find LayerMask {leafName} on {serialized.targetObject.GetType().Name}.");
                return;
            }

            property.intValue = mask;
            changed.Add(leafName);
        }

        private static PlayerValidationResult BuildPlayerValidation()
        {
            var spawners = VE2Reflection.FindComponentsByTypeName("V_PlayerSpawner").ToArray();
            var errors = new List<string>();
            var warnings = new List<string>();
            if (spawners.Length == 0)
            {
                errors.Add("No V_PlayerSpawner exists in the active scene.");
            }
            else if (spawners.Length > 1)
            {
                errors.Add($"Expected one V_PlayerSpawner but found {spawners.Length}.");
            }

            var activeCount = spawners.Count(IsComponentActive);
            if (spawners.Length > 0 && activeCount != 1)
            {
                errors.Add($"Expected one active V_PlayerSpawner but found {activeCount}.");
            }

            var details = new List<object>();
            foreach (var spawner in spawners)
            {
                var serialized = new SerializedObject(spawner);
                var modes = ReadSerializedLeaf(serialized, "SupportedPlayerModes");
                var interactableMask = ReadSerializedInteger(serialized, "InteractableLayers");
                var traversableMask = ReadSerializedInteger(serialized, "TraversableLayers");
                var collisionMask = ReadSerializedInteger(serialized, "CollisionLayers");
                var near = ReadSerializedFloat(serialized, "NearClippingPlane");
                var far = ReadSerializedFloat(serialized, "FarClippingPlane");
                var frequency = ReadSerializedFloat(serialized, "TransmissionFrequency");

                if (interactableMask == 0)
                {
                    errors.Add($"{spawner.gameObject.name} has no InteractableLayers.");
                }

                if (traversableMask == 0)
                {
                    warnings.Add($"{spawner.gameObject.name} has no TraversableLayers, so VR teleport surfaces will not be found.");
                }

                if (collisionMask == 0)
                {
                    warnings.Add($"{spawner.gameObject.name} has no CollisionLayers.");
                }

                if (near.HasValue && far.HasValue && near.Value >= far.Value)
                {
                    errors.Add($"{spawner.gameObject.name} NearClippingPlane must be less than FarClippingPlane.");
                }

                if (frequency.HasValue && (frequency.Value < 3f || frequency.Value > 15f))
                {
                    warnings.Add($"{spawner.gameObject.name} player TransmissionFrequency is outside VE2's documented 3-15 Hz range.");
                }

                details.Add(new
                {
                    target = VE2Reflection.GameObjectData(spawner.gameObject),
                    active = IsComponentActive(spawner),
                    supportedPlayerModes = modes,
                    interactableLayers = LayerNamesFromMask(interactableMask),
                    traversableLayers = LayerNamesFromMask(traversableMask),
                    collisionLayers = LayerNamesFromMask(collisionMask),
                    nearClippingPlane = near,
                    farClippingPlane = far,
                    transmissionFrequency = frequency
                });
            }

            return new PlayerValidationResult
            {
                valid = errors.Count == 0,
                spawnerCount = spawners.Length,
                activeSpawnerCount = activeCount,
                errors = errors.Distinct().ToArray(),
                warnings = warnings.Distinct().ToArray(),
                spawners = details.ToArray()
            };
        }

        private static TeleportValidationResult BuildTeleportValidation()
        {
            var anchors = VE2Reflection.FindComponentsByTypeName("V_TeleportAnchor").ToArray();
            var errors = new List<string>();
            var warnings = new List<string>();
            var spawner = VE2Reflection.FindComponentsByTypeName("V_PlayerSpawner").FirstOrDefault(IsComponentActive);
            if (spawner == null)
            {
                errors.Add("No active V_PlayerSpawner exists.");
            }
            else
            {
                var serialized = new SerializedObject(spawner);
                if (ReadSerializedInteger(serialized, "TraversableLayers") == 0)
                {
                    errors.Add("The active V_PlayerSpawner has no TraversableLayers configured.");
                }

                if (ReadSerializedInteger(serialized, "CollisionLayers") == 0)
                {
                    warnings.Add("The active V_PlayerSpawner has no CollisionLayers configured.");
                }
            }

            var anchorDetails = anchors.Select(anchor =>
            {
                var range = ReadSerializedFloat(new SerializedObject(anchor), "Range");
                if (range.HasValue && (range.Value < 0.75f || range.Value > 2.5f))
                {
                    errors.Add($"Teleport anchor '{anchor.gameObject.name}' has unsupported range {range.Value}.");
                }

                if (!anchor.gameObject.activeInHierarchy)
                {
                    warnings.Add($"Teleport anchor '{anchor.gameObject.name}' is inactive.");
                }

                return new
                {
                    target = VE2Reflection.GameObjectData(anchor.gameObject),
                    range
                };
            }).ToArray();

            if (anchors.Length == 0)
            {
                warnings.Add("No V_TeleportAnchor objects are present. Surface teleportation may still work through traversable layers.");
            }

            return new TeleportValidationResult
            {
                valid = errors.Count == 0,
                anchorCount = anchors.Length,
                errors = errors.Distinct().ToArray(),
                warnings = warnings.Distinct().ToArray(),
                anchors = anchorDetails
            };
        }

        private static object ReadSerializedLeaf(SerializedObject serialized, string leafName)
        {
            var property = FindSerializedPropertyByLeafName(serialized, leafName);
            return property == null ? null : SerializedPropertyValue(property);
        }

        private static int ReadSerializedInteger(SerializedObject serialized, string leafName)
        {
            var property = FindSerializedPropertyByLeafName(serialized, leafName);
            return property != null && property.propertyType == SerializedPropertyType.Integer ? property.intValue : 0;
        }

        private static float? ReadSerializedFloat(SerializedObject serialized, string leafName)
        {
            var property = FindSerializedPropertyByLeafName(serialized, leafName);
            return property != null && property.propertyType == SerializedPropertyType.Float ? property.floatValue : null;
        }

        private static string[] LayerNamesFromMask(int mask)
        {
            var names = new List<string>();
            for (var layer = 0; layer < 32; layer++)
            {
                if ((mask & (1 << layer)) == 0)
                {
                    continue;
                }

                var name = LayerMask.LayerToName(layer);
                names.Add(string.IsNullOrWhiteSpace(name) ? $"Layer{layer}" : name);
            }

            return names.ToArray();
        }

        private sealed class PlayerValidationResult
        {
            public bool valid;
            public int spawnerCount;
            public int activeSpawnerCount;
            public string[] errors;
            public string[] warnings;
            public object[] spawners;
        }

        private sealed class TeleportValidationResult
        {
            public bool valid;
            public int anchorCount;
            public string[] errors;
            public string[] warnings;
            public object[] anchors;
        }
    }
}
