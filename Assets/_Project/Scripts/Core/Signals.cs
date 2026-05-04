using Project.Domain;

namespace Project.Core
{
    /// <summary>
    /// Preview: палец зажат над целью; <c>HasTarget=false</c> означает выход из preview.
    /// </summary>
    public readonly struct TargetPreviewSignal
    {
        public readonly UnitId TargetId;
        public readonly bool HasTarget;
        public TargetPreviewSignal(UnitId id) { TargetId = id; HasTarget = true; }
        public static TargetPreviewSignal None => default;
    }

    /// <summary>
    /// Сигнал: пользователь подтвердил выбор цели (отпустил палец на ней).
    /// </summary>
    public readonly struct TargetSelectedSignal { public readonly UnitId TargetId; public TargetSelectedSignal(UnitId id) => TargetId = id; }

    /// <summary>
    /// Сигнал: атака удалась (цель уничтожена, сила игрока выросла).
    /// </summary>
    public readonly struct AttackResolvedSignal { public readonly UnitId AttackerId, TargetId; public AttackResolvedSignal(UnitId a, UnitId t) { AttackerId = a; TargetId = t; } }

    /// <summary>
    /// Сигнал: цель оказалась сильнее, игрок отскакивает без потери силы.
    /// </summary>
    public readonly struct AttackFailedSignal { public readonly UnitId AttackerId, TargetId; public AttackFailedSignal(UnitId a, UnitId t) { AttackerId = a; TargetId = t; } }

    /// <summary>
    /// Сигнал: открыт сундук, несёт прибавку силы для реакции VFX/UI.
    /// </summary>
    public readonly struct ChestCollectedSignal { public readonly UnitId ChestId; public readonly int PowerGain; public ChestCollectedSignal(UnitId id, int gain) { ChestId = id; PowerGain = gain; } }

    /// <summary>
    /// Сигнал: сила игрока изменилась (после атаки или сундука).
    /// </summary>
    public readonly struct PlayerPowerChangedSignal { public readonly int NewValue; public PlayerPowerChangedSignal(int value) => NewValue = value; }

    /// <summary>
    /// Сигнал: все враги уничтожены. EndCard ждёт именно его.
    /// </summary>
    public readonly struct BattleWonSignal { }

    /// <summary>
    /// Сигнал: явный fail-флоу (используется опционально для retry-механики).
    /// </summary>
    public readonly struct BattleLostSignal { }

    /// <summary>
    /// Сигнал: пользователь нажал CTA. MraidBridge переводит его в click-through.
    /// </summary>
    public readonly struct CtaClickedSignal { }
}
