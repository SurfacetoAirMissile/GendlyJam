using System;
using UnityEngine.SceneManagement;
using CGDP2.Utilities;

namespace CGDP2.UtilityComponents
{
	public class IntersceneMessenger
	{
		public static void LoadScene(string sceneName, Action onSceneLoaded)
		{
			InstantiateMessenger(onSceneLoaded);
			SceneManager.LoadScene(sceneName);
		}

		public static void LoadScene(int sceneBuildIndex, Action onSceneLoaded)
		{
			InstantiateMessenger(onSceneLoaded);
			SceneManager.LoadScene(sceneBuildIndex);
		}

		private static void InstantiateMessenger(Action onAwakeInNewScene)
		{
			var messenger = new IntersceneMessenger(onAwakeInNewScene);
			SceneManager.activeSceneChanged += messenger.SceneManager_activeSceneChanged;
		}

		private IntersceneMessenger(Action onSceneLoaded)
		{
			this.onSceneLoaded = onSceneLoaded;
		}

		private Action onSceneLoaded;

		private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
		{
			SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
			InvokeSceneLoaded();
		}

		private void InvokeSceneLoaded()
		{
			if (onSceneLoaded is null)
				return;

			CgdUtils.TryElseLog(onSceneLoaded);
			onSceneLoaded = null;
		}
	}
}
