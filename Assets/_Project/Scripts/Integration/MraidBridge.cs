using System.Runtime.InteropServices;
using Project.Audio;
using Project.Core;
using UnityEngine;

namespace Project.Integration
{
    /// <summary>
    /// Мост MRAID/браузер: JS-вызовы на WebGL, mock в редакторе; pause/resume + click-through.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public sealed class MraidBridge : MonoBehaviour
    {
        [SerializeField] private GameRoot _root;
        [SerializeField] private AudioService _audio;
        [SerializeField] private string _ctaUrl = "https://example.com";

        private SignalBus _signals;

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void Playable_Init();
        [DllImport("__Internal")] private static extern void Playable_OpenStore(string url);
        [DllImport("__Internal")] private static extern void Playable_LogEvent(string json);
#else
        private static void Playable_Init() { }
        private static void Playable_OpenStore(string url) => Debug.Log($"[playable mock] open {url}");
        private static void Playable_LogEvent(string json) => Debug.Log("[playable] " + json);
#endif

        private void Awake() => gameObject.name = "MraidBridge";

        private void Start()
        {
            if (_root == null) return;
            _signals = _root.Signals;
            _signals.Subscribe<CtaClickedSignal>(OnCta);
            Playable_Init();
            Playable_LogEvent("{\"event\":\"start\"}");
        }

        private void OnDestroy()
        {
            if (_signals != null) _signals.Unsubscribe<CtaClickedSignal>(OnCta);
        }

        private void OnCta(CtaClickedSignal _)
        {
            Playable_LogEvent("{\"event\":\"cta_click\"}");
            Playable_OpenStore(_ctaUrl);
        }

        public void OnPause()
        {
            Time.timeScale = 0f;
            if (_audio != null) _audio.SetPaused(true);
        }

        public void OnResume()
        {
            Time.timeScale = 1f;
            if (_audio != null) _audio.SetPaused(false);
        }

        public void OnMute()   { if (_audio != null) _audio.SetMuted(true); }
        public void OnUnmute() { if (_audio != null) _audio.SetMuted(false); }
    }
}
