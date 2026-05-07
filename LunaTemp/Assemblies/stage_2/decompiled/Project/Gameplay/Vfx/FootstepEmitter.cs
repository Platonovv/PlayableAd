using UnityEngine;

namespace Project.Gameplay.Vfx
{
	public sealed class FootstepEmitter : MonoBehaviour
	{
		[SerializeField]
		private Sprite _footSprite;

		[SerializeField]
		private float _emitDistance = 0.4f;

		[SerializeField]
		private float _sideOffset = 0.18f;

		[SerializeField]
		private float _lifetime = 0.9f;

		[SerializeField]
		private float _scale = 0.35f;

		[SerializeField]
		private Color _color = new Color(0.18f, 0.13f, 0.08f, 0.7f);

		[SerializeField]
		private float _yOffset = 0.02f;

		[SerializeField]
		private float _minStepSpeed = 1.5f;

		private float _accumDistance;

		private bool _leftFoot;

		private Vector3 _lastPos;

		private bool _initialized;

		private void OnEnable()
		{
			_lastPos = base.transform.position;
			_accumDistance = 0f;
			_initialized = true;
		}

		private void Update()
		{
			if (!_initialized)
			{
				return;
			}
			Vector3 lastPos = _lastPos;
			_lastPos = base.transform.position;
			float delta = Vector3.Distance(base.transform.position, lastPos);
			float dt = Mathf.Max(Time.deltaTime, 0.0001f);
			if (delta / dt < _minStepSpeed)
			{
				_accumDistance = 0f;
				return;
			}
			_accumDistance += delta;
			if (_accumDistance >= _emitDistance)
			{
				Emit();
				_accumDistance = 0f;
			}
		}

		private void Emit()
		{
			if (!(_footSprite == null))
			{
				GameObject go = new GameObject("Footstep");
				Vector3 pos = base.transform.position;
				pos += base.transform.right * (_leftFoot ? (0f - _sideOffset) : _sideOffset);
				pos.y += _yOffset;
				go.transform.position = pos;
				go.transform.rotation = Quaternion.Euler(90f, base.transform.eulerAngles.y, 0f);
				go.transform.localScale = Vector3.one * _scale;
				SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
				sr.sprite = _footSprite;
				sr.color = _color;
				FootprintFader fader = go.AddComponent<FootprintFader>();
				fader.Configure(_lifetime, _color);
				_leftFoot = !_leftFoot;
			}
		}
	}
}
