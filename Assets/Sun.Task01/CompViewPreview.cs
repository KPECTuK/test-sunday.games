using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Sun.Base;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Sun.Task01
{
	[RequireComponent(typeof(RawImage))]
	// ReSharper disable once UnusedType.Global
	public class CompViewPreview : MonoBehaviour, IWidgetController
	{
		private RawImage _image;
		private ModelContentItem _model;
		private readonly (Vector3 intent, Func<RectTransform, Vector2> getterSize)[] _intent =
		{
			(Vector3.down, GetSizePort),
			(Vector3.up, GetSizePort),
			(Vector3.left, GetSizeLand),
			(Vector3.right, GetSizeLand),
		};

		//? cache
		public CompScreen ScreenSelf => GetComponentInParent<CompScreen>();

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

			// no control over rotation process
			// ui must be adaptive
			// = useless
			// Screen.orientation = ScreenOrientation.AutoRotation;
		}

		public void OnWidgetDisable()
		{
			_model = null;
			_image.texture = null;

			"preview disabled".Log();
		}

		public IEnumerator TaskViewUpdate(int idModel)
		{
			_model ??= Singleton<ServiceModel>.I.GetModelById(idModel);

			// know nothing about pics loaded
			// have no task option with image scale or resolution matching requirements
			// probably got different resources to display as gallery icon and preview
			// will set problem exposed for now
			_image.texture = _model.Texture;

			SyncOrientation();

			yield break;
		}

		public IEnumerator TaskViewRotate(int indexVector)
		{
			var rectTransform = GetComponent<RectTransform>();
			transform.localRotation = Quaternion.LookRotation(rectTransform.forward, -_intent[indexVector].intent);
			var parentSize = _intent[indexVector].getterSize(rectTransform);

			var sizeSelf = rectTransform.rect.size;
			var aspect = sizeSelf.x / sizeSelf.y;

			rectTransform.sizeDelta = new Vector2(parentSize.y * aspect, parentSize.y);

			yield break;
		}

		private static Vector2 GetSizePort(RectTransform @this)
		{
			var parentSize = @this.parent.GetComponent<RectTransform>().rect.size;
			return parentSize;
		}

		private static Vector2 GetSizeLand(RectTransform @this)
		{
			var parentSize = @this.parent.GetComponent<RectTransform>().rect.size;
			return new Vector2(parentSize.y, parentSize.x);
		}

		private static bool DetectThreshold(Vector3 thresholdRotation, Vector3 state)
		{
			return Vector3.Dot(thresholdRotation, state.normalized) > .75f;
		}

		private void SyncOrientation()
		{
			if(GravitySensor.current != null)
			{
				var value = GravitySensor.current.gravity.ReadValue();
				for(var index = 0; index < _intent.Length; index++)
				{
					if(DetectThreshold(_intent[index].intent, value))
					{
						Singleton<ServiceUI>.I.EventsViewPreview.Enqueue(
							new CmdViewPreviewRotate { IndexVector = index });
					}
				}
			}
		}

		// ReSharper disable once UnusedMember.Global
		public void Awake()
		{
			_image = GetComponent<RawImage>();

			SetScheduler<SchedulerTaskAll>();

			if(GravitySensor.current != null)
			{
				InputSystem.EnableDevice(GravitySensor.current);
			}
			else
			{
				"no 'GravitySensor' available".Log();
			}
		}

		// ReSharper disable once UnusedMember.Local
		private void Update()
		{
			// after screen change
			if(ScreenSelf.IsActiveScreen)
			{
				SyncOrientation();
			}

			if(!_tasks.ExecuteTasksSequentially())
			{
				Singleton<ServiceUI>.I.EventsViewPreview.TryExecuteCommandQueue(this);
			}
		}
	}
}
