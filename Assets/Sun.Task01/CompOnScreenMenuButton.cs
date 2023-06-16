using System;
using Assets.Sun.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Assets.Sun.Task01
{
	// ReSharper disable once UnusedType.Global
	public class CompOnScreenMenuButton : OnScreenControl, IPointerDownHandler, IPointerUpHandler
	{
		public double ButtonHoldInterval = .5d;

		[InputControl(layout = "Button")] [SerializeField] private string _controlPath;

		protected override string controlPathInternal { get => _controlPath; set => _controlPath = value; }

		private DateTime _timestampClickBegin;

		public void OnPointerDown(PointerEventData data)
		{
			_timestampClickBegin = DateTime.UtcNow;
		}

		public void OnPointerUp(PointerEventData data)
		{
			if(DateTime.UtcNow - _timestampClickBegin > TimeSpan.FromSeconds(ButtonHoldInterval))
			{
				Singleton<ServiceUI>.I.EventsViewScreens.Enqueue(new CmdViewScreenChange { NameScreen = ServiceUI.SCREEN_GALLERY_S });
			}
		}

		// ReSharper disable once UnusedMember.Local
		private void Awake()
		{
			// TODO: modifier
		}
	}
}
