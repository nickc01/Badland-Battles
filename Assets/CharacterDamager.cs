using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDamager : MonoBehaviour, ICharacterDamager
{
	[SerializeField]
	protected int Damage = 100;

	//When a character collides with the object
	public virtual void OnHit(Character character)
	{
		//Damage any character that hits this object
		character.CharacterHealth.Damage(Damage);
	}
}
