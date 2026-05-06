using System;
using System.Collections;
using Project.Core;
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
        [SerializeField] private Text _title;

        public event Action CtaClicked;
        public event Action RetryClicked;

        private Coroutine _co;

        private void Awake()
        {
            _group.alpha = 0f;
            _group.blocksRaycasts = false;
            _ctaButton.onClick.AddListener(() => CtaClicked?.Invoke());
            if (_retryButton != null) _retryButton.onClick.AddListener(() => RetryClicked?.Invoke());
        }

        public void Show(string title, float delay, bool showCta)
        {
            Debug.Log($"[endcard] View.Show title={title} delay={delay} cta={showCta} group={(_group != null)} title_field={(_title != null)}");
            if (_title != null) _title.text = title;
            if (_ctaButton != null) _ctaButton.gameObject.SetActive(showCta);
            if (_retryButton != null) _retryButton.gameObject.SetActive(true);

            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(ShowRoutine(delay));
        }

        private IEnumerator ShowRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (_group != null)
            {
                _group.blocksRaycasts = true;
                _group.alpha = 1f;
            }
            Debug.Log("[endcard] View shown (alpha=1)");
            _co = null;
        }

        public void Hide()
        {
            if (_co != null) { StopCoroutine(_co); _co = null; }
            _group.alpha = 0f;
            _group.blocksRaycasts = false;
        }
    }
}
