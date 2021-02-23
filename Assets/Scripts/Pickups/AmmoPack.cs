using UnityEngine;

public sealed class AmmoPack : Pickup
{
	[SerializeField]
	int ammoAmount = 10;

	public override void OnPickup(CharacterController sourceCharacter)
	{
		sourceCharacter.Ammo += ammoAmount;
	}
}

