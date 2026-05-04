using UnityEngine;

namespace Project.Gameplay.Vfx
{
	/// <summary>
	/// Спавнер VFX из <see cref="VfxBank"/>; инстансы саморазрушаются по таймеру.
	/// </summary>
	public sealed class VfxService : MonoBehaviour
	{
		[SerializeField] private VfxBank _bank;
		[SerializeField] private float _autoDestroy = 2.5f;

		public void Play(string key, Vector3 position, Quaternion rotation = default)
		{
			var prefab = _bank != null ? _bank.Get(key) : null;
			if (prefab == null)
				return;

			var inst = Instantiate(prefab, position, rotation == default ? Quaternion.identity : rotation);
			if (_autoDestroy > 0f)
				Destroy(inst, _autoDestroy);
		}
	}
}