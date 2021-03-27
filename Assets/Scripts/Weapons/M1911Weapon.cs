using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The class for the M1911 Weapon
public sealed class M1911Weapon : Weapon
{
	public override void Shoot(Vector3 muzzle, Vector3 target)
	{
		//Play a random pistol sound
		GameAudioSource.PlayAudioOnce(AudioDatabase.Instance.PistolSounds.GetRandom(), muzzle);
		base.Shoot(muzzle, target);
	}
}
