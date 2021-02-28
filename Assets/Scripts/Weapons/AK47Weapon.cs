using UnityEngine;

//The class representing the AK-47 weapon
public sealed class AK47Weapon : Weapon
{
	public override void Shoot(Vector3 muzzle, Vector3 target)
	{
		base.Shoot(muzzle, target);
	}
}
