using Project.Core;
using Project.Domain;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Gameplay.Targeting
{
    /// <summary>
    /// Press-and-release инпут: down → preview, drag → swap, up → commit или cancel.
    /// </summary>
    public sealed class TapInput : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _tappableMask = ~0;

        private SignalBus _signals;
        private bool _enabled = true;
        private bool _pressed;
        private UnitId? _previewId;

        public void Init(SignalBus signals)
        {
            _signals = signals;
            if (_camera == null) _camera = Camera.main;
        }

        public void SetEnabled(bool value)
        {
            _enabled = value;
            if (!value) ClearPreview();
        }

        private void Update()
        {
            if (!_enabled || _signals == null) return;

            if (Input.GetMouseButtonDown(0))
            {
                if (PointerOverUi()) return;
                _pressed = true;
                UpdatePreview();
            }
            else if (Input.GetMouseButton(0) && _pressed)
            {
                UpdatePreview();
            }
            else if (Input.GetMouseButtonUp(0) && _pressed)
            {
                _pressed = false;
                CommitOrCancel();
            }
        }

        private void UpdatePreview()
        {
            var hit = RaycastTappable();
            if (hit == null)
            {
                if (_previewId != null) ClearPreview();
                return;
            }

            if (hit.Kind == UnitKind.Player)
            {
                if (_previewId != null) ClearPreview();
                return;
            }

            if (_previewId != null && _previewId.Value == hit.Id) return;
            _previewId = hit.Id;
            _signals.Fire(new TargetPreviewSignal(hit.Id));
        }

        private void CommitOrCancel()
        {
            var preview = _previewId;
            ClearPreview();
            if (preview == null) return;
            _signals.Fire(new TargetSelectedSignal(preview.Value));
        }

        private void ClearPreview()
        {
            if (_previewId == null) return;
            _previewId = null;
            _signals.Fire(TargetPreviewSignal.None);
        }

        private ITappable RaycastTappable()
        {
            if (_camera == null) return null;
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 100f, _tappableMask)) return null;
            return hit.collider.GetComponentInParent<ITappable>();
        }

        private bool PointerOverUi()
            => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
