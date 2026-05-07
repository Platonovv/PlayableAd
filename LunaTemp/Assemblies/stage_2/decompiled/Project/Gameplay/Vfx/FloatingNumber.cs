using System;
using System.Collections;
using Project.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Gameplay.Vfx
{
	public sealed class FloatingNumber : MonoBehaviour
	{
		[SerializeField]
		private Text _text;

		[SerializeField]
		private CanvasGroup _group;

		[SerializeField]
		private float _arcHeight = 1.6f;

		private Camera _camera;

		private void OnEnable()
		{
			if (_camera == null)
			{
				_camera = Camera.main;
			}
		}

		private void LateUpdate()
		{
			if (_camera != null)
			{
				base.transform.forward = _camera.transform.forward;
			}
		}

		public void Play(string value, Vector3 worldPosition, float rise, float duration, Pool<FloatingNumber> pool)
		{
			base.transform.position = worldPosition;
			_text.text = value;
			_group.alpha = 1f;
			StartCoroutine(PlayRoutine(worldPosition, rise, duration, pool));
		}

		private IEnumerator PlayRoutine(Vector3 worldPosition, float rise, float duration, Pool<FloatingNumber> pool)
		{
			Vector3 target = worldPosition + Vector3.up * rise;
			Coroutine moveCo = StartCoroutine(Tween.Move(base.transform, target, duration, Ease.OutQuad));
			Coroutine fadeCo = StartCoroutine(Tween.Fade(_group, 0f, duration));
			yield return moveCo;
			yield return fadeCo;
			pool.Release(this);
		}

		public void PlayFlying(string value, Vector3 from, Transform target, float duration, Pool<FloatingNumber> pool)
		{
			base.transform.position = from;
			_text.text = value;
			_group.alpha = 1f;
			StartCoroutine(FlyingRoutine(from, target, duration, pool));
		}

		private IEnumerator FlyingRoutine(Vector3 from, Transform target, float duration, Pool<FloatingNumber> pool)
		{
			float elapsed = 0f;
			duration = Mathf.Max(0.0001f, duration);
			Vector3 baseScale = base.transform.localScale;
			while (elapsed < duration && !(this == null) && !(target == null))
			{
				elapsed += Time.deltaTime;
				float i = Mathf.Clamp01(elapsed / duration);
				Vector3 pos = Vector3.LerpUnclamped(from, target.position, Ease.InQuad(i));
				pos.y += Mathf.Sin(i * 3.14159265f) * _arcHeight;
				base.transform.position = pos;
				float pop = ((i < 0.15f) ? Mathf.SmoothStep(1.4f, 1f, i / 0.15f) : 1f);
				base.transform.localScale = baseScale * pop;
				yield return null;
			}
			if (this != null)
			{
				base.transform.localScale = baseScale;
			}
			pool.Release(this);
		}
	}
}
