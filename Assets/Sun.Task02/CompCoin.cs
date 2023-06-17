using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Sun.Task02
{
	[RequireComponent(typeof(Collider))]
	// ReSharper disable once UnusedType.Global
	public class CompCoin : MonoBehaviour
	{
		public Material MatVariant00;
		public Material MatVariant01;

		public float _angularSpeedPerSecond;

		public Transform View;
		public InputAction ActionPress;
		public InputAction ActionPosition;

		private Collider _collider;
		private Vector3 _axis;
		private Material[] _available;
		private int _current;

		// ReSharper disable once UnusedMember.Local
		private void OnValidate()
		{
			_angularSpeedPerSecond = _angularSpeedPerSecond is < .1f and > -.1f
				? 10
				: _angularSpeedPerSecond;
		}

		// ReSharper disable once UnusedMember.Local
		private void OnEnable()
		{
			_axis = View.up;
			_collider = GetComponent<Collider>();

			_available = new[]
			{
				MatVariant00,
				MatVariant01,
			};

			_current = 0;
			View.GetComponent<MeshRenderer>().sharedMaterial = _available[_current];

			ActionPress.Enable();
			ActionPosition.Enable();
		}

		// ReSharper disable once UnusedMember.Local
		private void OnDisable()
		{
			ActionPosition.Disable();
			ActionPress.Disable();

			_collider = null;
			_available = null;
		}

		// ReSharper disable once UnusedMember.Global
		public void Update()
		{
			var delta = Time.deltaTime * _angularSpeedPerSecond % 180f;
			delta = delta == 180f ? 0f : delta;
			View.localRotation *= Quaternion.AngleAxis(delta, _axis);

			if(ActionPress.triggered)
			{
				var posScreen = ActionPosition.ReadValue<Vector2>();

				var ray = Camera.main.ScreenPointToRay(posScreen);
				if(_collider.Raycast(ray, out var hit, float.MaxValue))
				{
					_current = ++_current % _available.Length;
					View.GetComponent<MeshRenderer>().sharedMaterial = _available[_current];
				}
			}
		}
	}
}
