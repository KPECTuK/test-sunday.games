using System.Collections.Generic;
using Assets.Sun.Base;
using UnityEngine;

namespace Assets.Sun.Task01
{
	public class ServiceUI : IService
	{
		//? what is it for
		public const string SCREEN_LOADER_S = "loader";

		public const string SCREEN_MENU_S = "menu";
		public const string SCREEN_GALLERY_S = "gallery";
		public const string SCREEN_PREVIEW_S = "preview";

		public readonly Queue<ICommand<CompScreens>> EventsViewScreens = new();
		public readonly Queue<ICommand<CompViewPreview>> EventsViewPreview = new();

		public Texture2D TextureInProgress { get; private set; }
		public Texture2D TextureError { get; private set; }

		public Resolution ResolutionPort { get; private set; }
		public Resolution ResolutionLand { get; private set; }

		public void Reset()
		{
			EventsViewScreens.Enqueue(
				new CmdViewScreenChange
				{
					// set this for screen debugging
					NameScreen = SCREEN_MENU_S
				});

			TextureInProgress = Resources.Load<Texture2D>("img.stub.inprogress");
			TextureError = Resources.Load<Texture2D>("img.stub.error");

			ResolutionPort = Screen.currentResolution;
			ResolutionLand = new Resolution
			{
				height = Screen.currentResolution.width,
				width = Screen.currentResolution.height,
				refreshRate = Screen.currentResolution.refreshRate,
			};

			"initialized: ui".Log();
		}

		public void Dispose() { }
	}

	public class CmdViewScreenChange : ICommandBreak<CompScreens>
	{
		public string NameScreen;

		public bool Assert(CompScreens context)
		{
			//? is the same screen constraint
			var active = context.GetActiveScreen();
			return active == null || !active.name.Contains(NameScreen);
		}

		public void Execute(CompScreens context)
		{
			context.Scheduler.Schedule(NameScreen, context.TaskScreenChange);
		}
	}

	public class CmdViewPreviewUpdate : ICommandBreak<CompViewPreview>
	{
		public int IdModel;

		public bool Assert(CompViewPreview context)
		{
			var model = Singleton<ServiceModel>.I.GetModelById(IdModel);
			return model != null;
		}

		public void Execute(CompViewPreview context)
		{
			context.Scheduler.Schedule(IdModel, context.TaskViewUpdate);
		}
	}

	public class CmdViewPreviewRotate : ICommandBreak<CompViewPreview>
	{
		public int IndexVector;

		public bool Assert(CompViewPreview context)
		{
			return true;
		}

		public void Execute(CompViewPreview context)
		{
			context.Scheduler.Schedule(IndexVector, context.TaskViewRotate);
		}
	}
}
