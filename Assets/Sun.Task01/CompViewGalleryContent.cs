using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Sun.Base;
using BH.Components;
using UnityEngine;

namespace Assets.Sun.Task01
{
	// ReSharper disable once UnusedType.Global
	public class CompViewGalleryContent : MonoBehaviour, IWidgetController
	{
		public CompViewGalleryContentItem Proto;

		private delegate bool GetPointLocalOnAnimLine(float normalized, RectTransform item, out Vector3 localPos);

		private readonly List<CompViewGalleryContentItem> _itemsLeft = new();
		private readonly List<CompViewGalleryContentItem> _itemsRight = new();
		private RangeWindow _windowLeft;
		private RangeWindow _windowRight;

		private readonly Queue<IEnumerator> _tasks = new();
		private IScheduler _scheduler;

		public IScheduler Scheduler => _scheduler ?? new SchedulerTaskDefault();

		public void SetScheduler<T>() where T : IScheduler, new()
		{
			_scheduler = _tasks.BuildScheduler<T>();
		}

		public void OnWidgetEnable()
		{
			"content enabled".Log();

			SetScheduler<SchedulerViewContentScroll>();

			Scheduler.PassThrough.Schedule(TaskWidgetsInitialize);
		}

		public void OnWidgetDisable()
		{
			Scheduler.PassThrough.Schedule(TaskWidgetsRetain);

			"content disabled".Log();
		}

		public void OnModelUpdated(int idModel)
		{
			for(var index = 0; index < _itemsLeft.Count; index++)
			{
				if(_itemsLeft[index].Model.IdModel == idModel)
				{
					_itemsLeft[index].OnModelUpdated();
				}
			}

			for(var index = 0; index < _itemsRight.Count; index++)
			{
				if(_itemsRight[index].Model.IdModel == idModel)
				{
					_itemsRight[index].OnModelUpdated();
				}
			}
		}

		public float GetStepNorm(float absDelta)
		{
			return absDelta / transform.GetComponent<RectTransform>().rect.height;
		}

		private bool GetPointLocalOnAnimLineLeft(float normalized, RectTransform item, out Vector3 localPos)
		{
			var containerRect = transform.GetComponent<RectTransform>().rect;
			var baseStart = (new Vector3(containerRect.xMax, containerRect.yMax) + new Vector3(containerRect.xMin, containerRect.yMax)) * .5f;
			var baseStop = (new Vector3(containerRect.xMax, containerRect.yMin) + new Vector3(containerRect.xMin, containerRect.yMin)) * .5f;
			baseStart = (baseStart + new Vector3(containerRect.xMin, containerRect.yMax)) * .5f;
			baseStop = (baseStop + new Vector3(containerRect.xMin, containerRect.yMin)) * .5f;

			#if UNITY_EDITOR
			ExtensionsGizmos.DrawLineDirect(
				transform.TransformPoint(baseStart),
				transform.TransformPoint(baseStop),
				Color.green,
				Color.black);
			#endif

			var itemSize = item.sizeDelta;
			var itemCorrection = new Vector3(
				// to center
				itemSize.x * (.5f - item.pivot.x),
				// to top
				itemSize.y * item.pivot.y);
			localPos = (baseStop - baseStart) * normalized + baseStart - itemCorrection;
			var animDirectionNorm = (baseStop - baseStart).normalized;
			var startToCurrent = localPos - baseStart;
			var stopToCurrent = localPos - baseStop;
			var isAboveOutside =
				Vector3.Dot(startToCurrent.normalized, animDirectionNorm) < 0f &&
				startToCurrent.magnitude >= itemSize.y * .5f;
			var isBelowOutside =
				Vector3.Dot(stopToCurrent.normalized, animDirectionNorm) > 0f &&
				stopToCurrent.magnitude >= itemSize.y * .5f;
			return isAboveOutside || isBelowOutside;
		}

		private bool GetPointLocalOnAnimLineRight(float normalized, RectTransform item, out Vector3 localPos)
		{
			var containerRect = transform.GetComponent<RectTransform>().rect;
			var baseStart = (new Vector3(containerRect.xMax, containerRect.yMax) + new Vector3(containerRect.xMin, containerRect.yMax)) * .5f;
			var baseStop = (new Vector3(containerRect.xMax, containerRect.yMin) + new Vector3(containerRect.xMin, containerRect.yMin)) * .5f;
			baseStart = (baseStart + new Vector3(containerRect.xMax, containerRect.yMax)) * .5f;
			baseStop = (baseStop + new Vector3(containerRect.xMax, containerRect.yMin)) * .5f;

			#if UNITY_EDITOR
			ExtensionsGizmos.DrawLineDirect(
				transform.TransformPoint(baseStart),
				transform.TransformPoint(baseStop),
				Color.cyan,
				Color.black);
			#endif

			var itemSize = item.sizeDelta;
			var itemCorrection = new Vector3(
				// to center
				itemSize.x * (.5f - item.pivot.x),
				// to top
				itemSize.y * item.pivot.y);
			localPos = (baseStop - baseStart) * normalized + baseStart - itemCorrection;
			var animDirectionNorm = (baseStop - baseStart).normalized;
			var startToCurrent = localPos - baseStart;
			var stopToCurrent = localPos - baseStop;
			var isAboveOutside =
				Vector3.Dot(startToCurrent.normalized, animDirectionNorm) < 0f &&
				startToCurrent.magnitude >= itemSize.y * .5f;
			var isBelowOutside =
				Vector3.Dot(stopToCurrent.normalized, animDirectionNorm) > 0f &&
				stopToCurrent.magnitude >= itemSize.y * .5f;
			return isAboveOutside || isBelowOutside;
		}

		private IEnumerator TaskWidgetsInitialize()
		{
			"content initialized".Log();

			var widgetRect = GetComponent<RectTransform>().rect;
			var itemRectSource = Proto.GetComponent<RectTransform>().rect;
			var itemScale = widgetRect.width * .5f / itemRectSource.width;
			var itemSizeActual = new Vector2(
				itemRectSource.width * itemScale,
				itemRectSource.height * itemScale);
			var stepNorm = itemSizeActual.y / widgetRect.height;
			var positionNorm = 0f;

			_windowLeft = Singleton<ServiceModel>.I.GetInitialWindow;
			_windowLeft.Rebuild(0, int.MaxValue, 2);

			_windowRight = Singleton<ServiceModel>.I.GetInitialWindow;
			_windowRight.Rebuild(1, int.MaxValue, 2);

			using var enumeratorLeft = _windowLeft.GetEnumeratorRangeAll();
			using var enumeratorRight = _windowRight.GetEnumeratorRangeAll();

			var isLeftComplete = false;
			var isRightComplete = false;

			var indexLeftLast = _windowLeft.IndexBegin;
			var indexRightLast = _windowRight.IndexBegin;

			// or separate for each row
			while(true)
			{
				if(!isLeftComplete && enumeratorLeft.MoveNext())
				{
					indexLeftLast = enumeratorLeft.Current;
					var compLeft = Instantiate(Proto.gameObject, transform).GetComponent<CompViewGalleryContentItem>();
					compLeft.RectTransform.sizeDelta = itemSizeActual;
					compLeft.PositionActualNorm = positionNorm;
					compLeft.SizeNorm = stepNorm;
					compLeft.IsOutside = GetPointLocalOnAnimLineLeft(
						positionNorm,
						compLeft.RectTransform,
						out var positionLocalLeft);
					compLeft.RectTransform.localPosition = positionLocalLeft;
					compLeft.name = $"{compLeft.name.CleanUpName()}_l.id:{indexLeftLast:00}";
					compLeft.ModelAttachByIndex(indexLeftLast);
					_itemsLeft.Add(compLeft);

					isLeftComplete = compLeft.IsOutside;
				}
				else
				{
					isLeftComplete = true;
				}

				if(!isRightComplete && enumeratorRight.MoveNext())
				{
					indexRightLast = enumeratorRight.Current;
					var compRight = Instantiate(Proto.gameObject, transform).GetComponent<CompViewGalleryContentItem>();
					compRight.RectTransform.sizeDelta = itemSizeActual;
					compRight.PositionActualNorm = positionNorm;
					compRight.SizeNorm = stepNorm;
					compRight.IsOutside = GetPointLocalOnAnimLineRight(
						positionNorm,
						compRight.RectTransform,
						out var positionLocalRight);
					compRight.RectTransform.localPosition = positionLocalRight;
					compRight.name = $"{compRight.name.CleanUpName()}_r.is:{indexRightLast:00}";
					compRight.ModelAttachByIndex(indexRightLast);
					_itemsRight.Add(compRight);

					isRightComplete = compRight.IsOutside;
				}
				else
				{
					isRightComplete = true;
				}

				if(isLeftComplete && isRightComplete)
				{
					break;
				}

				// separate for each row
				positionNorm += stepNorm;
			}

			_windowLeft.Rebuild(_windowLeft.IndexBegin, indexLeftLast, _windowLeft.Step);
			_windowRight.Rebuild(_windowRight.IndexBegin, indexRightLast, _windowRight.Step);

			$"initial window left : {_windowLeft}".Log();
			$"initial window right: {_windowRight}".Log();

			yield break;
		}

		private IEnumerator TaskWidgetsRetain()
		{
			var indexLeft = 0;
			var indexRight = 0;

			for(; indexLeft < _itemsLeft.Count && indexRight < _itemsRight.Count; indexLeft++, indexRight++)
			{
				if(indexLeft < _itemsLeft.Count)
				{
					Destroy(_itemsLeft[indexLeft].gameObject);
				}

				if(indexRight < _itemsRight.Count)
				{
					Destroy(_itemsRight[indexRight].gameObject);
				}
			}

			_itemsLeft.Clear();
			_itemsRight.Clear();

			yield return null;
		}

		private void UpdateIntent(List<CompViewGalleryContentItem> items, float step, GetPointLocalOnAnimLine getPoint)
		{
			for(var index = 0; index < items.Count; index++)
			{
				var comp = items[index];
				comp.PositionIntentNorm = comp.PositionActualNorm + step;
				comp.IsOutside = getPoint(
					comp.PositionIntentNorm,
					comp.RectTransform,
					out comp.PositionIntentLocal);
			}
		}

		private RangeWindow ShiftDownByIntent(List<CompViewGalleryContentItem> items, RangeWindow window, GetPointLocalOnAnimLine getPoint)
		{
			var size = items.Count;
			var count = 0;
			while(++count < size && items[0].IsOutside)
			{
				var comp = items[0];
				var compPrev = items[^1];

				var positionNorm =
					compPrev.PositionIntentNorm +
					compPrev.SizeNorm;
				comp.IsOutside = getPoint(
					positionNorm,
					comp.RectTransform,
					out var positionLocal);

				var current = window;
				if(current.Shift())
				{
					items.RemoveAt(0);
					items.Add(comp);

					comp.PositionIntentNorm = positionNorm;
					comp.PositionIntentLocal = positionLocal;

					window = current;

					$"shift performed: {window} over: {count} items".Log();
				}
				else
				{
					$"shift back performed: {window} over: {count} items".Log();

					break;
				}
			}

			if(count == 0)
			{
				throw new Exception("all the items are outside");
			}

			return window;
		}

		private RangeWindow ShiftUpByIntent(List<CompViewGalleryContentItem> items, RangeWindow window, GetPointLocalOnAnimLine getPoint)
		{
			var size = items.Count;
			var count = 0;
			while(++count < size && items[^1].IsOutside)
			{
				var comp = items[^1];
				var compNext = items[0];

				var positionNorm =
					compNext.PositionIntentNorm -
					compNext.SizeNorm;
				comp.IsOutside = getPoint(
					positionNorm,
					comp.RectTransform,
					out var positionLocal);

				var current = window;
				if(current.Shift(-1))
				{
					items.RemoveAt(items.Count - 1);
					items.Insert(0, comp);

					comp.PositionIntentNorm = positionNorm;
					comp.PositionIntentLocal = positionLocal;

					window = current;

					$"shift performed: {window} over: {count} items".Log();
				}
				else
				{
					$"shift back performed: {window} over: {count} items".Log();

					break;
				}
			}

			if(count == 0)
			{
				throw new Exception("all the items are outside");
			}

			return window;
		}

		private void SyncModels(List<CompViewGalleryContentItem> items, RangeWindow window)
		{
			"-- ".Log();

			if(window.Size != items.Count)
			{
				throw new Exception("unexpected: set counts are not equals");
			}

			using var enumeratorIndices = window.GetEnumeratorRangeAll();
			using var enumeratorItem = items.GetEnumerator();
			while(true)
			{
				var isIdModel = enumeratorIndices.MoveNext();
				var isItem = enumeratorItem.MoveNext();

				if(!isIdModel && !isItem)
				{
					break;
				}

				var item = isItem ? enumeratorItem.Current : null;

				if(item == null)
				{
					throw new Exception("unexpected: item not found");
				}

				if(!window.IsValidCollectionIndex(enumeratorIndices.Current))
				{
					if(item.Model == null)
					{
						$"trying to sync: '{null,3}' > '{null,3}': passing ..".Log();
					}
					else
					{
						$"trying to sync: '{null,3}' > '{item.Model.IdModel,3}': detaching ..".Log();

						item.ModelDetach();
					}
				}
				else
				{
					// log purpose
					var model = Singleton<ServiceModel>.I.GetModelById(enumeratorIndices.Current);

					if(item.Model == null)
					{
						$"trying to sync: '{model.IdModel,3}' > '{null,3}': attaching ..".Log();

						item.ModelAttachByIndex(enumeratorIndices.Current);
					}
					else if(item.Model.IdModel == model.IdModel)
					{
						$"trying to sync: '{model.IdModel,3}' > '{item.Model.IdModel,3}': passing ..".Log();
					}
					else
					{
						$"trying to sync: '{model.IdModel,3}' > '{item.Model.IdModel,3}': re-attaching ..".Log();

						item.ModelDetach();
						item.ModelAttachByIndex(enumeratorIndices.Current);
					}
				}
			}
		}

		private bool IsCorrectIntentByBoundDown(List<CompViewGalleryContentItem> items, RangeWindow windowSelf)
		{
			if(!windowSelf.IsIndexEndBound)
			{
				return false;
			}

			var last = items[^1];
			var correction = last.PositionBottomVisibleNorm;
			if(last.PositionIntentNorm > correction)
			{
				return false;
			}

			return true;
		}

		private bool IsCorrectIntentByBoundUp(List<CompViewGalleryContentItem> items, RangeWindow windowSelf)
		{
			if(!windowSelf.IsIndexBeginBound)
			{
				return false;
			}

			var first = items[0];
			var correction = 0f;
			if(first.PositionIntentNorm < correction)
			{
				return false;
			}

			return true;
		}

		private void CorrectIntentByBoundDown(List<CompViewGalleryContentItem> items, RangeWindow windowSelf, RangeWindow windowOpposite)
		{
			// assuming all plates are equal size
			// to count for different sizes, deliver opposite items set or append additional step for every column used
			var delta = Mathf.Clamp(windowOpposite.IndexEnd - windowSelf.IndexEnd, 0, int.MaxValue);
			var last = items[^1];
			var correction = last.PositionBottomVisibleNorm - last.SizeNorm * delta;
			for(var index = items.Count - 1; index > -1; index--)
			{
				items[index].PositionIntentNorm = correction;
				correction -= index > 0 ? items[index - 1].SizeNorm : 0f;
			}
		}

		private void CorrectIntentByBoundUp(List<CompViewGalleryContentItem> items)
		{
			var correction = 0f;
			for(var index = 0; index < items.Count; index++)
			{
				items[index].PositionIntentNorm = correction;
				correction += items[index].SizeNorm;
			}
		}

		private void ApplyIntent(List<CompViewGalleryContentItem> items, GetPointLocalOnAnimLine getPoint)
		{
			for(var index = 0; index < items.Count; index++)
			{
				var comp = items[index];
				comp.IsOutside = getPoint(
					comp.PositionIntentNorm,
					comp.RectTransform,
					out comp.PositionIntentLocal);
				comp.RectTransform.localPosition = comp.PositionIntentLocal;
				comp.PositionActualNorm = comp.PositionIntentNorm;
			}
		}

		public IEnumerator TaskScrollDown(float stepNormWithDirect)
		{
			UpdateIntent(_itemsLeft, stepNormWithDirect, GetPointLocalOnAnimLineLeft);
			UpdateIntent(_itemsRight, stepNormWithDirect, GetPointLocalOnAnimLineRight);

			var windowLeft = ShiftDownByIntent(_itemsLeft, _windowLeft, GetPointLocalOnAnimLineLeft);
			var windowRight = ShiftDownByIntent(_itemsRight, _windowRight, GetPointLocalOnAnimLineRight);

			if(windowLeft != _windowLeft)
			{
				SyncModels(_itemsLeft, windowLeft);
			}

			if(windowRight != _windowRight)
			{
				SyncModels(_itemsRight, windowRight);
			}

			var isCorrectByBoundLeft = IsCorrectIntentByBoundDown(_itemsLeft, windowLeft);
			var isCorrectByBoundRight = IsCorrectIntentByBoundDown(_itemsRight, windowRight);

			if(isCorrectByBoundLeft && isCorrectByBoundRight)
			{
				CorrectIntentByBoundDown(_itemsLeft, windowLeft, windowRight);
				CorrectIntentByBoundDown(_itemsRight, windowRight, windowLeft);
			}

			ApplyIntent(_itemsLeft, GetPointLocalOnAnimLineLeft);
			ApplyIntent(_itemsRight, GetPointLocalOnAnimLineRight);

			_windowLeft = windowLeft;
			_windowRight = windowRight;

			yield break;
		}

		public IEnumerator TaskScrollUp(float stepNormWithDirect)
		{
			UpdateIntent(_itemsLeft, stepNormWithDirect, GetPointLocalOnAnimLineLeft);
			UpdateIntent(_itemsRight, stepNormWithDirect, GetPointLocalOnAnimLineRight);

			var windowLeft = ShiftUpByIntent(_itemsLeft, _windowLeft, GetPointLocalOnAnimLineLeft);
			var windowRight = ShiftUpByIntent(_itemsRight, _windowRight, GetPointLocalOnAnimLineRight);

			if(windowLeft != _windowLeft)
			{
				SyncModels(_itemsLeft, windowLeft);
			}

			if(windowRight != _windowRight)
			{
				SyncModels(_itemsRight, windowRight);
			}

			var isCorrectByBoundLeft = IsCorrectIntentByBoundUp(_itemsLeft, windowLeft);
			var isCorrectByBoundRight = IsCorrectIntentByBoundUp(_itemsRight, windowRight);

			if(isCorrectByBoundLeft && isCorrectByBoundRight)
			{
				CorrectIntentByBoundUp(_itemsLeft);
				CorrectIntentByBoundUp(_itemsRight);
			}

			ApplyIntent(_itemsLeft, GetPointLocalOnAnimLineLeft);
			ApplyIntent(_itemsRight, GetPointLocalOnAnimLineRight);

			_windowLeft = windowLeft;
			_windowRight = windowRight;

			yield break;
		}

		// ReSharper disable once UnusedMember.Local
		private void Update()
		{
			if(!_tasks.ExecuteTasksSequentially())
			{
				Singleton<ServiceModel>.I.EventsModelUpdate.TryExecuteCommandQueue(this);
			}

			#if UNITY_EDITOR
			if(_itemsLeft.Count > 0)
			{
				var item = _itemsLeft[0].RectTransform;

				GetPointLocalOnAnimLineLeft(0f, item, out _);
				GetPointLocalOnAnimLineRight(0f, item, out _);
			}
			#endif
		}
	}
}
