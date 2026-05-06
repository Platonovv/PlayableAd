using System.Collections;
using Project.Configs;
using Project.Core;
using UnityEngine;

namespace Project.Gameplay.Units
{
	/// <summary>
	/// View игрока: анимации, движение к цели и апгрейд (получение меча из сундука).
	/// </summary>
	public sealed class PlayerView : UnitView
	{
		private const string IdleHash = "Idle";
		private const string RunHash = "Run";
		private const string AttackHash = "Attack";
		private const string HitHash = "Hit";
		private const string DeathHash = "Death";
		private const string VictoryHash = "Victory";
		private const string UpgradeHash = "Upgrade";
		private const string SuperAttackHash = "SuperAttack";

		[SerializeField] private Animator _animator;
		[SerializeField] private Transform _sword;

		private BalanceConfig _balance;

		private Transform _swordOriginalParent;
		private Vector3 _swordOriginalLocalPos;
		private Quaternion _swordOriginalLocalRot;
		private Vector3 _swordOriginalLocalScale;

		private Vector3 _flairBaseScale = Vector3.one;
		private float _flairBaseY;
		private Coroutine _flairCo;

		public Transform Sword => _sword;

		public Vector3 SwordTarget =>
			_swordOriginalParent != null
				? _swordOriginalParent.TransformPoint(_swordOriginalLocalPos)
				: _sword != null ? _sword.position : Anchor.position;

		public void Configure(BalanceConfig balance) => _balance = balance;

		protected override void Awake()
		{
			base.Awake();
			if (_sword != null)
			{
				_swordOriginalParent = _sword.parent;
				_swordOriginalLocalPos = _sword.localPosition;
				_swordOriginalLocalRot = _sword.localRotation;
				_swordOriginalLocalScale = _sword.localScale.sqrMagnitude > 0.0001f ? _sword.localScale : Vector3.one;
				_sword.localScale = Vector3.zero;
			}
			if (_animator != null)
			{
				_animator.applyRootMotion = false;
				_animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			}

			_flairBaseScale = transform.localScale;
		}

		public override void Bind(Project.Domain.Unit unit, Camera camera)
		{
			base.Bind(unit, camera);
			_flairBaseY = transform.position.y;
		}

		public void DetachSwordTo(Vector3 worldPos)
		{
			if (_sword == null) return;
			_sword.SetParent(null, false);
			_sword.position = worldPos;
			_sword.localScale = _swordOriginalLocalScale;
		}

		public void ReattachSword()
		{
			if (_sword == null || _swordOriginalParent == null) return;
			_sword.SetParent(_swordOriginalParent, false);
			_sword.localPosition = _swordOriginalLocalPos;
			_sword.localRotation = _swordOriginalLocalRot;
			_sword.localScale = _swordOriginalLocalScale;
		}

		public IEnumerator MoveTo(Vector3 destination)
		{
			destination.y = transform.position.y;

			PlayState(RunHash);
			FaceTowards(destination);
			var stopSqr = _balance.StopDistance * _balance.StopDistance;
			while (this != null && (transform.position - destination).sqrMagnitude > stopSqr)
			{
				var step = _balance.MoveSpeed * Time.deltaTime;
				transform.position = Vector3.MoveTowards(transform.position, destination, step);
				FaceTowards(destination);
				yield return null;
			}

			PlayState(IdleHash);
		}

		public IEnumerator PlayAttack()
		{
			PlayState(AttackHash);
			yield return new WaitForSeconds(_balance.AttackWindup + _balance.AttackImpactDelay);
		}

		public IEnumerator PlaySuperAttack()
		{
			PlayState(SuperAttackHash);
			yield return new WaitForSeconds(_balance.AttackWindup + _balance.AttackImpactDelay);
		}

		public IEnumerator PlayRecover()
		{
			yield return new WaitForSeconds(_balance.AttackRecover);
			PlayState(IdleHash);
		}

		public IEnumerator PlayBounceBack(Vector3 origin)
		{
			PlayState(HitHash);
			yield return Tween.Move(transform, origin, _balance.FailedAttackBounce, Ease.OutBack);
			PlayState(IdleHash);
		}

		public IEnumerator PlayUpgrade()
		{
			PlayState(UpgradeHash);
			if (_sword != null)
				_sword.localScale = Vector3.one;

			StartFlair(_balance.UpgradeDuration);
			yield return new WaitForSeconds(_balance.UpgradeDuration);
			yield return new WaitForSeconds(_balance.UpgradeAnimTail);
		}

		public void PlayPowerGain() => StartFlair(_balance.UpgradeDuration * 0.6f);

		public void PlayIdle() => PlayState(IdleHash);

		public void PlayVictory() => PlayState(VictoryHash);

		public void PlayHurt() => PlayState(HitHash);

		public IEnumerator PlayDeath()
		{
			PlayState(DeathHash);
			yield return new WaitForSeconds(_balance.DeathAnimDuration);
			yield return Tween.Scale(transform, Vector3.zero, _balance.DeathFadeDuration, Ease.InQuad);
		}

		public override void FaceTowards(Vector3 worldTarget)
		{
			var localUp = transform.up;
			var dir = Vector3.ProjectOnPlane(worldTarget - transform.position, localUp);
			if (dir.sqrMagnitude < 0.0001f)
				return;

			transform.rotation = Quaternion.Slerp(transform.rotation,
			                                      Quaternion.LookRotation(dir, localUp),
			                                      18f * Time.deltaTime);
		}

		private void StartFlair(float duration)
		{
			if (_flairCo != null) StopCoroutine(_flairCo);
			_flairCo = StartCoroutine(RunFlair(duration));
		}

		private IEnumerator RunFlair(float duration)
		{
			IEnumerator flair;
			switch (Random.Range(0, 5))
			{
				case 0: flair = PunchFlair(duration); break;
				case 1: flair = HopUp(duration); break;
				case 2: flair = Squash(duration); break;
				case 3: flair = DoubleHop(duration); break;
				default: flair = SpeedBurst(duration); break;
			}
			yield return flair;

			if (this != null)
			{
				transform.localScale = _flairBaseScale;
				var p = transform.position;
				p.y = _flairBaseY;
				transform.position = p;
			}
			_flairCo = null;
		}

		private IEnumerator PunchFlair(float duration)
		{
			const float amplitude = 0.30f;
			duration = Mathf.Max(0.0001f, duration);
			var elapsed = 0f;
			while (elapsed < duration && this != null)
			{
				elapsed += Time.deltaTime;
				var k = elapsed / duration;
				var bump = Mathf.Sin(k * Mathf.PI) * amplitude;
				transform.localScale = _flairBaseScale * (1f + bump);
				yield return null;
			}
		}

		private IEnumerator SpeedBurst(float duration)
		{
			if (_animator == null) yield break;
			var prev = _animator.speed;
			_animator.speed = 1.6f;

			var elapsed = 0f;
			while (elapsed < duration && this != null)
			{
				elapsed += Time.deltaTime;
				yield return null;
			}

			if (this != null && _animator != null)
				_animator.speed = prev;
		}

		private IEnumerator HopUp(float duration)
		{
			const float hopHeight = 0.7f;
			var elapsed = 0f;
			while (elapsed < duration && this != null)
			{
				elapsed += Time.deltaTime;
				var k = Mathf.Clamp01(elapsed / duration);
				var y = _flairBaseY + Mathf.Sin(k * Mathf.PI) * hopHeight;
				var p = transform.position;
				p.y = y;
				transform.position = p;
				yield return null;
			}
		}

		private IEnumerator Squash(float duration)
		{
			var elapsed = 0f;
			while (elapsed < duration && this != null)
			{
				elapsed += Time.deltaTime;
				var k = Mathf.Clamp01(elapsed / duration);
				var s = Mathf.Sin(k * Mathf.PI * 2f);
				transform.localScale = new Vector3(_flairBaseScale.x * (1f + s * 0.18f),
				                                   _flairBaseScale.y * (1f - s * 0.18f),
				                                   _flairBaseScale.z * (1f + s * 0.18f));
				yield return null;
			}
		}

		private IEnumerator DoubleHop(float duration)
		{
			const float hopHeight = 0.4f;
			var elapsed = 0f;
			while (elapsed < duration && this != null)
			{
				elapsed += Time.deltaTime;
				var k = Mathf.Clamp01(elapsed / duration);
				var y = _flairBaseY + Mathf.Abs(Mathf.Sin(k * Mathf.PI * 2f)) * hopHeight;
				var p = transform.position;
				p.y = y;
				transform.position = p;
				yield return null;
			}
		}

		private void PlayState(string state) => PlayAnim(state);
	}
}
