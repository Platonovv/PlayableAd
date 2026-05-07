using UnityEngine;

namespace Project.Gameplay.Vfx
{
	public sealed class SpriteSequencePlayer : MonoBehaviour
	{
		[SerializeField]
		private SpriteRenderer _renderer;

		[SerializeField]
		private Sprite[] _frames;

		[SerializeField]
		private float _fps = 15f;

		[SerializeField]
		private bool _loop;

		[SerializeField]
		private bool _disableOnFinish = true;

		[SerializeField]
		private bool _billboardToCamera = true;

		private float _elapsed;

		private bool _playing;

		private Camera _camera;

		private void OnEnable()
		{
			if (_camera == null)
			{
				_camera = Camera.main;
			}
			Play();
		}

		public void Play()
		{
			_elapsed = 0f;
			_playing = _frames != null && _frames.Length != 0 && _renderer != null;
			if (_playing)
			{
				_renderer.sprite = _frames[0];
			}
		}

		private void LateUpdate()
		{
			if (_billboardToCamera && _camera != null)
			{
				base.transform.forward = _camera.transform.forward;
			}
		}

		private void Update()
		{
			if (!_playing)
			{
				return;
			}
			_elapsed += Time.deltaTime;
			int idx = Mathf.FloorToInt(_elapsed * _fps);
			if (idx >= _frames.Length)
			{
				if (!_loop)
				{
					_playing = false;
					if (_disableOnFinish)
					{
						base.gameObject.SetActive(false);
					}
					return;
				}
				_elapsed = 0f;
				idx = 0;
			}
			_renderer.sprite = _frames[idx];
		}
	}
}
