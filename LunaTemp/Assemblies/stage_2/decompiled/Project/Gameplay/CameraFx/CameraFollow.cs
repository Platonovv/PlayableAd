using UnityEngine;

namespace Project.Gameplay.CameraFx
{
	public sealed class CameraFollow : MonoBehaviour
	{
		[SerializeField]
		private Transform _target;

		[SerializeField]
		private Vector3 _offset = new Vector3(0f, 7f, -6f);

		[SerializeField]
		private float _smoothTime = 0.35f;

		[SerializeField]
		private bool _followX = true;

		[SerializeField]
		private bool _followY = false;

		[SerializeField]
		private bool _followZ = true;

		private Vector3 _velocity;

		private Vector3 _basePosition;

		private void Awake()
		{
			_basePosition = base.transform.position;
		}

		public void SetTarget(Transform target)
		{
			_target = target;
			if (_target != null)
			{
				_offset = base.transform.position - _target.position;
			}
		}

		private void LateUpdate()
		{
			if (!(_target == null))
			{
				Vector3 desired = _target.position + _offset;
				Vector3 current = base.transform.position;
				if (!_followX)
				{
					desired.x = current.x;
				}
				if (!_followY)
				{
					desired.y = current.y;
				}
				if (!_followZ)
				{
					desired.z = current.z;
				}
				base.transform.position = Vector3.SmoothDamp(current, desired, ref _velocity, _smoothTime);
			}
		}
	}
}
