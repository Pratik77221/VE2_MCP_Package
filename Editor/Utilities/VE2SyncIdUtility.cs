using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Imperial.VE2.MCP.Editor
{
    internal static class VE2SyncIdUtility
    {
        private static readonly Dictionary<string, string> PrefixByComponentName = new()
        {
            ["V_ToggleActivatable"] = "Activatable-",
            ["V_HoldActivatable"] = "HoldActivatable-",
            ["V_FreeGrabbable"] = "FreeGrabbable-",
            ["V_PressurePlateActivatable"] = "PressurePlate-",
            ["V_RotatingAdjustable"] = "RotationalAdjustable-",
            ["V_Rotating2DAdjustable"] = "RotationalAdjustable-",
            ["V_SlidingAdjustable"] = "LinearAdjustable-",
            ["V_Sliding2DAdjustable"] = "LinearAdjustable-",
            ["V_HandheldActivatable"] = "HHActivatable-",
            ["V_HandheldAdjustable"] = "HHAdjustable-",
            ["V_NetworkObject"] = "NetObj-",
            ["V_TransformSyncable"] = "TS-",
            ["V_RigidbodySyncable"] = "RBS-",
            ["V_InstantMessageHandler"] = "IMH-"
        };

        public static SyncValidationReport Scan()
        {
            var entries = new Dictionary<string, List<SyncIdEntry>>();

            foreach (var transform in VE2Reflection.GetAllSceneTransforms())
            {
                foreach (var component in transform.GetComponents<Component>())
                {
                    if (component == null)
                    {
                        continue;
                    }

                    var typeName = component.GetType().Name;
                    if (PrefixByComponentName.TryGetValue(typeName, out var prefix))
                    {
                        Add(entries, prefix + transform.name, transform.gameObject, typeName);
                    }

                    if (typeName == "V_CustomInfoPoint")
                    {
                        var triggerHandler = VE2Reflection.GetComponentInChildrenByTypeName(transform.gameObject, "InfoPointTriggerAnimationHandler");
                        var triggerObject = triggerHandler != null ? triggerHandler.gameObject : transform.gameObject;
                        Add(entries, "Activatable-" + triggerObject.name, triggerObject, typeName);
                    }
                }
            }

            return new SyncValidationReport
            {
                totalSyncIds = entries.Count,
                clashCount = entries.Count(pair => pair.Value.Count > 1),
                clashes = entries
                    .Where(pair => pair.Value.Count > 1)
                    .Select(pair => new SyncIdClash { id = pair.Key, entries = pair.Value.ToArray() })
                    .ToArray()
            };
        }

        public static object Fix(bool dryRun)
        {
            var before = Scan();
            if (dryRun || before.clashes.Length == 0)
            {
                return new
                {
                    dryRun,
                    renamed = 0,
                    before,
                    after = before
                };
            }

            var renamed = 0;
            var usedNames = new HashSet<string>(VE2Reflection.GetAllSceneTransforms().Select(t => t.name));

            foreach (var clash in before.clashes)
            {
                var keepFirst = true;
                foreach (var entry in clash.entries)
                {
                    var go = EditorUtility.EntityIdToObject((EntityId)entry.instanceID) as GameObject;
                    if (go == null)
                    {
                        continue;
                    }

                    if (keepFirst)
                    {
                        keepFirst = false;
                        continue;
                    }

                    var newName = NextAvailableName(go.name, usedNames);
                    Undo.RecordObject(go, "VE2 MCP Fix GameObject Name Clash");
                    go.name = newName;
                    usedNames.Add(newName);
                    renamed++;
                }
            }

            VE2Reflection.MarkActiveSceneDirty();
            return new
            {
                dryRun = false,
                renamed,
                before,
                after = Scan()
            };
        }

        private static void Add(Dictionary<string, List<SyncIdEntry>> entries, string id, GameObject go, string componentType)
        {
            if (!entries.TryGetValue(id, out var list))
            {
                list = new List<SyncIdEntry>();
                entries[id] = list;
            }

            list.Add(new SyncIdEntry
            {
                id = id,
                gameObjectName = go.name,
                gameObjectPath = VE2Reflection.GetHierarchyPath(go.transform),
                componentType = componentType,
                instanceID = go.GetInstanceID()
            });
        }

        private static string NextAvailableName(string currentName, HashSet<string> usedNames)
        {
            var baseName = Regex.Replace(currentName, @"\d+$", "");
            if (string.IsNullOrWhiteSpace(baseName))
            {
                baseName = currentName;
            }

            var number = 2;
            string candidate;
            do
            {
                candidate = $"{baseName}{number++}";
            } while (usedNames.Contains(candidate));

            return candidate;
        }
    }

    internal sealed class SyncValidationReport
    {
        public int totalSyncIds;
        public int clashCount;
        public SyncIdClash[] clashes;
    }

    internal sealed class SyncIdClash
    {
        public string id;
        public SyncIdEntry[] entries;
    }

    internal sealed class SyncIdEntry
    {
        public string id;
        public string gameObjectName;
        public string gameObjectPath;
        public string componentType;
        public int instanceID;
    }
}
