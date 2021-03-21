using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

class JointPart
{
	public CharacterJoint Joint; //The joint piece of the ragdoll object. Not all objects will have this
	public Rigidbody Rigidbody; //The rigidbody of the ragdoll object
	public Collider Collider; //The collider of the ragdoll object
}


public class RagdollManager : MonoBehaviour
{
	[SerializeField]
	[Tooltip("This filter is used to select any game objects that are a part of the ragdoll system")]
	List<string> objectFilters;
	[SerializeField]
	[Tooltip("How long the character will wait before it starts sinking into the ground")]
	float waitTimeBeforeSinking = 3.5f;
	[SerializeField]
	[Tooltip("How much the enemy's y value should change when sinking")]
	float sinkAmount = -1f;
	[SerializeField]
	[Tooltip("How long the enemy takes to sink")]
	float sinkTime = 1f;

	//The main animator on the character
	Animator mainAnimator;
	//The main rigidbody on the character
	Rigidbody mainRigidbody;
	//The main collider on the character
	Collider mainCollider;
	//The main nav mesh agent on the character
	NavMeshAgent mainAgent;

	//A list of all the objects that are a part of the ragdoll system on the character
	List<JointPart> RagdollJoints;

	private void Awake()
	{
		//Get the main animator
		mainAnimator = GetComponent<Animator>();
		//Get the main rigidbody
		mainRigidbody = GetComponent<Rigidbody>();
		//Get the main collider
		mainCollider = GetComponent<Collider>();
		//Get the main nav agent
		mainAgent = GetComponent<NavMeshAgent>();

		//Create the list for storing the ragdoll objects
		RagdollJoints = new List<JointPart>();

		//Get a lists of all the colliders on the character
		var joints = GetComponentsInChildren<Collider>();

		foreach (var joint in joints)
		{
			//If the object's name satifies one of many filters
			if (IsPartOfFilter(joint.gameObject.name))
			{
				//Then add that object to the list of ragdoll joints
				RagdollJoints.Add(new JointPart
				{
					Joint = joint.GetComponent<CharacterJoint>(),
					Collider = joint.GetComponent<Collider>(),
					Rigidbody = joint.GetComponent<Rigidbody>()
				});
			}
		}

		//Once all the ragdoll parts have been found, disable them all
		DisableRagdoll();
	}

	//Enables ragdoll physics
	public void EnableRagdoll()
	{
		//Disable the main animator
		if (mainAnimator != null)
		{
			mainAnimator.enabled = false;
		}
		//Disable the main rigidbody
		if (mainRigidbody != null)
		{
			mainRigidbody.isKinematic = true;
		}
		//Disable the main collider
		if (mainCollider != null)
		{
			mainCollider.enabled = false;
		}
		//Disable the main nav agent
		if (mainAgent != null)
		{
			mainAgent.enabled = false;
		}

		//Loop over all the ragdoll joints
		foreach (var joint in RagdollJoints)
		{
			//Set them to respond to gravity and physics
			joint.Rigidbody.isKinematic = false;
			//Enable collision for them
			joint.Collider.enabled = true;
		}
	}

	//Disables ragdoll physics
	public void DisableRagdoll()
	{
		//Enable the main animator
		if (mainAnimator != null)
		{
			mainAnimator.enabled = true;
		}
		//Enable the main rigidbody
		if (mainRigidbody != null)
		{
			mainRigidbody.isKinematic = false;
		}
		//Enable the main collider
		if (mainCollider != null)
		{
			mainCollider.enabled = true;
		}
		//Enable the main nav agent
		if (mainAgent != null)
		{
			mainAgent.enabled = true;
		}

		//Loop over all the ragdoll joints
		foreach (var joint in RagdollJoints)
		{
			//Set them to not respond to gravity and physics
			joint.Rigidbody.isKinematic = true;
			//Disable collision for them
			joint.Collider.enabled = false;
		}
	}

	//Causes the enemy to sink into the ground
	public void SinkIntoGround(bool destroyOnDone)
	{
		//Start the sink routine
		StartCoroutine(SinkRoutine(destroyOnDone));
	}

	IEnumerator SinkRoutine(bool destroyOnDone)
	{
		//Wait for the specified delay
		yield return new WaitForSeconds(waitTimeBeforeSinking);

		//Loop over all the ragdoll joints
		foreach (var joint in RagdollJoints)
		{
			//Set them to not respond to gravity and physics
			joint.Rigidbody.isKinematic = true;
		}

		//Store the original height
		var previousHeight = transform.position.y;
		//Store the destination height
		var newHeight = previousHeight + sinkAmount;

		//Linearly interpolate to the new height value
		for (float i = 0; i < sinkTime; i += Time.deltaTime)
		{
			yield return null;

			transform.position = new Vector3(transform.position.x,Mathf.Lerp(previousHeight,newHeight,i / sinkTime),transform.position.z);
		}

		if (destroyOnDone)
		{
			//Destroy the enemy
			Destroy(gameObject);
		}
	}

	//Checks if a name satifies one or many filters
	bool IsPartOfFilter(string name)
	{
		//Loop over all the filters
		for (int i = 0; i < objectFilters.Count; i++)
		{
			//If a filter text is contained in the name string
			if (name.ToLower().Contains(objectFilters[i].ToLower()))
			{
				return true;
			}
		}
		//If the name is not in any filters, then return false
		return false;
	}
}
