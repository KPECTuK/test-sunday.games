using System.Collections;
using System.Collections.Generic;
using Assets.Sun.Base;
using UnityEngine;

namespace Assets.Sun.Task01
{
	[RequireComponent(typeof(CompScreen))]
	// ReSharper disable once UnusedType.Global
	public class CompScreenGallery : MonoBehaviour, IWidgetController
	{
		public CompViewGalleryContent CompViewGalleryContent;

		private readonly Queue<IEnumerator> _tasks = new();

		private IScheduler _scheduler;

		public IScheduler Scheduler => _scheduler ?? new SchedulerTaskDefault();

		public void SetScheduler<T>() where T : IScheduler, new()
		{
			_scheduler = _tasks.BuildScheduler<T>();
		}

		public void OnWidgetEnable()
		{
			//? to Screen for all
			CompViewGalleryContent.OnWidgetEnable();
		}

		public void OnWidgetDisable()
		{
			//? to Screen for all
			CompViewGalleryContent.OnWidgetDisable();
		}
	}
}
