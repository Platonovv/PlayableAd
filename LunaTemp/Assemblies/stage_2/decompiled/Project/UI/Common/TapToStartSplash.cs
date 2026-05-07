using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Common
{
	public sealed class TapToStartSplash : MonoBehaviour
	{
		[SerializeField]
		private CanvasGroup _group;

		[SerializeField]
		private Button _tapButton;

		[SerializeField]
		private RectTransform _ctaPulse;

		[SerializeField]
		private float _ctaPulseAmplitude = 0.06f;

		[SerializeField]
		private float _ctaPulseSpeed = 4f;

		[SerializeField]
		private float _fadeDuration = 0.25f;

		public bool IsDismissed { get; private set; }

		public event Action Tapped;

		private void Awake()
		{
			if (_group != null)
			{
				_group.alpha = 1f;
				_group.blocksRaycasts = true;
			}
			if (_tapButton != null)
			{
				_tapButton.onClick.AddListener(OnTap);
			}
			AudioListener.volume = 0f;
			Time.timeScale = 0f;
		}

		private void Update()
		{
			if (!IsDismissed && !(_ctaPulse == null))
			{
				float pulse = 1f + Mathf.Sin(Time.unscaledTime * _ctaPulseSpeed) * _ctaPulseAmplitude;
				_ctaPulse.localScale = Vector3.one * pulse;
			}
		}

		private void OnTap()
		{
			if (!IsDismissed)
			{
				IsDismissed = true;
				AudioListener.volume = 1f;
				Time.timeScale = 1f;
				StartCoroutine(FadeOut());
				this.Tapped?.Invoke();
			}
		}

		private IEnumerator FadeOut()
		{
			if (_group != null)
			{
				_group.blocksRaycasts = false;
			}
			float elapsed = 0f;
			float startAlpha = ((_group != null) ? _group.alpha : 1f);
			while (elapsed < _fadeDuration)
			{
				elapsed += Time.unscaledDeltaTime;
				float i = Mathf.Clamp01(elapsed / _fadeDuration);
				if (_group != null)
				{
					_group.alpha = Mathf.Lerp(startAlpha, 0f, i);
				}
				yield return null;
			}
			if (_group != null)
			{
				_group.alpha = 0f;
			}
			base.gameObject.SetActive(false);
		}
	}
}
