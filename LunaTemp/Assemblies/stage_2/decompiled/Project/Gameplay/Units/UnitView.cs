using System.Collections;
using Project.Core;
using Project.Domain;
using Project.Gameplay.Targeting;
using UnityEngine;
using UnityEngine.Rendering;

namespace Project.Gameplay.Units
{
	public abstract class UnitView : MonoBehaviour, ITappable
	{
		[Header("Geometry anchors")]
		[SerializeField]
		protected PowerLabel Label;

		[SerializeField]
		protected Transform StopPoint;

		[SerializeField]
		protected Transform AnchorPoint;

		[SerializeField]
		protected Transform VfxPoint;

		[Header("Highlight rings")]
		[SerializeField]
		protected GameObject HighlightRing;

		[SerializeField]
		protected GameObject WarningRing;

		[Header("Animation")]
		[SerializeField]
		protected Animation LegacyAnim;

		[SerializeField]
		protected AnimMapping[] LegacyClipMap;

		[Header("Preview tween")]
		[SerializeField]
		private float _previewScale = 1.15f;

		[SerializeField]
		private float _previewDuration = 0.12f;

		[SerializeField]
		private float _ringPulseAmplitude = 0.08f;

		[SerializeField]
		private float _ringPulseSpeed = 5f;

		private Vector3 _baseScale;

		private bool _baseScaleCached;

		private Vector3 _highlightRingBaseScale = Vector3.one;

		private Vector3 _warningRingBaseScale = Vector3.one;

		private Coroutine _ringPulseCo;

		public Unit Unit { get; private set; }

		public UnitId Id => Unit.Id;

		public UnitKind Kind => Unit.Kind;

		public Transform Anchor => (AnchorPoint != null) ? AnchorPoint : base.transform;

		public Transform Stop => (StopPoint != null) ? StopPoint : base.transform;

		public Transform Vfx => (VfxPoint != null) ? VfxPoint : Anchor;

		public PowerLabel PowerLabel => Label;

		public virtual void FaceTowards(Vector3 worldPosition)
		{
			Vector3 localUp = base.transform.up;
			Vector3 dir = Vector3.ProjectOnPlane(worldPosition - base.transform.position, localUp);
			if (!(dir.sqrMagnitude < 0.0001f))
			{
				base.transform.rotation = Quaternion.LookRotation(dir, localUp);
			}
		}

		protected virtual void Awake()
		{
			CacheBaseScale();
			CacheRingScales();
			DisableShadows();
			HideRings();
			InitLegacyAnim();
		}

		public virtual void Bind(Unit unit, Camera camera)
		{
			Unit = unit;
			CacheBaseScale();
			InitLabel(camera);
			HideRings();
			AssignWorldSpaceCamera(camera);
		}

		public virtual void RefreshPower()
		{
			if (Label != null)
			{
				Label.Set(Unit.Power.Value);
			}
		}

		public virtual void SetHighlighted(bool value)
		{
			if (HighlightRing != null)
			{
				HighlightRing.SetActive(value);
			}
			if (value && WarningRing != null)
			{
				WarningRing.SetActive(false);
			}
			RestartRingPulse(value ? HighlightRing : null);
		}

		public virtual void SetPreview(bool value, bool isWarning = false)
		{
			CacheBaseScale();
			GameObject activeRing = null;
			if (value)
			{
				bool useWarning = isWarning && WarningRing != null;
				if (HighlightRing != null)
				{
					HighlightRing.SetActive(!useWarning);
				}
				if (WarningRing != null)
				{
					WarningRing.SetActive(useWarning);
				}
				activeRing = (useWarning ? WarningRing : HighlightRing);
			}
			else
			{
				HideRings();
			}
			RestartRingPulse(activeRing);
			Vector3 target = (value ? (_baseScale * _previewScale) : _baseScale);
			StartCoroutine(Tween.Scale(base.transform, target, _previewDuration, Ease.OutBack));
		}

		public void PlayAnim(string state)
		{
			if (!(LegacyAnim == null))
			{
				AnimMapping entry = ResolveLegacyEntry(state);
				if (!(entry.Clip == null))
				{
					LegacyAnim.Play(entry.Clip.name);
				}
			}
		}

		private void CacheBaseScale()
		{
			if (!_baseScaleCached)
			{
				_baseScale = base.transform.localScale;
				_baseScaleCached = true;
			}
		}

		private void DisableShadows()
		{
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(true);
			foreach (Renderer r in componentsInChildren)
			{
				r.shadowCastingMode = ShadowCastingMode.Off;
				r.receiveShadows = false;
			}
		}

		private void HideRings()
		{
			if (HighlightRing != null)
			{
				HighlightRing.SetActive(false);
			}
			if (WarningRing != null)
			{
				WarningRing.SetActive(false);
			}
		}

		private void CacheRingScales()
		{
			if (HighlightRing != null)
			{
				_highlightRingBaseScale = HighlightRing.transform.localScale;
			}
			if (WarningRing != null)
			{
				_warningRingBaseScale = WarningRing.transform.localScale;
			}
		}

		private void RestartRingPulse(GameObject target)
		{
			if (_ringPulseCo != null)
			{
				StopCoroutine(_ringPulseCo);
				_ringPulseCo = null;
			}
			if (HighlightRing != null)
			{
				HighlightRing.transform.localScale = _highlightRingBaseScale;
			}
			if (WarningRing != null)
			{
				WarningRing.transform.localScale = _warningRingBaseScale;
			}
			if (!(target == null))
			{
				Vector3 baseScale = ((target == HighlightRing) ? _highlightRingBaseScale : _warningRingBaseScale);
				_ringPulseCo = StartCoroutine(RingPulseRoutine(target.transform, baseScale));
			}
		}

		private IEnumerator RingPulseRoutine(Transform ringTransform, Vector3 baseScale)
		{
			while (ringTransform != null && ringTransform.gameObject.activeInHierarchy)
			{
				float pulse = 1f + Mathf.Sin(Time.unscaledTime * _ringPulseSpeed) * _ringPulseAmplitude;
				ringTransform.localScale = baseScale * pulse;
				yield return null;
			}
			if (ringTransform != null)
			{
				ringTransform.localScale = baseScale;
			}
			_ringPulseCo = null;
		}

		private void InitLegacyAnim()
		{
			ApplyLegacyWrapModes();
			PlayAnim("Idle");
		}

		private void InitLabel(Camera camera)
		{
			if (!(Label == null))
			{
				Label.Init(Unit.Kind, camera);
				Label.Set(Unit.Power.Value);
			}
		}

		private void AssignWorldSpaceCamera(Camera camera)
		{
			Canvas[] componentsInChildren = GetComponentsInChildren<Canvas>(true);
			foreach (Canvas canvas in componentsInChildren)
			{
				if (canvas.renderMode == RenderMode.WorldSpace)
				{
					canvas.worldCamera = camera;
				}
			}
		}

		private AnimMapping ResolveLegacyEntry(string state)
		{
			if (LegacyClipMap == null)
			{
				return default(AnimMapping);
			}
			for (int i = 0; i < LegacyClipMap.Length; i++)
			{
				if (LegacyClipMap[i].StateName == state)
				{
					return LegacyClipMap[i];
				}
			}
			return default(AnimMapping);
		}

		private void ApplyLegacyWrapModes()
		{
			if (LegacyClipMap == null)
			{
				return;
			}
			for (int i = 0; i < LegacyClipMap.Length; i++)
			{
				AnimMapping entry = LegacyClipMap[i];
				if (!(entry.Clip == null))
				{
					entry.Clip.wrapMode = ((!entry.Loop) ? WrapMode.Once : WrapMode.Loop);
				}
			}
		}
	}
}
