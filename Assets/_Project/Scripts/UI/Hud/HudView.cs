using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Hud
{
    /// <summary>
    /// HUD-view: опциональные поля для счётчика силы и хинта (по дефолту HUD пустой).
    /// </summary>
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private Text _playerPower;
        [SerializeField] private CanvasGroup _hint;

        public void SetPower(int value)
        {
            if (_playerPower != null) _playerPower.text = value.ToString();
        }

        public void ShowHint(bool value)
        {
            if (_hint == null) return;
            _hint.alpha = value ? 1f : 0f;
            _hint.blocksRaycasts = false;
        }
    }
}
