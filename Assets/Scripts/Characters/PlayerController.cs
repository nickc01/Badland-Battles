using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerController : Character
{
    /// <summary>
    /// The singleton for accessing the player from anywhere
    /// </summary>
    public static PlayerController Instance { get; private set; }

    [Space]
    [Header("Player Properties")]
    [Tooltip("How fast the player rotates towards the mouse")]
    [SerializeField]
    float RotationSpeed = 7f;
    [Tooltip("When set to true, the player will rotate to whereever the mouse is pointing at")]
    [SerializeField]
    bool turnTowardsMouse = true;
    [SerializeField]
    [Tooltip("The amount of force propelled upwards when the player jumps")]
    float jumpForce = 10f;
    [SerializeField]
    [Tooltip("The amount of lives the player starts with")]
    int lives = 3;
    [SerializeField]
    [Tooltip("The weapon the player will start with")]
    Weapon startingWeapon;

    [Space]
    [Header("Player Events")]
    [SerializeField]
    [Tooltip("Called anytime the player spawns/respawns in the game")]
    UnityEvent OnPlayerRespawn;
    [SerializeField]
    [Tooltip("Called when the player looses a life. Also returns how many lives the player has left")]
    UnityEvent<int> OnLiveLost;
    [SerializeField]
    [Tooltip("Called when all the player's lives are lost")]
    UnityEvent OnGameOver;


    //The rigidbody of the player
    Rigidbody body;
    //The ID used to set the "Horizontal" animator parameter
    int horizontalID;
    //The ID used to set the "Vertical" animator parameter
    int verticalID;
    //The ID used to set the "Jumping" animator parameter
    int jumpingID;
    //The ID used to set the "Landing" animator parameter
    int landingID;
    //The ID used to set the "On Ground" animator parameter
    int onGroundID;


    //The position the player started at
    Vector3 startPosition;


    //Whether the player is jumping
    public bool Jumping
    {
        get => animator.GetBool(jumpingID);
        private set => animator.SetBool(jumpingID, value);
    }

    //Whether the player is falling
    public bool Falling
    {
        get => !OnGround && !Jumping && body.velocity.y <= 0f;
    }

    private void Awake()
    {
        startPosition = transform.position;
        //Set the singleton
        Instance = this;

        //Get  the player's rigid body
        body = GetComponent<Rigidbody>();

        //Get the ID for setting the "Horizontal" animator parameter
        horizontalID = Animator.StringToHash("Horizontal");
        //Get the ID for setting the "Vertical" animator parameter
        verticalID = Animator.StringToHash("Vertical");
        //Get the ID for setting the "Vertical" animator parameter
        jumpingID = Animator.StringToHash("Jumping");
        //Get the ID for setting the "Vertical" animator parameter
        landingID = Animator.StringToHash("Landing");
        //Get the ID for setting the "Vertical" animator parameter
        onGroundID = Animator.StringToHash("On Ground");


        //Update the Ammo HUD
        AmmoHUD.Instance.SetMaxAmmo(MaxAmmo);
        AmmoHUD.Instance.UpdateAmmoRaw(Ammo);

        EquipWeapon(startingWeapon);

		if (OnPlayerRespawn != null)
		{
            OnPlayerRespawn.Invoke();
        }
    }

	protected override void OnEquip()
	{
        WeaponIconHUD.Instance.SetWeaponImage(EquippedWeapon.DisplayImage, EquippedWeapon.DisplayText);
        base.OnEquip();
	}

	protected override void OnUnequip()
	{
        WeaponIconHUD.Instance.SetWeaponImage(null, "");
        base.OnUnequip();
	}

	protected override void Update()
    {
        base.Update();

        if (!IsDead)
        {

            //If the player is on the ground
            if (OnGround && body.velocity.y <= 0f)
            {
                //Update the animation parameter
                animator.SetBool(onGroundID, OnGround);
            }

            if (OnGround && !GameManager.Instance.GamePaused)
            {
                //The movement vector along the X and Z axis
                //The Raw version is used here, since we don't need the axis' to be interpolated. The interpolation is already handled further below
                Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

                //Normalize the vector to a length of 1. This ensures that the movement remains fair whether you are using a keyboard or controller
                input.Normalize();

                //Convert it from local-space coordinates to world-space coordinates.
                //This is to ensure that when we want the player to go down, it moves down in world space, not in it's local space. I.E, the player goes south, rather than backwards
                input = transform.InverseTransformDirection(input);

                //Set the player's movement to the movement vector
                Movement = new Vector2(input.x, input.z);
            }

            //Look at the mouse pointer and get the target that the mouse is pointing at
            var target = LookAtMouse();
            //If the player is jumping in the air and the player is falling down
            if (Jumping && body.velocity.y <= 0f)
            {
                //The player is no longer jumping and is switching to the falling state
                Jumping = false;
            }

            //If we are pressing space or pressing the fire button and the player is on the ground
            if ((Input.GetKeyDown(KeyCode.Space)) && OnGround && !GameManager.Instance.GamePaused)
            {
                animator.Play("Jumping");
                //Jump
                Jumping = true;

                animator.SetBool(onGroundID, false);
                //Add a jump force
                body.AddRelativeForce(0f, jumpForce, 0f, ForceMode.Impulse);
            }

            if (EquippedWeapon != null)
            {
                //Set the weapon target to the mouse pointer location, so the gun points in that direction
                WeaponTarget = target;

                //If the fire button is pressed
                if (Input.GetButton("Fire1") && CanFireWeapon && !GameManager.Instance.GamePaused)
                {
                    //Fire a ray wherever the mouse is pointing at
                    Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                    //If the ray collided with anything
                    if (Physics.Raycast(mouseRay, out var hit))
                    {
                        //Fire the weapon towards it
                        FireWeapon(hit.point);
                    }
                    //if the ray didn't hit anything
                    else
                    {
                        //Fire at the target the mouse pointer is located on the plane from earlier
                        FireWeapon(target);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        //If we are not on the ground, then do movement by modifying the rigidbody velocity to move in the air
        if (!IsDead && !OnGround && !GameManager.Instance.GamePaused)
        {
            //Set the x velocity to the horizontal input axis and the z axis to the vertical input axis
            body.velocity = new Vector3(Input.GetAxis("Horizontal") * MovementSpeed, body.velocity.y, Input.GetAxis("Vertical") * MovementSpeed);
        }
    }

    //This function rotates the player to face whereever the mouse pointer is pointing at. Returns the target the mouse is pointing at
    Vector3 LookAtMouse()
    {
        //Create a plane the faces upwards at the player's position. This will create a plane that spans the x and z axis
        Plane mousePlane = new Plane(Vector3.up, transform.position);

        //Create a ray from the current mouse position
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        //If the ray collides with the plane, then we get the distance from the camera to the collided point on the plane
        if (mousePlane.Raycast(mouseRay, out var distanceToPlane))
        {
            //Convert the distance value to a 3d point value. This point is where the ray collided with the plane
            var collisionPoint = mouseRay.GetPoint(distanceToPlane);
            if (!GameManager.Instance.GamePaused)
            {
                //Store the previous rotation
                var previousRotation = transform.rotation;

                //Look at the collision point
                transform.LookAt(collisionPoint);

                //Get the updated rotation
                var newRotation = transform.rotation;
                //Interpolate from the previous rotation to the new rotation
                transform.rotation = Quaternion.Lerp(previousRotation, newRotation, RotationSpeed * Time.deltaTime);
            }

            return collisionPoint;
        }

        return transform.position;
    }

    //Called when the player collides with something
    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        //If the collided object has a pickup component
        var pickup = collision.gameObject.GetComponent<Pickup>();
        if (pickup != null)
        {
            //Pickup the pickup
            pickup.OnPickup(this);
            //If the pickup is to be destroyed afterwards, then destroy it
            if (pickup.DestroyOnPickup)
            {
                Destroy(pickup.gameObject);
            }
        }
    }

    //Called when the amount of ammo has changed
    protected override void OnAmmoChange(int ammo)
    {
        //Update the ammo HUD
        AmmoHUD.Instance.UpdateAmmo(ammo);
    }

    public void LooseLife()
	{
        lives--;
		if (OnLiveLost != null)
		{
            OnLiveLost.Invoke(lives);
		}
		if (lives == 0 && OnGameOver != null)
		{
            OnGameOver.Invoke();
		}
	}

    /// <summary>
    /// Respawns the player after a set amount of time
    /// </summary>
    /// <param name="time">Time to respawn</param>
    public void RespawnPlayer(float time)
	{
        //If the player has lives left
		if (lives > 0)
		{
            //Respawn the player
            StartCoroutine(RespawnRoutine(time));
        }
	}

    IEnumerator RespawnRoutine(float time)
	{
        //Wait a specified amoutn of time
        yield return new WaitForSeconds(time);

        //Disable the ragdolling
        GetComponent<RagdollManager>().DisableRagdoll();

        //Move the player to it's spawn location
        transform.position = startPosition;

        //Heal the player to it's full health
        CharacterHealth.Heal(CharacterHealth.InitialHealth);
        Ammo = MaxAmmo;

        //Call the respawner event
		if (OnPlayerRespawn != null)
		{
            OnPlayerRespawn.Invoke();
		}
    }
}
