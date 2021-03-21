using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class GraphicsSettingsBox : MonoBehaviour
{
	//The list of all the possible quality settings
	string[] qualityNames;

	//The dropdown for containing the list of options
	TMP_Dropdown dropdown;

	private void Awake()
	{
		//Get the dropdown component
		dropdown = GetComponent<TMP_Dropdown>();
		//Get the list of quality names
		qualityNames = QualitySettings.names;

		//Clear the dropdown of any of it's existing options
		dropdown.options.Clear();

		//Add all the quality names to the dropdown
		dropdown.AddOptions(qualityNames.ToList());

		//Set the dropdown's value to the currently configured quality setting
		dropdown.value = QualitySettings.GetQualityLevel();
	}

	/// <summary>
	/// Called when the value on the dropdown changes
	/// </summary>
	/// <param name="index">The newly selected dropdown value</param>
	public void SetGraphicsSettings(int index)
	{
		//Set the quality level to the selected index
		QualitySettings.SetQualityLevel(index,true);
	}
}
