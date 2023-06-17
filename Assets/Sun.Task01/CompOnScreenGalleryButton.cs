using Assets.Sun.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Assets.Sun.Task01
{
	[RequireComponent(typeof(CompViewGalleryContentItem))]
	// ReSharper disable once UnusedType.Global
	public class CompOnScreenGalleryButton : OnScreenControl, IPointerClickHandler
	{
		[InputControl(layout = "Button")] [SerializeField] private string _controlPath;

		protected override string controlPathInternal { get => _controlPath; set => _controlPath = value; }

		public void OnPointerClick(PointerEventData eventData)
		{
			// TODO: use gallery scheduler\queue as a target instead of casting: TL; TI;
			if(GetComponentInParent<CompViewGalleryContent>().Scheduler is SchedulerViewContentScroll { IsScrolling: false })
			{
				var component = GetComponent<CompViewGalleryContentItem>();
				if(component.Model.IsRequestSuccessful)
				{
					Singleton<ServiceUI>.I.EventsViewPreview.Enqueue(new CmdViewPreviewUpdate { IdModel = component.Model.IdModel, });
					Singleton<ServiceUI>.I.EventsViewScreens.Enqueue(new CmdViewScreenChange { NameScreen = ServiceUI.SCREEN_PREVIEW_S, });
				}
				else
				{
					"trying to preview: request unsuccessful or in progress..".LogWarning();
				}
			}
		}
	}
}
