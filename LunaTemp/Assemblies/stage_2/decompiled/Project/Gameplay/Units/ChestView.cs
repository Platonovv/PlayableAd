using System;
using System.Collections;
using Project.Core;
using Project.Domain;
using UnityEngine;

namespace Project.Gameplay.Units
{
	public sealed class ChestView : UnitView
	{
		[SerializeField]
		private Transform _flyingSword;

		[SerializeField]
		private float _idleBobAmplitude = 0f;

		[SerializeField]
		private float _idleBobSpeed = 2f;

		private Vector3 _basePosition;

		private bool _basePositionCached;

		public Transform FlyingSword => _flyingSword;

		protected override void Awake()
		{
			base.Awake();
			if (_flyingSword != null)
			{
				_flyingSword.gameObject.SetActive(false);
			}
		}

		public override void Bind(Unit unit, Camera camera)
		{
			base.Bind(unit, camera);
			_basePosition = base.transform.localPosition;
			_basePositionCached = true;
		}

		public IEnumerator PlayPreOpen()
		{
			Quaternion baseRot = base.transform.rotation;
			float elapsed = 0f;
			while (elapsed < 0.28f && this != null)
			{
				elapsed += Time.deltaTime;
				float i = elapsed / 0.28f;
				float jitter = Mathf.Sin(i * 3.14159265f * 12f) * 6f * (1f - i);
				base.transform.rotation = baseRot * Quaternion.Euler(0f, jitter, 0f);
				yield return null;
			}
			if (this != null)
			{
				base.transform.rotation = baseRot;
			}
		}

		public IEnumerator PlayOpen()
		{
			PlayAnim("Open");
			yield return Tween.Punch(base.transform, 0.2f, 0.45f);
			yield return new WaitForSeconds(0.2f);
			if (this != null)
			{
				base.gameObject.SetActive(false);
			}
		}

		private void Update()
		{
			if (_basePositionCached && base.Unit != null && base.Unit.IsAlive && !(_idleBobAmplitude <= 0f))
			{
				float bob = Mathf.Sin(Time.time * _idleBobSpeed) * _idleBobAmplitude;
				base.transform.localPosition = _basePosition + new Vector3(0f, bob, 0f);
			}
		}
	}
}
