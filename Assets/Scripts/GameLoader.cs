using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameLoader
{
	/// <summary>
	/// The delay between loading scenes
	/// </summary>
	public const float sceneLoadDelay = 0.50f;
	/// <summary>
	/// The scene that is loaded to show the loading screen
	/// </summary>
	public const string loadingSceneName = "Loading Scene";

	/// <summary>
	/// Whether a game is being loaded or not
	/// </summary>
	public static bool Loading { get; private set; }

	/// <summary>
	/// Called when the specified scene is loaded
	/// </summary>
	public static event Action<Scene> OnGameLoad;

	/// <summary>
	/// Loads the specified scene
	/// </summary>
	/// <param name="sceneName">The name of the scene to load</param>
	public static void LoadScene(string sceneName)
	{
		//If it's already loading something, throw an exception
		if (Loading)
		{
			throw new Exception("A game is already being loaded");
		}
		Loading = true;
		OnGameLoad = null;
		//Start a new routine for loading the scene
		UnboundRoutine.StartUnboundRoutine(LoadGameRoutine(sceneName));
	}


	static IEnumerator LoadGameRoutine(string sceneName)
	{
		//Load the loading scene
		yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);

		//Wait the specified delay
		yield return new WaitForSecondsRealtime(sceneLoadDelay);

		//Load the specified scene
		yield return SceneManager.LoadSceneAsync(sceneName);

		//Wait a specified delay
		yield return new WaitForSecondsRealtime(sceneLoadDelay);

		Loading = false;

		//Call the on game load event
		OnGameLoad?.Invoke(SceneManager.GetSceneByName(sceneName));
	}
}

