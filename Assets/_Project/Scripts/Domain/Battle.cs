using System.Collections.Generic;
using System.Linq;
using Project.Domain.Actions;

namespace Project.Domain
{
    /// <summary>
    /// Чисто-C# модель боя.
    /// Хранит игрока и цели, применяет <see cref="IBattleAction"/> и транслирует изменения через <see cref="Events"/>.
    /// </summary>
    public sealed class Battle
    {
        private readonly Dictionary<UnitId, Unit> _units = new();

        public Unit Player { get; }
        public IReadOnlyDictionary<UnitId, Unit> Units => _units;
        public BattleEvents Events { get; } = new();
        public bool IsOver { get; private set; }

        public Battle(Unit player, IEnumerable<Unit> targets)
        {
            Player = player;
            _units[player.Id] = player;
            foreach (var u in targets) _units[u.Id] = u;
        }

        public bool TryGetUnit(UnitId id, out Unit unit) => _units.TryGetValue(id, out unit);

        public IEnumerable<Unit> AliveEnemies => _units.Values.Where(u => u.IsAlive && u.Kind == UnitKind.Enemy);
        public IEnumerable<Unit> AliveChests => _units.Values.Where(u => u.IsAlive && u.Kind == UnitKind.Chest);

        public void Apply(IBattleAction action)
        {
            if (IsOver) return;
            if (!_units.TryGetValue(action.TargetId, out var target) || !target.IsAlive) return;

            switch (action)
            {
                case AttackAction:    ResolveAttack(target);  break;
                case CollectChestAction: ResolveChest(target); break;
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
