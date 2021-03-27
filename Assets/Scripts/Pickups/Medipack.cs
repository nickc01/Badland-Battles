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

	public override void OnPickup(PlayerController sourceCharacter)
	{
		//On pickup, heal the character
		sourceCharacter.GetComponent<Health>().Heal(healAmount);

		GameAudioSource.PlayAudioOnce(AudioDatabase.Instance.ReloadSound, transform.position);
	}
}

