using System;
using System.Collections.Generic;
using Project.Domain;
using UnityEngine;

namespace Project.Configs
{
	[CreateAssetMenu(menuName = "Playable/Level Config", fileName = "LevelConfig")]
	public sealed class LevelConfig : ScriptableObject
	{
		[Serializable]
		public struct PlayerSpawn
		{
			public Vector2 Position;

			public int Power;
		}

		[Serializable]
		public struct TargetSpawn
		{
			public UnitKind Kind;

			public Vector2 Position;

			public int Power;

			public string PrefabKey;
		}

		public PlayerSpawn Player = new PlayerSpawn
		{
			Position = new Vector2(0f, -3f),
			Power = 5
		};

		public List<TargetSpawn> Targets = new List<TargetSpawn>();
	}
}
