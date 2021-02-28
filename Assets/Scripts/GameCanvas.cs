using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents the main game canvas
public class GameCanvas : MonoBehaviour
{
	//A singleton for accessing the game canvas anywhere
	public static GameCanvas Instance { get; private set; }

	//A public accessor for the game canvas
	public Canvas Canvas { get; private set; }

	private void Awake()
	{
		//Set the singleton
		Instance = this;
		//Set the canvas
		Canvas = GetComponent<Canvas>();
	}
}
