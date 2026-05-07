using System.Collections.Generic;
using Project.Configs;
using Project.Domain;
using Project.Gameplay.Units;
using UnityEngine;

namespace Project.Gameplay
{
	public sealed class UnitSpawner : MonoBehaviour
	{
		public sealed class Result
		{
			public PlayerView Player;

			public Battle Battle;

			public Dictionary<UnitId, UnitView> Views;
		}

		[SerializeField]
		private UnitsBank _bank;

		[SerializeField]
		private Transform _root;

		[SerializeField]
		private Vector3 _spawnRotation = new Vector3(-15f, -180f, 0f);

		public Result Spawn(LevelConfig level, BalanceConfig balance, Camera camera)
		{
			Dictionary<UnitId, UnitView> views = new Dictionary<UnitId, UnitView>();
			Transform parent = ((_root != null) ? _root : base.transform);
			if (_bank == null || _bank.PlayerPrefab == null)
			{
				Debug.LogError("[UnitSpawner] UnitsBank или PlayerPrefab не заданы.");
				Result result = new Result();
				result.Views = views;
				result.Battle = new Battle(new Unit(UnitKind.Player, Power.Zero), new Unit[0]);
				return result;
			}
			Unit playerUnit = new Unit(UnitKind.Player, new Power(level.Player.Power));
			PlayerView playerView = Object.Instantiate(_bank.PlayerPrefab, parent);
			playerView.transform.position = ToWorld(level.Player.Position);
			FaceCamera(playerView);
			playerView.Configure(balance);
			playerView.Bind(playerUnit, camera);
			views[playerUnit.Id] = playerView;
			List<Unit> targets = new List<Unit>(level.Targets.Count);
			foreach (LevelConfig.TargetSpawn t in level.Targets)
			{
				Unit unit = new Unit(t.Kind, new Power(t.Power));
				UnitView view;
				switch (t.Kind)
				{
				case UnitKind.Enemy:
					view = InstantiateEnemy(t.PrefabKey, parent);
					break;
				case UnitKind.Chest:
					view = ((_bank.ChestPrefab != null) ? Object.Instantiate(_bank.ChestPrefab, parent) : null);
					break;
				default:
					view = null;
					break;
				}
				if (view == null)
				{
					Debug.LogWarning($"[UnitSpawner] Нет префаба для {t.Kind} key='{t.PrefabKey}'.");
					continue;
				}
				view.transform.position = ToWorld(t.Position);
				FaceCamera(view);
				if (view is EnemyView e)
				{
					e.Configure(balance);
				}
				view.Bind(unit, camera);
				views[unit.Id] = view;
				targets.Add(unit);
			}
			return new Result
			{
				Player = playerView,
				Battle = new Battle(playerUnit, targets),
				Views = views
			};
		}

		private EnemyView InstantiateEnemy(string key, Transform parent)
		{
			EnemyView prefab = _bank.GetEnemy(key);
			return (prefab != null) ? Object.Instantiate(prefab, parent) : null;
		}

		private Vector3 ToWorld(Vector2 p)
		{
			return new Vector3(p.x, 0f, p.y);
		}

		private void FaceCamera(UnitView view)
		{
			if (!(view == null))
			{
				view.transform.rotation = Quaternion.Euler(_spawnRotation);
			}
		}
	}
}
