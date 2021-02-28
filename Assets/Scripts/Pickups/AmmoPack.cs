using UnityEngine;

//The class for the ammo pack pickup
public sealed class AmmoPack : Pickup
{
	[SerializeField]
	int ammoAmount = 10;

	public override void OnPickup(CharacterController sourceCharacter)
	{
		//On pickup, increase the character's ammo
		sourceCharacter.Ammo += ammoAmount;
	}
}

