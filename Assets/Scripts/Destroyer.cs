using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Destroys an object when the object's health reaches zero
[RequireComponent(typeof(Health))]
public class Destroyer : MonoBehaviour
{
	public void DestroyObject()
	{
		//Destroy the object
		Destroy(gameObject);
	}
}
