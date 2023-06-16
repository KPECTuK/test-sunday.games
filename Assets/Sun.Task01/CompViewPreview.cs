using System.Collections;
using System.Collections.Generic;
using Assets.Sun.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Sun.Task01
{
	[RequireComponent(typeof(RawImage))]
	// ReSharper disable once UnusedType.Global
	public class CompViewPreview : MonoBehaviour, IWidgetController
	{
		public Button ButtonClose;

		private readonly Queue<IEnumerator> _tasks = new();
		private IScheduler _scheduler;

		public IScheduler Scheduler => _scheduler ?? new SchedulerTaskDefault();

		public void SetScheduler<T>() where T : IScheduler, new()
		{
			_scheduler = _tasks.BuildScheduler<T>();
		}

		public void OnWidgetEnable()
		{
			"preview enabled".Log();

			SetScheduler<SchedulerTaskAll>();
		}

		public void OnWidgetDisable()
		{
			"preview disabled".Log();
		}

		private void OnClick()
		{
			Singleton<ServiceUI>.I.EventsViewScreens.Enqueue(new CmdViewScreenChange { NameScreen = ServiceUI.SCREEN_GALLERY_S });
		}

		public IEnumerator TaskInitialize(int idModel)
		{
			yield break;
		}

		// ReSharper disable once UnusedMember.Global
		public void Awake()
		{
			ButtonClose.onClick.AddListener(OnClick);
		}

		// ReSharper disable once UnusedMember.Local
		private void Update()
		{
			if(!_tasks.ExecuteTasksSequentially())
			{
				Singleton<ServiceUI>.I.EventsViewPreview.TryExecuteCommandQueue(this);
			}
		}
	}
}
