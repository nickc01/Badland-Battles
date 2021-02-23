using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public sealed class HealthHUD : ColorScrollHUD
{
	public static HealthHUD Instance { get; private set; }

	[SerializeField]
	int maximumHealth = 100;

	int currentHealth = 100;

	public int MaximumHealth => maximumHealth;

	private void Awake()
	{
		Instance = this;
	}

	/// <summary>
	/// Updates the health without applying animation effects
	/// </summary>
	/// <param name="newValue">The new health value</param>
	public void UpdateHealthRaw(int newValue)
	{
		SetMaxHealth(newValue);
		SetColorProgressRaw(currentHealth,maximumHealth);
	}

	/// <summary>
	/// Sets the maximum health the hud can display
	/// </summary>
	/// <param name="max">The maximum value</param>
	public void SetMaxHealth(int max)
	{
		maximumHealth = max;
		SetColorProgress(currentHealth, maximumHealth);
	}

	/// <summary>
	/// Updates the health
	/// </summary>
	/// <param name="newValue">The new health value</param>
	public void UpdateHealth(int newValue)
	{
		currentHealth = newValue;
		SetColorProgress(currentHealth, maximumHealth);
	}
}
