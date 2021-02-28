using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathUI : MonoBehaviour
{
	//The text component
	TextMeshProUGUI text;

	private void Awake()
	{
		//Get the text component
		text = GetComponent<TextMeshProUGUI>();
	}

	//Shows the death text
	public void ShowDeathText()
	{
		//Enable the text
		text.enabled = true;
	}
}
