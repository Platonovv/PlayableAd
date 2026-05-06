using Project.Core;
using Project.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Gameplay.Units
{
	/// <summary>
	/// Лейбл силы юнита: круглая подложка с цветом по типу + белый текст, billboard к камере.
	/// </summary>
	public sealed class PowerLabel : MonoBehaviour
	{
		[SerializeField] private Text _text;
		[SerializeField] private Image _background;
		[SerializeField] private Color _playerColor = new Color(0.25f, 0.55f, 1f);
		[SerializeField] private Color _enemyColor = new Color(0.95f, 0.25f, 0.25f);
		[SerializeField] private Color _chestColor = new Color(0.30f, 0.85f, 0.35f);

		private Camera _camera;

		public void Init(UnitKind kind, Camera cameraMain)
		{
			_camera = cameraMain;

			if (_text == null)
				_text = GetComponentInChildren<Text>(includeInactive: true);
			if (_background == null)
				_background = GetComponentInChildren<Image>(includeInactive: true);

			Color color;
			switch (kind)
			{
				case UnitKind.Player: color = _playerColor; break;
				case UnitKind.Enemy:  color = _enemyColor;  break;
				case UnitKind.Chest:  color = _chestColor;  break;
				default:              color = Color.white;  break;
			}
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

		public void Pop()
		{
			StartCoroutine(Tween.Punch(transform, 0.65f, 0.35f));
		}

		public void SetVisible(bool value)
		{
			gameObject.SetActive(value);
		}

		private void LateUpdate()
		{
			if (_camera != null)
				transform.forward = _camera.transform.forward;
		}
	}
}