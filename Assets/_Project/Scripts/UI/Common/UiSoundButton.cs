using Project.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Common
{
	/// <summary>
	/// Воспроизводит звук-ключ из <see cref="AudioBank"/> на клик по подвязанному <see cref="Button"/>.
	/// Все ссылки сериализованы — без GetComponent в рантайме.
	/// </summary>
	public sealed class UiSoundButton : MonoBehaviour
	{
		[SerializeField] private AudioService _audio;
		[SerializeField] private Button _button;
		[SerializeField] private string _key = "click";

		private void Awake()
		{
			if (_button != null)
				_button.onClick.AddListener(Play);
		}

		private void Play()
		{
			if (_audio != null && !string.IsNullOrEmpty(_key))
				_audio.Play(_key);
		}
	}
}