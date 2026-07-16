using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Imperial.VE2.MCP.Editor
{
    public static partial class VE2McpTools
    {
        private const string BuilderTypeName = "VE2.NonCore.FileSystem.Internal.VE2PluginBuilderWindow";
        private const string UploaderTypeName = "VE2.NonCore.FileSystem.Internal.PluginUploaderLogic";
        private static EditorWindow BuildWindow;
        private static object Uploader;
        private static string UploaderPlatform;
        private static bool AutoUpload;
        private static bool UploaderTicking;

        public static object ExportPlugin(VE2BuildExportParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || !parameters.ConfirmBuild)
                    return Response.Error("ConfirmBuild=true is required because VE2's build workflow changes build settings and creates deployment files.");
                if (!BuildPlatform(parameters.Platform, out var platform, out var platformError))
                    return Response.Error(platformError);
                if (EditorApplication.isPlaying || EditorApplication.isCompiling || BuildPipeline.isBuildingPlayer)
                    return Response.Error("Cannot start a VE2 plugin build while Unity is playing, compiling, or already building.");

                var scene = SceneManager.GetActiveScene();
                if (string.IsNullOrWhiteSpace(scene.path))
                    return Response.Error("Save the active scene before building a VE2 plugin.");
                if (scene.isDirty && !EditorSceneManager.SaveScene(scene))
                    return Response.Error("Unity could not save the active scene before the VE2 build.");
                if (!RunDeploymentPreflight(true).valid)
                    return Response.Error("VE2 deployment preflight failed. Resolve its errors before building.");

                var versions = LocalVersions(platform, scene.name);
                var version = parameters.Version > 0 ? parameters.Version : (versions.Length == 0 ? 1 : versions.Max() + 1);
                if (version < 1 || version > 999) return Response.Error("Version must be between 1 and 999.");

                var type = VE2Reflection.FindType(BuilderTypeName);
                if (type == null || !typeof(EditorWindow).IsAssignableFrom(type))
                    return Response.Error("Could not find VE2PluginBuilderWindow in the installed VE2 version.");
                BuildWindow = ScriptableObject.CreateInstance(type) as EditorWindow;
                if (BuildWindow == null) return Response.Error("Could not create VE2's plugin builder window.");
                BuildWindow.titleContent = new GUIContent("Build and Upload VE2 plugin");
                BuildWindow.Show();
                SetEnumField(BuildWindow, "_environmentType", platform);
                SetEnumField(BuildWindow, "_workflowMode", "BuildOnly");
                InvokeMember(BuildWindow, "GetSceneDataAndScripts", scene);

                if (CollectionCount(ReadField(BuildWindow, "_scriptsWithoutAsmDef")) > 0)
                {
                    BuildWindow.Close();
                    return Response.Error("VE2's builder found scripts without an asmdef. Move them under the VE2 plugin asmdef before building.");
                }
                if (ReadField(BuildWindow, "assembliesValid") is bool valid && !valid)
                {
                    var errors = ReadField(BuildWindow, "assemblyErrors");
                    BuildWindow.Close();
                    return Response.Error("VE2 assembly validation failed: " + errors);
                }

                var destination = Path.Combine("files", "VE2", "Worlds", platform, scene.name, version.ToString("D3"));
                InvokeMember(BuildWindow, "ExecuteBuild", ReadField(BuildWindow, "locatedAssemblies"), destination,
                    scene.name, parameters.BuildWithEcsBurst, version, false);
                return Response.Success("Started VE2's plugin build workflow.", new
                {
                    scene = scene.name, platform, version, parameters.BuildWithEcsBurst,
                    destination = Path.Combine(Application.persistentDataPath, destination),
                    compiling = EditorApplication.isCompiling,
                    buildingPlayer = BuildPipeline.isBuildingPlayer
                });
            });
        }

        public static object GetBuildStatus()
        {
            return GuardVE2(() =>
            {
                var window = FindBuildWindow();
                return Response.Success("Inspected VE2 build and deployment state.", new
                {
                    isCompiling = EditorApplication.isCompiling,
                    isUpdating = EditorApplication.isUpdating,
                    isBuildingPlayer = BuildPipeline.isBuildingPlayer,
                    builder = window == null ? null : new
                    {
                        pendingBuildAfterReload = ReadField(window, "_pendingBuildAfterReload"),
                        pendingVersion = ReadField(window, "_pendingBuildVersion"),
                        environment = ReadField(window, "_environmentType")?.ToString(),
                        buildComplete = ReadField(window, "_showBuildCompleteMessage"),
                        lastBuiltVersion = ReadField(window, "_lastBuiltVersion"),
                        lastBuiltWorld = ReadField(window, "_lastBuiltWorldName"),
                        assembliesValid = ReadField(window, "assembliesValid"),
                        assemblyErrors = ReadField(window, "assemblyErrors")
                    },
                    uploader = UploaderStatus()
                });
            });
        }

        public static object GetBuildVersion(VE2BuildPlatformParams parameters)
        {
            return GuardVE2(() =>
            {
                if (!BuildPlatform(parameters?.Platform, out var platform, out var error)) return Response.Error(error);
                var scene = SceneManager.GetActiveScene().name;
                var versions = LocalVersions(platform, scene);
                return Response.Success("Inspected local VE2 plugin versions.", new
                {
                    scene, platform, root = BuildRoot(platform, scene), versions,
                    highestLocalVersion = versions.Length == 0 ? 0 : versions.Max(),
                    nextLocalVersion = versions.Length == 0 ? 1 : versions.Max() + 1
                });
            });
        }

        private static bool BuildPlatform(string requested, out string platform, out string error)
        {
            platform = null;
            error = null;
            if (string.Equals(requested, "Windows", StringComparison.OrdinalIgnoreCase)) platform = "Windows";
            else if (string.Equals(requested, "Android", StringComparison.OrdinalIgnoreCase)) platform = "Android";
            else error = "Platform must be Windows or Android.";
            return platform != null;
        }

        private static string BuildRoot(string platform, string scene) =>
            Path.Combine(Application.persistentDataPath, "files", "VE2", "Worlds", platform, scene);

        private static int[] LocalVersions(string platform, string scene)
        {
            var root = BuildRoot(platform, scene);
            if (!Directory.Exists(root)) return Array.Empty<int>();
            return Directory.GetDirectories(root).Select(Path.GetFileName)
                .Where(name => name != null && name.Length == 3 && name.All(char.IsDigit))
                .Select(int.Parse).Where(version => version > 0).Distinct().OrderBy(version => version).ToArray();
        }

        private static EditorWindow FindBuildWindow()
        {
            if (BuildWindow != null) return BuildWindow;
            var type = VE2Reflection.FindType(BuilderTypeName);
            if (type != null) BuildWindow = Resources.FindObjectsOfTypeAll(type).OfType<EditorWindow>().FirstOrDefault();
            return BuildWindow;
        }

        private static FieldInfo Field(Type type, string name)
        {
            while (type != null)
            {
                var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (field != null) return field;
                type = type.BaseType;
            }
            return null;
        }

        private static object ReadField(object instance, string name) => instance == null ? null : Field(instance.GetType(), name)?.GetValue(instance);
        private static object ReadMember(object instance, string name) => instance?.GetType()
            .GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(instance);

        private static void SetEnumField(object instance, string name, string value)
        {
            var field = Field(instance.GetType(), name);
            if (field == null || !field.FieldType.IsEnum) throw new MissingFieldException(instance.GetType().FullName, name);
            field.SetValue(instance, Enum.Parse(field.FieldType, value, true));
        }

        private static object InvokeMember(object instance, string name, params object[] args)
        {
            args ??= Array.Empty<object>();
            foreach (var method in instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                         .Where(method => method.Name == name && method.GetParameters().Length == args.Length))
            {
                var parameters = method.GetParameters();
                if (parameters.Where((parameter, index) => args[index] != null && !parameter.ParameterType.IsInstanceOfType(args[index])).Any()) continue;
                return method.Invoke(instance, args);
            }
            throw new MissingMethodException(instance.GetType().FullName, name);
        }

        private static object[] Items(object value) => value is IEnumerable enumerable && value is not string
            ? enumerable.Cast<object>().ToArray()
            : Array.Empty<object>();
        private static int CollectionCount(object value) => value is ICollection collection ? collection.Count : Items(value).Length;
    }
}
