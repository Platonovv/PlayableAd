using Project.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.UI.Common
{
	[RequireComponent(typeof(RectTransform))]
	public sealed class TweenButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		[SerializeField]
		private float _pressedScale = 0.94f;

		[SerializeField]
		private float _duration = 0.08f;

		private Vector3 _origin;

		private RectTransform _rect;

		private void Awake()
		{
			_rect = (RectTransform)base.transform;
			_origin = _rect.localScale;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			StartCoroutine(Tween.Scale(_rect, _origin * _pressedScale, _duration, Ease.OutQuad));
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			StartCoroutine(Tween.Scale(_rect, _origin, _duration, Ease.OutBack));
		}
	}
}
