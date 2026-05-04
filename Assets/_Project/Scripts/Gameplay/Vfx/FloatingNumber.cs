using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Core;
using TMPro;
using UnityEngine;

namespace Project.Gameplay.Vfx
{
	/// <summary>
	/// Всплывающее число фидбэка: <see cref="Play"/> поднимается на месте, <see cref="PlayFlying"/> летит дугой к цели.
	/// </summary>
	public sealed class FloatingNumber : MonoBehaviour
	{
		[SerializeField] private TMP_Text _text;
		[SerializeField] private CanvasGroup _group;
		[SerializeField] private float _arcHeight = 1.6f;

		private Camera _camera;

		private void OnEnable()
		{
			if (_camera == null)
				_camera = Camera.main;
		}

		private void LateUpdate()
		{
			if (_camera != null)
				transform.forward = _camera.transform.forward;
		}

		public async UniTask Play(string value,
		                          Vector3 worldPosition,
		                          float rise,
		                          float duration,
		                          Pool<FloatingNumber> pool,
		                          CancellationToken ct)
		{
			transform.position = worldPosition;
			_text.text = value;
			_group.alpha = 1f;

			var target = worldPosition + Vector3.up * rise;
			var move = Tween.Move(transform, target, duration, Ease.OutQuad, ct);
			var fade = Tween.Fade(_group, 0f, duration, ct);
			await UniTask.WhenAll(move, fade);

			pool.Release(this);
		}

		public async UniTask PlayFlying(string value,
		                                Vector3 from,
		                                Transform target,
		                                float duration,
		                                Pool<FloatingNumber> pool,
		                                CancellationToken ct)
		{
			transform.position = from;
			_text.text = value;
			_group.alpha = 1f;

			var elapsed = 0f;
			duration = Mathf.Max(0.0001f, duration);
			while (elapsed < duration)
			{
				if (this == null || target == null)
					break;
				if (ct.IsCancellationRequested)
					break;

				elapsed += Time.deltaTime;
				var k = Mathf.Clamp01(elapsed / duration);
				var pos = Vector3.LerpUnclamped(from, target.position, Ease.InQuad(k));
				pos.y += Mathf.Sin(k * Mathf.PI) * _arcHeight; // дуга вверх
				transform.position = pos;
				await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
			}

			// Долетели чётко, без подкрашивания/scale — дальше «эффект приёма» делает PowerLabel получателя.
			pool.Release(this);
		}
	}
}