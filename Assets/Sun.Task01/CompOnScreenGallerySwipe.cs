using Assets.Sun.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.InputSystem.UI;

namespace Assets.Sun.Task01
{
	[RequireComponent(typeof(CompViewGalleryContent))]
	// ReSharper disable once UnusedType.Global
	public class CompOnScreenGallerySwipe : OnScreenControl, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler
	{
		private CompViewGalleryContent _control;
		private Vector2 _lastPosition;

		[InputControl(layout = "Vector2")] [SerializeField] private string _controlPath;

		protected override string controlPathInternal { get => _controlPath; set => _controlPath = value; }

		// ReSharper disable once UnusedMember.Local
		private void Awake()
		{
			_control = GetComponent<CompViewGalleryContent>();
		}

		public void OnPointerMove(PointerEventData eventData)
		{
			// its a pointer control path, universal one..

			var delta = _lastPosition - eventData.position;
			var deltaNorm = _control.GetStepNorm(delta.y);
			_lastPosition = eventData.position;

			var ext = eventData as ExtendedPointerEventData;

			// the solution (input system) does not behave as expected, so i cannot just pick it and use
			// so i need to waste my time to debug and study.. and to make bicycles
			// the solution produces more problems instead of solving ones, so its a junk.. again.. Unity, hello.. are you there?..

			if(IsPhaseBound())
			{
				return;
			}

			// get phase is not pure combined solution: TL; TI;
			if(IsPhaseMove() || IsLmbHold())
			{
				// $"-- <color=yellow>scroll</color> move: {(Input.touches.Length > 0 ? Input.touches[0].phase : "no")}".Log();

				// may throw null over ext (unexpectedly)
				// var eventState = $"clicks: {eventData.clickCount:00} delta: {delta.x,7:F4} x {delta.y,7:F4} type: {ext.pointerType}";

				if(deltaNorm < 0)
				{
					// $"    call move up  : {deltaNorm,7:F4} (event state: {eventState})".Log();

					_control.Scheduler.Schedule(deltaNorm, _control.TaskScrollDown);
					// _control.TaskScrollDown(deltaNorm).MoveNext();
				}
				else
				{
					// $"    call move down: {deltaNorm,7:F4} (event state: {eventState})".Log();

					_control.Scheduler.Schedule(deltaNorm, _control.TaskScrollUp);
					//_control.TaskScrollUp(deltaNorm).MoveNext();
				}

			}

			ext.Use();
		}

		private bool IsLmbHold()
		{
			// legacy is used anyway
			return Input.mousePresent && Input.GetMouseButton(0);
		}

		private bool IsPhaseBound()
		{
			return
				Input.touches.Length > 0 &&
				Input.touches[0].phase is TouchPhase.Ended or TouchPhase.Canceled or TouchPhase.Began;
		}

		private bool IsPhaseComplete()
		{
			if(Input.touches.Length > 0)
			{
				// Input.touchCount, gain.. currently?.. or supported?.. simple, hugh.. that's why its a junk.. its everywhere..
				// Input.touchCount and Input.touches.Length are not the same over phase so it throws on touch start..
				$"[count: {Input.touchCount} array: {Input.touches.Length} phase: {(Input.touches.Length > 0 ? Input.touches[0].phase.ToString() : "absent")}]".Log();
			}

			return
				Input.touches.Length > 0 &&
				Input.touches[0].phase is TouchPhase.Ended or TouchPhase.Canceled;
		}

		private bool IsPhaseMove()
		{
			return
				Input.touches.Length > 0 &&
				Input.touches[0].phase is TouchPhase.Moved;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			// useless for touches: not guaranty to be called
			//! even double enter on the same interval

			"-- <color=yellow>scroll</color>: down".Log();

			eventData.Use();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			// useless for touches: not guaranty to be called

			"-- <color=yellow>scroll</color>: up".Log();

			eventData.Use();
		}
	}
}
