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

	//The internal field for the movement
	Vector2 _movement;
	//A list of all the objects the character has collided with
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
	public bool OnGround { get; private set; }

	/// <summary>
	/// The movement vector that governs horizontal movement. This vector is internally multiplied by <see cref="MovementSpeed"/> when applying it to movement
	/// </summary>
	public Vector2 Movement
	{
		get => _movement;
		set
		{
			_movement = value;
		}
	}

	/// <summary>
	/// The health component on the character
	/// </summary>
	Health _characterHealth;
	public Health CharacterHealth
	{
		get
		{
			if (_characterHealth == null)
			{
				_characterHealth = GetComponent<Health>();
			}
			return _characterHealth;
		}
	}

	/// <summary>
	/// Whether the character is dead or not
	/// </summary>
	public bool IsDead => CharacterHealth.CurrentHealth == 0;

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

			//Only move the character if they are not dead
			if (!IsDead)
			{
				var animatorMovement = _movement * movementSpeed;

				if (animatorMovement.magnitude > 3f)
				{
					animator.speed = animatorMovement.magnitude / 3f;
				}
				else
				{
					animator.speed = 1f;
				}

				//Get the old values
				var oldHorizontal = animator.GetFloat(m_horizontalID);
				var oldVertical = animator.GetFloat(m_verticalID);

				//Interpolate to the new horizontal and vertical values. This is to help smooth out the animations
				animator.SetFloat(m_horizontalID, Mathf.Lerp(oldHorizontal, animatorMovement.x, AnimationTransitionSpeed * Time.deltaTime));
				animator.SetFloat(m_verticalID, Mathf.Lerp(oldVertical, animatorMovement.y, AnimationTransitionSpeed * Time.deltaTime));
			}
			else
			{
				animator.SetFloat(m_horizontalID, 0f);
				animator.SetFloat(m_verticalID, 0f);
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

			//Filter out any collisions that no longer exist
			for (int i = terrainCollisions.Count - 1; i >= 0; i--)
			{
				if (terrainCollisions[i] == null)
				{
					terrainCollisions.RemoveAt(i);
				}
			}

			if (terrainCollisions.Count == 1)
			{
				OnGround = true;
			}
		}
		
		//if the collided object has a damager
		var damager = collision.gameObject.GetComponent<ICharacterDamager>();

		if (damager != null)
		{
			//Damage the character
			damager.OnHit(this);
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

			//Filter out any collisions that no longer exist
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

	//Called whenever the character triggers something
	protected virtual void OnTriggerEnter(Collider other)
	{
		//if the collided object has a damager
		var damager = other.gameObject.GetComponent<ICharacterDamager>();

		if (damager != null)
		{
			//Damage the character
			damager.OnHit(this);
		}
	}

	/// <summary>
	/// Plays a sound on death
	/// </summary>
	public void PlayDeathSound()
	{
		//Plays a random death sound at the character's location
		GameAudioSource.PlayAudioOnce(AudioDatabase.Instance.DeathSounds.GetRandom(), transform.position);
	}
}
