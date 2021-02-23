using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
	public virtual bool DestroyOnPickup => true;



	public abstract void OnPickup(CharacterController sourceCharacter);
}