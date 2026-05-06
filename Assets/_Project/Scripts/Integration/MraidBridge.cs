using System.Runtime.InteropServices;
using Project.Audio;
using Project.Core;
using UnityEngine;

namespace Project.Integration
{
    /// <summary>
    /// Мост MRAID/браузер: JS-вызовы на WebGL, mock в редакторе; pause/resume + click-through.
    /// CTA-клик и конец игры идут через Luna SDK API (требование рекламных сетей).
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public sealed class MraidBridge : MonoBehaviour
    {
        [SerializeField] private GameRoot _root;
        [SerializeField] private AudioService _audio;
        [SerializeField] private string _ctaUrl = "https://example.com";

        private SignalBus _signals;
        private bool _gameEndedReported;

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
            _signals.Subscribe<BattleWonSignal>(OnBattleEnded);
            _signals.Subscribe<BattleLostSignal>(OnBattleEnded);
            Playable_Init();
            Playable_LogEvent("{\"event\":\"start\"}");
        }

        private void OnDestroy()
        {
            if (_signals != null)
            {
                _signals.Unsubscribe<CtaClickedSignal>(OnCta);
                _signals.Unsubscribe<BattleWonSignal>(OnBattleEnded);
                _signals.Unsubscribe<BattleLostSignal>(OnBattleEnded);
            }
        }

        private void OnCta(CtaClickedSignal _)
        {
            Playable_LogEvent("{\"event\":\"cta_click\"}");
            // Luna API: открытие стора через SDK — обязательное требование рекламных сетей.
            // В редакторе fallback на свой OpenStore (mock).
            if (!TryLunaInstallFullGame())
                Playable_OpenStore(_ctaUrl);
        }

        private void OnBattleEnded<T>(T _)
        {
            if (_gameEndedReported) return;
            _gameEndedReported = true;
            // Luna API: уведомление о конце плейбла. Без него сети не показывают свой нативный end-card.
            TryLunaGameEnded();
        }

        private static bool TryLunaInstallFullGame()
        {
            var t = System.Type.GetType("Luna.Unity.Playable, Unity.Luna")
                 ?? System.Type.GetType("Luna.Unity.Playable, RuntimeScripts");
            var m = t?.GetMethod("InstallFullGame",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (m == null) return false;
            try { m.Invoke(null, null); return true; }
            catch (System.Exception e) { Debug.LogWarning("Luna InstallFullGame failed: " + e.Message); return false; }
        }

        private static bool TryLunaGameEnded()
        {
            var t = System.Type.GetType("Luna.Unity.LifeCycle, Unity.Luna")
                 ?? System.Type.GetType("Luna.Unity.LifeCycle, RuntimeScripts");
            var m = t?.GetMethod("GameEnded",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (m == null) return false;
            try { m.Invoke(null, null); return true; }
            catch (System.Exception e) { Debug.LogWarning("Luna GameEnded failed: " + e.Message); return false; }
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
