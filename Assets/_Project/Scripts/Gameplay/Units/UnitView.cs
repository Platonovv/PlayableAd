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
		[Tooltip("Точка, к которой бежит игрок (например, перед противником). Если не задана — используется корень префаба.")]
		[SerializeField] protected Transform StopPoint;
		[SerializeField] protected Transform AnchorPoint;
		[SerializeField] protected GameObject HighlightRing;

		[Tooltip(
			"Опционально: красное кольцо для preview слишком сильного врага. Если не задано — fallback на обычный HighlightRing.")]
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

		public virtual void FaceTowards(Vector3 worldPosition)
		{
			var dir = worldPosition - transform.position;
			dir.y = 0f;
			if (dir.sqrMagnitude < 0.0001f) return;
			transform.rotation = Quaternion.LookRotation(dir);
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
				// Если есть отдельный warning-ring — показываем его, иначе fallback на обычный.
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