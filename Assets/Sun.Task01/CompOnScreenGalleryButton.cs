using Assets.Sun.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Assets.Sun.Task01
{
	// ReSharper disable once UnusedType.Global
	public class CompOnScreenGalleryButton : OnScreenControl, IPointerDownHandler, IPointerUpHandler
	{
		public void OnPointerDown(PointerEventData data)
		{
			SendValueToControl(1.0f);

			"-- <color=yellow>button</color>: down".Log();
		}

		public void OnPointerUp(PointerEventData data)
		{
			SendValueToControl(0.0f);

			"-- <color=yellow>button</color>: up".Log();
		}

		[InputControl(layout = "Button")] [SerializeField] private string _controlPath;

		protected override string controlPathInternal { get => _controlPath; set => _controlPath = value; }
	}
}
