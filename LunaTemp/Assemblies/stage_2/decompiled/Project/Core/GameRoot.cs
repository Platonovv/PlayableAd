using Project.Configs;
using Project.Gameplay;
using Project.Gameplay.Flow;
using UnityEngine;

namespace Project.Core
{
	[DefaultExecutionOrder(-1000)]
	public sealed class GameRoot : MonoBehaviour
	{
		[SerializeField]
		private LevelConfig _levelConfig;

		[SerializeField]
		private BalanceConfig _balanceConfig;

		[SerializeField]
		private UnitSpawner _spawner;

		[SerializeField]
		private BattleFlow _battleFlow;

		[SerializeField]
		private Camera _camera;

		public SignalBus Signals { get; private set; }

		public LevelConfig Level => _levelConfig;

		public BalanceConfig Balance => _balanceConfig;

		private void Awake()
		{
			Signals = new SignalBus();
			Application.targetFrameRate = 60;
			QualitySettings.vSyncCount = 0;
		}

		private void Start()
		{
			UnitSpawner.Result spawn = _spawner.Spawn(_levelConfig, _balanceConfig, _camera);
			_battleFlow.Init(_balanceConfig, Signals, spawn.Player, spawn.Battle, spawn.Views);
			Signals.Fire(default(PlayableStartedSignal));
		}
	}
}
