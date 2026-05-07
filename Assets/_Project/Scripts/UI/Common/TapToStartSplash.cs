using System.Collections;
using Project.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Common
{
    /// <summary>
    /// Стартовый сплеш «Tap to Play»: блокирует игру до первого тапа, разблокирует AudioListener
    /// (для Web Audio Context на WebGL), фейдится в 0 alpha и отдаёт фокус игре.
    /// </summary>
    public sealed class TapToStartSplash : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _tapButton;
        [SerializeField] private RectTransform _ctaPulse;
        [SerializeField] private float _ctaPulseAmplitude = 0.06f;
        [SerializeField] private float _ctaPulseSpeed = 4f;
        [SerializeField] private float _fadeDuration = 0.25f;

        public bool IsDismissed { get; private set; }
        public event System.Action Tapped;

        private void Awake()
        {
            if (_group != null)
            {
                _group.alpha = 1f;
                _group.blocksRaycasts = true;
            }
            if (_tapButton != null)
                _tapButton.onClick.AddListener(OnTap);

            AudioListener.volume = 0f;
            Time.timeScale = 0f;
        }

        private void Update()
        {
            if (IsDismissed || _ctaPulse == null) return;
            var pulse = 1f + Mathf.Sin(Time.unscaledTime * _ctaPulseSpeed) * _ctaPulseAmplitude;
            _ctaPulse.localScale = Vector3.one * pulse;
        }

        private void OnTap()
        {
            if (IsDismissed) return;
            IsDismissed = true;

            AudioListener.volume = 1f;
            Time.timeScale = 1f;
            StartCoroutine(FadeOut());
            Tapped?.Invoke();
        }

        private IEnumerator FadeOut()
        {
            if (_group != null) _group.blocksRaycasts = false;

            var elapsed = 0f;
            var startAlpha = _group != null ? _group.alpha : 1f;
            while (elapsed < _fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                var k = Mathf.Clamp01(elapsed / _fadeDuration);
                if (_group != null) _group.alpha = Mathf.Lerp(startAlpha, 0f, k);
                yield return null;
            }
            if (_group != null) _group.alpha = 0f;
            gameObject.SetActive(false);
        }
    }
}
