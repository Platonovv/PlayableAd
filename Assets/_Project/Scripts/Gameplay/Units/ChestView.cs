using System.Collections;
using Project.Core;
using Project.Domain;
using UnityEngine;

namespace Project.Gameplay.Units
{
	/// <summary>
	/// View сундука: idle, анимация открытия с VFX, опциональный idle-бобинг (выключен по дефолту).
	/// </summary>
	public sealed class ChestView : UnitView
	{
		[SerializeField] private Transform _flyingSword;
		[SerializeField] private float _idleBobAmplitude = 0f;
		[SerializeField] private float _idleBobSpeed = 2f;

		private Vector3 _basePosition;
		private bool _basePositionCached;

		public Transform FlyingSword => _flyingSword;

		protected override void Awake()
		{
			base.Awake();
			if (_flyingSword != null)
				_flyingSword.gameObject.SetActive(false);
		}

		public override void Bind(Unit unit, Camera camera)
		{
			base.Bind(unit, camera);
			_basePosition = transform.localPosition;
			_basePositionCached = true;
		}

		public IEnumerator PlayPreOpen()
		{
			const float duration = 0.28f;
			const float jitterAmplitude = 6f;
			var baseRot = transform.rotation;
			var elapsed = 0f;
			while (elapsed < duration && this != null)
			{
				elapsed += Time.deltaTime;
				var k = elapsed / duration;
				var jitter = Mathf.Sin(k * Mathf.PI * 12f) * jitterAmplitude * (1f - k);
				transform.rotation = baseRot * Quaternion.Euler(0f, jitter, 0f);
				yield return null;
			}
			if (this != null) transform.rotation = baseRot;
		}

		public IEnumerator PlayOpen()
		{
			PlayAnim("Open");
			yield return Tween.Punch(transform, 0.2f, 0.45f);
			yield return new WaitForSeconds(0.2f);

			if (this != null)
				gameObject.SetActive(false);
		}

		private void Update()
		{
			if (!_basePositionCached)
				return;
			if (Unit == null || !Unit.IsAlive)
				return;
			if (_idleBobAmplitude <= 0f)
				return;

			var bob = Mathf.Sin(Time.time * _idleBobSpeed) * _idleBobAmplitude;
			transform.localPosition = _basePosition + new Vector3(0f, bob, 0f);
		}
	}
}