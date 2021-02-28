using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public sealed class HealthHUD : ColorScrollHUD
{
	//A singleton for the health HUD so it can be accessed anywhere
	public static HealthHUD Instance { get; private set; }

	//The current health to be displayed
	int currentHealth = 100;

	//The maximum health to be displayed
	public int MaximumHealth { get; private set; } = 100;

	private void Awake()
	{
		//Set the singleton
		Instance = this;
	}

	/// <summary>
	/// Updates the health without applying animation effects
	/// </summary>
	/// <param name="newValue">The new health value</param>
	public void UpdateHealthRaw(int newValue)
	{
		currentHealth = newValue;
		SetColorProgressRaw(currentHealth,MaximumHealth);
	}

	/// <summary>
	/// Sets the maximum health the hud can display
	/// </summary>
	/// <param name="max">The maximum value</param>
	public void SetMaxHealth(int max)
	{
		MaximumHealth = max;
		SetColorProgress(currentHealth, MaximumHealth);
	}

	/// <summary>
	/// Updates the health
	/// </summary>
	/// <param name="newValue">The new health value</param>
	public void UpdateHealth(int newValue)
	{
		currentHealth = newValue;
		SetColorProgress(currentHealth, MaximumHealth);
	}
}
