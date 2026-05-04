using System.Collections.Generic;
using Project.Configs;
using Project.Domain;
using Project.Gameplay.Units;
using UnityEngine;

namespace Project.Gameplay
{
	/// <summary>
	/// Спавнит юнитов из <see cref="LevelConfig"/>, префабы берёт из <see cref="UnitsBank"/>.
	/// </summary>
	public sealed class UnitSpawner : MonoBehaviour
	{
		[SerializeField] private UnitsBank _bank;
		[SerializeField] private Transform _root;
		[SerializeField] private Vector3 _spawnRotation = new(-15f, -180f, 0f);

		public sealed class Result
		{
			public PlayerView Player;
			public Battle Battle;
			public Dictionary<UnitId, UnitView> Views;
		}

		public Result Spawn(LevelConfig level, BalanceConfig balance, Camera camera)
		{
			var views = new Dictionary<UnitId, UnitView>();
			var parent = _root != null ? _root : transform;

			if (_bank == null || _bank.PlayerPrefab == null)
			{
				Debug.LogError("[UnitSpawner] UnitsBank или PlayerPrefab не заданы.");
				return new Result
				{
					Views = views,
					Battle = new Battle(new Unit(UnitKind.Player, Power.Zero), System.Array.Empty<Unit>())
				};
			}

			var playerUnit = new Unit(UnitKind.Player, new Power(level.Player.Power));
			var playerView = Instantiate(_bank.PlayerPrefab, parent);
			playerView.transform.position = ToWorld(level.Player.Position);
			FaceCamera(playerView);
			playerView.Configure(balance);
			playerView.Bind(playerUnit, camera);
			views[playerUnit.Id] = playerView;

			var targets = new List<Unit>(level.Targets.Count);
			foreach (var t in level.Targets)
			{
				var unit = new Unit(t.Kind, new Power(t.Power));
				UnitView view = t.Kind switch
				{
					UnitKind.Enemy => InstantiateEnemy(t.PrefabKey, parent),
					UnitKind.Chest => _bank.ChestPrefab != null ? Instantiate(_bank.ChestPrefab, parent) : null,
					_              => null
				};
				if (view == null)
				{
					Debug.LogWarning($"[UnitSpawner] Нет префаба для {t.Kind} key='{t.PrefabKey}'.");
					continue;
				}

				view.transform.position = ToWorld(t.Position);
				FaceCamera(view);
				if (view is EnemyView e)
					e.Configure(balance);
				view.Bind(unit, camera);
				views[unit.Id] = view;
				targets.Add(unit);
			}

			return new Result {Player = playerView, Battle = new Battle(playerUnit, targets), Views = views};
		}

		private EnemyView InstantiateEnemy(string key, Transform parent)
		{
			var prefab = _bank.GetEnemy(key);
			return prefab != null ? Instantiate(prefab, parent) : null;
		}

		private Vector3 ToWorld(Vector2 p) => new(p.x, 0f, p.y);

		private void FaceCamera(UnitView view)
		{
			if (view == null)
				return;

			view.transform.rotation = Quaternion.Euler(_spawnRotation);
		}
	}
}