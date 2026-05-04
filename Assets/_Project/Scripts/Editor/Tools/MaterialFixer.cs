using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Project.EditorTools.Tools
{
    /// <summary>
    /// Чинит «розовые» материалы (URP-шейдер): перевешивает на <c>Mobile/Diffuse</c>, переносит текстуру.
    /// </summary>
    public static class MaterialFixer
    {
        [MenuItem("Playable/Tools/Fix Pink Materials")]
        public static void FixAll()
        {
            var fallback = Shader.Find("Mobile/Diffuse");
            var standard = Shader.Find("Standard");
            if (fallback == null) fallback = standard;
            if (fallback == null)
            {
                Debug.LogError("[MaterialFixer] Не нашёл ни Mobile/Diffuse, ни Standard. Built-in shaders отсутствуют?");
                return;
            }

            var fixedCount = 0;
            var guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/_Project" });
            var queue = new List<string>(guids);
            foreach (var guid in queue)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat == null) continue;
                if (!IsBroken(mat)) continue;

                var albedo  = ExtractTexture(mat, "_BaseMap", "_MainTex", "_MainColor");
                var color   = ExtractColor(mat, "_BaseColor", "_Color");

                mat.shader = fallback;
                if (albedo != null && mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", albedo);
                if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
                EditorUtility.SetDirty(mat);
                fixedCount++;
            }

            // Дополнительно: материалы, встроенные в FBX, требуют их «выгрузки» — пройдёмся по импортёрам.
            var modelGuids = AssetDatabase.FindAssets("t:Model", new[] { "Assets/_Project" });
            foreach (var g in modelGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                if (importer == null) continue;
                if (importer.materialImportMode == ModelImporterMaterialImportMode.None) continue;
                importer.materialLocation = ModelImporterMaterialLocation.External;
                importer.SaveAndReimport();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[MaterialFixer] Перевешано шейдеров: {fixedCount}. FBX переимпортированы с External Materials.");
        }

        private static bool IsBroken(Material mat)
        {
            if (mat.shader == null) return true;
            var name = mat.shader.name;
            return name == "Hidden/InternalErrorShader"
                || name.StartsWith("Universal Render Pipeline/")
                || name.StartsWith("HDRP/")
                || name.StartsWith("Lightweight Render Pipeline/");
        }

        private static Texture ExtractTexture(Material mat, params string[] propNames)
        {
            foreach (var p in propNames)
                if (mat.HasProperty(p))
                {
                    var t = mat.GetTexture(p);
                    if (t != null) return t;
                }
            return mat.mainTexture;
        }

        private static Color ExtractColor(Material mat, params string[] propNames)
        {
            foreach (var p in propNames)
                if (mat.HasProperty(p))
                    return mat.GetColor(p);
            return Color.white;
        }
    }
}
