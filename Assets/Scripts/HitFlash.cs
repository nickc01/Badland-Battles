using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
	//The particle system on the hit flash
	ParticleSystem particles;

	private void Awake()
	{
		//Get the particle system
		particles = GetComponentInChildren<ParticleSystem>();
		//Emit 1 particle
		particles.Emit(1);
		//Destroy the object after the particle is done
		Destroy(gameObject, particles.main.startLifetime.constant);
	}

	//Disables the light source after a set amount of time
	public void DisableLightAfter(float time)
	{
		//Start the coroutine
		StartCoroutine(LightDisableRoutine(time));
	}

	//Disables the light source after a set amount of time
	IEnumerator LightDisableRoutine(float time)
	{
		//Wait a specified amount of time
		yield return new WaitForSeconds(time);

		//Disable the light source
		GetComponent<Light>().enabled = false;
	}
}
