using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Common
{
	/// <summary>
	/// Эффект пульса на тап. Пул объектов создаётся заранее (см. SceneBuilder),
	/// рантайм только переключает позицию/scale/alpha — без GetComponent/AddComponent.
	/// </summary>
	public sealed class TouchRipple : MonoBehaviour
	{
		[SerializeField] private RectTransform _canvasRect;
		[SerializeField] private Camera _camera;
		[SerializeField] private RectTransform[] _poolRects;
		[SerializeField] private Image[] _poolImages;
		[SerializeField] private float _maxScale = 220f;
		[SerializeField] private float _duration = 0.45f;
		[SerializeField] private Color _color = new Color(1f, 1f, 1f, 0.55f);

		private int _next;

		private void Awake()
		{
			if (_poolRects == null)
				return;

			for (int i = 0; i < _poolRects.Length; i++)
				if (_poolRects[i] != null)
					_poolRects[i].gameObject.SetActive(false);
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
				Spawn(Input.mousePosition);
		}

		private void Spawn(Vector2 screenPos)
		{
			if (_canvasRect == null || _poolRects == null || _poolRects.Length == 0)
				return;

			Vector2 local;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, screenPos, _camera, out local))
				return;

			var rect = _poolRects[_next];
			var img = _poolImages != null && _next < _poolImages.Length ? _poolImages[_next] : null;
			_next = (_next + 1) % _poolRects.Length;
			if (rect == null)
				return;

			rect.anchoredPosition = local;
			rect.sizeDelta = new Vector2(_maxScale, _maxScale);
			rect.localScale = Vector3.one * 0.05f;
			rect.gameObject.SetActive(true);
			if (img != null)
				img.color = _color;

			StartCoroutine(Animate(rect, img));
		}

		private IEnumerator Animate(RectTransform rect, Image img)
		{
			var elapsed = 0f;
			var startColor = img != null ? img.color : _color;
			while (elapsed < _duration)
			{
				elapsed += Time.unscaledDeltaTime;
				var k = Mathf.Clamp01(elapsed / _duration);
				rect.localScale = Vector3.one * Mathf.Lerp(0.05f, 1f, Ease01(k));
				if (img != null)
				{
					var c = startColor;
					c.a = Mathf.Lerp(startColor.a, 0f, k);
					img.color = c;
				}

				yield return null;
			}

			if (rect != null)
				rect.gameObject.SetActive(false);
		}

		private static float Ease01(float k) => 1f - (1f - k) * (1f - k);
	}
}