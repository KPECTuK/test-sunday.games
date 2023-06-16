using Assets.Sun.Base;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Sun.Task01
{
	// ReSharper disable once UnusedType.Global
	public class CompApp : MonoBehaviour
	{
		// ReSharper disable once UnusedMember.Local
		private void Awake()
		{
			Application.targetFrameRate = 60;
			Screen.orientation = ScreenOrientation.Portrait;

			Singleton<ServiceModel>.I.Reset();
			Singleton<ServiceUI>.I.Reset();

			"initialized: app".Log();
		}

		// ReSharper disable once UnusedMember.Local
		private void OnDestroy()
		{
			Singleton<ServiceUI>.Dispose();
			Singleton<ServiceModel>.Dispose();

			"disposed".Log();
		}

		// ReSharper disable once UnusedMember.Local
		private void Update()
		{
			if(Input.GetKey(KeyCode.Escape))
			{
				#if UNITY_EDITOR
				EditorApplication.isPlaying = false;
				#else
				Application.Quit();
				#endif
			}

			Singleton<ServiceModel>.I.Update();
		}
	}
}
