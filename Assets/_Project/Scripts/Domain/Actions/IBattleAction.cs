namespace Project.Domain.Actions
{
    /// <summary>
    /// Команда, принимаемая <see cref="Battle"/>: одно игровое намерение к одной цели.
    /// </summary>
    public interface IBattleAction
    {
        UnitId TargetId { get; }
    }

    /// <summary>
    /// Атака врага. Исход решается сравнением силы внутри <see cref="Battle"/>.
    /// </summary>
    public sealed class AttackAction : IBattleAction
    {
        public UnitId TargetId { get; }
        public AttackAction(UnitId targetId) => TargetId = targetId;
    }

    /// <summary>
    /// Забор сундука: игрок получает его силу, сундук убирается с поля.
    /// </summary>
    public sealed class CollectChestAction : IBattleAction
    {
        public UnitId TargetId { get; }
        public CollectChestAction(UnitId targetId) => TargetId = targetId;
    }
}
