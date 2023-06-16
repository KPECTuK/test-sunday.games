using Assets.Sun.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Assets.Sun.Task01
{
	// ReSharper disable once UnusedType.Global
	public class CompOnScreenPreviewButton : OnScreenControl, IPointerClickHandler
	{
		[InputControl(layout = "Button")] [SerializeField] private string _controlPath;

		protected override string controlPathInternal { get => _controlPath; set => _controlPath = value; }

		public void OnPointerClick(PointerEventData eventData)
		{
			Singleton<ServiceUI>.I.EventsViewScreens.Enqueue(new CmdViewScreenChange { NameScreen = ServiceUI.SCREEN_GALLERY_S });
		}
	}
}
