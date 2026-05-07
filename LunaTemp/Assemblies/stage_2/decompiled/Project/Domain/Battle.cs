using System.Collections.Generic;
using System.Linq;
using Project.Domain.Actions;

namespace Project.Domain
{
	public sealed class Battle
	{
		private readonly Dictionary<UnitId, Unit> _units = new Dictionary<UnitId, Unit>();

		public Unit Player { get; }

		public IReadOnlyDictionary<UnitId, Unit> Units => _units;

		public BattleEvents Events { get; } = new BattleEvents();


		public bool IsOver { get; private set; }

		public IEnumerable<Unit> AliveEnemies => _units.Values.Where((Unit u) => u.IsAlive && u.Kind == UnitKind.Enemy);

		public IEnumerable<Unit> AliveChests => _units.Values.Where((Unit u) => u.IsAlive && u.Kind == UnitKind.Chest);

		public Battle(Unit player, IEnumerable<Unit> targets)
		{
			Player = player;
			_units[player.Id] = player;
			foreach (Unit u in targets)
			{
				_units[u.Id] = u;
			}
		}

		public bool TryGetUnit(UnitId id, out Unit unit)
		{
			return _units.TryGetValue(id, out unit);
		}

		public void Apply(IBattleAction action)
		{
			if (!IsOver && _units.TryGetValue(action.TargetId, out var target) && target.IsAlive)
			{
				if (action is AttackAction)
				{
					ResolveAttack(target);
				}
				else if (action is CollectChestAction)
				{
					ResolveChest(target);
				}
			}
		}

		private void ResolveAttack(Unit target)
		{
			if (Player.Power >= target.Power)
			{
				Player.AddPower(target.Power);
				target.Kill();
				Events.RaiseAttackResolved(Player, target);
				Events.RaisePlayerPowerChanged(Player);
				if (!AliveEnemies.Any())
				{
					IsOver = true;
					Events.RaiseBattleWon();
				}
			}
			else
			{
				Events.RaiseAttackFailed(Player, target);
			}
		}

		private void ResolveChest(Unit chest)
		{
			Player.AddPower(chest.Power);
			chest.Kill();
			Events.RaiseChestCollected(Player, chest);
			Events.RaisePlayerPowerChanged(Player);
		}
	}
}
