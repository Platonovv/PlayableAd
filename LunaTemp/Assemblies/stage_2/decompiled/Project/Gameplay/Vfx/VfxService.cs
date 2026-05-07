using UnityEngine;

namespace Project.Gameplay.Vfx
{
	public sealed class VfxService : MonoBehaviour
	{
		[SerializeField]
		private VfxBank _bank;

		[SerializeField]
		private float _autoDestroy = 2.5f;

		public void Play(string key, Vector3 position, Quaternion rotation = default(Quaternion))
		{
			GameObject prefab = ((_bank != null) ? _bank.Get(key) : null);
			if (!(prefab == null))
			{
				GameObject inst = Object.Instantiate(prefab, position, (rotation == default(Quaternion)) ? Quaternion.identity : rotation);
				if (_autoDestroy > 0f)
				{
					Object.Destroy(inst, _autoDestroy);
				}
			}
		}
	}
}
