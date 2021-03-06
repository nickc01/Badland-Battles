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


	NavMeshAgent navAgent;

	public Transform Target
	{
		get => target;
		set => target = value;
	}

	private void Awake()
	{
		navAgent = GetComponent<NavMeshAgent>();

		if (startingWeapon != null)
		{
			EquipWeapon(startingWeapon);
		}

		Target = PlayerController.Instance.transform;
	}


	protected override void Update()
	{
		base.Update();

		//Debug.Log("Before Has ROot Motion Before = " + animator.applyRootMotion);

		//animator.applyRootMotion = true;

		//Debug.Log("Has ROot Motion After = " + animator.applyRootMotion);
		if (target != null)
		{
			navAgent.SetDestination(target.position);
		}

		//Get the vector towards the target
		if (target != null)
		{
			var toTarget = target.position + targetOffset - transform.position;

			//Debug.DrawLine(transform.position, transform.position + navAgent.desiredVelocity, Color.red);
			Debug.DrawLine(transform.position, transform.position + toTarget, Color.yellow);
			//The movement vector along the X and Z axis
			//Vector3 movementVector = new Vector3(toTarget.x, 0f, toTarget.z);
			Vector3 movementVector = navAgent.desiredVelocity;

			//Normalize the vector to a length of 1
			movementVector.Normalize();

			//Convert it from local-space coordinates to world-space coordinates so the animator can move in the correct direction
			movementVector = transform.InverseTransformDirection(movementVector);

			Movement = new Vector2(movementVector.x, movementVector.z);

			Debug.DrawLine(transform.position, transform.position + (Vector3)(Movement * 10f), Color.white);

			if (Vector3.Distance(transform.position, target.position + targetOffset) <= shootingRange)
			{
				LookAt(target.position + targetOffset);

				if (CanFireWeapon && InLineOfSight(target.position + targetOffset))
				{
					FireWeapon(target.position + targetOffset);
				}
			}
			else
			{
				//Look at the direction the enemy is traveling in
				LookAt(transform.position + toTarget);
			}
		}
		else
		{
			Movement = Vector2.zero;
		}

		//LookAt(target.position);


		
	}

	private void OnAnimatorMove()
	{
		navAgent.velocity = animator.velocity;
	}

	//private void OnAnimatorMove()
	//{
	//Debug.DrawLine(transform.position, transform.position + navAgent.velocity, Color.green);
	//Debug.DrawLine(transform.position, transform.position + animator.velocity, Color.blue);
	//navAgent.velocity = animator.velocity;
	//navAgent.velocity = Movement;
	//}

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

