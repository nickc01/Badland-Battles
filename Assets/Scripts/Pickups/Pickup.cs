using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
	/// <summary>
	/// If true, the pickup will be destroyed as soon as it's picked up
	/// </summary>
	public virtual bool DestroyOnPickup => true;


	/// <summary>
	/// Called when the character touches the pickup
	/// </summary>
	/// <param name="sourceCharacter">The character picking it up</param>
	public abstract void OnPickup(CharacterController sourceCharacter);
}