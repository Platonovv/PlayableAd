using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Project.EditorTools.Tools
{
    /// <summary>
    /// Печатает в консоль список ассетов под <c>Assets/_Project/</c>, которые НЕ ссылаются ни одной включённой сценой.
    /// Сортирует по размеру и предлагает удалить — это самый быстрый способ снять «жир» с билда.
    /// </summary>
    public static class BuildSizeAudit
    {
        private static readonly string[] Roots =
        {
            "Assets/_Project/Art",
            "Assets/_Project/Audio",
            "Assets/_Project/Configs",
            "Assets/_Project/Plugins",
            "Assets/_Project/SO"
        };

        [MenuItem("Playable/Tools/Build Size Audit")]
        public static void Audit()
        {
            // 1. Все ассеты под нашими корнями.
            var allAssets = new HashSet<string>();
            foreach (var root in Roots)
            {
                if (!AssetDatabase.IsValidFolder(root)) continue;
                foreach (var guid in AssetDatabase.FindAssets(string.Empty, new[] { root }))
                {
                    var p = AssetDatabase.GUIDToAssetPath(guid);
                    if (string.IsNullOrEmpty(p)) continue;
                    if (AssetDatabase.IsValidFolder(p)) continue;
                    if (p.EndsWith(".meta")) continue;
                    allAssets.Add(p);
                }
            }

            // 2. Зависимости всех включённых сцен (рекурсивно).
            var used = new HashSet<string>();
            foreach (var s in EditorBuildSettings.scenes)
            {
                if (!s.enabled || !File.Exists(s.path)) continue;
                foreach (var dep in AssetDatabase.GetDependencies(s.path, true))
                    used.Add(dep);
            }

            // 3. Diff — то, что не используется.
            var unused = allAssets.Where(p => !used.Contains(p))
                                  .Select(p => (path: p, size: GetSize(p)))
                                  .Where(t => t.size > 0)
                                  .OrderByDescending(t => t.size)
                                  .ToList();

            var totalUnused = unused.Sum(t => t.size);
            var totalAll = allAssets.Sum(p => GetSize(p));

            // 4. Отчёт.
            var sb = new StringBuilder();
            sb.AppendLine("=== Build Size Audit ===");
            sb.AppendLine($"Корни: {string.Join(", ", Roots)}");
            sb.AppendLine($"Всего ассетов: {allAssets.Count}, общий размер: {Format(totalAll)}");
            sb.AppendLine($"Используется сценой: {used.Intersect(allAssets).Count()}");
            sb.AppendLine($"НЕ используется: {unused.Count} ({Format(totalUnused)})");
            sb.AppendLine();
            sb.AppendLine("Top 30 неиспользуемых:");
            foreach (var (path, size) in unused.Take(30))
                sb.AppendLine($"  {Format(size),10}  {path}");
            sb.AppendLine();
            sb.AppendLine("Полный список — см. ниже в Console (если обрезано — увеличь Log limit).");

            Debug.Log(sb.ToString());

            // Полный список отдельным сообщением.
            if (unused.Count > 30)
            {
                var full = new StringBuilder();
                full.AppendLine($"=== Build Size Audit — ALL {unused.Count} unused ===");
                foreach (var (path, size) in unused)
                    full.AppendLine($"  {Format(size),10}  {path}");
                Debug.Log(full.ToString());
            }

            // 5. Предложить удалить.
            if (unused.Count == 0)
            {
                EditorUtility.DisplayDialog("Build Size Audit", "Чисто! Все ассеты используются.", "OK");
                return;
            }

            var ok = EditorUtility.DisplayDialog(
                "Build Size Audit",
                $"Найдено {unused.Count} неиспользуемых ассетов ({Format(totalUnused)}).\n" +
                "Удалить их сейчас? Действие необратимо без git.",
                "Удалить", "Отмена");

            if (!ok) return;

            var deleted = 0;
            foreach (var (path, _) in unused)
            {
                if (AssetDatabase.DeleteAsset(path)) deleted++;
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[Audit] Удалено: {deleted} ассетов, освобождено {Format(totalUnused)}.");
        }

        private static long GetSize(string path)
        {
            if (string.IsNullOrEmpty(path)) return 0;
            try { return new FileInfo(path).Length; }
            catch { return 0; }
        }

        private static string Format(long bytes)
        {
            if (bytes >= 1024 * 1024) return $"{bytes / 1024f / 1024f:F2} MB";
            if (bytes >= 1024) return $"{bytes / 1024f:F1} KB";
            return $"{bytes} B";
        }
    }
}
