using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Imperial.VE2.MCP.Editor
{
    public static partial class VE2McpTools
    {
        private static readonly string[] PublicApiPathSegments =
        {
            "/Common/Scripts/API/",
            "/Core/VComponents/Scripts/API/",
            "/Core/Player/Scripts/API/",
            "/Core/UI/Scripts/API/",
            "/Non-Core/Instancing/Scripts/API/",
            "/Non-Core/Platform/Scripts/API/"
        };

        public static object SearchApi(VE2ContextSearchParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.Query))
                {
                    return Response.Error("Query is required.");
                }

                var query = parameters.Query.Trim();
                var maxResults = Mathf.Clamp(parameters.MaxResults, 1, 50);
                var contextLines = Mathf.Clamp(parameters.ContextLines, 0, 5);
                var results = new List<object>();

                foreach (var path in FindPublicApiSourcePaths())
                {
                    var text = ReadAssetText(path);
                    if (string.IsNullOrEmpty(text))
                    {
                        continue;
                    }

                    var lines = text.Replace("\r\n", "\n").Split('\n');
                    for (var index = 0; index < lines.Length && results.Count < maxResults; index++)
                    {
                        if (lines[index].IndexOf(query, StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            continue;
                        }

                        var first = Mathf.Max(0, index - contextLines);
                        var last = Mathf.Min(lines.Length - 1, index + contextLines);
                        var snippet = new StringBuilder();
                        for (var line = first; line <= last; line++)
                        {
                            snippet.Append(line + 1).Append(": ").AppendLine(lines[line]);
                        }

                        results.Add(new
                        {
                            path,
                            line = index + 1,
                            snippet = snippet.ToString().TrimEnd()
                        });
                    }

                    if (results.Count >= maxResults)
                    {
                        break;
                    }
                }

                return Response.Success("Searched installed VE2 public API source.", new
                {
                    resourceUri = "ve2://api/search?q=" + Uri.EscapeDataString(query),
                    query,
                    resultCount = results.Count,
                    truncated = results.Count == maxResults,
                    results
                });
            });
        }

        public static object GetInterfaceSource(VE2ContextGetInterfaceParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.TypeName))
                {
                    return Response.Error("TypeName is required.");
                }

                var typeName = parameters.TypeName.Trim();
                if (!Regex.IsMatch(typeName, "^[A-Za-z_][A-Za-z0-9_]*$"))
                {
                    return Response.Error("TypeName must be a simple C# type name without a namespace or path.");
                }

                var candidates = FindPublicApiSourcePaths()
                    .Where(path => string.Equals(Path.GetFileNameWithoutExtension(path), typeName,
                        StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (candidates.Length == 0)
                {
                    var declaration = new Regex($@"\b(interface|class|struct|enum)\s+{Regex.Escape(typeName)}\b");
                    candidates = FindPublicApiSourcePaths()
                        .Where(path => declaration.IsMatch(ReadAssetText(path) ?? string.Empty))
                        .ToArray();
                }

                if (candidates.Length == 0)
                {
                    return Response.Error($"No installed VE2 public API source declares '{typeName}'.");
                }

                var sources = candidates.Select(path => new
                {
                    path,
                    text = ReadAssetText(path)
                }).ToArray();

                return Response.Success("Returned installed VE2 public API source.", new
                {
                    resourceUri = "ve2://api/interfaces/" + typeName,
                    typeName,
                    sources
                });
            });
        }

        public static object GetPrefabContract(VE2PrefabContractParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.ResourceName))
                {
                    return Response.Error("ResourceName is required.");
                }

                var resourceName = parameters.ResourceName.Trim().Replace('\\', '/').Trim('/');
                var maxDepth = Mathf.Clamp(parameters.MaxDepth, 1, 20);
                var candidates = AssetDatabase.FindAssets("t:Prefab")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Where(path => IsVE2ResourceAsset(path) &&
                                   string.Equals(VE2Reflection.ResourceNameFromAssetPath(path), resourceName,
                                       StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                if (candidates.Length == 0)
                {
                    return Response.Error($"Could not find VE2 Resources prefab '{resourceName}'.");
                }

                var contracts = new List<object>();
                foreach (var path in candidates)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab == null)
                    {
                        continue;
                    }

                    contracts.Add(new
                    {
                        assetPath = path,
                        resourceName = VE2Reflection.ResourceNameFromAssetPath(path),
                        hierarchy = BuildPrefabNode(prefab.transform, 0, maxDepth),
                        componentTypes = prefab.GetComponentsInChildren<Component>(true)
                            .Where(component => component != null)
                            .Select(component => component.GetType().Name)
                            .Distinct()
                            .OrderBy(name => name)
                            .ToArray(),
                        colliderCount = prefab.GetComponentsInChildren<Collider>(true).Length,
                        rigidbodyCount = prefab.GetComponentsInChildren<Rigidbody>(true).Length,
                        syncComponents = prefab.GetComponentsInChildren<Component>(true)
                            .Where(component => component != null && SyncComponentTypes.Contains(component.GetType().Name))
                            .Select(component => new
                            {
                                path = GetRelativeHierarchyPath(prefab.transform, component.transform),
                                componentType = component.GetType().Name
                            })
                            .ToArray()
                    });
                }

                return Response.Success("Inspected VE2 prefab contract.", new
                {
                    resourceUri = "ve2://prefabs/" + resourceName,
                    resourceName,
                    duplicateResourceNames = candidates.Length > 1,
                    contracts
                });
            });
        }

        public static object GetSceneManifest(VE2SceneManifestParams parameters)
        {
            return GuardVE2(() =>
            {
                parameters ??= new VE2SceneManifestParams();
                var maxDepth = Mathf.Clamp(parameters.MaxDepth, 1, 20);
                var scene = SceneManager.GetActiveScene();
                var roots = scene.GetRootGameObjects()
                    .Where(root => parameters.IncludeInactive || root.activeInHierarchy)
                    .Select(root => BuildSceneNode(root.transform, 0, maxDepth, parameters.IncludeInactive))
                    .Where(node => node != null)
                    .ToArray();

                var interactionComponents = ActivatableComponentTypes.Concat(AdjustableComponentTypes)
                    .Append("V_FreeGrabbable")
                    .SelectMany(VE2Reflection.FindComponentsByTypeName)
                    .Distinct()
                    .Select(component => new
                    {
                        path = VE2Reflection.GetHierarchyPath(component.transform),
                        componentType = component.GetType().Name,
                        active = IsComponentActive(component)
                    })
                    .OrderBy(item => item.path)
                    .ToArray();

                return Response.Success("Returned VE2-aware active-scene manifest.", new
                {
                    resourceUri = "ve2://scene/current",
                    scene = new
                    {
                        scene.name,
                        scene.path,
                        scene.isDirty,
                        scene.isLoaded,
                        rootCount = roots.Length
                    },
                    hierarchy = roots,
                    providers = BuildProviderSummary(),
                    interactions = interactionComponents,
                    player = BuildPlayerValidation(),
                    teleportation = BuildTeleportValidation(),
                    syncValidation = VE2SyncIdUtility.Scan()
                });
            });
        }

        public static object ScaffoldInteractionScript(VE2ScaffoldInteractionParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.ClassName) ||
                    string.IsNullOrWhiteSpace(parameters.InteractionKind))
                {
                    return Response.Error("ClassName and InteractionKind are required.");
                }

                if (!ValidateCodeIdentity(parameters.ClassName.Trim(), parameters.Namespace, out var identityError))
                {
                    return Response.Error(identityError);
                }

                var source = BuildInteractionScaffold(parameters.ClassName.Trim(), parameters.Namespace?.Trim(),
                    parameters.InteractionKind.Trim(), out var interfaceName, out var scaffoldError);
                if (source == null)
                {
                    return Response.Error(scaffoldError);
                }

                var result = WriteScaffold(parameters.OutputFolder, parameters.ClassName.Trim(), source,
                    parameters.Overwrite, out var writeError);
                if (result == null)
                {
                    return Response.Error(writeError);
                }

                return Response.Success("Created VE2 public-interface interaction scaffold.", new
                {
                    assetPath = result,
                    className = parameters.ClassName.Trim(),
                    interfaceName,
                    interactionKind = parameters.InteractionKind.Trim(),
                    usesInternalVe2Api = false
                });
            });
        }

        public static object ScaffoldNetworkObjectScript(VE2ScaffoldNetworkObjectParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || string.IsNullOrWhiteSpace(parameters.ClassName))
                {
                    return Response.Error("ClassName is required.");
                }

                if (!ValidateCodeIdentity(parameters.ClassName.Trim(), parameters.Namespace, out var identityError))
                {
                    return Response.Error(identityError);
                }

                var source = BuildNetworkObjectScaffold(parameters.ClassName.Trim(), parameters.Namespace?.Trim());
                var result = WriteScaffold(parameters.OutputFolder, parameters.ClassName.Trim(), source,
                    parameters.Overwrite, out var writeError);
                if (result == null)
                {
                    return Response.Error(writeError);
                }

                return Response.Success("Created VE2 IV_NetworkObject scaffold.", new
                {
                    assetPath = result,
                    className = parameters.ClassName.Trim(),
                    interfaceName = "IV_NetworkObject",
                    usesInternalVe2Api = false
                });
            });
        }

        public static object ValidatePluginScripts(VE2ValidatePluginScriptsParams parameters)
        {
            return GuardVE2(() =>
            {
                parameters ??= new VE2ValidatePluginScriptsParams();
                var rootPath = NormalizeAssetFolder(parameters.RootPath, out var rootError);
                if (rootPath == null)
                {
                    return Response.Error(rootError);
                }

                var absoluteRoot = AssetPathToAbsolute(rootPath);
                if (!Directory.Exists(absoluteRoot))
                {
                    return Response.Error($"Script root '{rootPath}' does not exist.");
                }

                var findings = new List<object>();
                var missingAsmdef = new List<string>();
                var scripts = Directory.GetFiles(absoluteRoot, "*.cs", SearchOption.AllDirectories)
                    .Select(AbsoluteToAssetPath)
                    .Where(path => !string.IsNullOrWhiteSpace(path))
                    .OrderBy(path => path)
                    .ToArray();

                var internalNamespace = new Regex(@"\bVE2\.[A-Za-z0-9_.]*\.Internal\b");
                var internalComponent = new Regex(@"\bV_(ToggleActivatable|HoldActivatable|PressurePlateActivatable|HandheldActivatable|CustomInfoPoint|FreeGrabbable|SlidingAdjustable|Sliding2DAdjustable|RotatingAdjustable|Rotating2DAdjustable|TransformSyncable|RigidbodySyncable|NetworkObject|GameObjectSpawnManager|PlayerSpawner)\b");
                var ve2ApiProperty = new Regex(@"\bVE2API\.([A-Za-z_][A-Za-z0-9_]*)");
                var allowedApiProperties = new HashSet<string>(StringComparer.Ordinal)
                {
                    "Player",
                    "PrimaryUIService",
                    "SecondaryUIService",
                    "InstanceService",
                    "HasMultiPlayerSupport",
                    "PlatformService"
                };

                foreach (var path in scripts)
                {
                    var text = ReadAssetText(path) ?? string.Empty;
                    AddRegexFindings(findings, path, text, internalNamespace, "error",
                        "Plugin code references a VE2 Internal namespace. Use the matching public API or PluginInterface.");
                    AddRegexFindings(findings, path, text, internalComponent, "warning",
                        "Plugin code directly names an internal V_* implementation. Serialize MonoBehaviour and cast to a public IV_* interface instead.");

                    foreach (Match match in ve2ApiProperty.Matches(text))
                    {
                        var property = match.Groups[1].Value;
                        if (!allowedApiProperties.Contains(property))
                        {
                            findings.Add(new
                            {
                                severity = "error",
                                path,
                                line = LineNumberAt(text, match.Index),
                                message = $"VE2API.{property} is not in VE2's public plugin-facing service list."
                            });
                        }
                    }

                    if (!HasAsmdefForScript(path))
                    {
                        missingAsmdef.Add(path);
                    }
                }

                var compileState = GetCompileState();
                var errors = findings.Count(finding =>
                    string.Equals(ReadAnonymousProperty(finding, "severity") as string, "error", StringComparison.Ordinal));
                if (missingAsmdef.Count > 0)
                {
                    errors += missingAsmdef.Count;
                }

                return Response.Success(errors == 0
                    ? "VE2 plugin script validation passed."
                    : "VE2 plugin script validation found errors.", new
                {
                    resourceUri = "ve2://scripts/validation",
                    valid = errors == 0 && !compileState.isCompiling && compileState.scriptCompilationFailed != true,
                    rootPath,
                    scriptCount = scripts.Length,
                    findings,
                    scriptsWithoutAsmdef = missingAsmdef.Distinct().ToArray(),
                    compileState
                });
            });
        }

        private static IEnumerable<string> FindPublicApiSourcePaths()
        {
            return AssetDatabase.FindAssets("t:MonoScript")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => path.Replace('\\', '/'))
                .Where(path => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) &&
                               path.Contains("/FRAMEWORK/", StringComparison.Ordinal) &&
                               PublicApiPathSegments.Any(segment => path.Contains(segment, StringComparison.Ordinal)))
                .Distinct()
                .OrderBy(path => path);
        }

        private static bool IsVE2ResourceAsset(string path)
        {
            var normalized = path.Replace('\\', '/');
            return normalized.Contains("/FRAMEWORK/", StringComparison.Ordinal) &&
                   normalized.Contains("/Resources/", StringComparison.Ordinal);
        }

        private static PrefabContractNode BuildPrefabNode(Transform transform, int depth, int maxDepth)
        {
            var node = new PrefabContractNode
            {
                name = transform.name,
                activeSelf = transform.gameObject.activeSelf,
                layer = LayerMask.LayerToName(transform.gameObject.layer),
                components = transform.GetComponents<Component>()
                    .Select(component => component == null ? "MissingScript" : component.GetType().Name)
                    .ToArray(),
                children = Array.Empty<PrefabContractNode>()
            };

            if (depth >= maxDepth)
            {
                node.truncatedChildren = transform.childCount;
                return node;
            }

            node.children = Enumerable.Range(0, transform.childCount)
                .Select(index => BuildPrefabNode(transform.GetChild(index), depth + 1, maxDepth))
                .ToArray();
            return node;
        }

        private static string GetRelativeHierarchyPath(Transform root, Transform target)
        {
            var full = VE2Reflection.GetHierarchyPath(target);
            var rootPath = VE2Reflection.GetHierarchyPath(root);
            return full == rootPath ? root.name : full.Substring(Mathf.Min(full.Length, rootPath.Length + 1));
        }

        private static SceneManifestNode BuildSceneNode(Transform transform, int depth, int maxDepth,
            bool includeInactive)
        {
            if (!includeInactive && !transform.gameObject.activeInHierarchy)
            {
                return null;
            }

            var components = transform.GetComponents<Component>();
            var node = new SceneManifestNode
            {
                name = transform.name,
                path = VE2Reflection.GetHierarchyPath(transform),
                activeSelf = transform.gameObject.activeSelf,
                activeInHierarchy = transform.gameObject.activeInHierarchy,
                layer = LayerMask.LayerToName(transform.gameObject.layer),
                components = components.Select(component => component == null ? "MissingScript" : component.GetType().Name).ToArray(),
                ve2Components = components.Where(component => component != null &&
                                                               component.GetType().Assembly.GetName().Name.StartsWith("VE2.", StringComparison.Ordinal))
                    .Select(component => component.GetType().Name)
                    .ToArray(),
                children = Array.Empty<SceneManifestNode>()
            };

            if (depth >= maxDepth)
            {
                node.truncatedChildren = transform.childCount;
                return node;
            }

            node.children = Enumerable.Range(0, transform.childCount)
                .Select(index => BuildSceneNode(transform.GetChild(index), depth + 1, maxDepth, includeInactive))
                .Where(child => child != null)
                .ToArray();
            return node;
        }

        private static bool ValidateCodeIdentity(string className, string namespaceName, out string error)
        {
            error = null;
            if (!Regex.IsMatch(className, "^[A-Za-z_][A-Za-z0-9_]*$"))
            {
                error = "ClassName must be a valid simple C# identifier.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(namespaceName) &&
                !Regex.IsMatch(namespaceName.Trim(), "^[A-Za-z_][A-Za-z0-9_]*(\\.[A-Za-z_][A-Za-z0-9_]*)*$"))
            {
                error = "Namespace must be a valid dotted C# namespace.";
                return false;
            }

            return true;
        }

        private static string BuildInteractionScaffold(string className, string namespaceName, string kind,
            out string interfaceName, out string error)
        {
            interfaceName = null;
            error = null;
            var normalized = kind.ToLowerInvariant();
            string valueType = null;
            string[] events;
            switch (normalized)
            {
                case "toggle_activatable":
                    interfaceName = "IV_ToggleActivatable";
                    events = new[] { "OnActivate", "OnDeactivate" };
                    break;
                case "info_point":
                    interfaceName = "IV_InfoPoint";
                    events = new[] { "OnActivate", "OnDeactivate" };
                    break;
                case "handheld_activatable":
                    interfaceName = "IV_HandheldActivatable";
                    events = new[] { "OnActivate", "OnDeactivate" };
                    break;
                case "hold_activatable":
                    interfaceName = "IV_HoldActivatable";
                    events = new[] { "OnActivate", "OnDeactivate" };
                    break;
                case "pressure_plate":
                    interfaceName = "IV_PressurePlateActivatable";
                    events = new[] { "OnActivate", "OnDeactivate" };
                    break;
                case "free_grabbable":
                    interfaceName = "IV_FreeGrabbable";
                    events = new[] { "OnGrab", "OnDrop" };
                    break;
                case "sliding_adjustable":
                    interfaceName = "IV_SlidingAdjustable";
                    valueType = "float";
                    events = new[] { "OnValueAdjusted" };
                    break;
                case "rotating_adjustable":
                    interfaceName = "IV_RotatingAdjustable";
                    valueType = "float";
                    events = new[] { "OnValueAdjusted" };
                    break;
                case "handheld_adjustable":
                    interfaceName = "IV_HandheldAdjustable";
                    valueType = "float";
                    events = new[] { "OnValueAdjusted" };
                    break;
                case "sliding_2d_adjustable":
                    interfaceName = "IV_Sliding2DAdjustable";
                    valueType = "Vector2";
                    events = new[] { "OnValueAdjusted" };
                    break;
                case "rotating_2d_adjustable":
                    interfaceName = "IV_Rotating2DAdjustable";
                    valueType = "Vector2";
                    events = new[] { "OnValueAdjusted" };
                    break;
                default:
                    error = "InteractionKind must be toggle_activatable, info_point, handheld_activatable, hold_activatable, pressure_plate, free_grabbable, sliding_adjustable, rotating_adjustable, handheld_adjustable, sliding_2d_adjustable, or rotating_2d_adjustable.";
                    return null;
            }

            return BuildListenerScaffold(className, namespaceName, interfaceName, events, valueType);
        }

        private static string BuildListenerScaffold(string className, string namespaceName, string interfaceName,
            string[] events, string valueType)
        {
            var namespaceIndent = string.IsNullOrWhiteSpace(namespaceName) ? string.Empty : "    ";
            var lines = new List<string>
            {
                "using UnityEngine;",
                "using VE2.Core.VComponents.API;",
                string.Empty
            };
            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                lines.Add("namespace " + namespaceName);
                lines.Add("{");
            }

            var i = namespaceIndent;
            lines.Add(i + "public sealed class " + className + " : MonoBehaviour");
            lines.Add(i + "{");
            lines.Add(i + "    [SerializeField] private MonoBehaviour _source;");
            lines.Add(string.Empty);
            lines.Add(i + "    private " + interfaceName + " _interaction;");
            lines.Add(string.Empty);
            lines.Add(i + "    private void Awake()");
            lines.Add(i + "    {");
            lines.Add(i + "        _interaction = _source as " + interfaceName + ";");
            lines.Add(i + "        if (_interaction == null)");
            lines.Add(i + "        {");
            lines.Add(i + "            Debug.LogError(\"" + className + " requires a component implementing " + interfaceName + ".\", this);");
            lines.Add(i + "            enabled = false;");
            lines.Add(i + "            return;");
            lines.Add(i + "        }");
            lines.Add(string.Empty);
            foreach (var eventName in events)
            {
                lines.Add(i + "        _interaction." + eventName + ".AddListener(Handle" + eventName.Substring(2) + ");");
            }
            lines.Add(i + "    }");
            lines.Add(string.Empty);
            lines.Add(i + "    private void OnDestroy()");
            lines.Add(i + "    {");
            lines.Add(i + "        if (_interaction == null)");
            lines.Add(i + "        {");
            lines.Add(i + "            return;");
            lines.Add(i + "        }");
            lines.Add(string.Empty);
            foreach (var eventName in events)
            {
                lines.Add(i + "        _interaction." + eventName + ".RemoveListener(Handle" + eventName.Substring(2) + ");");
            }
            lines.Add(i + "    }");
            foreach (var eventName in events)
            {
                lines.Add(string.Empty);
                var argument = eventName == "OnValueAdjusted" ? valueType + " value" : string.Empty;
                lines.Add(i + "    private void Handle" + eventName.Substring(2) + "(" + argument + ")");
                lines.Add(i + "    {");
                lines.Add(i + "    }");
            }
            lines.Add(i + "}");
            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                lines.Add("}");
            }

            return string.Join("\n", lines) + "\n";
        }

        private static string BuildNetworkObjectScaffold(string className, string namespaceName)
        {
            var namespaceIndent = string.IsNullOrWhiteSpace(namespaceName) ? string.Empty : "    ";
            var lines = new List<string>
            {
                "using UnityEngine;",
                "using VE2.NonCore.Instancing.API;",
                string.Empty
            };
            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                lines.Add("namespace " + namespaceName);
                lines.Add("{");
            }

            var i = namespaceIndent;
            lines.Add(i + "public sealed class " + className + " : MonoBehaviour");
            lines.Add(i + "{");
            lines.Add(i + "    [SerializeField] private MonoBehaviour _networkObjectSource;");
            lines.Add(string.Empty);
            lines.Add(i + "    private IV_NetworkObject _networkObject;");
            lines.Add(string.Empty);
            lines.Add(i + "    private void Awake()");
            lines.Add(i + "    {");
            lines.Add(i + "        _networkObject = _networkObjectSource as IV_NetworkObject;");
            lines.Add(i + "        if (_networkObject == null)");
            lines.Add(i + "        {");
            lines.Add(i + "            Debug.LogError(\"" + className + " requires a component implementing IV_NetworkObject.\", this);");
            lines.Add(i + "            enabled = false;");
            lines.Add(i + "            return;");
            lines.Add(i + "        }");
            lines.Add(string.Empty);
            lines.Add(i + "        _networkObject.OnDataChange.AddListener(HandleDataChanged);");
            lines.Add(i + "    }");
            lines.Add(string.Empty);
            lines.Add(i + "    private void OnDestroy()");
            lines.Add(i + "    {");
            lines.Add(i + "        _networkObject?.OnDataChange.RemoveListener(HandleDataChanged);");
            lines.Add(i + "    }");
            lines.Add(string.Empty);
            lines.Add(i + "    public void PublishString(string value)");
            lines.Add(i + "    {");
            lines.Add(i + "        _networkObject?.UpdateData(value);");
            lines.Add(i + "    }");
            lines.Add(string.Empty);
            lines.Add(i + "    private void HandleDataChanged(object value)");
            lines.Add(i + "    {");
            lines.Add(i + "    }");
            lines.Add(i + "}");
            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                lines.Add("}");
            }

            return string.Join("\n", lines) + "\n";
        }

        private static string WriteScaffold(string requestedFolder, string className, string source, bool overwrite,
            out string error)
        {
            error = null;
            var folder = NormalizeAssetFolder(requestedFolder, out error);
            if (folder == null)
            {
                return null;
            }

            var absoluteFolder = AssetPathToAbsolute(folder);
            Directory.CreateDirectory(absoluteFolder);
            var assetPath = folder.TrimEnd('/') + "/" + className + ".cs";
            var absolutePath = AssetPathToAbsolute(assetPath);
            if (File.Exists(absolutePath) && !overwrite)
            {
                error = $"Script '{assetPath}' already exists. Set Overwrite=true to replace it.";
                return null;
            }

            File.WriteAllText(absolutePath, source, new UTF8Encoding(false));
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            try
            {
                VE2Reflection.InvokeStatic("VE2.Common.Shared.VE2AutoAsmDef", "CreateOrUpdateAsmdef");
            }
            catch
            {
                // The script remains valid; preflight reports a missing asmdef if VE2 setup cannot create one.
            }

            AssetDatabase.Refresh();
            return assetPath;
        }

        private static string NormalizeAssetFolder(string requested, out string error)
        {
            error = null;
            var folder = string.IsNullOrWhiteSpace(requested) ? "Assets/Scripts" : requested.Trim().Replace('\\', '/').TrimEnd('/');
            if ((folder != "Assets" && !folder.StartsWith("Assets/", StringComparison.Ordinal)) || folder.Contains("..", StringComparison.Ordinal))
            {
                error = "Output folder must be a traversal-free path under Assets.";
                return null;
            }

            return folder;
        }

        private static string AssetPathToAbsolute(string assetPath)
        {
            var projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? string.Empty;
            return Path.Combine(projectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
        }

        private static string AbsoluteToAssetPath(string absolutePath)
        {
            var projectRoot = (Directory.GetParent(Application.dataPath)?.FullName ?? string.Empty).Replace('\\', '/').TrimEnd('/');
            var normalized = absolutePath.Replace('\\', '/');
            return normalized.StartsWith(projectRoot + "/", StringComparison.OrdinalIgnoreCase)
                ? normalized.Substring(projectRoot.Length + 1)
                : string.Empty;
        }

        private static void AddRegexFindings(ICollection<object> findings, string path, string text, Regex regex,
            string severity, string message)
        {
            foreach (Match match in regex.Matches(text))
            {
                findings.Add(new
                {
                    severity,
                    path,
                    line = LineNumberAt(text, match.Index),
                    symbol = match.Value,
                    message
                });
            }
        }

        private static int LineNumberAt(string text, int index)
        {
            var line = 1;
            for (var position = 0; position < index && position < text.Length; position++)
            {
                if (text[position] == '\n')
                {
                    line++;
                }
            }

            return line;
        }

        private static bool HasAsmdefForScript(string assetPath)
        {
            var directory = Path.GetDirectoryName(AssetPathToAbsolute(assetPath));
            var assetsRoot = Application.dataPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            while (!string.IsNullOrWhiteSpace(directory) &&
                   directory.StartsWith(assetsRoot, StringComparison.OrdinalIgnoreCase))
            {
                if (Directory.GetFiles(directory, "*.asmdef", SearchOption.TopDirectoryOnly).Length > 0)
                {
                    return true;
                }

                directory = Directory.GetParent(directory)?.FullName;
            }

            return false;
        }

        private static object ReadAnonymousProperty(object instance, string propertyName)
        {
            return instance?.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)?.GetValue(instance);
        }

        private sealed class PrefabContractNode
        {
            public string name;
            public bool activeSelf;
            public string layer;
            public string[] components;
            public int truncatedChildren;
            public PrefabContractNode[] children;
        }

        private sealed class SceneManifestNode
        {
            public string name;
            public string path;
            public bool activeSelf;
            public bool activeInHierarchy;
            public string layer;
            public string[] components;
            public string[] ve2Components;
            public int truncatedChildren;
            public SceneManifestNode[] children;
        }
    }
}
