using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public sealed class HealthEvent : UnityEvent<int>
{
	
}


public class Health : MonoBehaviour
{
	[SerializeField]
	int initialHealth = 100;

	[SerializeField]
	int maxHealth = 100;

	int health;

	private void Awake()
	{
		if (initialHealth > maxHealth)
		{
			initialHealth = maxHealth;
		}
		health = initialHealth;
		onStart.Invoke(health);
	}



	public float InitialHealth => initialHealth;
	public float MaxHealth => maxHealth;
	public float CurrentHealth => health;

	/// <summary>
	/// Heals the health component by a set amount
	/// </summary>
	/// <param name="amount">The amount to heal</param>
	public void Heal(int amount)
	{
		if (health < maxHealth)
		{
			health += amount;
			if (health > maxHealth)
			{
				health = maxHealth;
			}
			onHeal.Invoke(health);
		}
	}

	/// <summary>
	/// Damages the health component by a set amount
	/// </summary>
	/// <param name="amount">The amount to damage</param>
	public void Damage(int amount)
	{
		if (health > 0)
		{
			health -= amount;
			if (health < 0)
			{
				health = 0;
			}
			onDamage.Invoke(health);
			if (health == 0)
			{
				onDeath.Invoke();
			}
		}
	}


	#region HEALTH CALLBACKS

	[SerializeField]
	[Tooltip("Called when the health component starts")]
	HealthEvent onStart;
	/// <summary>
	/// Called when the health component is initialized. The first parameter represents the starting health
	/// </summary>
	public event UnityAction<int> OnStart
	{
		add => onStart.AddListener(value);
		remove => onStart.RemoveListener(value);
	}


	[SerializeField]
	[Tooltip("Called when the health component dies")]
	UnityEvent onDeath;
	/// <summary>
	/// Called when the health component dies
	/// </summary>
	public event UnityAction OnDeath
	{
		add => onDeath.AddListener(value);
		remove => onDeath.RemoveListener(value);
	}


	[SerializeField]
	[Tooltip("Called when the health component heals")]
	HealthEvent onHeal;
	/// <summary>
	/// Called when the health component heals. The first parameter represents the new health
	/// </summary>
	public event UnityAction<int> OnHeal
	{
		add => onHeal.AddListener(value);
		remove => onHeal.RemoveListener(value);
	}


	[SerializeField]
	[Tooltip("Called when the health component takes damage")]
	HealthEvent onDamage;
	/// <summary>
	/// Called when the health component is damaged. The first parameter represents the new health
	/// </summary>
	public event UnityAction<int> OnDamage
	{
		add => onDamage.AddListener(value);
		remove => onDamage.RemoveListener(value);
	}

	#endregion
}
