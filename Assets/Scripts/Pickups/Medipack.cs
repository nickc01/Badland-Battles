using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class Medipack : Pickup
{
	[SerializeField]
	int healAmount = 40;

	public override void OnPickup(CharacterController sourceCharacter)
	{
		//On pickup, heal the character
		sourceCharacter.GetComponent<Health>().Heal(healAmount);
	}
}

