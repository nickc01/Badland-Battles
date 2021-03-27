using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionSelector : MonoBehaviour
{
	//The dropdown component
	TMP_Dropdown resolutionDropdown;

	//The list of all supported resolutions
	Resolution[] resolutions;
	//A list of all the options for the dropdown
	List<string> resolutionOptions;

	private void Awake()
	{
		//Get the dropdown component
		resolutionDropdown = GetComponentInChildren<TMP_Dropdown>();
		//Get a list of all supported resolutions
		resolutions = Screen.resolutions;

		//Get the index of the currently set resolution
		var resolutionIndex = Array.IndexOf(resolutions, Screen.currentResolution);

		//Create the list of dropdown options
		resolutionOptions = new List<string>();

		//Loop over all the supported resolutions
		foreach (var res in resolutions)
		{
			//Add each resolution to the list of options
			resolutionOptions.Add(res.ToString());
		}

		//Clear the existing options
		resolutionDropdown.ClearOptions();
		//Add the options to the dropdown component
		resolutionDropdown.AddOptions(resolutionOptions);
		//Set the dropdown's value to the currently set resolution
		resolutionDropdown.value = resolutionIndex;
	}

	//Called when the dropdown value is changed
	public void OnResolutionChange(int index)
	{
#if !UNITY_EDITOR
		var resolution = resolutions[index];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRate);
#else
		Debug.Log($"Resolution Changed to {resolutions[index]}");
#endif
	}
}
