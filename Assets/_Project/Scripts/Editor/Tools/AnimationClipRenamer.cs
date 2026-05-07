#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Project.EditorTools
{
    /// <summary>
    /// Переименовывает клипы внутри FBX-импортеров: убирает символы, недопустимые в Windows-путях
    /// (в частности `|` — стандартный разделитель FBX `Armature|Run`). Без этого Playworks падает
    /// в Path.Combine при перечислении анимаций и не упаковывает их в билд.
    /// </summary>
    public static class AnimationClipRenamer
    {
        private static readonly char[] InvalidChars = { '|', ':', '<', '>', '"', '?', '*' };

        [MenuItem("Playable/Tools/Sanitize FBX Animation Clip Names")]
        public static void Run()
        {
            int totalFbx = 0;
            int totalClipsRenamed = 0;
            var renames = new List<string>();

            var guids = AssetDatabase.FindAssets("t:Model", new[] { "Assets/_Project" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase) &&
                    !path.EndsWith(".dae", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                if (importer == null) continue;

                totalFbx++;

                var clips = importer.clipAnimations;
                if (clips == null || clips.Length == 0)
                    clips = importer.defaultClipAnimations;

                bool changed = false;
                for (int i = 0; i < clips.Length; i++)
                {
                    var origName = clips[i].name;
                    var sanitized = Sanitize(origName);
                    if (sanitized != origName)
                    {
                        clips[i].name = sanitized;
                        renames.Add(path + ": '" + origName + "' -> '" + sanitized + "'");
                        totalClipsRenamed++;
                        changed = true;
                    }
                }

                if (changed)
                {
                    importer.clipAnimations = clips;
                    importer.SaveAndReimport();
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            foreach (var r in renames) Debug.Log("[ClipRenamer] " + r);
            EditorUtility.DisplayDialog("Sanitize FBX Animation Names",
                $"FBX обработано: {totalFbx}\nКлипов переименовано: {totalClipsRenamed}\n\nПодробности — в Console.",
                "OK");
        }

        private static string Sanitize(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var arr = name.ToCharArray();
            for (int i = 0; i < arr.Length; i++)
            {
                foreach (var bad in InvalidChars)
                    if (arr[i] == bad) { arr[i] = '_'; break; }
            }
            return new string(arr);
        }
    }
}
#endif
