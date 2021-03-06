using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Represents all characters in the game, such as players and enemies
/// Contains data that is common to all characters, such as weapon usage and movement
/// </summary>
public abstract class Character : WeaponUser
{
	[Space]
	[Header("Character Properties")]
	[SerializeField]
	[Tooltip("How fast the character should move on the ground")]
	float movementSpeed = 3f;
	[Tooltip("How fast the character should transition between animations. Setting this to zero would give the most control to the player, but the animations would look jerky")]
	[SerializeField]
	float AnimationTransitionSpeed = 3f;


	Vector2 movement;
	List<GameObject> terrainCollisions = new List<GameObject>();
	int m_horizontalID;
	int m_verticalID;

	/// <summary>
	/// How fast the character moves
	/// </summary>
	public float MovementSpeed => movementSpeed;

	/// <summary>
	/// Whether ground movement is enabled or not. The movement system uses root motion to move the character around. Turning this off also turns off root motion and vice versa
	/// </summary>
	public bool AllowGroundMovement
	{
		get => animator.applyRootMotion;
		set => animator.applyRootMotion = value;
	}

	/// <summary>
	/// Whether the character is on the ground or not
	/// </summary>
	public bool OnGround { get; set; }
	//{
	
		/*get
		{
			//First, loop over all the objects in the terrain collision list to see if all the objects in there exist
			for (int i = terrainCollisions.Count - 1; i >= 0; i--)
			{
				if (terrainCollisions[i] == null)
				{
					terrainCollisions.RemoveAt(i);
				}
			}
			//Return whether the character is colliding with any terrain
			return terrainCollisions.Count > 0;
		}*/
	//}

	/// <summary>
	/// The movement vector that governs horizontal movement. This vector is internally multiplied by <see cref="MovementSpeed"/> when applying it to movement
	/// </summary>
	public Vector2 Movement
	{
		get => movement;
		set
		{
			movement = value;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (OnGround)
		{
			if (m_horizontalID == 0)
			{
				m_horizontalID = Animator.StringToHash("Horizontal");
				m_verticalID = Animator.StringToHash("Vertical");
			}
			/*if (movement == Vector2.zero)
			{
				animator.SetFloat(m_horizontalID,0f);
				animator.SetFloat(m_verticalID,0f);
			}
			else*/
			{
				var animatorMovement = movement * movementSpeed;

				//Get the old values
				var oldHorizontal = animator.GetFloat(m_horizontalID);
				var oldVertical = animator.GetFloat(m_verticalID);

				//Interpolate to the new horizontal and vertical values. This is to help smooth out the animations
				animator.SetFloat(m_horizontalID, Mathf.Lerp(oldHorizontal, animatorMovement.x, AnimationTransitionSpeed * Time.deltaTime));
				animator.SetFloat(m_verticalID, Mathf.Lerp(oldVertical, animatorMovement.y, AnimationTransitionSpeed * Time.deltaTime));
			}
			
		}
	}

	//Called whenver the character collides with something
	protected virtual void OnCollisionEnter(Collision collision)
	{
		//If the character collided with terrain
		if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
		{
			//Add it to the list of terrain collisions
			terrainCollisions.Add(collision.gameObject);

			for (int i = terrainCollisions.Count - 1; i >= 0; i--)
			{
				if (terrainCollisions[i] == null)
				{
					terrainCollisions.RemoveAt(i);
				}
			}

			//Debug.Log("A - Collisions = " + terrainCollisions.Count);

			if (terrainCollisions.Count == 1)
			{
				//Debug.Log("In Air!");
				OnGround = true;
			}
		}
	}

	//Called whenever the character is no longer colliding with somethimg
	protected virtual void OnCollisionExit(Collision collision)
	{
		//If the character collided with terrain
		if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
		{
			//Remove it from the list of terrain collisions
			terrainCollisions.Remove(collision.gameObject);

			for (int i = terrainCollisions.Count - 1; i >= 0; i--)
			{
				if (terrainCollisions[i] == null)
				{
					terrainCollisions.RemoveAt(i);
				}
			}

			if (terrainCollisions.Count == 0)
			{
				OnGround = false;
			}
		}
	}
}
