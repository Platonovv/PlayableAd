using System;
using Project.Gameplay.Units;
using UnityEngine;

namespace Project.Configs
{
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
			if (Enemies == null || Enemies.Length == 0)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(key))
			{
				for (int i = 0; i < Enemies.Length; i++)
				{
					if (Enemies[i].Key == key)
					{
						return Enemies[i].Prefab;
					}
				}
			}
			for (int j = 0; j < Enemies.Length; j++)
			{
				if (Enemies[j].Prefab != null)
				{
					return Enemies[j].Prefab;
				}
			}
			return null;
		}
	}
}
