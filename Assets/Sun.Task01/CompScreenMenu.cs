using System;
using Assets.Sun.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Sun.Task01
{
	[RequireComponent(typeof(CompScreen))]
	// ReSharper disable once UnusedType.Global
	public class CompScreenMenu : MonoBehaviour
	{
		public double ButtonHoldInterval = 2d;
		public Button Button;
		public CompOnScreenMenuButton Counter;

		// ReSharper disable once UnusedMember.Global
		public void Awake()
		{
			Button.onClick.AddListener(OnClick);
		}

		private void OnClick()
		{
			if(DateTime.UtcNow - Counter.PointerDownTimeStamp > TimeSpan.FromSeconds(ButtonHoldInterval))
			{
				Singleton<ServiceUI>.I.EventsViewScreens.Enqueue(
					new CmdViewScreenChange { NameScreen = ServiceUI.SCREEN_GALLERY_S });
			}
		}
	}
}
