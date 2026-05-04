using UnityEngine;

namespace Project.Configs
{
    /// <summary>
    /// Глобальные параметры баланса: тайминги, скорости, амплитуды.
    /// </summary>
    [CreateAssetMenu(menuName = "Playable/Balance Config", fileName = "BalanceConfig")]
    public sealed class BalanceConfig : ScriptableObject
    {
        [Header("Movement")]
        public float MoveSpeed = 6f;
        public float StopDistance = 1.2f;

        [Header("Combat")]
        public float AttackWindup = 0.15f;
        public float AttackImpactDelay = 0.25f;
        public float AttackRecover = 0.25f;
        public float FailedAttackBounce = 0.35f;
        public float UpgradeDuration = 0.6f;
        public float HitReactionDelay = 0.35f;
        public float DeathAnimDuration = 1.0f;
        public float DeathFadeDuration = 0.25f;

        [Header("Camera")]
        public float HitShakeAmplitude = 0.12f;
        public float HitShakeDuration = 0.18f;

        [Header("UI")]
        public float FloatingNumberRise = 1.2f;
        public float FloatingNumberDuration = 0.9f;
        public float EndCardDelay = 0.6f;
    }
}
