using System;
using System.Collections;
using UnityEngine;

namespace Project.Core
{
	public static class Tween
	{
		public static IEnumerator Move(Transform t, Vector3 to, float duration, Func<float, float> ease = null)
		{
			if (t == null)
			{
				yield break;
			}
			if (ease == null)
			{
				ease = Ease.Linear;
			}
			duration = Mathf.Max(0.0001f, duration);
			Vector3 from = t.position;
			float elapsed = 0f;
			while (elapsed < duration)
			{
				if (t == null)
				{
					yield break;
				}
				elapsed += Time.deltaTime;
				float i = ease(Mathf.Clamp01(elapsed / duration));
				t.position = Vector3.LerpUnclamped(from, to, i);
				yield return null;
			}
			if (t != null)
			{
				t.position = to;
			}
		}

		public static IEnumerator LocalMove(Transform t, Vector3 to, float duration, Func<float, float> ease = null)
		{
			if (t == null)
			{
				yield break;
			}
			if (ease == null)
			{
				ease = Ease.Linear;
			}
			duration = Mathf.Max(0.0001f, duration);
			Vector3 from = t.localPosition;
			float elapsed = 0f;
			while (elapsed < duration)
			{
				if (t == null)
				{
					yield break;
				}
				elapsed += Time.deltaTime;
				float i = ease(Mathf.Clamp01(elapsed / duration));
				t.localPosition = Vector3.LerpUnclamped(from, to, i);
				yield return null;
			}
			if (t != null)
			{
				t.localPosition = to;
			}
		}

		public static IEnumerator Scale(Transform t, Vector3 to, float duration, Func<float, float> ease = null)
		{
			if (t == null)
			{
				yield break;
			}
			if (ease == null)
			{
				ease = Ease.Linear;
			}
			duration = Mathf.Max(0.0001f, duration);
			Vector3 from = t.localScale;
			float elapsed = 0f;
			while (elapsed < duration)
			{
				if (t == null)
				{
					yield break;
				}
				elapsed += Time.deltaTime;
				float i = ease(Mathf.Clamp01(elapsed / duration));
				t.localScale = Vector3.LerpUnclamped(from, to, i);
				yield return null;
			}
			if (t != null)
			{
				t.localScale = to;
			}
		}

		public static IEnumerator Fade(CanvasGroup g, float to, float duration)
		{
			if (g == null)
			{
				yield break;
			}
			duration = Mathf.Max(0.0001f, duration);
			float from = g.alpha;
			float elapsed = 0f;
			while (elapsed < duration)
			{
				if (g == null)
				{
					yield break;
				}
				elapsed += Time.deltaTime;
				float i = Mathf.Clamp01(elapsed / duration);
				g.alpha = Mathf.LerpUnclamped(from, to, i);
				yield return null;
			}
			if (g != null)
			{
				g.alpha = to;
			}
		}

		public static IEnumerator Punch(Transform t, float amplitude, float duration)
		{
			if (!(t == null))
			{
				Vector3 origin = t.localScale;
				duration = Mathf.Max(0.0001f, duration);
				float elapsed = 0f;
				while (elapsed < duration && t != null)
				{
					elapsed += Time.deltaTime;
					float i = elapsed / duration;
					float bump = Mathf.Sin(i * 3.14159265f) * amplitude;
					t.localScale = origin * (1f + bump);
					yield return null;
				}
				if (t != null)
				{
					t.localScale = origin;
				}
			}
		}
	}
}
