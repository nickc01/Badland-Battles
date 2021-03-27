using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenModeSelector : MonoBehaviour
{
	//The dropdown component
    TMP_Dropdown modeDropdown;

	private void Awake()
	{
		//Get the dropdown component
		modeDropdown = GetComponentInChildren<TMP_Dropdown>();

		//Create a list of the possible modes
		var modeOptions = new List<string>();
		//Get the names of the possible enum values
		foreach (var modeName in Enum.GetNames(typeof(FullScreenMode)))
		{
			//Add the name to the list of options
			modeOptions.Add(modeName);
		}

		//Clear the existing options
		modeDropdown.ClearOptions();
		//Add all the options to the dropdown
		modeDropdown.AddOptions(modeOptions);
		//Set the current value to the current screen mode
		modeDropdown.value = (int)Screen.fullScreenMode;
	}

	//Called when the dropdown value is changed
	public void OnScreenModeChange(int index)
	{
#if !UNITY_EDITOR
		Screen.fullScreenMode = (FullScreenMode)index;
#else
		Debug.Log($"Screen Mode Changed to {(FullScreenMode)index}");
#endif
	}
}
