using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The delay before the countdown starts")]
	float startDelay = 1f;
	[SerializeField]
	[Tooltip("The number the countdown will start counting down from")]
	int startingCountdownNumber = 3;
	[SerializeField]
	[Tooltip("How long the countdown will take")]
	float countdownTime = 1.5f;
	[SerializeField]
	[Tooltip("The final text that is displayed when the countdown is done")]
	string finalText = "Go!";
	[SerializeField]
	[Tooltip("The text object for displaying the countdown")]
	TextMeshProUGUI textObject;
	[SerializeField]
	[Tooltip("After the countdown has occured, this is how long it will stay on screen before it dissapears")]
	float endingDelay = 1f;

	private void Awake()
	{
		StartCoroutine(CountdownRoutine());
	}

	IEnumerator CountdownRoutine()
	{
		//Wait the starting delay
		yield return new WaitForSecondsRealtime(startDelay);

		//Get the amount of time each number is displayed
		float timePerNumber = countdownTime / startingCountdownNumber;

		//Loop over each number in the countdown
		for (int i = startingCountdownNumber; i > 0; i--)
		{
			//Display the number
			textObject.text = i.ToString();
			//Wait the time per number delay
			yield return new WaitForSecondsRealtime(timePerNumber);
		}

		//Set the final text
		textObject.text = finalText;
		//Start the game
		GameManager.Instance.StartGame();

		//Wait the ending delay
		yield return new WaitForSecondsRealtime(endingDelay);

		//Hide the text object
		textObject.enabled = false;
	}
}
