using System.Collections.Generic;
using UnityEngine;

public sealed class DoublePistolWeapon : Weapon
{
	Transform muzzle1; //The muzzle of the first gun
	Transform muzzle2; //The muzzle of the second gun

	//Determines if we are shooting from the first gun or the second gun
	bool shootingFirstMuzzle = true;

	public override void OnEquip(WeaponUser character)
	{
		muzzle1 = transform.Find("Muzzle");
		muzzle2 = transform.Find("Muzzle 2");
	}


	public override void Shoot(Vector3 muzzle, Vector3 target)
	{
		//Alternate between shooting from gun 1 and gun 2
		if (shootingFirstMuzzle)
		{
			//Shoot bullet from the first gun
			ShootBullet(muzzle1.transform.position, target);
			//Play a random pistol sound
			GameAudioSource.PlayAudioOnce(AudioDatabase.Instance.PistolSounds.GetRandom(), muzzle1.transform.position);
		}
		else
		{
			//Shoot bullet from the second gun
			ShootBullet(muzzle2.transform.position, target);
			//Play a random pistol sound
			GameAudioSource.PlayAudioOnce(AudioDatabase.Instance.PistolSounds.GetRandom(), muzzle2.transform.position);
		}

		//Flip to the other gun
		shootingFirstMuzzle = !shootingFirstMuzzle;
	}
}
