using UnityEngine;

namespace Project.Gameplay.Vfx
{
	/// <summary>
	/// Спавнит дискретные следы шагов (left/right) при движении объекта; следы плавно затухают.
	/// </summary>
	public sealed class FootstepEmitter : MonoBehaviour
	{
		[SerializeField] private Sprite _footSprite;
		[SerializeField] private Animator _animator;
		[SerializeField] private string _runStateName = "Run";
		[SerializeField] private float _emitDistance = 0.4f;
		[SerializeField] private float _sideOffset = 0.18f;
		[SerializeField] private float _lifetime = 0.9f;
		[SerializeField] private float _scale = 0.35f;
		[SerializeField] private Color _color = new Color(0.18f, 0.13f, 0.08f, 0.7f);
		[SerializeField] private float _yOffset = 0.02f;
		[SerializeField] private float _minStepSpeed = 1.5f;

		private int _runHash;
		private float _accumDistance;
		private bool _leftFoot;
		private Vector3 _lastPos;
		private bool _initialized;

		private void Awake()
		{
			_runHash = Animator.StringToHash(_runStateName);
		}

		private void OnEnable()
		{
			_lastPos = transform.position;
			_accumDistance = 0f;
			_initialized = true;
		}

		private void Update()
		{
			if (!_initialized)
				return;

			var lastPos = _lastPos;
			_lastPos = transform.position;

			if (!IsRunning())
			{
				_accumDistance = 0f;
				return;
			}

			var delta = Vector3.Distance(transform.position, lastPos);
			var dt = Mathf.Max(Time.deltaTime, 0.0001f);
			if (delta / dt < _minStepSpeed)
			{
				_accumDistance = 0f;
				return;
			}

			_accumDistance += delta;
			if (!(_accumDistance >= _emitDistance))
				return;

			Emit();
			_accumDistance = 0f;
		}

		private bool IsRunning()
		{
			if (_animator == null)
				return true;

			var info = _animator.GetCurrentAnimatorStateInfo(0);
			return info.shortNameHash == _runHash;
		}

		private void Emit()
		{
			if (_footSprite == null)
				return;

			var go = new GameObject("Footstep");
			var pos = transform.position;
			pos += transform.right * (_leftFoot ? -_sideOffset : _sideOffset);
			pos.y += _yOffset;
			go.transform.position = pos;
			go.transform.rotation = Quaternion.Euler(90f, transform.eulerAngles.y, 0f);
			go.transform.localScale = Vector3.one * _scale;

			var sr = go.AddComponent<SpriteRenderer>();
			sr.sprite = _footSprite;
			sr.color = _color;

			var fader = go.AddComponent<FootprintFader>();
			fader.Configure(_lifetime, _color);
			_leftFoot = !_leftFoot;
		}
	}

	/// <summary>
	/// Затухает SpriteRenderer на ноде и саморазрушается по истечении lifetime.
	/// </summary>
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
			var k = Mathf.Clamp01(_elapsed / _lifetime);
			if (_sr != null)
			{
				var c = _base;
				c.a = _base.a * (1f - k);
				_sr.color = c;
			}

			if (_elapsed >= _lifetime)
				Destroy(gameObject);
		}
	}
}