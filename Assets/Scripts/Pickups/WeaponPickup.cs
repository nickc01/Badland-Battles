using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents a pickup for a weapon
public sealed class WeaponPickup : Pickup
{
	[SerializeField]
	[Tooltip("The weapon that will be given to the player. This weapon will be instantiated into the player's hands")]
	Weapon weaponPrefab;

	/// <summary>
	/// The weapon that will be given to the player. This weapon will be instantiated into the player's hands
	/// </summary>
	public Weapon WeaponPrefab => weaponPrefab;

	public override void OnPickup(PlayerController sourceCharacter)
	{
		//On pickup, tell the player to equip the weapon
		sourceCharacter.EquipWeapon(WeaponPrefab);
	}
}
