using System;
using UnityEngine;

namespace Project.Core
{
	public static class Ease
	{
		public static readonly Func<float, float> Linear = (float k) => k;

		public static readonly Func<float, float> OutQuad = (float k) => 1f - (1f - k) * (1f - k);

		public static readonly Func<float, float> InQuad = (float k) => k * k;

		public static readonly Func<float, float> InOutQuad = (float k) => (k < 0.5f) ? (2f * k * k) : (1f - Mathf.Pow(-2f * k + 2f, 2f) * 0.5f);

		public static readonly Func<float, float> OutBack = delegate(float k)
		{
			float num = k - 1f;
			return 1f + 2.70158f * num * num * num + 1.70158f * num * num;
		};

		public static readonly Func<float, float> OutSine = (float k) => Mathf.Sin(k * 3.14159265f * 0.5f);
	}
}
