using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Project.EditorTools.Tools
{
    /// <summary>
    /// Урезает FBX'ки под playable: оставляет только используемые animation clips,
    /// агрессивно жмёт остальные данные. Один проход — экономия 3–5 MB на hero FBX
    /// (там 28 клипов, из них реально нужны 8).
    /// </summary>
    public static class FbxSlimmer
    {
        private static readonly string[] KeepClips =
        {
            "Idle",
            "Run",
            "Hit",
            "Death",
            "Win",
            "Victory",
            "SuperAttack",
            "AttackPVP_1",
            "Shop_Update",
            "Engage",
            "Open",
            "Close",
            "Attack",
        };

        [MenuItem("Playable/Tools/Slim FBX Animations")]
        public static void Slim()
        {
            var fbxPaths = AssetDatabase.FindAssets("t:Model", new[] { "Assets/_Project/Art" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => p.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase) ||
                            p.EndsWith(".FBX", System.StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (fbxPaths.Length == 0)
            {
                EditorUtility.DisplayDialog("FBX Slimmer", "FBX-файлы не найдены под Assets/_Project/Art.", "OK");
                return;
            }

            var totalRemoved = 0;
            var totalKept = 0;

            foreach (var p in fbxPaths)
            {
                if (!(AssetImporter.GetAtPath(p) is ModelImporter mi)) continue;

                var src = mi.clipAnimations;
                if (src == null || src.Length == 0) src = mi.defaultClipAnimations;
                if (src == null || src.Length == 0) continue;

                var keep = new List<ModelImporterClipAnimation>();
                foreach (var c in src)
                {
                    if (IsKept(c.name))
                        keep.Add(c);
                }

                mi.animationCompression = ModelImporterAnimationCompression.Optimal;
                mi.animationRotationError = 2.0f;
                mi.animationPositionError = 2.0f;
                mi.animationScaleError = 2.0f;

                mi.meshCompression = ModelImporterMeshCompression.High;
                mi.isReadable = false;
                mi.optimizeMeshPolygons = true;
                mi.optimizeMeshVertices = true;
                mi.importBlendShapes = false;
                mi.importVisibility = false;
                mi.importCameras = false;
                mi.importLights = false;
                mi.weldVertices = true;
                mi.optimizeBones = true;

                if (keep.Count > 0 && keep.Count < src.Length)
                {
                    mi.clipAnimations = keep.ToArray();
                    totalRemoved += src.Length - keep.Count;
                    totalKept += keep.Count;
                    Debug.Log($"[FbxSlimmer] {System.IO.Path.GetFileName(p)}: оставлено {keep.Count}/{src.Length} клипов.");
                }
                else
                {
                    Debug.Log($"[FbxSlimmer] {System.IO.Path.GetFileName(p)}: клипы не тронуты ({src.Length} шт.), сжатие усилено.");
                }

                AssetDatabase.WriteImportSettingsIfDirty(p);
                mi.SaveAndReimport();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("FBX Slimmer",
                $"Готово. Обработано FBX: {fbxPaths.Length}.\n" +
                $"Удалено клипов: {totalRemoved}, оставлено: {totalKept}.\n" +
                "Перебилди playable.", "OK");
        }

        private static bool IsKept(string clipName)
        {
            foreach (var k in KeepClips)
                if (clipName.IndexOf(k, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            return false;
        }
    }
}
