using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Character
{
	[Space]
	[Header("Enemy Properties")]
	[SerializeField]
	[Tooltip("The target the enemy should go towards")]
	Transform target;
	[SerializeField]
	[Tooltip("An offset to the target position. Used to aim the gun at the right spot")]
	Vector3 targetOffset;
	[SerializeField]
	[Tooltip("A prefab pointing to the weapon the enemy will start with.")]
	Weapon startingWeapon;
	[SerializeField]
	[Tooltip("The minimum distance the enemy needs to be to the target for it to begin shooting")]
	float shootingRange = 10f;
	[SerializeField]
	[Tooltip("How fast the enemy should rotate towards the target")]
	float rotationSpeed = 5f;

	//The nav mesh of the enemy
	NavMeshAgent navAgent;

	//The target the enemy will move towards
	public Transform Target
	{
		get => target;
		set => target = value;
	}

	//Called when the enemy starts
	private void Awake()
	{
		//Get the enemy's nav mesh
		navAgent = GetComponent<NavMeshAgent>();

		//If the enemy has a starting weapon
		if (startingWeapon != null)
		{
			//Equip the weapon
			EquipWeapon(startingWeapon);
		}

		//Set the main target to the player
		Target = PlayerController.Instance.transform;
	}


	protected override void Update()
	{
		base.Update();
		//If the target is set and the nav mesh is enabled
		if (target != null && navAgent.enabled)
		{
			//Make the nav mesh move to the target
			navAgent.SetDestination(target.position);
		}

		//If the enemy is not dead and a target is set
		if (!IsDead && target != null)
		{
			//Get the velocity the nav mesh wants to move at
			Vector3 movementVector = navAgent.desiredVelocity;

			//Normalize the vector to a length of 1
			movementVector.Normalize();

			//Convert it from local-space coordinates to world-space coordinates so the animator can move in the correct direction
			movementVector = transform.InverseTransformDirection(movementVector);

			//Set the enemy's movement to the movement vector
			Movement = new Vector2(movementVector.x, movementVector.z);

			//If the target is within shooting range
			if (Vector3.Distance(transform.position, target.position + targetOffset) <= shootingRange)
			{
				//Look towards the target
				LookAt(target.position + targetOffset);

				//If the weapon can be fired and the enemy is within line of sight
				if (CanFireWeapon && InLineOfSight(target.position + targetOffset))
				{
					//Fire the weapon
					FireWeapon(target.position + targetOffset);
				}
			}
			//If the target is not within shooting range
			else
			{
				//Look at the direction the enemy is traveling in
				LookAt(transform.position + navAgent.desiredVelocity);
			}
		}
		//If the enemy is dead or the target is not set. Stop the enemy's movement
		else
		{
			Movement = Vector2.zero;
		}
	}

	//Called when the animator wants to apply root motion. This function allows us to customize the root motion behaviour
	private void OnAnimatorMove()
	{
		//Set the nav mesh velocity to be equal to the animation's root motion velocity
		navAgent.velocity = animator.velocity;
	}

	//Rotates a target on the y-axis to look at a target
	void LookAt(Vector3 target)
	{
		var oldRotation = transform.rotation;

		transform.LookAt(target);

		var newRotation = transform.rotation;

		//Interpolate to the new rotation
		transform.rotation = Quaternion.Lerp(oldRotation,newRotation, rotationSpeed * Time.deltaTime);

		//Do not change the x and z axis, only the y axis
		transform.eulerAngles = new Vector3(oldRotation.x, transform.rotation.eulerAngles.y, oldRotation.z);
	}
}

