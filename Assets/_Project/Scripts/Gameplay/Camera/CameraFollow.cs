using UnityEngine;

namespace Project.Gameplay.CameraFx
{
    /// <summary>
    /// SmoothDamp-следование камеры за целью с фиксированным оффсетом (в текущей сборке не используется).
    /// </summary>
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new(0f, 7f, -6f);
        [SerializeField] private float _smoothTime = 0.35f;
        [SerializeField] private bool _followX = true;
        [SerializeField] private bool _followY = false;
        [SerializeField] private bool _followZ = true;

        private Vector3 _velocity;
        private Vector3 _basePosition;

        private void Awake() => _basePosition = transform.position;

        public void SetTarget(Transform target)
        {
            _target = target;
            if (_target != null) _offset = transform.position - _target.position;
        }

        private void LateUpdate()
        {
            if (_target == null) return;
            var desired = _target.position + _offset;
            var current = transform.position;
            if (!_followX) desired.x = current.x;
            if (!_followY) desired.y = current.y;
            if (!_followZ) desired.z = current.z;
            transform.position = Vector3.SmoothDamp(current, desired, ref _velocity, _smoothTime);
        }
    }
}
