using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarmCharacter : MonoBehaviour
{
	[SerializeField]
	[Tooltip("How much damage the character will take when colliding with this object")]
	int damage = 5;

	public int Damage => damage;
}
