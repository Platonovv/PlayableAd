using System;
using System.Collections;
using Project.Configs;
using Project.Core;
using Project.Domain;
using UnityEngine;

namespace Project.Gameplay.Units
{
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

		[SerializeField]
		private Transform _sword;

		private BalanceConfig _balance;

		private Transform _swordOriginalParent;

		private Vector3 _swordOriginalLocalPos;

		private Quaternion _swordOriginalLocalRot;

		private Vector3 _swordOriginalLocalScale;

		private bool _swordRevealed;

		private Vector3 _flairBaseScale = Vector3.one;

		private float _flairBaseY;

		private Coroutine _flairCo;

		public Transform Sword => _sword;

		public Vector3 SwordTarget => (_swordOriginalParent != null) ? _swordOriginalParent.TransformPoint(_swordOriginalLocalPos) : ((_sword != null) ? _sword.position : base.Anchor.position);

		public void Configure(BalanceConfig balance)
		{
			_balance = balance;
		}

		protected override void Awake()
		{
			base.Awake();
			if (_sword != null)
			{
				_swordOriginalParent = _sword.parent;
				_swordOriginalLocalPos = _sword.localPosition;
				_swordOriginalLocalRot = _sword.localRotation;
				_swordOriginalLocalScale = ((_sword.localScale.sqrMagnitude > 0.0001f) ? _sword.localScale : Vector3.one);
				_sword.localScale = Vector3.zero;
			}
			_flairBaseScale = base.transform.localScale;
		}

		private void LateUpdate()
		{
			if (!_swordRevealed && !(_sword == null))
			{
				_sword.localScale = Vector3.zero;
			}
		}

		public void RevealSword()
		{
			if (!(_sword == null))
			{
				_swordRevealed = true;
				_sword.localScale = _swordOriginalLocalScale;
			}
		}

		public override void Bind(Unit unit, Camera camera)
		{
			base.Bind(unit, camera);
			_flairBaseY = base.transform.position.y;
		}

		public IEnumerator MoveTo(Vector3 destination)
		{
			destination.y = base.transform.position.y;
			PlayState("Run");
			FaceTowards(destination);
			float stopSqr = _balance.StopDistance * _balance.StopDistance;
			while (this != null && (base.transform.position - destination).sqrMagnitude > stopSqr)
			{
				float step = _balance.MoveSpeed * Time.deltaTime;
				base.transform.position = Vector3.MoveTowards(base.transform.position, destination, step);
				FaceTowards(destination);
				yield return null;
			}
			PlayState("Idle");
		}

		public IEnumerator PlayAttack()
		{
			PlayState("Attack");
			yield return new WaitForSeconds(_balance.AttackWindup + _balance.AttackImpactDelay);
		}

		public IEnumerator PlaySuperAttack()
		{
			PlayState("SuperAttack");
			yield return new WaitForSeconds(_balance.AttackWindup + _balance.AttackImpactDelay);
		}

		public IEnumerator PlayRecover()
		{
			yield return new WaitForSeconds(_balance.AttackRecover);
			PlayState("Idle");
		}

		public IEnumerator PlayBounceBack(Vector3 origin)
		{
			PlayState("Hit");
			yield return Tween.Move(base.transform, origin, _balance.FailedAttackBounce, Ease.OutBack);
			PlayState("Idle");
		}

		public IEnumerator PlayUpgrade()
		{
			PlayState("Upgrade");
			StartFlair(_balance.UpgradeDuration);
			yield return new WaitForSeconds(_balance.UpgradeDuration);
			yield return new WaitForSeconds(_balance.UpgradeAnimTail);
		}

		public void PlayPowerGain()
		{
			StartFlair(_balance.UpgradeDuration * 0.6f);
		}

		public void PlayIdle()
		{
			PlayState("Idle");
		}

		public void PlayVictory()
		{
			PlayState("Victory");
		}

		public void PlayHurt()
		{
			PlayState("Hit");
		}

		public IEnumerator PlayDeath()
		{
			PlayState("Death");
			yield return new WaitForSeconds(_balance.DeathAnimDuration);
			yield return Tween.Scale(base.transform, Vector3.zero, _balance.DeathFadeDuration, Ease.InQuad);
		}

		public override void FaceTowards(Vector3 worldTarget)
		{
			Vector3 localUp = base.transform.up;
			Vector3 dir = Vector3.ProjectOnPlane(worldTarget - base.transform.position, localUp);
			if (!(dir.sqrMagnitude < 0.0001f))
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(dir, localUp), 18f * Time.deltaTime);
			}
		}

		private void StartFlair(float duration)
		{
			if (_flairCo != null)
			{
				StopCoroutine(_flairCo);
			}
			_flairCo = StartCoroutine(RunFlair(duration));
		}

		private IEnumerator RunFlair(float duration)
		{
			IEnumerator flair;
			switch (UnityEngine.Random.Range(0, 4))
			{
			case 0:
				flair = PunchFlair(duration);
				break;
			case 1:
				flair = HopUp(duration);
				break;
			case 2:
				flair = Squash(duration);
				break;
			default:
				flair = DoubleHop(duration);
				break;
			}
			yield return flair;
			if (this != null)
			{
				base.transform.localScale = _flairBaseScale;
				Vector3 p = base.transform.position;
				p.y = _flairBaseY;
				base.transform.position = p;
			}
			_flairCo = null;
		}

		private IEnumerator PunchFlair(float duration)
		{
			duration = Mathf.Max(0.0001f, duration);
			float elapsed = 0f;
			while (elapsed < duration && this != null)
			{
				elapsed += Time.deltaTime;
				float i = elapsed / duration;
				float bump = Mathf.Sin(i * 3.14159265f) * 0.3f;
				base.transform.localScale = _flairBaseScale * (1f + bump);
				yield return null;
			}
		}

		private IEnumerator HopUp(float duration)
		{
			float elapsed = 0f;
			while (elapsed < duration && this != null)
			{
				elapsed += Time.deltaTime;
				float i = Mathf.Clamp01(elapsed / duration);
				float y = _flairBaseY + Mathf.Sin(i * 3.14159265f) * 0.7f;
				Vector3 p = base.transform.position;
				p.y = y;
				base.transform.position = p;
				yield return null;
			}
		}

		private IEnumerator Squash(float duration)
		{
			float elapsed = 0f;
			while (elapsed < duration && this != null)
			{
				elapsed += Time.deltaTime;
				float i = Mathf.Clamp01(elapsed / duration);
				float s = Mathf.Sin(i * 3.14159265f * 2f);
				base.transform.localScale = new Vector3(_flairBaseScale.x * (1f + s * 0.18f), _flairBaseScale.y * (1f - s * 0.18f), _flairBaseScale.z * (1f + s * 0.18f));
				yield return null;
			}
		}

		private IEnumerator DoubleHop(float duration)
		{
			float elapsed = 0f;
			while (elapsed < duration && this != null)
			{
				elapsed += Time.deltaTime;
				float i = Mathf.Clamp01(elapsed / duration);
				float y = _flairBaseY + Mathf.Abs(Mathf.Sin(i * 3.14159265f * 2f)) * 0.4f;
				Vector3 p = base.transform.position;
				p.y = y;
				base.transform.position = p;
				yield return null;
			}
		}

		private void PlayState(string state)
		{
			PlayAnim(state);
		}
	}
}
