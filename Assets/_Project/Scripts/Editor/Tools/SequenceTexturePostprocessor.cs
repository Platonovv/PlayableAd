using UnityEditor;
using UnityEngine;

namespace Project.EditorTools.Tools
{
    /// <summary>
    /// Авто-импорт PNG из <c>Art/Sequences/</c> и <c>Art/FX/</c> как спрайтов с crunch-сжатием.
    /// </summary>
    public sealed class SequenceTexturePostprocessor : AssetPostprocessor
    {
        private static readonly string[] Roots = { "/Art/Sequences/", "/Art/FX/" };

        private void OnPreprocessTexture()
        {
            var match = false;
            for (var i = 0; i < Roots.Length && !match; i++)
                match = assetPath.Contains(Roots[i]);
            if (!match) return;
            if (assetImporter is not TextureImporter t) return;

            t.textureType = TextureImporterType.Sprite;
            t.spriteImportMode = SpriteImportMode.Single;
            t.spritePixelsPerUnit = 100f;
            t.mipmapEnabled = false;
            t.filterMode = FilterMode.Bilinear;
            t.alphaIsTransparency = true;
            t.maxTextureSize = 256;
            t.crunchedCompression = true;
            t.compressionQuality = 40;

            var settings = t.GetDefaultPlatformTextureSettings();
            settings.format = TextureImporterFormat.Automatic;
            settings.maxTextureSize = 256;
            settings.crunchedCompression = true;
            settings.compressionQuality = 40;
            t.SetPlatformTextureSettings(settings);
        }
    }
}
