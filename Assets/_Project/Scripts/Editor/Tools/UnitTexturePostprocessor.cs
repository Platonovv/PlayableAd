using UnityEditor;
using UnityEngine;

namespace Project.EditorTools.Tools
{
    /// <summary>
    /// Меню для разового пере-импорта всех текстур юнитов и FX по новым правилам.
    /// </summary>
    public static class TextureReimporter
    {
        [MenuItem("Playable/Tools/Reimport All Textures")]
        public static void ReimportAll()
        {
            var paths = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/_Project/Art" });
            foreach (var guid in paths)
            {
                var p = AssetDatabase.GUIDToAssetPath(guid);
                AssetDatabase.ImportAsset(p, ImportAssetOptions.ForceUpdate);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[Reimport] Re-imported {paths.Length} textures.");
        }
    }
}

namespace Project.EditorTools.Tools.Postprocessors
{
    /// <summary>
    /// Авто-настройка текстур юнитов (Heroes/Mobs/Chest): 256px max, crunch q=40.
    /// На портретном экране юниты рендерятся ~200×400px, 256 хватает с запасом.
    /// </summary>
    public sealed class UnitTexturePostprocessor : AssetPostprocessor
    {
        private static readonly string[] Roots = { "/Art/Heroes/", "/Art/Mobs/", "/Art/Chest/", "/Art/Backgrounds/" };
        private static readonly int[] Maxes    = { 256, 128, 128, 512 };

        private void OnPreprocessTexture()
        {
            int max = -1;
            for (var i = 0; i < Roots.Length; i++)
            {
                if (assetPath.Contains(Roots[i])) { max = Maxes[i]; break; }
            }
            if (max < 0) return;
            if (!(assetImporter is TextureImporter t)) return;
            if (t.textureType != TextureImporterType.Default) return;

            t.mipmapEnabled = false;
            t.filterMode = FilterMode.Bilinear;
            t.maxTextureSize = max;
            t.crunchedCompression = true;
            t.compressionQuality = 40;

            var settings = t.GetDefaultPlatformTextureSettings();
            settings.format = TextureImporterFormat.Automatic;
            settings.maxTextureSize = max;
            settings.crunchedCompression = true;
            settings.compressionQuality = 40;
            t.SetPlatformTextureSettings(settings);
        }
    }
}
