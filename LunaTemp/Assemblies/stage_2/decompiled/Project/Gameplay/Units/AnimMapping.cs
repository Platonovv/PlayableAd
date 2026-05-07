using System;
using UnityEngine;

namespace Project.Gameplay.Units
{
	[Serializable]
	public struct AnimMapping
	{
		public string StateName;

		public AnimationClip Clip;

		public bool Loop;
	}
}
