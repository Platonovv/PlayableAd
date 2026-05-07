using System.Collections;
using Project.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Common
{
	/// <summary>
	/// Подсказка-нудж для первого тапа: если игрок не выбрал цель за <see cref="_idleSeconds"/>,
	/// плавно показывает текст и пульсирующую стрелку. Скрывается при первом
	/// <see cref="TargetSelectedSignal"/> и больше не возвращается.
	/// </summary>
	public sealed class TutorialNudge : MonoBehaviour
	{
		[SerializeField] private GameRoot _root;
		[SerializeField] private CanvasGroup _group;
		[SerializeField] private RectTransform _arrow;
		[SerializeField] private float _idleSeconds = 2.5f;
		[SerializeField] private float _fadeDuration = 0.3f;
		[SerializeField] private float _arrowBobAmplitude = 18f;
		[SerializeField] private float _arrowBobSpeed = 4f;

		private SignalBus _signals;
		private Coroutine _showCo;
		private bool _interacted;
		private Vector2 _arrowBaseAnchor;

		private void Awake()
		{
			if (_group != null)
			{
				_group.alpha = 0f;
				_group.blocksRaycasts = false;
				_group.interactable = false;
			}

			if (_arrow != null)
				_arrowBaseAnchor = _arrow.anchoredPosition;
		}

		private void Start()
		{
			if (_root == null)
				return;

			_signals = _root.Signals;
			_signals.Subscribe<TargetSelectedSignal>(OnTargetSelected);
			_signals.Subscribe<TargetPreviewSignal>(OnTargetPreview);
			_showCo = StartCoroutine(WaitAndShow());
		}

		private void OnDestroy()
		{
			if (_signals != null)
			{
				_signals.Unsubscribe<TargetSelectedSignal>(OnTargetSelected);
				_signals.Unsubscribe<TargetPreviewSignal>(OnTargetPreview);
			}
		}

		private void Update()
		{
			if (_arrow == null || _group == null || _group.alpha <= 0f)
				return;

			var bob = Mathf.Sin(Time.unscaledTime * _arrowBobSpeed) * _arrowBobAmplitude;
			_arrow.anchoredPosition = _arrowBaseAnchor + new Vector2(0f, bob);
		}

		private void OnTargetSelected(TargetSelectedSignal _) => MarkInteracted();

		private void OnTargetPreview(TargetPreviewSignal s)
		{
			if (s.HasTarget)
				MarkInteracted();
		}

		private void MarkInteracted()
		{
			if (_interacted)
				return;

			_interacted = true;
			if (_showCo != null)
			{
				StopCoroutine(_showCo);
				_showCo = null;
			}

			StartCoroutine(FadeTo(0f));
		}

		private IEnumerator WaitAndShow()
		{
			var elapsed = 0f;
			while (elapsed < _idleSeconds)
			{
				elapsed += Time.unscaledDeltaTime;
				yield return null;

				if (_interacted)
					yield break;
			}

			yield return FadeTo(1f);
		}

		private IEnumerator FadeTo(float target)
		{
			if (_group == null)
				yield break;

			var start = _group.alpha;
			var elapsed = 0f;
			while (elapsed < _fadeDuration)
			{
				elapsed += Time.unscaledDeltaTime;
				var k = Mathf.Clamp01(elapsed / _fadeDuration);
				_group.alpha = Mathf.Lerp(start, target, k);
				yield return null;
			}

			_group.alpha = target;
			if (target <= 0.01f && _interacted)
				gameObject.SetActive(false);
		}
	}
}