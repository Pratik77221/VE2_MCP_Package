using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Imperial.VE2.MCP.Editor
{
    public static partial class VE2McpTools
    {
        private static string UploaderLastMessage;

        public static object RescanBuildVersions(VE2BuildPlatformParams parameters)
        {
            return GuardVE2(() =>
            {
                if (!BuildPlatform(parameters?.Platform, out var platform, out var error))
                    return Response.Error(error);
                if (EditorApplication.isPlaying || EditorApplication.isCompiling)
                    return Response.Error("Cannot scan VE2 deployment versions while Unity is playing or compiling.");
                if (ReadMember(Uploader, "Uploading") as bool? == true)
                    return Response.Error("A VE2 deployment upload is already running. Cancel or wait for it before rescanning.");

                var scene = SceneManager.GetActiveScene();
                if (string.IsNullOrWhiteSpace(scene.path))
                    return Response.Error("Save the active scene before scanning VE2 deployment versions.");
                if (string.IsNullOrWhiteSpace(scene.name))
                    return Response.Error("The active scene must have a valid name before scanning VE2 deployment versions.");

                AutoUpload = false;
                UploaderLastMessage = null;
                var uploader = CreateUploader(platform, true);
                InvokeMember(uploader, "SearchForVersion");
                EnsureUploaderTicking();
                return Response.Success("Started VE2's local and remote version scan.", UploaderStatus());
            });
        }

        public static object UploadDeployment(VE2DeploymentUploadParams parameters)
        {
            return GuardVE2(() =>
            {
                if (parameters == null || !parameters.ConfirmUpload)
                    return Response.Error("ConfirmUpload=true is required because this command publishes files to VE2's remote deployment service.");
                if (!BuildPlatform(parameters.Platform, out var platform, out var error))
                    return Response.Error(error);
                if (EditorApplication.isPlaying || EditorApplication.isCompiling || BuildPipeline.isBuildingPlayer)
                    return Response.Error("Cannot upload a VE2 deployment while Unity is playing, compiling, or building.");
                if (ReadMember(Uploader, "Uploading") as bool? == true)
                    return Response.Error("A VE2 deployment upload is already running.");

                var preflight = RunDeploymentPreflight(true);
                if (!preflight.valid)
                    return Response.Error("VE2 deployment preflight failed. Resolve its errors before uploading.");

                var scene = SceneManager.GetActiveScene();
                var localVersions = LocalVersions(platform, scene.name);
                if (localVersions.Length == 0)
                    return Response.Error($"No local {platform} build exists for scene '{scene.name}'. Build the plugin before uploading.");

                UploaderLastMessage = null;
                var uploader = CreateUploader(platform, true);
                AutoUpload = true;
                InvokeMember(uploader, "SearchForVersion");
                EnsureUploaderTicking();
                return Response.Success(
                    "Started VE2's remote version scan. The newest valid local build will upload automatically when the scan completes.",
                    new { scene = scene.name, platform, highestLocalVersion = localVersions.Max(), status = UploaderStatus() });
            });
        }

        public static object CancelDeployment()
        {
            return GuardVE2(() =>
            {
                AutoUpload = false;
                var cancelled = 0;
                foreach (var task in GetUploaderTasks())
                {
                    if (ReadMember(task, "IsCancellable") is not bool canCancel || !canCancel)
                        continue;

                    InvokeMember(task, "CancelRemoteFileTask");
                    cancelled++;
                }

                UploaderLastMessage = cancelled > 0
                    ? $"Cancelled {cancelled} VE2 deployment task(s)."
                    : "No cancellable VE2 deployment tasks were found.";
                TickUploader();
                return Response.Success(UploaderLastMessage, UploaderStatus());
            });
        }

        private static object CreateUploader(string platform, bool reset)
        {
            if (Uploader == null || reset || !string.Equals(UploaderPlatform, platform, StringComparison.Ordinal))
            {
                var type = VE2Reflection.FindType(UploaderTypeName);
                if (type == null)
                    throw new TypeLoadException("Could not find VE2's PluginUploaderLogic in the installed VE2 version.");
                StopUploaderTicking();
                Uploader = Activator.CreateInstance(type, true);
                UploaderPlatform = platform;
            }

            var builderType = VE2Reflection.FindType(BuilderTypeName)
                ?? throw new TypeLoadException("Could not find VE2's plugin builder in the installed VE2 version.");
            var environmentType = builderType.GetNestedType("EnvironmentType", BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new TypeLoadException("Could not find VE2's build environment type.");
            var environment = Enum.Parse(environmentType, platform, true);
            InvokeMember(Uploader, "SetContext", null, SceneManager.GetActiveScene().name, environment);
            if (reset) InvokeMember(Uploader, "ForceRescan");
            EnsureUploaderTicking();
            return Uploader;
        }

        private static void EnsureUploaderTicking()
        {
            if (UploaderTicking) return;
            EditorApplication.update += TickUploader;
            UploaderTicking = true;
        }

        private static void StopUploaderTicking()
        {
            if (!UploaderTicking) return;
            EditorApplication.update -= TickUploader;
            UploaderTicking = false;
        }

        private static void TickUploader()
        {
            if (Uploader == null)
            {
                StopUploaderTicking();
                return;
            }

            try
            {
                InvokeMember(Uploader, "Tick");
                var searching = ReadField(Uploader, "_searchingForVersion") as bool? == true;
                var uploading = ReadMember(Uploader, "Uploading") as bool? == true;
                var done = ReadMember(Uploader, "DoneUpload") as bool? == true;
                var failed = ReadMember(Uploader, "ErrorUploading") as bool? == true;

                if (AutoUpload && !searching)
                {
                    if (ReadMember(Uploader, "CanUpload") as bool? == true)
                    {
                        InvokeMember(Uploader, "BeginUpload");
                        UploaderLastMessage = "VE2 remote version scan completed and upload started.";
                    }
                    else
                    {
                        UploaderLastMessage = "VE2 upload did not start because no eligible local build is newer than the remote version.";
                    }
                    AutoUpload = false;
                    uploading = ReadMember(Uploader, "Uploading") as bool? == true;
                }

                if (done) UploaderLastMessage = "VE2 deployment upload completed.";
                if (failed) UploaderLastMessage = "VE2 deployment upload failed. Inspect task status and the Unity Console.";
                if (!searching && !uploading && !AutoUpload) StopUploaderTicking();
            }
            catch (Exception ex)
            {
                UploaderLastMessage = $"VE2 deployment update failed: {ex.GetBaseException().Message}";
                AutoUpload = false;
                StopUploaderTicking();
            }
        }

        private static object UploaderStatus()
        {
            if (Uploader == null)
            {
                return new { available = false, message = UploaderLastMessage };
            }

            var messages = Items(InvokeMember(Uploader, "GetStatusMessages"))
                .Select(message => new
                {
                    text = ReadField(message, "text")?.ToString(),
                    type = ReadField(message, "type")?.ToString()
                }).ToArray();
            var tasks = GetUploaderTasks().Select(task => new
            {
                type = ReadMember(task, "Type")?.ToString(),
                nameAndPath = ReadMember(task, "NameAndPath")?.ToString(),
                progress = ReadMember(task, "Progress"),
                status = ReadMember(task, "Status")?.ToString(),
                isCancellable = ReadMember(task, "IsCancellable")
            }).ToArray();

            return new
            {
                available = true,
                platform = UploaderPlatform,
                searchingForVersion = ReadField(Uploader, "_searchingForVersion"),
                highestLocalVersion = ReadMember(Uploader, "HighestLocalVersionFound"),
                highestRemoteVersion = ReadField(Uploader, "_highestRemoteVersionFound"),
                canUpload = ReadMember(Uploader, "CanUpload"),
                uploading = ReadMember(Uploader, "Uploading"),
                done = ReadMember(Uploader, "DoneUpload"),
                error = ReadMember(Uploader, "ErrorUploading"),
                files = Items(ReadMember(Uploader, "ExportFiles")).Select(item => item?.ToString()).ToArray(),
                messages,
                tasks,
                message = UploaderLastMessage
            };
        }

        private static object[] GetUploaderTasks() => Uploader == null
            ? Array.Empty<object>()
            : Items(ReadMember(Uploader, "UploadTasks"));
    }
}
