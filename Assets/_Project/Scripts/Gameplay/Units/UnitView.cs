using Cysharp.Threading.Tasks;
using Project.Core;
using Project.Domain;
using Project.Gameplay.Targeting;
using UnityEngine;

namespace Project.Gameplay.Units
{
	/// <summary>
	/// Базовый view юнита: реализует <see cref="ITappable"/>, держит лейбл силы и кольца подсветки.
	/// </summary>
	public abstract class UnitView : MonoBehaviour, ITappable
	{
		[SerializeField] protected PowerLabel Label;
		[SerializeField] protected Transform StopPoint;
		[SerializeField] protected Transform AnchorPoint;
		[SerializeField] protected Transform VfxPoint;
		[SerializeField] protected GameObject HighlightRing;

		[SerializeField] protected GameObject WarningRing;
		[SerializeField] private float _previewScale = 1.15f;
		[SerializeField] private float _previewDuration = 0.12f;

		private Vector3 _baseScale;
		private bool _baseScaleCached;

		public Unit Unit { get; private set; }
		public UnitId Id => Unit.Id;
		public UnitKind Kind => Unit.Kind;
		public Transform Anchor => AnchorPoint != null ? AnchorPoint : transform;
		public Transform Stop => StopPoint != null ? StopPoint : transform;
		public Transform Vfx => VfxPoint != null ? VfxPoint : Anchor;
		public PowerLabel PowerLabel => Label;

		public virtual void FaceTowards(Vector3 worldPosition)
		{
			var localUp = transform.up;
			var dir = Vector3.ProjectOnPlane(worldPosition - transform.position, localUp);
			if (dir.sqrMagnitude < 0.0001f) return;
			transform.rotation = Quaternion.LookRotation(dir, localUp);
		}

		protected virtual void Awake()
		{
			_baseScale = transform.localScale;
			_baseScaleCached = true;
		}

		public virtual void Bind(Unit unit, Camera camera)
		{
			Unit = unit;
			if (!_baseScaleCached)
			{
				_baseScale = transform.localScale;
				_baseScaleCached = true;
			}

			if (Label != null)
			{
				Label.Init(unit.Kind, camera);
				Label.Set(unit.Power.Value);
			}

			if (HighlightRing != null)
				HighlightRing.SetActive(false);
			if (WarningRing != null)
				WarningRing.SetActive(false);
		}

		public virtual void RefreshPower()
		{
			if (Label != null)
				Label.Set(Unit.Power.Value);
		}

		public virtual void SetHighlighted(bool value)
		{
			if (HighlightRing != null)
				HighlightRing.SetActive(value);
			if (value && WarningRing != null)
				WarningRing.SetActive(false);
		}

		public virtual void SetPreview(bool value, bool isWarning = false)
		{
			if (!_baseScaleCached)
			{
				_baseScale = transform.localScale;
				_baseScaleCached = true;
			}

			if (value)
			{
				var ring = isWarning && WarningRing != null ? WarningRing : HighlightRing;
				if (HighlightRing != null)
					HighlightRing.SetActive(ring == HighlightRing);
				if (WarningRing != null)
					WarningRing.SetActive(ring == WarningRing);
			}
			else
			{
				if (HighlightRing != null)
					HighlightRing.SetActive(false);
				if (WarningRing != null)
					WarningRing.SetActive(false);
			}

			var target = value ? _baseScale * _previewScale : _baseScale;
			Tween.Scale(transform, target, _previewDuration, Ease.OutBack).Forget();
		}
	}
}