using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIconHUD : MonoBehaviour
{
	//The singleton for accessing the weapon icon hud anywhere
    public static WeaponIconHUD Instance { get; private set; }

    [SerializeField]
	[Tooltip("The text for displaying the weapon name")]
    TextMeshProUGUI weaponText;
    [SerializeField]
	[Tooltip("The image field for displaying an image of the weapon")]
    Image weaponImage;

	private void Awake()
	{
		Instance = this;
		weaponText.text = "";
		weaponImage.sprite = null;
		weaponImage.enabled = false;
	}

	/// <summary>
	/// Sets the image and text of the weapon display
	/// </summary>
	/// <param name="image">The image to display</param>
	/// <param name="text">The text to display</param>
	public void SetWeaponImage(Sprite image, string text)
	{
		//Set the weapon text
        weaponText.text = text;
		//Set the weapon image
        weaponImage.sprite = image;

		//Enable the image if one is set
		weaponImage.enabled = image != null;
	}
}
