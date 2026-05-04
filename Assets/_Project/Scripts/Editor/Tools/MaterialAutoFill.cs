using System.IO;
using UnityEditor;
using UnityEngine;

namespace Project.EditorTools.Tools
{
    /// <summary>
    /// Авто-заполнение <c>_MainTex</c> в материалах под <c>Art/</c> по совпадению имён + фикс URP-шейдеров.
    /// </summary>
    public static class MaterialAutoFill
    {
        [MenuItem("Playable/Tools/Auto Fill Material Textures")]
        public static void Fill()
        {
            var mobile = Shader.Find("Mobile/Diffuse") ?? Shader.Find("Standard");
            if (mobile == null)
            {
                Debug.LogError("[AutoFill] Не найден шейдер Mobile/Diffuse или Standard.");
                return;
            }

            var fixedCount = 0;
            var matGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/_Project/Art" });
            foreach (var guid in matGuids)
            {
                var matPath = AssetDatabase.GUIDToAssetPath(guid);
                var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                if (mat == null) continue;

                var changed = false;

                // 1. Шейдер: если сломан/URP — переходим на Mobile/Diffuse.
                if (IsBrokenShader(mat.shader))
                {
                    mat.shader = mobile;
                    changed = true;
                }

                // 2. Главная текстура: если пусто — ищем подходящую.
                if (mat.HasProperty("_MainTex") && mat.mainTexture == null)
                {
                    var tex = FindTextureNear(matPath, mat.name);
                    if (tex != null)
                    {
                        mat.mainTexture = tex;
                        changed = true;
                        Debug.Log($"[AutoFill] {mat.name} ← {tex.name}");
                    }
                }

                if (changed) EditorUtility.SetDirty(mat);
                if (changed) fixedCount++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[AutoFill] Готово. Обновлено материалов: {fixedCount}");
        }

        private static bool IsBrokenShader(Shader sh)
        {
            if (sh == null) return true;
            var n = sh.name;
            return n == "Hidden/InternalErrorShader"
                || n.StartsWith("Universal Render Pipeline/")
                || n.StartsWith("HDRP/")
                || n.StartsWith("Lightweight Render Pipeline/");
        }

        private static Texture2D FindTextureNear(string materialPath, string materialName)
        {
            var matFolder = Path.GetDirectoryName(materialPath)?.Replace('\\', '/');
            if (string.IsNullOrEmpty(matFolder)) return null;
            var parentFolder = Path.GetDirectoryName(matFolder)?.Replace('\\', '/');

            var matName = Normalize(materialName);

            Texture2D best = null;
            var bestScore = 0;

            foreach (var folder in new[] { matFolder, parentFolder })
            {
                if (string.IsNullOrEmpty(folder)) continue;
                var texGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });
                foreach (var tg in texGuids)
                {
                    var tpath = AssetDatabase.GUIDToAssetPath(tg);
                    var dir = Path.GetDirectoryName(tpath)?.Replace('\\', '/');
                    if (dir != folder) continue; // только прямой каталог, не вложенные
                    var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(tpath);
                    if (tex == null) continue;

                    var score = MatchScore(matName, Normalize(tex.name));
                    if (score > bestScore)
                    {
                        bestScore = score;
                        best = tex;
                    }
                }
                if (best != null) break; // нашли в matFolder — в parent уже не лезем
            }

            return best;
        }

        private static string Normalize(string s)
        {
            // Убираем регистр, цифровые суффиксы и общие слова, чтобы лучше матчить.
            s = s.ToLowerInvariant().Replace(" copy", "");
            for (var i = 0; i < 10; i++) s = s.Replace(i.ToString(), "");
            s = s.Replace("up", "").Replace("hq", "").Replace("fb_", "").Replace("df_", "").Trim('_', ' ');
            return s;
        }

        private static int MatchScore(string a, string b)
        {
            // Длина общего префикса: чем длиннее — тем ближе соответствие.
            var n = Mathf.Min(a.Length, b.Length);
            var i = 0;
            while (i < n && a[i] == b[i]) i++;
            // Бонус если одно содержит другое целиком.
            if (i < n && (a.Contains(b) || b.Contains(a))) i += Mathf.Min(a.Length, b.Length) / 2;
            return i;
        }
    }
}
