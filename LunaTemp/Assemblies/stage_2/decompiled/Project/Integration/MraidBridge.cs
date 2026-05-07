using System;
using System.Reflection;
using Project.Audio;
using Project.Core;
using UnityEngine;

namespace Project.Integration
{
	[DefaultExecutionOrder(-100)]
	public sealed class MraidBridge : MonoBehaviour
	{
		[SerializeField]
		private GameRoot _root;

		[SerializeField]
		private AudioService _audio;

		[SerializeField]
		private string _ctaUrl = "https://example.com";

		private SignalBus _signals;

		private bool _gameEndedReported;

		private static void Playable_Init()
		{
		}

		private static void Playable_OpenStore(string url)
		{
			Debug.Log("[playable mock] open " + url);
		}

		private static void Playable_LogEvent(string json)
		{
			Debug.Log("[playable] " + json);
		}

		private void Awake()
		{
			base.gameObject.name = "MraidBridge";
		}

		private void Start()
		{
			if (!(_root == null))
			{
				_signals = _root.Signals;
				_signals.Subscribe<CtaClickedSignal>(OnCta);
				_signals.Subscribe<BattleWonSignal>(OnBattleEnded);
				_signals.Subscribe<BattleLostSignal>(OnBattleEnded);
				Playable_Init();
				Playable_LogEvent("{\"event\":\"start\"}");
			}
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
			if (!TryLunaInstallFullGame())
			{
				Playable_OpenStore(_ctaUrl);
			}
		}

		private void OnBattleEnded<T>(T _)
		{
			if (!_gameEndedReported)
			{
				_gameEndedReported = true;
				TryLunaGameEnded();
			}
		}

		private static bool TryLunaInstallFullGame()
		{
			MethodInfo i = (Type.GetType("Luna.Unity.Playable, Unity.Luna") ?? Type.GetType("Luna.Unity.Playable, RuntimeScripts"))?.GetMethod("InstallFullGame", BindingFlags.Public | BindingFlags.Static);
			if (i == null)
			{
				return false;
			}
			try
			{
				i.Invoke(null, null);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static bool TryLunaGameEnded()
		{
			MethodInfo i = (Type.GetType("Luna.Unity.LifeCycle, Unity.Luna") ?? Type.GetType("Luna.Unity.LifeCycle, RuntimeScripts"))?.GetMethod("GameEnded", BindingFlags.Public | BindingFlags.Static);
			if (i == null)
			{
				return false;
			}
			try
			{
				i.Invoke(null, null);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void OnPause()
		{
			Time.timeScale = 0f;
			if (_audio != null)
			{
				_audio.SetPaused(true);
			}
		}

		public void OnResume()
		{
			Time.timeScale = 1f;
			if (_audio != null)
			{
				_audio.SetPaused(false);
			}
		}

		public void OnMute()
		{
			if (_audio != null)
			{
				_audio.SetMuted(true);
			}
		}

		public void OnUnmute()
		{
			if (_audio != null)
			{
				_audio.SetMuted(false);
			}
		}
	}
}
