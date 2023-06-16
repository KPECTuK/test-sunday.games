using System;
using Assets.Sun.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Sun.Task01
{
	[RequireComponent(typeof(Button))]
	// ReSharper disable once UnusedType.Global
	public class CompViewGalleryContentItem : MonoBehaviour
	{
		public TextMeshProUGUI Text;
		public RawImage Image;

		/// <summary> driven buffer </summary>
		[NonSerialized]
		public Vector3 PositionIntentLocal;
		/// <summary> driven buffer </summary>
		[NonSerialized]
		public bool IsOutside;
		/// <summary> driver </summary>
		[NonSerialized]
		public float PositionIntentNorm;
		/// <summary> data: previous step </summary>
		[NonSerialized]
		public float PositionActualNorm;
		/// <summary> data: initializer (? or previous step could be used also) </summary>
		[NonSerialized]
		public float SizeNorm;

		public float PositionBottomVisibleNorm => 1f - SizeNorm;

		[NonSerialized] public ModelContentItem Model;

		//? caching
		public RectTransform RectTransform => GetComponent<RectTransform>();

		public void ModelAttach(int idModel)
		{
			Model = Singleton<ServiceModel>.I.GetModelById(idModel);

			if(!ReferenceEquals(null, Model.Texture))
			{
				Image.texture = Model.Texture;
			}
			else
			{
				Singleton<ServiceModel>.I.RunRequest(Model.IdModel);
			}
		}

		public void ModelAttachByIndex(int index)
		{
			Model = Singleton<ServiceModel>.I.GetModelByIndex(index);

			if(!ReferenceEquals(null, Model.Texture))
			{
				Image.texture = Model.Texture;
			}
			else
			{
				Singleton<ServiceModel>.I.RunRequest(Model.IdModel);
			}
		}

		public void ModelDetach()
		{
			Model = null;
			Image.texture = null;
		}

		public void OnModelUpdated()
		{
			Image.texture = Model.Texture;
		}

		private void OnClick()
		{
			Singleton<ServiceUI>.I.EventsViewPreview.Enqueue(new CmdViewPreviewInitialize { IdModel = Model.IdModel, });
			Singleton<ServiceUI>.I.EventsViewScreens.Enqueue(new CmdViewScreenChange { NameScreen = ServiceUI.SCREEN_PREVIEW_S, });
		}

		// ReSharper disable once UnusedMember.Local
		private void Awake()
		{
			var button = GetComponent<Button>();
			button.onClick.AddListener(OnClick);
		}

		// ReSharper disable once UnusedMember.Global
		public void LateUpdate()
		{
			#if DEBUG
			// debug
			Text.text = $"pos: {PositionActualNorm,6:F3} id: {Model?.IdModel.ToString() ?? "unset"}";
			Text.color = IsOutside ? Color.red : Color.white;
			#endif
		}
	}
}
