using System;
using System.Collections.Generic;
using Assets.Sun.Base;
using UnityEngine.Networking;

namespace Assets.Sun.Task01
{
	public class ServiceModel : IService
	{
		public const int ITEMS_TOTAL = 17;

		public readonly Queue<ICommand<CompViewGalleryContent>> EventsModelUpdate = new();

		private readonly List<ModelContentItem> _models = new();

		public RangeWindow GetInitialWindow => RangeWindow.CreateFrom(_models);

		public void Reset()
		{
			// assuming well known addresses
			for(var index = 0; index < ITEMS_TOTAL; index++)
			{
				var model = new ModelContentItem
				{
					IdModel = index,
					UriResource = new Uri($"http://data.ikppbb.com/test-task-unity-data/pics/{index + 1}.jpg"),
				};
				_models.Add(model);
			}
		}

		public ModelContentItem GetModelById(int idModel)
		{
			var index = _models.FindIndex(_ => _.IdModel == idModel);
			if(index == -1)
			{
				throw new Exception($"model set does not found for id: {idModel}");
			}

			return _models[index];
		}

		public ModelContentItem GetModelByIndex(int index)
		{
			if(!index.IsIndexValid(_models.Count))
			{
				throw new Exception($"index is out of range for models set of element: {_models.Count}");
			}

			return _models[index];
		}

		public void RunRequest(int idModel)
		{
			#if REQUESTS_ENABLE
			var model = GetModelById(idModel);

			if(model.IsRequestSuccessful)
			{
				return;
			}

			if(!ReferenceEquals(null, model.Request))
			{
				return;
			}

			if(DateTime.UtcNow - model.RequestSent < TimeSpan.FromSeconds(3d))
			{
				return;
			}

			model.Request = UnityWebRequest.Get(model.UriResource);
			model.Request.SendWebRequest();

			EventsModelUpdate.Enqueue(new CmdModelRequestUpdated { IdModel = model.IdModel });
			#endif
		}

		public void Update()
		{
			for(var index = 0; index < _models.Count; index++)
			{
				var model = _models[index];
				if(ReferenceEquals(null, model.Request))
				{
					continue;
				}

				if(!model.Request.isDone)
				{
					continue;
				}

				EventsModelUpdate.Enqueue(new CmdModelRequestUpdated { IdModel = model.IdModel });
			}
		}

		/// <summary> just a sign to be released by singleton, not a pattern due Unity impl </summary>
		public void Dispose()
		{
			for(var index = 0; index < _models.Count; index++)
			{
				_models[index].Dispose();
			}
		}
	}

	public class CmdModelRequestUpdated : ICommandBreak<CompViewGalleryContent>
	{
		public int IdModel;

		public bool Assert(CompViewGalleryContent context)
		{
			return true;
		}

		public void Execute(CompViewGalleryContent context)
		{
			var model = Singleton<ServiceModel>.I.GetModelById(IdModel);

			if(!ReferenceEquals(null, model.Request))
			{
				if(!model.Request.isDone)
				{
					model.LoadTextureInProgress();
				}
				else if(model.Request.result == UnityWebRequest.Result.Success)
				{
					$"request <color=green>success</color>: {model.Request.error}".Log();

					model.LoadTextureFromRequest();
					model.Request = null;
				}
				else
				{
					$"request <color=red>error</color>: {model.Request.error} to: {model.Request.url}".LogWarning();

					model.LoadTextureError();
					model.Request = null;
				}
			}

			context.OnModelUpdated(IdModel);
		}
	}
}
