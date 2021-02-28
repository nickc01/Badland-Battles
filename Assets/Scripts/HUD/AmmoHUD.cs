using UnityEngine;

public sealed class AmmoHUD : ColorScrollHUD
{
	//The singleton for accessing the HUD anywhere
	public static AmmoHUD Instance { get; private set; }

	int currentAmmo = 15;

	//The maximum amount of ammo the character can hold
	public int MaximumAmmo { get; private set; } = 15;

	private void Awake()
	{
		//Set the singleton
		Instance = this;
	}

	/// <summary>
	/// Updates the ammo without applying animation effects
	/// </summary>
	/// <param name="newValue">The new ammo value</param>
	public void UpdateAmmoRaw(int newValue)
	{
		currentAmmo = newValue;
		//Set the color progress bar without doing any animations
		SetColorProgressRaw(currentAmmo,MaximumAmmo);
	}

	/// <summary>
	/// Sets the maximum ammo the hud can display
	/// </summary>
	/// <param name="max">The maximum value</param>
	public void SetMaxAmmo(int max)
	{
		MaximumAmmo = max;
		//Update the color progress bar
		SetColorProgress(currentAmmo, MaximumAmmo);
	}

	/// <summary>
	/// Updates the ammo
	/// </summary>
	/// <param name="newValue">The new ammo value</param>
	public void UpdateAmmo(int newValue)
	{
		currentAmmo = newValue;
		//Update the color progress bar
		SetColorProgress(currentAmmo, MaximumAmmo);
	}
}
