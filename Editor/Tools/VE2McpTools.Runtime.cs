using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Imperial.VE2.MCP.Editor
{
    public static partial class VE2McpTools
    {
        public static object InspectPlayer()
        {
            return GuardVE2(() =>
            {
                var player = RequireRuntimeService("Player", out var error);
                if (player == null)
                {
                    return Response.Error(error);
                }

                var camera = VE2Reflection.ReadProperty(player, "ActiveCamera") as Camera;
                var controls = VE2Reflection.ReadProperty(player, "PlayerControlOverrides");
                return Response.Success("Inspected VE2 player service.", new
                {
                    isVRMode = VE2Reflection.ReadProperty(player, "IsVRMode"),
                    hasVRViewBeenCalibrated = VE2Reflection.ReadProperty(player, "HasVRViewBeenCalibrated"),
                    isVRTracking = VE2Reflection.ReadProperty(player, "IsVRTracking"),
                    playerPosition = RuntimeValue(VE2Reflection.ReadProperty(player, "PlayerPosition")),
                    playerRotation = RuntimeValue(VE2Reflection.ReadProperty(player, "PlayerRotation")),
                    playerSpawnPoint = RuntimeValue(VE2Reflection.ReadProperty(player, "PlayerSpawnPoint")),
                    activeCamera = camera == null ? null : new
                    {
                        path = VE2Reflection.GetHierarchyPath(camera.transform),
                        camera.fieldOfView,
                        camera.nearClipPlane,
                        camera.farClipPlane
                    },
                    controls = controls == null ? null : new
                    {
                        teleportVR = VE2Reflection.ReadProperty(controls, "IsTeleportVREnabled"),
                        snapTurnVR = VE2Reflection.ReadProperty(controls, "IsSnapTurnVREnabled"),
                        horizontalDragVR = VE2Reflection.ReadProperty(controls, "IsHorizontalDragVREnabled"),
                        verticalDragVR = VE2Reflection.ReadProperty(controls, "IsVerticalDragVREnabled"),
                        jump2D = VE2Reflection.ReadProperty(controls, "IsJump2DEnabled"),
                        crouch2D = VE2Reflection.ReadProperty(controls, "IsCrouch2DEnabled"),
                        move2D = VE2Reflection.ReadProperty(controls, "IsMove2DEnabled")
                    }
                });
            });
        }

        public static object MovePlayer(VE2MovePlayerParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null ||
                    (parameters.Position == null && parameters.RotationEuler == null))
                {
                    return Response.Error("Position and/or RotationEuler is required.");
                }

                var player = RequireRuntimeService("Player", out var error);
                if (player == null)
                {
                    return Response.Error(error);
                }

                var actions = new List<string>();
                if (parameters.Position != null)
                {
                    if (parameters.Position.Length < 3)
                    {
                        return Response.Error("Position must be [x,y,z].");
                    }

                    InvokePublicInstance(player, "SetLocalPlayerPosition",
                        new Vector3(parameters.Position[0], parameters.Position[1], parameters.Position[2]));
                    actions.Add("position");
                }

                if (parameters.RotationEuler != null)
                {
                    if (parameters.RotationEuler.Length < 3)
                    {
                        return Response.Error("RotationEuler must be [x,y,z].");
                    }

                    InvokePublicInstance(player, "SetLocalPlayerRotation",
                        Quaternion.Euler(parameters.RotationEuler[0], parameters.RotationEuler[1], parameters.RotationEuler[2]));
                    actions.Add("rotation");
                }

                return Response.Success("Moved the local VE2 player through IPlayerService.", new
                {
                    actions,
                    playerPosition = RuntimeValue(VE2Reflection.ReadProperty(player, "PlayerPosition")),
                    playerRotation = RuntimeValue(VE2Reflection.ReadProperty(player, "PlayerRotation"))
                });
            });
        }

        public static object InspectClients()
        {
            return GuardVE2(() =>
            {
                var instance = RequireRuntimeService("InstanceService", out var error);
                if (instance == null)
                {
                    return Response.Error(error);
                }

                var localId = ConvertToUShort(VE2Reflection.ReadProperty(instance, "LocalClientID"));
                var ids = EnumerateValues(VE2Reflection.ReadProperty(instance, "ClientIDsInCurrentInstance"))
                    .Select(ConvertToUShort)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .Distinct()
                    .ToArray();
                var clients = new List<object>();
                foreach (var id in ids)
                {
                    object position = null;
                    if (!localId.HasValue || id != localId.Value)
                    {
                        try
                        {
                            position = RuntimeValue(InvokePublicInstance(instance, "GetRemotePlayerPositionFromClientID", id));
                        }
                        catch (Exception exception)
                        {
                            position = new { unavailable = true, reason = exception.GetBaseException().Message };
                        }
                    }

                    clients.Add(new
                    {
                        clientId = id,
                        isLocal = localId.HasValue && id == localId.Value,
                        isHost = EqualsNumeric(VE2Reflection.ReadProperty(instance, "HostID"), id),
                        position
                    });
                }

                return Response.Success("Inspected VE2 instance clients.", new
                {
                    isConnectedToServer = VE2Reflection.ReadProperty(instance, "IsConnectedToServer"),
                    localClientId = localId,
                    hostId = VE2Reflection.ReadProperty(instance, "HostID"),
                    isHost = VE2Reflection.ReadProperty(instance, "IsHost"),
                    clientCount = VE2Reflection.ReadProperty(instance, "NumberOfClientsInCurrentInstance"),
                    ping = VE2Reflection.ReadProperty(instance, "Ping"),
                    smoothPing = VE2Reflection.ReadProperty(instance, "SmoothPing"),
                    clients
                });
            });
        }

        public static object InspectRuntimeInteractions(VE2RuntimeObjectParams parameters)
        {
            return GuardVE2(() =>
            {
                if (!EditorApplication.isPlaying)
                {
                    return Response.Error("Runtime interaction inspection requires Play Mode.");
                }

                GameObject target = null;
                if (!string.IsNullOrWhiteSpace(parameters?.GameObjectPath))
                {
                    target = VE2Reflection.FindGameObjectByPath(parameters.GameObjectPath);
                    if (target == null)
                    {
                        return Response.Error($"Could not find GameObject '{parameters.GameObjectPath}'.");
                    }
                }

                var allowed = new HashSet<string>(ActivatableComponentTypes
                    .Concat(AdjustableComponentTypes)
                    .Append("V_FreeGrabbable"), StringComparer.Ordinal);
                var components = (target == null
                        ? VE2Reflection.GetAllSceneTransforms().SelectMany(transform => transform.GetComponents<Component>())
                        : target.GetComponentsInChildren<Component>(true))
                    .Where(component => component != null && allowed.Contains(component.GetType().Name))
                    .Distinct()
                    .Select(BuildRuntimeInteractionState)
                    .ToArray();

                return Response.Success("Inspected live VE2 interactions.", new
                {
                    count = components.Length,
                    interactions = components
                });
            });
        }

        public static object SetRuntimeActivatable(VE2RuntimeSetActivatableParams parameters)
        {
            return GuardVE2(() =>
            {
                if (!EditorApplication.isPlaying)
                {
                    return Response.Error("Changing activatable state requires Play Mode.");
                }

                if (parameters == null || string.IsNullOrWhiteSpace(parameters.GameObjectPath))
                {
                    return Response.Error("GameObjectPath is required.");
                }

                var gameObject = VE2Reflection.FindGameObjectByPath(parameters.GameObjectPath);
                if (gameObject == null)
                {
                    return Response.Error($"Could not find GameObject '{parameters.GameObjectPath}'.");
                }

                var component = ResolveKnownComponent(gameObject, null, ActivatableComponentTypes, out var resolveError);
                if (component == null)
                {
                    return Response.Error(resolveError);
                }

                var setActivated = FindPublicMethod(component, "SetActivated", typeof(bool));
                var toggleAlways = FindPublicMethod(component, "ToggleAlwaysActivated", typeof(bool));
                if (setActivated != null)
                {
                    setActivated.Invoke(component, new object[] { parameters.Activated });
                }
                else if (toggleAlways != null)
                {
                    toggleAlways.Invoke(component, new object[] { parameters.Activated });
                }
                else
                {
                    var methodName = parameters.Activated ? "Activate" : "Deactivate";
                    var method = FindPublicMethod(component, methodName);
                    if (method == null)
                    {
                        return Response.Error($"{component.GetType().Name} does not expose a supported activation method.");
                    }

                    method.Invoke(component, null);
                }

                return Response.Success("Updated VE2 activatable state through its public interface.", new
                {
                    target = VE2Reflection.GameObjectData(gameObject),
                    componentType = component.GetType().Name,
                    requestedState = parameters.Activated,
                    currentState = ReadPublicPropertySafe(component, "IsActivated")
                });
            });
        }

        public static object SetRuntimeAdjustable(VE2RuntimeSetAdjustableParams parameters)
        {
            return GuardVE2(() =>
            {
                if (!EditorApplication.isPlaying)
                {
                    return Response.Error("Changing adjustable state requires Play Mode.");
                }

                if (parameters == null || string.IsNullOrWhiteSpace(parameters.GameObjectPath))
                {
                    return Response.Error("GameObjectPath is required.");
                }

                var gameObject = VE2Reflection.FindGameObjectByPath(parameters.GameObjectPath);
                if (gameObject == null)
                {
                    return Response.Error($"Could not find GameObject '{parameters.GameObjectPath}'.");
                }

                var component = ResolveKnownComponent(gameObject, null, AdjustableComponentTypes, out var resolveError);
                if (component == null)
                {
                    return Response.Error(resolveError);
                }

                if (parameters.ResetToStartingValue)
                {
                    var reset = FindPublicMethod(component, "ResetToStartingValue");
                    if (reset == null)
                    {
                        return Response.Error($"{component.GetType().Name} does not expose ResetToStartingValue().");
                    }

                    reset.Invoke(component, null);
                }
                else
                {
                    if (parameters.Value == null || parameters.Value.Length == 0)
                    {
                        return Response.Error("Value is required unless ResetToStartingValue is true.");
                    }

                    var methods = component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                        .Where(method => method.Name == "SetValue" && method.GetParameters().Length == 1)
                        .ToArray();
                    var floatMethod = methods.FirstOrDefault(method => method.GetParameters()[0].ParameterType == typeof(float));
                    var vectorMethod = methods.FirstOrDefault(method => method.GetParameters()[0].ParameterType == typeof(Vector2));
                    if (vectorMethod != null)
                    {
                        if (parameters.Value.Length < 2)
                        {
                            return Response.Error("This adjustable requires Value [x,y].");
                        }

                        vectorMethod.Invoke(component, new object[] { new Vector2(parameters.Value[0], parameters.Value[1]) });
                    }
                    else if (floatMethod != null)
                    {
                        floatMethod.Invoke(component, new object[] { parameters.Value[0] });
                    }
                    else
                    {
                        return Response.Error($"{component.GetType().Name} does not expose a supported SetValue method.");
                    }
                }

                return Response.Success("Updated VE2 adjustable through its public interface.", new
                {
                    target = VE2Reflection.GameObjectData(gameObject),
                    componentType = component.GetType().Name,
                    reset = parameters.ResetToStartingValue,
                    value = RuntimeValue(ReadPublicPropertySafe(component, "Value")),
                    startingValue = RuntimeValue(ReadPublicPropertySafe(component, "StartingValue"))
                });
            });
        }

        public static object SpawnRuntimeNetworkObject(VE2RuntimeSpawnParams parameters)
        {
            return GuardVE2(() =>
            {
                if (!EditorApplication.isPlaying)
                {
                    return Response.Error("Network-object spawning requires Play Mode.");
                }

                var manager = ResolveRuntimeSpawnManager(parameters?.SpawnManagerPath, out var error);
                if (manager == null)
                {
                    return Response.Error(error);
                }

                var spawned = InvokePublicInstance(manager, "SpawnAndReturnGameObject") as GameObject;
                if (spawned == null)
                {
                    return Response.Error("VE2 SpawnAndReturnGameObject returned null.");
                }

                return Response.Success("Spawned a network object through IV_GameObjectSpawnManager.", new
                {
                    manager = VE2Reflection.GameObjectData(manager.gameObject),
                    spawned = VE2Reflection.GameObjectData(spawned),
                    syncEntries = BuildSyncEntries(spawned).ToArray()
                });
            });
        }

        public static object DespawnRuntimeNetworkObject(VE2RuntimeDespawnParams parameters)
        {
            return GuardVE2(() =>
            {
                if (!EditorApplication.isPlaying)
                {
                    return Response.Error("Network-object despawning requires Play Mode.");
                }

                if (parameters == null || string.IsNullOrWhiteSpace(parameters.TargetGameObjectPath))
                {
                    return Response.Error("TargetGameObjectPath is required.");
                }

                var manager = ResolveRuntimeSpawnManager(parameters.SpawnManagerPath, out var error);
                if (manager == null)
                {
                    return Response.Error(error);
                }

                var target = VE2Reflection.FindGameObjectByPath(parameters.TargetGameObjectPath);
                if (target == null)
                {
                    return Response.Error($"Could not find GameObject '{parameters.TargetGameObjectPath}'.");
                }

                var targetData = VE2Reflection.GameObjectData(target);
                InvokePublicInstance(manager, "DespawnGameObject", target);
                return Response.Success("Requested network-object despawn through IV_GameObjectSpawnManager.", new
                {
                    manager = VE2Reflection.GameObjectData(manager.gameObject),
                    despawned = targetData
                });
            });
        }

        public static object CaptureSyncSnapshot(VE2RuntimeSyncSnapshotParams parameters)
        {
            return GuardVE2(() =>
            {
                parameters ??= new VE2RuntimeSyncSnapshotParams();
                var maxObjects = Mathf.Clamp(parameters.MaxObjects, 1, 500);
                GameObject target = null;
                if (!string.IsNullOrWhiteSpace(parameters.GameObjectPath))
                {
                    target = VE2Reflection.FindGameObjectByPath(parameters.GameObjectPath);
                    if (target == null)
                    {
                        return Response.Error($"Could not find GameObject '{parameters.GameObjectPath}'.");
                    }
                }

                var names = new HashSet<string>(SyncComponentTypes, StringComparer.Ordinal);
                var components = (target == null
                        ? VE2Reflection.GetAllSceneTransforms().SelectMany(transform => transform.GetComponents<Component>())
                        : target.GetComponentsInChildren<Component>(true))
                    .Where(component => component != null && names.Contains(component.GetType().Name))
                    .Take(maxObjects)
                    .Select(BuildSyncComponentSnapshot)
                    .ToArray();

                return Response.Success("Captured VE2 synchronization snapshot.", new
                {
                    playMode = EditorApplication.isPlaying,
                    count = components.Length,
                    truncated = components.Length == maxObjects,
                    components,
                    syncValidation = VE2SyncIdUtility.Scan()
                });
            });
        }

        public static object RunMultiplayerSmokeTest()
        {
            return GuardVE2(() =>
            {
                if (!EditorApplication.isPlaying)
                {
                    return Response.Error("The multiplayer smoke test requires Play Mode.");
                }

                var checks = new List<object>();
                var errors = new List<string>();
                var warnings = new List<string>();
                var player = VE2Reflection.GetVE2ApiProperty("Player");
                var instance = VE2Reflection.GetVE2ApiProperty("InstanceService");
                checks.Add(new { check = "VE2API.Player", passed = player != null });
                checks.Add(new { check = "VE2API.InstanceService", passed = instance != null });
                if (player == null)
                {
                    errors.Add("VE2API.Player is null.");
                }

                if (instance == null)
                {
                    errors.Add("VE2API.InstanceService is null.");
                }
                else if (!Equals(ReadPublicPropertySafe(instance, "IsConnectedToServer"), true))
                {
                    warnings.Add("InstanceService is present but is not connected to the server.");
                }

                var providerSummary = BuildProviderSummary();
                var playerValidation = BuildPlayerValidation();
                errors.AddRange(playerValidation.errors);
                warnings.AddRange(playerValidation.warnings);
                var sync = VE2SyncIdUtility.Scan();
                if (sync.clashCount > 0)
                {
                    errors.Add($"Found {sync.clashCount} duplicate name-derived sync IDs.");
                }

                var interactionCount = ActivatableComponentTypes.Concat(AdjustableComponentTypes).Append("V_FreeGrabbable")
                    .SelectMany(VE2Reflection.FindComponentsByTypeName)
                    .Distinct()
                    .Count();
                checks.Add(new { check = "interactionComponents", passed = interactionCount > 0, count = interactionCount });
                if (interactionCount == 0)
                {
                    warnings.Add("No VE2 interaction components were found in the active scene.");
                }

                return Response.Success(errors.Count == 0
                    ? "VE2 multiplayer smoke test passed."
                    : "VE2 multiplayer smoke test found errors.", new
                {
                    valid = errors.Count == 0,
                    checks,
                    errors = errors.Distinct().ToArray(),
                    warnings = warnings.Distinct().ToArray(),
                    providers = providerSummary,
                    player = playerValidation,
                    instance = instance == null ? null : new
                    {
                        connected = ReadPublicPropertySafe(instance, "IsConnectedToServer"),
                        localClientId = ReadPublicPropertySafe(instance, "LocalClientID"),
                        hostId = ReadPublicPropertySafe(instance, "HostID"),
                        isHost = ReadPublicPropertySafe(instance, "IsHost"),
                        clientCount = ReadPublicPropertySafe(instance, "NumberOfClientsInCurrentInstance"),
                        ping = ReadPublicPropertySafe(instance, "Ping"),
                        smoothPing = ReadPublicPropertySafe(instance, "SmoothPing")
                    },
                    syncValidation = sync
                });
            });
        }

        private static object RequireRuntimeService(string propertyName, out string error)
        {
            error = null;
            if (!EditorApplication.isPlaying)
            {
                error = $"VE2API.{propertyName} requires Play Mode.";
                return null;
            }

            var service = VE2Reflection.GetVE2ApiProperty(propertyName);
            if (service == null)
            {
                error = $"VE2API.{propertyName} is null. Confirm the quickstart providers are active and Play Mode has finished initializing.";
            }

            return service;
        }

        private static object InvokePublicInstance(object instance, string methodName, params object[] arguments)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            arguments ??= Array.Empty<object>();
            var methods = instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method => method.Name == methodName && method.GetParameters().Length == arguments.Length)
                .ToArray();
            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                var compatible = true;
                for (var index = 0; index < parameters.Length; index++)
                {
                    if (arguments[index] != null && !parameters[index].ParameterType.IsInstanceOfType(arguments[index]))
                    {
                        compatible = false;
                        break;
                    }
                }

                if (compatible)
                {
                    return method.Invoke(instance, arguments);
                }
            }

            throw new MissingMethodException(instance.GetType().FullName, methodName);
        }

        private static MethodInfo FindPublicMethod(object instance, string name, params Type[] parameterTypes)
        {
            return instance?.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.Public, null,
                parameterTypes ?? Type.EmptyTypes, null);
        }

        private static object ReadPublicPropertySafe(object instance, string propertyName)
        {
            try
            {
                return VE2Reflection.ReadProperty(instance, propertyName);
            }
            catch (Exception exception)
            {
                return new { unavailable = true, reason = exception.GetBaseException().Message };
            }
        }

        private static object BuildRuntimeInteractionState(Component component)
        {
            return new
            {
                target = VE2Reflection.GameObjectData(component.gameObject),
                componentType = component.GetType().Name,
                isEnabled = ReadPublicPropertySafe(component, "IsEnabled"),
                isInteractable = ReadPublicPropertySafe(component, "IsInteractable"),
                adminOnly = ReadPublicPropertySafe(component, "AdminOnly"),
                isActivated = ReadPublicPropertySafe(component, "IsActivated"),
                isGrabbed = ReadPublicPropertySafe(component, "IsGrabbed"),
                isLocallyGrabbed = ReadPublicPropertySafe(component, "IsLocallyGrabbed"),
                value = RuntimeValue(ReadPublicPropertySafe(component, "Value")),
                startingValue = RuntimeValue(ReadPublicPropertySafe(component, "StartingValue")),
                mostRecentInteractingClient = ClientWrapperValue(ReadPublicPropertySafe(component, "MostRecentInteractingClientID")),
                mostRecentGrabbingClient = ClientWrapperValue(ReadPublicPropertySafe(component, "MostRecentGrabbingClientID")),
                mostRecentAdjustingClient = ClientWrapperValue(ReadPublicPropertySafe(component, "MostRecentAdjustingClientID"))
            };
        }

        private static object ClientWrapperValue(object wrapper)
        {
            if (wrapper == null)
            {
                return null;
            }

            return new
            {
                value = ReadPublicPropertySafe(wrapper, "Value"),
                isLocal = ReadPublicPropertySafe(wrapper, "IsLocal"),
                isRemote = ReadPublicPropertySafe(wrapper, "IsRemote")
            };
        }

        private static Component ResolveRuntimeSpawnManager(string requestedPath, out string error)
        {
            error = null;
            if (!string.IsNullOrWhiteSpace(requestedPath))
            {
                var gameObject = VE2Reflection.FindGameObjectByPath(requestedPath);
                if (gameObject == null)
                {
                    error = $"Could not find SpawnManager GameObject '{requestedPath}'.";
                    return null;
                }

                var manager = VE2Reflection.GetComponentInChildrenByTypeName(gameObject, "V_GameObjectSpawnManager");
                if (manager == null)
                {
                    error = $"'{requestedPath}' does not contain V_GameObjectSpawnManager.";
                }

                return manager;
            }

            var managers = VE2Reflection.FindComponentsByTypeName("V_GameObjectSpawnManager")
                .Where(IsComponentActive)
                .ToArray();
            if (managers.Length == 0)
            {
                error = "No active V_GameObjectSpawnManager was found.";
                return null;
            }

            if (managers.Length > 1)
            {
                error = "Multiple active V_GameObjectSpawnManager components were found. Provide SpawnManagerPath explicitly.";
                return null;
            }

            return managers[0];
        }

        private static object BuildSyncComponentSnapshot(Component component)
        {
            var serialized = new SerializedObject(component);
            var rigidbody = component.GetComponent<Rigidbody>();
            object currentData = null;
            if (EditorApplication.isPlaying && component.GetType().Name == "V_NetworkObject")
            {
                currentData = RuntimeValue(ReadPublicPropertySafe(component, "CurrentData"));
            }

            return new
            {
                target = VE2Reflection.GameObjectData(component.gameObject),
                componentType = component.GetType().Name,
                syncEntries = BuildSyncEntries(component.gameObject)
                    .Where(entry => entry != null)
                    .ToArray(),
                isNetworked = ReadSerializedLeaf(serialized, "IsNetworked"),
                transmissionFrequency = ReadSerializedLeaf(serialized, "TransmissionFrequency"),
                transmissionProtocol = ReadSerializedLeaf(serialized, "TransmissionType"),
                nonHostsCanModifyTransform = ReadSerializedLeaf(serialized, "NonHostsCanModifyTransform"),
                transform = new
                {
                    localPosition = RuntimeValue(component.transform.localPosition),
                    localRotation = RuntimeValue(component.transform.localRotation),
                    localScale = RuntimeValue(component.transform.localScale)
                },
                rigidbody = rigidbody == null ? null : new
                {
                    position = RuntimeValue(rigidbody.position),
                    rotation = RuntimeValue(rigidbody.rotation),
                    velocity = RuntimeValue(rigidbody.linearVelocity),
                    angularVelocity = RuntimeValue(rigidbody.angularVelocity),
                    rigidbody.isKinematic
                },
                currentData
            };
        }

        private static object RuntimeValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            return value switch
            {
                Vector2 vector => new[] { vector.x, vector.y },
                Vector3 vector => new[] { vector.x, vector.y, vector.z },
                Vector4 vector => new[] { vector.x, vector.y, vector.z, vector.w },
                Quaternion quaternion => new[] { quaternion.x, quaternion.y, quaternion.z, quaternion.w },
                Color color => new[] { color.r, color.g, color.b, color.a },
                string text => text,
                bool boolean => boolean,
                byte number => number,
                sbyte number => number,
                short number => number,
                ushort number => number,
                int number => number,
                uint number => number,
                long number => number,
                ulong number => number,
                float number => number,
                double number => number,
                Enum enumValue => enumValue.ToString(),
                UnityEngine.Object unityObject => ObjectReferenceValue(unityObject),
                IEnumerable enumerable => EnumerateValues(enumerable).Take(100).Select(RuntimeValue).ToArray(),
                _ => new { type = value.GetType().FullName, text = value.ToString() }
            };
        }

        private static IEnumerable<object> EnumerateValues(object value)
        {
            if (value is not IEnumerable enumerable || value is string)
            {
                yield break;
            }

            foreach (var item in enumerable)
            {
                yield return item;
            }
        }

        private static ushort? ConvertToUShort(object value)
        {
            try
            {
                return value == null ? null : Convert.ToUInt16(value);
            }
            catch
            {
                return null;
            }
        }

        private static bool EqualsNumeric(object value, ushort expected)
        {
            var converted = ConvertToUShort(value);
            return converted.HasValue && converted.Value == expected;
        }
    }
}
