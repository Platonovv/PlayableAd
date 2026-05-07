using System;
using UnityEngine;

namespace Project.Audio
{
	[CreateAssetMenu(menuName = "Playable/Audio Bank", fileName = "AudioBank")]
	public sealed class AudioBank : ScriptableObject
	{
		[Serializable]
		public struct Sfx
		{
			public string Key;

			public AudioClip Clip;

			[Range(0f, 1f)]
			public float Volume;
		}

		public AudioClip Music;

		[Range(0f, 1f)]
		public float MusicVolume = 0.6f;

		public Sfx[] Sfxs;
	}
}
