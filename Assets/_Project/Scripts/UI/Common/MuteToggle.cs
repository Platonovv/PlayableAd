using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Common
{
	/// <summary>
	/// Тоггл звука: подменяет sprite иконки между _onSprite и _offSprite,
	/// переключает <see cref="AudioListener.volume"/> между 0 и 1.
	/// </summary>
	public sealed class MuteToggle : MonoBehaviour
	{
		[SerializeField] private Button _button;
		[SerializeField] private Image _icon;
		[SerializeField] private Sprite _onSprite;
		[SerializeField] private Sprite _offSprite;
		[SerializeField] private bool _startMuted;

		private bool _muted;

		private void Awake()
		{
			_muted = _startMuted;
			ApplyState();
			if (_button != null)
				_button.onClick.AddListener(Toggle);
		}

		private void Toggle()
		{
			_muted = !_muted;
			ApplyState();
		}

		private void ApplyState()
		{
			AudioListener.volume = _muted ? 0f : 1f;
			if (_icon == null)
				return;

			_icon.sprite = _muted ? _offSprite : _onSprite;
			var c = _icon.color;
			c.a = _muted ? 0.7f : 1f;
			_icon.color = c;
		}
	}
}