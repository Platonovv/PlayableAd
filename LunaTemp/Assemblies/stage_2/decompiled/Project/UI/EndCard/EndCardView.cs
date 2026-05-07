using System;
using System.Collections;
using Project.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.EndCard
{
	public sealed class EndCardView : MonoBehaviour
	{
		[SerializeField]
		private CanvasGroup _group;

		[SerializeField]
		private Button _ctaButton;

		[SerializeField]
		private Button _retryButton;

		[SerializeField]
		private Button _skipButton;

		[SerializeField]
		private Text _title;

		[SerializeField]
		private Text _subtitle;

		[SerializeField]
		private Image[] _stars;

		[SerializeField]
		private Sprite _starFilledSprite;

		[SerializeField]
		private Sprite _starOutlineSprite;

		[SerializeField]
		private Color _starFilledColor = new Color(1f, 0.85f, 0.2f);

		[SerializeField]
		private Color _starEmptyColor = new Color(1f, 1f, 1f, 0.45f);

		[SerializeField]
		private float _ctaPulseAmplitude = 0.06f;

		[SerializeField]
		private float _ctaPulseSpeed = 4f;

		private Coroutine _co;

		private Coroutine _pulseCo;

		private Vector3 _ctaBaseScale = Vector3.one;

		public event Action CtaClicked;

		public event Action RetryClicked;

		private void Awake()
		{
			_group.alpha = 0f;
			_group.blocksRaycasts = false;
			_ctaButton.onClick.AddListener(delegate
			{
				this.CtaClicked?.Invoke();
			});
			if (_retryButton != null)
			{
				_retryButton.onClick.AddListener(delegate
				{
					this.RetryClicked?.Invoke();
				});
			}
			if (_skipButton != null)
			{
				_skipButton.onClick.AddListener(delegate
				{
					this.CtaClicked?.Invoke();
				});
			}
			if (_ctaButton != null)
			{
				_ctaBaseScale = _ctaButton.transform.localScale;
			}
		}

		public void Show(string title, string subtitle, float delay, bool showCta, int starCount = 0)
		{
			if (_title != null)
			{
				_title.text = title;
			}
			if (_subtitle != null)
			{
				_subtitle.text = subtitle ?? string.Empty;
				_subtitle.gameObject.SetActive(!string.IsNullOrEmpty(subtitle));
			}
			if (_ctaButton != null)
			{
				_ctaButton.gameObject.SetActive(showCta);
			}
			if (_retryButton != null)
			{
				_retryButton.gameObject.SetActive(true);
			}
			if (_skipButton != null)
			{
				_skipButton.gameObject.SetActive(!showCta);
			}
			ApplyStars(starCount);
			if (_co != null)
			{
				StopCoroutine(_co);
			}
			_co = StartCoroutine(ShowRoutine(delay, showCta, starCount));
		}

		private void ApplyStars(int filledCount)
		{
			if (_stars == null)
			{
				return;
			}
			for (int i = 0; i < _stars.Length; i++)
			{
				Image s = _stars[i];
				if (!(s == null))
				{
					bool filled = i < filledCount;
					s.sprite = (filled ? _starFilledSprite : _starOutlineSprite);
					s.color = (filled ? _starFilledColor : _starEmptyColor);
					s.transform.localScale = Vector3.one;
				}
			}
		}

		private IEnumerator ShowRoutine(float delay, bool showCta, int starCount)
		{
			if (_stars != null)
			{
				for (int i = 0; i < _stars.Length; i++)
				{
					if (_stars[i] != null)
					{
						_stars[i].transform.localScale = Vector3.zero;
					}
				}
			}
			yield return new WaitForSecondsRealtime(delay);
			RectTransform titleRect = ((_title != null) ? _title.rectTransform : null);
			Vector2 titleAnchored = ((titleRect != null) ? titleRect.anchoredPosition : Vector2.zero);
			if (titleRect != null)
			{
				titleRect.anchoredPosition = titleAnchored + new Vector2(0f, 220f);
			}
			if (_group != null)
			{
				_group.blocksRaycasts = true;
				_group.alpha = 0f;
			}
			float elapsed2 = 0f;
			while (elapsed2 < 0.25f)
			{
				elapsed2 += Time.unscaledDeltaTime;
				float j = Mathf.Clamp01(elapsed2 / 0.25f);
				if (_group != null)
				{
					_group.alpha = j;
				}
				if (titleRect != null)
				{
					titleRect.anchoredPosition = Vector2.Lerp(titleAnchored + new Vector2(0f, 220f), titleAnchored, Ease.OutQuad(j));
				}
				yield return null;
			}
			if (_group != null)
			{
				_group.alpha = 1f;
			}
			if (titleRect != null)
			{
				titleRect.anchoredPosition = titleAnchored;
			}
			if (titleRect != null)
			{
				Vector3 baseScale = titleRect.localScale;
				elapsed2 = 0f;
				while (elapsed2 < 0.35f)
				{
					elapsed2 += Time.unscaledDeltaTime;
					float k = Mathf.Clamp01(elapsed2 / 0.35f);
					float bump = Mathf.Sin(k * 3.14159265f) * 0.17999995f;
					titleRect.localScale = baseScale * (1f + bump);
					yield return null;
				}
				titleRect.localScale = baseScale;
			}
			yield return PopStarsRoutine(starCount);
			if (showCta && _ctaButton != null)
			{
				if (_pulseCo != null)
				{
					StopCoroutine(_pulseCo);
				}
				_pulseCo = StartCoroutine(CtaPulseLoop());
			}
			_co = null;
		}

		private IEnumerator PopStarsRoutine(int starCount)
		{
			if (_stars == null)
			{
				yield break;
			}
			for (int i = 0; i < _stars.Length; i++)
			{
				Image star = _stars[i];
				if (!(star == null))
				{
					Transform rect = star.transform;
					rect.localScale = Vector3.zero;
					float elapsed = 0f;
					while (elapsed < 0.25f)
					{
						elapsed += Time.unscaledDeltaTime;
						float j = Mathf.Clamp01(elapsed / 0.25f);
						float s = Mathf.Lerp(0f, 1.4f, Ease.OutBack(j));
						rect.localScale = Vector3.one * s;
						yield return null;
					}
					rect.localScale = Vector3.one;
					yield return new WaitForSecondsRealtime(0.12f);
				}
			}
		}

		private IEnumerator CtaPulseLoop()
		{
			Transform rect = _ctaButton.transform;
			while (rect != null && _ctaButton.gameObject.activeInHierarchy)
			{
				float pulse = 1f + Mathf.Sin(Time.unscaledTime * _ctaPulseSpeed) * _ctaPulseAmplitude;
				rect.localScale = _ctaBaseScale * pulse;
				yield return null;
			}
			if (rect != null)
			{
				rect.localScale = _ctaBaseScale;
			}
			_pulseCo = null;
		}

		public void Hide()
		{
			if (_co != null)
			{
				StopCoroutine(_co);
				_co = null;
			}
			if (_pulseCo != null)
			{
				StopCoroutine(_pulseCo);
				_pulseCo = null;
			}
			if (_ctaButton != null)
			{
				_ctaButton.transform.localScale = _ctaBaseScale;
			}
			_group.alpha = 0f;
			_group.blocksRaycasts = false;
		}
	}
}
