using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Assets.Sun.Task01
{
	// ReSharper disable once UnusedType.Global
	public class CompOnScreenMenuButton : OnScreenControl, IPointerDownHandler, IPointerUpHandler
	{
		public DateTime PointerDownTimeStamp { get; private set; } = DateTime.MaxValue;

		public void OnPointerDown(PointerEventData data)
		{
			SendValueToControl(1.0f);
			PointerDownTimeStamp = DateTime.UtcNow;
		}

		public void OnPointerUp(PointerEventData data)
		{
			SendValueToControl(0.0f);
			//? reset
			// PointerDownTimeStamp = DateTime.MaxValue;
		}

		[InputControl(layout = "Button")] [SerializeField] private string _controlPath;

		protected override string controlPathInternal { get => _controlPath; set => _controlPath = value; }
	}
}
