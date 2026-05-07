using System;
using UnityEngine;

namespace Project.Gameplay.Vfx
{
	[CreateAssetMenu(menuName = "Playable/Vfx Bank", fileName = "VfxBank")]
	public sealed class VfxBank : ScriptableObject
	{
		[Serializable]
		public struct Entry
		{
			public string Key;

			public GameObject Prefab;
		}

		public Entry[] Entries;

		public GameObject Get(string key)
		{
			if (string.IsNullOrEmpty(key) || Entries == null)
			{
				return null;
			}
			for (int i = 0; i < Entries.Length; i++)
			{
				if (Entries[i].Key == key)
				{
					return Entries[i].Prefab;
				}
			}
			return null;
		}
	}
}
