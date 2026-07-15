using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Imperial.VE2.MCP.Editor
{
    internal static class VE2Reflection
    {
        private static readonly Dictionary<string, Type> TypeLookupCache = new(StringComparer.Ordinal);
        private static readonly HashSet<string> MissingTypeLookupCache = new(StringComparer.Ordinal);
        private static bool? VE2PackageRegisteredCache;
        private static bool? VE2ApiAssetExistsCache;

        public static bool IsVE2Installed => FindType("VE2.Common.API.VE2API") != null || VE2PackageRegistered() || VE2ApiAssetExists();

        public static Type FindType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return null;
            }

            if (TypeLookupCache.TryGetValue(typeName, out var cached))
            {
                return cached;
            }

            if (MissingTypeLookupCache.Contains(typeName))
            {
                return null;
            }

            var exact = FindLoadedTypeByExactName(typeName);
            if (exact != null)
            {
                TypeLookupCache[typeName] = exact;
                return exact;
            }

            if (!typeName.Contains("."))
            {
                var component = TypeCache.GetTypesDerivedFrom<Component>()
                    .FirstOrDefault(type => type.Name == typeName || type.FullName == typeName);
                if (component != null)
                {
                    TypeLookupCache[typeName] = component;
                    return component;
                }
            }

            MissingTypeLookupCache.Add(typeName);
            return null;
        }

        public static object InvokeStatic(string typeName, string methodName, params object[] args)
        {
            args ??= Array.Empty<object>();
            var type = FindType(typeName);
            if (type == null)
            {
                throw new MissingMemberException($"Could not find type '{typeName}'. Is VE2 installed in this Unity project?");
            }

            var method = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(m => m.Name == methodName && ParametersCompatible(m.GetParameters(), args));

            if (method == null)
            {
                throw new MissingMethodException(typeName, methodName);
            }

            return method.Invoke(null, BuildInvokeArguments(method.GetParameters(), args));
        }

        public static GameObject InstantiateVE2Resource(string resourceName)
        {
            return InvokeStatic("VE2.Common.Shared.CommonUtils", "InstantiateResource", resourceName) as GameObject;
        }

        public static Component FindFirstComponentByTypeName(string typeName)
        {
            return FindComponentsByTypeName(typeName).FirstOrDefault();
        }

        public static IEnumerable<Component> FindComponentsByTypeName(string typeName)
        {
            var type = FindType(typeName);
            if (type == null)
            {
                return Enumerable.Empty<Component>();
            }

            return UnityEngine.Object.FindObjectsByType(type, FindObjectsInactive.Include, FindObjectsSortMode.None)
                .OfType<Component>();
        }

        public static Component GetComponentInChildrenByTypeName(GameObject root, string typeName)
        {
            var type = FindType(typeName);
            if (type == null || root == null)
            {
                return null;
            }

            return root.GetComponentsInChildren(type, true).OfType<Component>().FirstOrDefault();
        }

        public static object GetVE2ApiProperty(string propertyName)
        {
            var ve2Api = FindType("VE2.Common.API.VE2API");
            if (ve2Api == null)
            {
                return null;
            }

            var property = ve2Api.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return property?.GetValue(null);
        }

        public static object ReadProperty(object instance, string propertyName)
        {
            if (instance == null)
            {
                return null;
            }

            var property = instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return property?.GetValue(instance);
        }

        public static string AssetPathFromAbsolutePath(string absolutePath)
        {
            var normalized = absolutePath.Replace('\\', '/');
            var dataPath = Application.dataPath.Replace('\\', '/');
            return normalized.StartsWith(dataPath, StringComparison.OrdinalIgnoreCase)
                ? "Assets" + normalized.Substring(dataPath.Length)
                : string.Empty;
        }

        public static IEnumerable<Transform> GetAllSceneTransforms()
        {
            var scene = SceneManager.GetActiveScene();
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var child in root.GetComponentsInChildren<Transform>(true))
                {
                    yield return child;
                }
            }
        }

        public static string GetHierarchyPath(Transform transform)
        {
            var parts = new Stack<string>();
            var current = transform;
            while (current != null)
            {
                parts.Push(current.name);
                current = current.parent;
            }

            return string.Join("/", parts);
        }

        public static GameObject FindGameObjectByPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            return GetAllSceneTransforms()
                .Select(t => t.gameObject)
                .FirstOrDefault(go => GetHierarchyPath(go.transform) == path || go.name == path);
        }

        public static string MakeUniqueSceneName(string desiredName, GameObject ignore = null)
        {
            var used = new HashSet<string>(GetAllSceneTransforms()
                .Where(t => ignore == null || t.gameObject != ignore)
                .Select(t => t.name));

            if (!used.Contains(desiredName))
            {
                return desiredName;
            }

            var baseName = System.Text.RegularExpressions.Regex.Replace(desiredName, @"\d+$", "");
            if (string.IsNullOrWhiteSpace(baseName))
            {
                baseName = desiredName;
            }

            var index = 2;
            string candidate;
            do
            {
                candidate = $"{baseName}{index++}";
            } while (used.Contains(candidate));

            return candidate;
        }

        public static object GameObjectData(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

            return new
            {
                name = gameObject.name,
                path = GetHierarchyPath(gameObject.transform),
                instanceID = gameObject.GetInstanceID(),
                activeSelf = gameObject.activeSelf,
                activeInHierarchy = gameObject.activeInHierarchy
            };
        }

        public static string ResourceNameFromAssetPath(string assetPath)
        {
            var normalized = assetPath.Replace('\\', '/');
            var index = normalized.LastIndexOf("/Resources/", StringComparison.Ordinal);
            if (index < 0)
            {
                return System.IO.Path.GetFileNameWithoutExtension(assetPath);
            }

            var relative = normalized.Substring(index + "/Resources/".Length);
            return relative.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)
                ? relative.Substring(0, relative.Length - ".prefab".Length)
                : relative;
        }

        public static void MarkActiveSceneDirty()
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        private static Type FindLoadedTypeByExactName(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var direct = assembly.GetType(typeName, false);
                    if (direct != null)
                    {
                        return direct;
                    }
                }
                catch
                {
                    // Some Unity/editor assemblies can throw while resolving metadata.
                    // Keep MCP tool discovery responsive and continue with other assemblies.
                }
            }

            return null;
        }

        private static bool VE2PackageRegistered()
        {
            if (VE2PackageRegisteredCache.HasValue)
            {
                return VE2PackageRegisteredCache.Value;
            }

            try
            {
                VE2PackageRegisteredCache = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages()
                    .Any(package => string.Equals(package.name, "com.ic.ve2", StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                VE2PackageRegisteredCache = false;
            }

            return VE2PackageRegisteredCache.Value;
        }

        private static bool VE2ApiAssetExists()
        {
            if (VE2ApiAssetExistsCache.HasValue)
            {
                return VE2ApiAssetExistsCache.Value;
            }

            try
            {
                VE2ApiAssetExistsCache = AssetDatabase.FindAssets("VE2API t:MonoScript")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Any(path => path.Replace('\\', '/').EndsWith("/VE2API.cs", StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                VE2ApiAssetExistsCache = false;
            }

            return VE2ApiAssetExistsCache.Value;
        }

        private static bool ParametersCompatible(ParameterInfo[] parameters, object[] args)
        {
            if (args.Length > parameters.Length)
            {
                return false;
            }

            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == null)
                {
                    continue;
                }

                if (!parameters[i].ParameterType.IsInstanceOfType(args[i]) && !CanAssignPrimitive(parameters[i].ParameterType, args[i].GetType()))
                {
                    return false;
                }
            }

            for (var i = args.Length; i < parameters.Length; i++)
            {
                if (!parameters[i].IsOptional)
                {
                    return false;
                }
            }

            return true;
        }

        private static object[] BuildInvokeArguments(ParameterInfo[] parameters, object[] args)
        {
            var invokeArgs = new object[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                if (i < args.Length)
                {
                    invokeArgs[i] = args[i];
                }
                else if (parameters[i].DefaultValue != DBNull.Value)
                {
                    invokeArgs[i] = parameters[i].DefaultValue;
                }
                else
                {
                    invokeArgs[i] = Type.Missing;
                }
            }

            return invokeArgs;
        }

        private static bool CanAssignPrimitive(Type parameterType, Type argType)
        {
            if (!parameterType.IsPrimitive)
            {
                return false;
            }

            try
            {
                Convert.ChangeType(Activator.CreateInstance(argType), parameterType);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
