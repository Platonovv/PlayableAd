using Project.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Gameplay.Units
{
	/// <summary>
	/// Лейбл силы юнита: круглая подложка с цветом по типу + белый текст, billboard к камере.
	/// </summary>
	public sealed class PowerLabel : MonoBehaviour
	{
		[SerializeField] private TMP_Text _text;
		[SerializeField] private Image _background;
		[SerializeField] private Color _playerColor = new(0.25f, 0.55f, 1f);
		[SerializeField] private Color _enemyColor = new(0.95f, 0.25f, 0.25f);
		[SerializeField] private Color _chestColor = new(0.30f, 0.85f, 0.35f);

		private Camera _camera;

		public void Init(UnitKind kind, Camera cameraMain)
		{
			_camera = cameraMain;

			var color = kind switch
			{
				UnitKind.Player => _playerColor,
				UnitKind.Enemy  => _enemyColor,
				UnitKind.Chest  => _chestColor,
				_               => Color.white
			};
			if (_background != null)
				_background.color = color;
			if (_text != null)
				_text.color = Color.white;
		}

		public void Set(int value)
		{
			if (_text != null)
				_text.text = value.ToString();
		}

		private void LateUpdate()
		{
			if (_camera != null)
				transform.forward = _camera.transform.forward;
		}
	}
}