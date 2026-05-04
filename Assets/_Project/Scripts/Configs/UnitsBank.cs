using System;
using Project.Gameplay.Units;
using UnityEngine;

namespace Project.Configs
{
    /// <summary>
    /// Банк префабов юнитов: игрок, сундук, варианты врагов по ключу.
    /// </summary>
    [CreateAssetMenu(menuName = "Playable/Units Bank", fileName = "UnitsBank")]
    public sealed class UnitsBank : ScriptableObject
    {
        [Serializable]
        public struct EnemyEntry
        {
            public string Key;
            public EnemyView Prefab;
        }

        public PlayerView PlayerPrefab;
        public ChestView ChestPrefab;
        public EnemyEntry[] Enemies;

        public EnemyView GetEnemy(string key)
        {
            if (Enemies == null || Enemies.Length == 0) return null;
            if (!string.IsNullOrEmpty(key))
            {
                for (var i = 0; i < Enemies.Length; i++)
                    if (Enemies[i].Key == key) return Enemies[i].Prefab;
            }
            // Fallback: первый ненулевой префаб, чтобы спавн не падал на пустом ключе.
            for (var i = 0; i < Enemies.Length; i++)
                if (Enemies[i].Prefab != null) return Enemies[i].Prefab;
            return null;
        }
    }
}
