using Project.Core;
using Project.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Gameplay.Units
{
	public sealed class PowerLabel : MonoBehaviour
	{
		[SerializeField]
		private Text _text;

		[SerializeField]
		private Image _background;

		[SerializeField]
		private Color _playerColor = new Color(0.25f, 0.55f, 1f);

		[SerializeField]
		private Color _enemyColor = new Color(0.95f, 0.25f, 0.25f);

		[SerializeField]
		private Color _chestColor = new Color(0.3f, 0.85f, 0.35f);

		private Camera _camera;

		public void Init(UnitKind kind, Camera cameraMain)
		{
			_camera = cameraMain;
			if (_text == null)
			{
				_text = GetComponentInChildren<Text>(true);
			}
			if (_background == null)
			{
				_background = GetComponentInChildren<Image>(true);
			}
			Color color;
			switch (kind)
			{
			case UnitKind.Player:
				color = _playerColor;
				break;
			case UnitKind.Enemy:
				color = _enemyColor;
				break;
			case UnitKind.Chest:
				color = _chestColor;
				break;
			default:
				color = Color.white;
				break;
			}
			if (_background != null)
			{
				_background.color = color;
			}
			if (_text != null)
			{
				_text.color = Color.white;
			}
		}

		public void Set(int value)
		{
			if (_text != null)
			{
				_text.text = value.ToString();
			}
		}

		public void Pop()
		{
			StartCoroutine(Tween.Punch(base.transform, 0.65f, 0.35f));
		}

		public void SetVisible(bool value)
		{
			base.gameObject.SetActive(value);
		}

		private void LateUpdate()
		{
			if (_camera != null)
			{
				base.transform.forward = _camera.transform.forward;
			}
		}
	}
}
