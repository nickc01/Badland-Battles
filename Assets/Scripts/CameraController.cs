using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[Tooltip("The object the camera will target")]
	public Transform Target;
	[SerializeField]
	[Tooltip("How fast the camera will move around")]
	float movementSpeed = 1f;
	[SerializeField]
	[Tooltip("How fast the camera will rotate towards the target")]
	float rotationSpeed = 1f;
	[SerializeField]
	[Tooltip("If set to true, the camera will rotate to face the target")]
	bool rotateTowardsTarget = true;
	[Tooltip("The positional offset of the camera")]
	public Vector3 Offset;


	private void Update()
	{
		//If a target is set
		if (Target != null)
		{
			//Linearly interpolate to the new position. The position is the target's position plus the offset
			transform.position = Vector3.Lerp(transform.position,Target.transform.position + Offset,Time.unscaledDeltaTime * movementSpeed);
			//If the camera rotates towards the target
			if (rotateTowardsTarget)
			{
				//Store the old rotation
				var oldRotation = transform.rotation;

				//Look towards the target
				transform.LookAt(Target);

				//Store the new rotation
				var newRotation = transform.rotation;

				//Interpolate between the old and new rotations
				transform.rotation = Quaternion.Lerp(oldRotation, newRotation, Time.unscaledDeltaTime * rotationSpeed);
			}
		}
	}
}
