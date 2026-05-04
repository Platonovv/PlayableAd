using System.Collections.Generic;
using Project.Configs;
using Project.Core;
using Project.Domain;
using Project.Gameplay.CameraFx;
using Project.Gameplay.Targeting;
using Project.Gameplay.Units;
using Project.Gameplay.Vfx;
using UnityEngine;

namespace Project.Gameplay.Flow
{
    /// <summary>
    /// Контекст, который машина состояний боя получает на вход (модель, view, сигналы, сервисы).
    /// </summary>
    public sealed class BattleFlowContext
    {
        public Battle Battle;
        public PlayerView Player;
        public Dictionary<UnitId, UnitView> Views;
        public TargetIndicator Indicator;
        public TapInput Input;
        public VfxService Vfx;
        public ScreenShake Shake;
        public Pool<FloatingNumber> Numbers;
        public BalanceConfig Balance;
        public SignalBus Signals;
        public UnitId PendingTarget;
        public Color EnemyArrowColor;
        public Color WinnableArrowColor;
        public Color ChestArrowColor;

        public Color ColorFor(UnitView view)
        {
            if (view == null) return Color.white;
            switch (view.Kind)
            {
                case UnitKind.Chest: return ChestArrowColor;
                case UnitKind.Enemy:
                    return Battle.Player.Power >= view.Unit.Power ? WinnableArrowColor : EnemyArrowColor;
                default: return Color.white;
            }
        }
    }
}
