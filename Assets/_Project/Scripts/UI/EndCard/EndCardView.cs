using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.EndCard
{
    /// <summary>
    /// View финального экрана: заголовок, CTA, Retry; нажатия уходят через C#-события.
    /// </summary>
    public sealed class EndCardView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _ctaButton;
        [SerializeField] private Button _retryButton;
        [SerializeField] private TMP_Text _title;

        public event Action CtaClicked;
        public event Action RetryClicked;

        private void Awake()
        {
            _group.alpha = 0f;
            _group.blocksRaycasts = false;
            _ctaButton.onClick.AddListener(() => CtaClicked?.Invoke());
            if (_retryButton != null) _retryButton.onClick.AddListener(() => RetryClicked?.Invoke());
        }

        public async UniTask Show(string title, float delay, bool showCta, CancellationToken ct)
        {
            _title.text = title;
            if (_ctaButton != null) _ctaButton.gameObject.SetActive(showCta);
            if (_retryButton != null) _retryButton.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: ct);
            _group.blocksRaycasts = true;
            await Tween.Fade(_group, 1f, 0.35f, ct);
        }

        public void Hide()
        {
            _group.alpha = 0f;
            _group.blocksRaycasts = false;
        }
    }
}
