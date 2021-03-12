using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamager : MonoBehaviour, ICharacterDamager
{
	[SerializeField]
	[Tooltip("How much damage the character will take when colliding with this object")]
	int damage = 5;

	/// <summary>
	/// The amount of damage it will do to the player
	/// </summary>
	public int Damage => damage;

	//When the player hits this object
	void ICharacterDamager.OnHit(Character character)
	{
		//If the character is a player
		if (character is PlayerController)
		{
			//Damage the player
			character.CharacterHealth.Damage(damage);
		}
	}
}
