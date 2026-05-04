using System;

namespace Project.Domain
{
    /// <summary>
    /// Канал доменных событий боя. View-слой подписывается, не трогая модель.
    /// </summary>
    public sealed class BattleEvents
    {
        public event Action<Unit, Unit> AttackResolved;
        public event Action<Unit, Unit> AttackFailed;
        public event Action<Unit, Unit> ChestCollected;
        public event Action<Unit> PlayerPowerChanged;
        public event Action BattleWon;
        public event Action BattleLost;

        internal void RaiseAttackResolved(Unit attacker, Unit target) => AttackResolved?.Invoke(attacker, target);
        internal void RaiseAttackFailed(Unit attacker, Unit target) => AttackFailed?.Invoke(attacker, target);
        internal void RaiseChestCollected(Unit player, Unit chest) => ChestCollected?.Invoke(player, chest);
        internal void RaisePlayerPowerChanged(Unit player) => PlayerPowerChanged?.Invoke(player);
        internal void RaiseBattleWon() => BattleWon?.Invoke();
        internal void RaiseBattleLost() => BattleLost?.Invoke();
    }
}
