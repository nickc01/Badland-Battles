using System.Collections.Generic;
using UnityEngine;

//The class representing the Shotgun weapon
public sealed class ShotgunWeapon : Weapon
{
	[Space]
	[Header("Shot Gun Details")]
	[SerializeField]
	int amountOfShots = 5;
	

	public override void Shoot(Vector3 muzzle, Vector3 target)
	{
		//Play a random shotgun sound
		GameAudioSource.PlayAudioOnce(AudioDatabase.Instance.ShotgunSounds.GetRandom(), muzzle);
		//Decrease the flash intensity by the amount of shots, since the flashes are going to stack on top of each other
		flashIntensity /= amountOfShots;
		//Run the shoot code for as many shots are needed
		for (int i = 0; i < amountOfShots; i++)
		{
			ShootBullet(muzzle, target);
		}
		//Reset the flash intensity
		flashIntensity *= amountOfShots;
	}
}
