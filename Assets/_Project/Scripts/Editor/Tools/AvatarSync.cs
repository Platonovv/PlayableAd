using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Project.EditorTools.Tools
{
    /// <summary>
    /// Синхронизация Avatar'ов FBX: первый — мастер, остальные копируют его Avatar (для Generic-rig'ов).
    /// </summary>
    public static class AvatarSync
    {
        [MenuItem("Playable/Tools/Sync Avatars in Selection")]
        public static void Sync()
        {
            var selected = Selection.objects;
            if (selected == null || selected.Length < 2)
            {
                Debug.LogError("[AvatarSync] Выдели в Project ≥ 2 FBX: первый — мастер (источник Avatar), остальные — копии.");
                return;
            }

            var masterPath = AssetDatabase.GetAssetPath(selected[0]);
            var masterImporter = AssetImporter.GetAtPath(masterPath) as ModelImporter;
            if (masterImporter == null)
            {
                Debug.LogError($"[AvatarSync] {masterPath} не является моделью.");
                return;
            }

            masterImporter.animationType = ModelImporterAnimationType.Generic;
            masterImporter.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            masterImporter.SaveAndReimport();

            var masterAvatar = AssetDatabase.LoadAllAssetsAtPath(masterPath)
                .OfType<Avatar>()
                .FirstOrDefault();
            if (masterAvatar == null)
            {
                Debug.LogError("[AvatarSync] Не удалось извлечь Avatar из мастера. Проверь что FBX рангирован (есть кости).");
                return;
            }

            var copied = 0;
            for (var i = 1; i < selected.Length; i++)
            {
                var path = AssetDatabase.GetAssetPath(selected[i]);
                var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                if (importer == null) continue;
                importer.animationType = ModelImporterAnimationType.Generic;
                importer.avatarSetup = ModelImporterAvatarSetup.CopyFromOther;
                importer.sourceAvatar = masterAvatar;
                importer.SaveAndReimport();
                copied++;
            }

            Debug.Log($"[AvatarSync] Мастер: {masterPath}. Avatar: {masterAvatar.name}. Скопировано в {copied} FBX.");
            EditorGUIUtility.PingObject(masterAvatar);
        }
    }
}
