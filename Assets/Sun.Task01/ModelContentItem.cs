using System;
using Assets.Sun.Base;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;

namespace Assets.Sun.Task01
{
	public class ModelContentItem : IDisposable
	{
		public int IdModel;
		public Uri UriResource;
		public Texture2D Texture;
		public UnityWebRequest Request;
		public DateTime RequestSent;
		public bool IsRequestSuccessful;
		public bool IsSharedTexture;

		public void LoadTextureFromRequest()
		{
			"set from request".Log();

			Texture = new Texture2D(100, 100, GraphicsFormat.B8G8R8A8_SRGB, 0, TextureCreationFlags.None);
			Texture.LoadImage(Request.downloadHandler.data);
			Texture.Apply(false);
			IsSharedTexture = false;
		}

		public void LoadTextureError()
		{
			"set from shared: error".Log();

			Texture = Singleton<ServiceUI>.I.TextureError;
			IsSharedTexture = true;
		}

		public void LoadTextureInProgress()
		{
			"set from shared: in progress".Log();

			Texture = Singleton<ServiceUI>.I.TextureInProgress;
			IsSharedTexture = true;
		}

		/// <summary> just a sign to be released by singleton, not a pattern due Unity impl </summary>
		public void Dispose()
		{
			if(!ReferenceEquals(null, Texture) && !IsSharedTexture)
			{
				UnityEngine.Object.DestroyImmediate(Texture);
			}

			if(!ReferenceEquals(null, Request))
			{
				Request.Abort();
				Request = null;
			}
		}
	}
}
