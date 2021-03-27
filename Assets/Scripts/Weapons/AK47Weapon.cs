using System.Collections.Generic;
using UnityEngine;

//The class representing the AK-47 weapon
public sealed class AK47Weapon : Weapon
{
	public override void Shoot(Vector3 muzzle, Vector3 target)
	{
		//Play a random pistol sound
		GameAudioSource.PlayAudioOnce(AudioDatabase.Instance.PistolSounds.GetRandom(), muzzle);
		base.Shoot(muzzle, target);
	}
}
