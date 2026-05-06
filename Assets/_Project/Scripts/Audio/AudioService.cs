using Project.Core;
using UnityEngine;

namespace Project.Audio
{
	/// <summary>
	/// Звук плейбла: лупит музыку и проигрывает SFX из <see cref="AudioBank"/> по сигналам геймплея.
	/// </summary>
	public sealed class AudioService : MonoBehaviour
	{
		[SerializeField] private GameRoot _root;
		[SerializeField] private AudioBank _bank;
		[SerializeField] private AudioSource _music;
		[SerializeField] private AudioSource[] _sfxPool;

		private SignalBus _signals;
		private bool _muted;
		private int _next;

		private void Start()
		{
			if (_root == null)
				return;

			_signals = _root.Signals;
			_signals.Subscribe<AttackResolvedSignal>(OnAttack);
			_signals.Subscribe<AttackFailedSignal>(OnFail);
			_signals.Subscribe<ChestCollectedSignal>(OnChest);
			_signals.Subscribe<BattleWonSignal>(OnWon);
			PlayMusic();
		}

		private void OnDestroy()
		{
			if (_signals == null)
				return;

			_signals.Unsubscribe<AttackResolvedSignal>(OnAttack);
			_signals.Unsubscribe<AttackFailedSignal>(OnFail);
			_signals.Unsubscribe<ChestCollectedSignal>(OnChest);
			_signals.Unsubscribe<BattleWonSignal>(OnWon);
		}

		public void SetMuted(bool muted)
		{
			_muted = muted;
			AudioListener.volume = muted ? 0f : 1f;
		}

		public void SetPaused(bool paused)
		{
			if (_music != null)
			{
				if (paused) _music.Pause();
				else        _music.UnPause();
			}
			if (_sfxPool != null)
			{
				for (var i = 0; i < _sfxPool.Length; i++)
				{
					if (_sfxPool[i] == null) continue;
					if (paused) _sfxPool[i].Pause();
					else        _sfxPool[i].UnPause();
				}
			}
		}

		public void Play(string key) => PlayKey(key);

		private void PlayKey(string key)
		{
			if (_muted || _bank == null || _sfxPool == null || _sfxPool.Length == 0)
				return;

			for (var i = 0; i < _bank.Sfxs.Length; i++)
			{
				if (_bank.Sfxs[i].Key != key)
					continue;

				var src = _sfxPool[_next];
				_next = (_next + 1) % _sfxPool.Length;
				src.PlayOneShot(_bank.Sfxs[i].Clip, _bank.Sfxs[i].Volume <= 0f ? 1f : _bank.Sfxs[i].Volume);
				return;
			}
		}

		private void PlayMusic()
		{
			if (_music == null || _bank == null || _bank.Music == null)
				return;

			_music.clip = _bank.Music;
			_music.volume = _bank.MusicVolume;
			_music.loop = true;
			_music.Play();
		}

		private void OnAttack(AttackResolvedSignal _) => Play("hit");
		private void OnFail(AttackFailedSignal _) => Play("fail");
		private void OnChest(ChestCollectedSignal _) => Play("chest");
		private void OnWon(BattleWonSignal _) => Play("win");
	}
}