using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
	/// <summary>
	/// The singleton for accessing the game manager anywhere
	/// </summary>
	public static GameManager Instance { get; private set; }

	[Header("Weapon System")]
	[SerializeField]
	[Tooltip("The prefab that is instantiated at the nuzzle of the gun to replicate a gun flash")]
	GameObject gunFirePrefab;
	public GameObject GunFirePrefab => gunFirePrefab;
	[SerializeField]
	[Tooltip("The prefab that is instantiated at the target that replicates a bullet hit")]
	GameObject gunHitPrefab;
	public GameObject GunHitPrefab => gunHitPrefab;
	[SerializeField]
	[Tooltip("The prefab that is used to represent the path of the bullet from the source to the target. This can be used to create a ray that shows where the bullet traveled")]
	GameObject gunRayPrefab;
	public GameObject GunRayPrefab => gunRayPrefab;

	[Space]
	[Header("Health System")]
	[SerializeField]
	[Tooltip("The health marker prefab")]
	HealthMarker markerPrefab;
	public HealthMarker MarkerPrefab => markerPrefab;

	[Space]
	[Header("Events")]
	[SerializeField]
	[Tooltip("Called when the game starts")]
	UnityEvent onGameStart;
	[SerializeField]
	[Tooltip("Called when the game is paused")]
	UnityEvent onGamePause;
	[SerializeField]
	[Tooltip("Called when the game is unpaused")]
	UnityEvent onGameUnPause;
	[SerializeField]
	[Tooltip("Called when the game is over")]
	UnityEvent onGameOver;

	[Space]
	[Header("Scenes")]
	[SerializeField]
	string mainMenuScene;

	/// <summary>
	/// Whether the game is paused
	/// </summary>
	public bool GamePaused { get; private set; } = true;
	/// <summary>
	/// Whether the game has started
	/// </summary>
	public bool GameStarted { get; private set; } = false;


	private void Awake()
	{
		//Set the singleton
		Instance = this;
		//When the game scene loads, pause the game
		Time.timeScale = 0f;
	}

	private void Update()
	{
		//If the game has started and the space bar is pressed
		if (GameStarted && Input.GetKeyDown(KeyCode.Escape))
		{
			//if the game is not paused
			if (!GamePaused)
			{
				//Pause the game
				PauseGame();
			}
		}
	}

	/// <summary>
	/// Starts the game
	/// </summary>
	public void StartGame()
	{
		//The game has started
		GameStarted = true;
		//The game is no longer paused
		GamePaused = false;
		//Set the time to normal time
		Time.timeScale = 1f;
		//Call the on game start event
		if (onGameStart != null)
		{
			onGameStart.Invoke();
		}
	}

	/// <summary>
	/// Pauses the game
	/// </summary>
	public void PauseGame()
	{
		//The game is paused
		GamePaused = true;
		//Stop time
		Time.timeScale = 0f;
		//Call the on game pause event
		if (onGamePause != null)
		{
			onGamePause.Invoke();
		}
	}

	/// <summary>
	/// Unpauses the game
	/// </summary>
	public void UnPauseGame()
	{
		//Unpause the game
		GamePaused = false;
		//Set the time to normal
		Time.timeScale = 1f;
		//Call the on game unpause event
		if (onGameUnPause != null)
		{
			onGameUnPause.Invoke();
		}
	}

	/// <summary>
	/// Ends the game
	/// </summary>
	public void EndGame()
	{
		//The game is paused
		GamePaused = true;
		//The game is no longer started
		GameStarted = false;
		//Call the on game over event
		if (onGameOver != null)
		{
			onGameOver.Invoke();
		}
	}

	/// <summary>
	/// Returns back to the main menu
	/// </summary>
	public void BackToMainMenu()
	{
		//The game is paused
		GamePaused = true;
		//The game is no longer started
		GameStarted = false;
		//Set the time to normal
		Time.timeScale = 1f;
		//Load the main menu scene
		GameLoader.LoadScene(mainMenuScene);
	}
}
