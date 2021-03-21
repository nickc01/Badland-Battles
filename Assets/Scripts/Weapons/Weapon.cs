using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Weapon : MonoBehaviour
{
	[Header("Weapon Details")]
	[SerializeField]
	[Tooltip("The color of the flash when a bullet is shot")]
	protected Color flashColor = Color.yellow;
	[SerializeField]
	[Tooltip("The intensity of the bullet flash")]
	protected float flashIntensity = 1;
	[SerializeField]
	[Tooltip("How large the bullet flash will be, in in-game units")]
	protected float flashRange = 2f;
	[SerializeField]
	[Tooltip("How long the flash should be visible")]
	protected float flashTime = 0.06f;
	[SerializeField]
	[Tooltip("How much damage each bullet shot does")]
	protected int bulletDamage = 5;
	[SerializeField]
	[Tooltip("How inaccurate the shots will be. An inaccuracy of 0 means perfect accuracy")]
	protected float shotInaccuracy = 0f;
	[SerializeField]
	[Tooltip("How many shots per second the gun can fire")]
	protected float fireRate = 3f;
	[SerializeField]
	[Tooltip("How much ammo the weapon consumes per shot")]
	protected int ammoPerShot = 1;
	[Space]
	[Header("HUD")]
	[SerializeField]
	[Tooltip("The text that will be displayed on the weapon HUD")]
	string displayText;
	[SerializeField]
	[Tooltip("The image that will be displayed on the weapon HUD")]
	Sprite displayImage;

	public string DisplayText => displayText;
	public Sprite DisplayImage => displayImage;
	public int AmmoPerShot => ammoPerShot;

	//Represents where the right hand should be
	Transform RightHandIK;
	//Represents where the left hand should be
	Transform LeftHandIK;

	/// <summary>
	/// Whether the gun can be fired or not
	/// </summary>
	public bool CanFire { get; private set; } = true;

	//Sets the inverse kinematics of the source character
	public virtual void SetCharacterIK(Animator characterAnimator)
	{
		if (RightHandIK == null)
		{
			//Retrive the right hand location
			RightHandIK = transform.Find("Right Hand IK");
		}
		if (LeftHandIK == null)
		{
			//Retrive the left hand rotation
			LeftHandIK = transform.Find("Left Hand IK");
		}

		//Set all the weights for the left and right hands to 1, so that 100% of the inverse kinematics apply
		characterAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
		characterAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
		characterAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
		characterAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

		//Set the position and rotation of the right and left hands
		characterAnimator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIK.transform.position);
		characterAnimator.SetIKRotation(AvatarIKGoal.RightHand, RightHandIK.transform.rotation);
		characterAnimator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIK.transform.position);
		characterAnimator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIK.transform.rotation);
	}

	//Called when the gun is equipped
	public virtual void OnEquip(WeaponUser character)
	{
		
	}

	//Called when the gun is unequipped
	public virtual void OnUnequip(WeaponUser character)
	{
		
	}

	/// <summary>
	/// Shoots the gun
	/// </summary>
	/// <param name="muzzle">The muzzle of the gun</param>
	/// <param name="target">The target</param>
	/// <returns>Returns true if the gun was successfully fired, and false otherwise</returns>
	public virtual void Shoot(Vector3 muzzle, Vector3 target)
	{
		//Shoot a bullet towards the target
		ShootBullet(muzzle, target);
	}

	/// <summary>
	/// Shoots a bullet
	/// </summary>
	/// <param name="source">The muzzle of the gun</param>
	/// <param name="target">The target</param>
	/// <returns>Returns true if the gun was successfully fired, and false otherwise</returns>
	protected void ShootBullet(Vector3 source, Vector3 target)
	{
		//Add some inaccuracy to the targetting system.
		//First, get a vector that points in a random direction, and has a length of one (onUnitSphere)
		//Two, multiply it by the shot accuracy so it's magnitude is equal to the shot accuracy
		//Three, add it to the target and now the target has some inaccuracy added
		var adjustedTarget = target + (UnityEngine.Random.onUnitSphere * shotInaccuracy * Vector3.Distance(source,target));

		StartCoroutine(ShootBulletRoutine(source, adjustedTarget));
	}

	//A routine for shooting at a target
	private IEnumerator ShootBulletRoutine(Vector3 source, Vector3 target)
	{
		CanFire = false;
		//Instantiate a flash at the muzzle
		var flash = GameObject.Instantiate(GameManager.Instance.GunFirePrefab, source, Quaternion.identity, transform);
		//Get the light component of the flash
		var flashLight = flash.GetComponent<Light>();
		//Set the light's intensity and range
		flashLight.intensity = flashIntensity;
		flashLight.range = flashRange;

		//Destroy the flash after a set amount of time
		Destroy(flash, flashTime);

		Vector3 hitPoint = Vector3.zero;
		Transform hitObject = null;

		//Fire a raycast and see if it hits something
		if (Physics.Raycast(source, (target - source).normalized, out var hit))
		{
			hitPoint = hit.point;
			hitObject = hit.transform;
		}
		else
		{
			hitPoint = source + ((target - source).normalized * 100f);
		}

		if (GameManager.Instance.GunRayPrefab != null)
		{
			var halfWayMark = Vector3.Lerp(source, hitPoint, 0.5f);
			var distance = Vector3.Distance(source, hitPoint);
			var yellowRay = GameObject.Instantiate(GameManager.Instance.GunRayPrefab, halfWayMark, Quaternion.identity);
			yellowRay.transform.LookAt(hitPoint);


			var oldScale = yellowRay.transform.localScale;
			yellowRay.transform.localScale = new Vector3(oldScale.x, oldScale.y, distance);
		}


		//If it hit something, then instantiate a hit prefab at wherever it hit (If a hit prefab is configured)
		if (GameManager.Instance.GunHitPrefab != null)
		{
			var hitFlash = GameObject.Instantiate(GameManager.Instance.GunHitPrefab, hitPoint, Quaternion.identity);
			Destroy(hitFlash, flashTime);
		}

		//If the hit object contains a health component, then subtract from it's health
		if (hitObject != null)
		{
			var health = hit.transform.GetComponent<Health>();
			if (health != null)
			{
				health.Damage(bulletDamage);
			}
		}

		//Wait for the flash time to end
		yield return new WaitForSeconds(flashTime);

		//Calculate how long we should wait before another shot can be fired
		var shotDelay = (1f / fireRate) - flashTime;
		if (shotDelay > 0f)
		{
			//Wait for the shot delay
			yield return new WaitForSeconds(shotDelay);
		}
		CanFire = true;
	}
}

