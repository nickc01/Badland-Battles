using UnityEngine;

public sealed class AmmoHUD : ColorScrollHUD
{
	public static AmmoHUD Instance { get; private set; }

	int currentAmmo = 15;

	public int MaximumAmmo { get; private set; } = 15;

	private void Awake()
	{
		Instance = this;
	}

	/// <summary>
	/// Updates the ammo without applying animation effects
	/// </summary>
	/// <param name="newValue">The new ammo value</param>
	public void UpdateAmmoRaw(int newValue)
	{
		currentAmmo = newValue;
		SetColorProgressRaw(currentAmmo,MaximumAmmo);
	}

	/// <summary>
	/// Sets the maximum ammo the hud can display
	/// </summary>
	/// <param name="max">The maximum value</param>
	public void SetMaxAmmo(int max)
	{
		MaximumAmmo = max;
		SetColorProgress(currentAmmo, MaximumAmmo);
	}

	/// <summary>
	/// Updates the ammo
	/// </summary>
	/// <param name="newValue">The new ammo value</param>
	public void UpdateAmmo(int newValue)
	{
		currentAmmo = newValue;
		SetColorProgress(currentAmmo, MaximumAmmo);
	}
}
