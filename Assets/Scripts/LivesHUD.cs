using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LivesHUD : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The text for displaying the lives")]
	TextMeshProUGUI livesText;
	[SerializeField]
	[Tooltip("The prefix text for the lives display")]
	string textPrefix = "Lives: ";

	/// <summary>
	/// Called whenver the amount of lives on the player is changed
	/// </summary>
	/// <param name="lives">The new lives value</param>
	public void UpdateLivesText(int lives)
	{
		//Update the lives text
		livesText.text = textPrefix + lives;
	}
}
