using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class WeaponPickup : Pickup
{
	[SerializeField]
	[Tooltip("The weapon that will be given to the player. This weapon will be instantiated into the player's hands")]
	Weapon weaponPrefab;

	public Weapon WeaponPrefab => weaponPrefab;

	public override void OnPickup(CharacterController sourceCharacter)
	{
		throw new NotImplementedException();

	}
}
