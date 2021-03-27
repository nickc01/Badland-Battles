using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SetAnimatorSpeed : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The speed the animation should play at")]
	float animationSpeed = 1f;

	//The animator component
	Animator animator;

	private void Awake()
	{
		//Get the animator component
		animator = GetComponent<Animator>();
		//Set the animation speed
		animator.speed = animationSpeed;
	}

	//Called anytime the value is changed in the inspector
	private void OnValidate()
	{
		if (animator != null)
		{
			//Set the animation speed to the updated value
			animator.speed = animationSpeed;
		}
	}
}
