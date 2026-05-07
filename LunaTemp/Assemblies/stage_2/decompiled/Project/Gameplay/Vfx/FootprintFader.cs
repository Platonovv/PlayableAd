using UnityEngine;

namespace Project.Gameplay.Vfx
{
	public sealed class FootprintFader : MonoBehaviour
	{
		private float _lifetime;

		private float _elapsed;

		private SpriteRenderer _sr;

		private Color _base;

		public void Configure(float lifetime, Color baseColor)
		{
			_lifetime = Mathf.Max(0.0001f, lifetime);
			_base = baseColor;
			_sr = GetComponent<SpriteRenderer>();
		}

		private void Update()
		{
			_elapsed += Time.deltaTime;
			float i = Mathf.Clamp01(_elapsed / _lifetime);
			if (_sr != null)
			{
				Color c = _base;
				c.a = _base.a * (1f - i);
				_sr.color = c;
			}
			if (_elapsed >= _lifetime)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
