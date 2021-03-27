using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
	private void Awake()
	{
		//When the muzzle flash starts, emit one particle
		GetComponentInChildren<ParticleSystem>().Emit(1);
	}
}
