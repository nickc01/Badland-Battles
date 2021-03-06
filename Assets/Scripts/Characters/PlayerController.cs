using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : Character
{
    /// <summary>
    /// The singleton for accessing the player from anywhere
    /// </summary>
    public static PlayerController Instance { get; private set; }

    [Space]
    [Header("Player Properties")]
    [Tooltip("How fast the character rotates towards the mouse")]
    [SerializeField]
    float RotationSpeed = 7f;
    [Tooltip("When set to true, the character will rotate to whereever the mouse is pointing at")]
    [SerializeField]
    bool turnTowardsMouse = true;
    [SerializeField]
    [Tooltip("The amount of force propelled upwards when the character jumps")]
    float jumpForce = 10f;


    //The health of the character
    Health health;
    //The rigidbody of the character
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


    //Whether the character is jumping
    public bool Jumping
    {
        get => animator.GetBool(jumpingID);
        private set => animator.SetBool(jumpingID, value);
    }

    //Whether the character is falling
    public bool Falling
    {
        get => !OnGround && !Jumping && body.velocity.y <= 0f;
    }

    private void Awake()
    {
        //Set the singleton
        Instance = this;

        health = GetComponent<Health>();
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
    }

    protected override void Update()
    {
        base.Update();

        if (OnGround)
        {
            animator.SetBool(onGroundID, OnGround);
        }

        if (OnGround)
        {
            //The movement vector along the X and Z axis
            //The Raw version is used here, since we don't need the axis' to be interpolated. The interpolation is already handled further below
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

            //Normalize the vector to a length of 1. This ensures that the movement remains fair whether you are using a keyboard or controller
            input.Normalize();

            //Convert it from local-space coordinates to world-space coordinates.
            //This is to ensure that when we want the character to go down, it moves down in world space, not in it's local space. I.E, the character goes south, rather than backwards
            input = transform.InverseTransformDirection(input);

            Movement = new Vector2(input.x,input.z);
        }

        //Look at the mouse pointer and get the target that the mouse is pointing at
        var target = LookAtMouse();

        //If the character is jumping in the air and the character is falling down
        if (Jumping && body.velocity.y <= 0f)
        {
            //The character is no longer jumping and is switching to the falling state
            Jumping = false;
        }

        //If we are pressing space or pressing the fire button and the character is on the ground
        if ((Input.GetKeyDown(KeyCode.Space)) && OnGround)
        {
            animator.Play("Jumping");
            //Jump
            Jumping = true;

            //OnGround = false;

            animator.SetBool(onGroundID, OnGround);
            //Add a jump force
            body.AddRelativeForce(0f, jumpForce, 0f, ForceMode.Impulse);
        }

        if (EquippedWeapon != null)
        {
            WeaponTarget = target;
            /*var oldGunRotation = EquippedWeapon.transform.rotation;

            EquippedWeapon.transform.LookAt(target);

            var newGunRotation = EquippedWeapon.transform.rotation;

            //Interpolate the gun's rotation to look at the collision point
            EquippedWeapon.transform.rotation = Quaternion.Lerp(oldGunRotation, newGunRotation, gunRotationSpeed * Time.deltaTime);


            //Zero out any x-rotations on the gun. This ensures that the gun doesn't point towards the ground or at the sky
            var anglesDegrees = EquippedWeapon.transform.eulerAngles;
            EquippedWeapon.transform.eulerAngles = new Vector3(0f, anglesDegrees.y, anglesDegrees.z);*/


            //If the fire button is pressed
            if (Input.GetButton("Fire1") && CanFireWeapon)
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(mouseRay,out var hit))
                {
                    FireWeapon(hit.point);
                }
                else
                {
                    FireWeapon(target);
                }
            }

            //If there is ammo left and the fire button is pressed
            /*if ((!GunsRequireAmmo || Ammo >= EquippedWeapon.AmmoPerShot) && Input.GetButton("Fire1"))
            {
                //Find the muzzle of the equipped weapon
                var muzzle = EquippedWeapon.transform.Find("Muzzle");
                //if no muzzle exists, then throw an exception
                if (muzzle == null)
                {
                    throw new System.Exception("Unable to find the Muzzle for the gun " + EquippedWeapon.gameObject.name);
                }

                //If the weapon can be fired
                if (EquippedWeapon.CanFire)
                {
                    //Tell the weapon to shoot a bullet.
                    EquippedWeapon.Shoot(muzzle.transform.position, target);
                    //Decrease the character's ammo
                    if (GunsRequireAmmo)
                    {
                        Ammo -= EquippedWeapon.AmmoPerShot;
                    }
                }
            }*/
        }
    }

    private void FixedUpdate()
    {
        //If we are not on the ground, then do movement by modifying the rigidbody velocity to move in the air
        if (!OnGround)
        {
            //Set the animator floats to zero
            //animator.SetFloat(horizontalID, 0f);
            //animator.SetFloat(verticalID, 0f);

            //Set the x velocity to the horizontal input axis and the z axis to the vertical input axis
            body.velocity = new Vector3(Input.GetAxis("Horizontal") * MovementSpeed, body.velocity.y, Input.GetAxis("Vertical") * MovementSpeed);
        }
    }

    //This function rotates the character to face whereever the mouse pointer is pointing at. Returns the target the mouse is pointing at
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

            //Store the previous rotation
            var previousRotation = transform.rotation;

            //Look at the collision point
            transform.LookAt(collisionPoint);

            //Get the updated rotation
            var newRotation = transform.rotation;

            //Interpolate from the previous rotation to the new rotation
            transform.rotation = Quaternion.Lerp(previousRotation, newRotation, RotationSpeed * Time.deltaTime);

            return collisionPoint;
        }

        return transform.position;
    }

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

        //If the collided object has a "Harm Character" component
        var harmCharacter = collision.gameObject.GetComponent<HarmCharacter>();
        if (harmCharacter != null)
        {
            //Harm the character
            health.Damage(harmCharacter.Damage);
        }
    }

    //If the player triggers something
    private void OnTriggerEnter(Collider other)
    {
        //If the collided object has a "Harm Character" component
        var harmCharacter = other.gameObject.GetComponent<HarmCharacter>();
        if (harmCharacter != null)
        {
            //Harm the character
            health.Damage(harmCharacter.Damage);
        }
    }

    protected override void OnAmmoChange(int ammo)
    {
        AmmoHUD.Instance.UpdateAmmo(ammo);
    }

    //Destroys the player. Used for the health OnDeath event
    public void DestroyPlayer()
    {
        Destroy(gameObject);
    }
}


/*public class CharacterController : MonoBehaviour
{
    [Tooltip("How fast the character moves")]
    [SerializeField]
    float MovementSpeed = 1f;
    [Tooltip("How fast the character rotates towards the mouse")]
    [SerializeField]
    float RotationSpeed = 7f;
    [Tooltip("How fast the character should transition between animations. Setting this to zero would give the most control to the player, but the animations would look jerky")]
    [SerializeField]
    float AnimationTransitionSpeed = 3f;
    [Tooltip("When set to true, the character will rotate to whereever the mouse is pointing at")]
    [SerializeField]
    bool turnTowardsMouse = true;
    [SerializeField]
    [Tooltip("The amount of force propelled upwards when the character jumps")]
    float jumpForce = 10f;

    [SerializeField]
    [Tooltip("The maximum amount of ammo the character can carry")]
    int maximumAmmo;
    [SerializeField]
    [Tooltip("The starting amount of ammo the character has")]
    int startingAmmo;
    [SerializeField]
    [Tooltip("How fast the equipped gun should rotate towards the target")]
    float gunRotationSpeed = 2f;
    [SerializeField]
    [Tooltip("The weapon prefab that the character will start with")]
    Weapon startingWeapon;

    [SerializeField]
    [Tooltip("The container object that will allow the character to hold the weapon")]
    Transform weaponContainer;

    //The health of the character
    Health health;
    //The rigidbody of the character
    Rigidbody body;
    //The animator for the character
    Animator animator;
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
    //The current amount of ammo the character has
    int currentAmmo;

    /// <summary>
    /// The currently equipped weapon
    /// </summary>
    public Weapon EquippedWeapon { get; private set; }

    //Whether the character is jumping
    public bool Jumping
    {
        get => animator.GetBool(jumpingID);
        private set => animator.SetBool(jumpingID, value);
    }

    //Whether the character is on the ground
    public bool OnGround
    {
        get => animator.GetBool(onGroundID);
        private set => animator.SetBool(onGroundID, value);
    }

    //Whether the character is falling
    public bool Falling
    {
        get => !OnGround && !Jumping && body.velocity.y <= 0f;
    }

    //The amount of ammo the character has
    public int Ammo
    {
        get => currentAmmo;
        set
        {
            //Limit the ammo to be between 0 and maximumAmmo
            currentAmmo = Mathf.Clamp(value, 0, maximumAmmo);
            //Update the ammo HUD
            AmmoHUD.Instance.UpdateAmmo(currentAmmo);
        }
    }


    void Awake()
    {
        //Get the character's health component
        health = GetComponent<Health>();
        //Get the character's animator component
        animator = GetComponent<Animator>();
        //Get the chracter's rigidbody component
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

        //Configure the starting ammo
        currentAmmo = startingAmmo;

        //Update the Ammo HUD
        AmmoHUD.Instance.SetMaxAmmo(maximumAmmo);
        AmmoHUD.Instance.UpdateAmmoRaw(startingAmmo);

        //If a starting weapon is specified
        if (startingWeapon != null)
        {
            //Equip the starting weapon
            EquipWeapon(startingWeapon);
        }
    }


    void Update()
    {
        //If we are on the ground, then do normal movement
        if (OnGround)
        {
            //The movement vector along the X and Z axis
            //The Raw version is used here, since we don't need the axis' to be interpolated. The interpolation is already handled further below
            Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

            //Normalize the vector to a length of 1. This ensures that the movement remains fair whether you are using a keyboard or controller
            movement.Normalize();

            //Multiply each component of the vector by the speed
            movement *= MovementSpeed;

            //Convert it from local-space coordinates to world-space coordinates.
            //This is to ensure that when we want the character to go down, it moves down in world space, not in it's local space. I.E, the character goes south, rather than backwards
            movement = transform.InverseTransformDirection(movement);


            //Get the old values
            var oldHorizontal = animator.GetFloat(horizontalID);
            var oldVertical = animator.GetFloat(verticalID);

            //Interpolate to the new horizontal and vertical values. This is to help smooth out the animations
            animator.SetFloat(horizontalID, Mathf.Lerp(oldHorizontal, movement.x, AnimationTransitionSpeed * Time.deltaTime));
            animator.SetFloat(verticalID, Mathf.Lerp(oldVertical, movement.z, AnimationTransitionSpeed * Time.deltaTime));
        }

        //Look at the mouse pointer and get the target that the mouse is pointing at
        var target = LookAtMouse();

        //If the character is jumping in the air and the character is falling down
        if (Jumping && body.velocity.y <= 0f)
        {
            //The character is no longer jumping and is switching to the falling state
            Jumping = false;
        }

        //If we are pressing space or pressing the fire button and the character is on the ground
        if ((Input.GetKeyDown(KeyCode.Space)) && OnGround)
        {
            animator.Play("Jumping");
            //Jump
            Jumping = true;
            //The character is no longer on the ground
            OnGround = false;
            //Add a jump force
            body.AddRelativeForce(0f, jumpForce, 0f, ForceMode.Impulse);
        }

        if (EquippedWeapon != null)
        {
            var oldGunRotation = EquippedWeapon.transform.rotation;

            EquippedWeapon.transform.LookAt(target);

            var newGunRotation = EquippedWeapon.transform.rotation;

            //Interpolate the gun's rotation to look at the collision point
            EquippedWeapon.transform.rotation = Quaternion.Lerp(oldGunRotation, newGunRotation, gunRotationSpeed * Time.deltaTime);


            //Zero out any x-rotations on the gun. This ensures that the gun doesn't point towards the ground or at the sky
            var anglesDegrees = EquippedWeapon.transform.eulerAngles;
            EquippedWeapon.transform.eulerAngles = new Vector3(0f, anglesDegrees.y, anglesDegrees.z);

            //If there is ammo left and the fire button is pressed
            if (Ammo > 0 && Input.GetButton("Fire1"))
            {
                //Find the muzzle of the equipped weapon
                var muzzle = EquippedWeapon.transform.Find("Muzzle");
                //if no muzzle exists, then throw an exception
                if (muzzle == null)
                {
                    throw new System.Exception("Unable to find the Muzzle for the gun " + EquippedWeapon.gameObject.name);
                }
                
                //If the weapon can be fired
                if (EquippedWeapon.CanFire)
                {
                    //Tell the weapon to shoot a bullet.
                    EquippedWeapon.Shoot(muzzle.transform.position, target);
                    //Decrease the character's ammo
                    Ammo -= EquippedWeapon.AmmoPerShot;
                }
                
            }
        }
    }

    private void FixedUpdate()
    {
        //If we are not on the ground, then do movement by modifying the rigidbody velocity to move in the air
        if (!OnGround)
        {
            //Set the animator floats to zero
            animator.SetFloat(horizontalID, 0f);
            animator.SetFloat(verticalID, 0f);
            
            //Set the x velocity to the horizontal input axis and the z axis to the vertical input axis
            body.velocity = new Vector3(Input.GetAxis("Horizontal") * MovementSpeed, body.velocity.y, Input.GetAxis("Vertical") * MovementSpeed);
        }
    }

    //This function rotates the character to face whereever the mouse pointer is pointing at. Returns the target the mouse is pointing at
    Vector3 LookAtMouse()
    {
        //Create a plane the faces upwards at the player's position. This will create a plane that spans the x and z axis
        Plane mousePlane = new Plane(Vector3.up, transform.position);

        //Create a ray from the current mouse position
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        //If the ray collides with the plane, then we get the distance from the camera to the collided point on the plane
        if (mousePlane.Raycast(mouseRay,out var distanceToPlane))
        {
            //Convert the distance value to a 3d point value. This point is where the ray collided with the plane
            var collisionPoint = mouseRay.GetPoint(distanceToPlane);

            //Store the previous rotation
            var previousRotation = transform.rotation;

            //Look at the collision point
            transform.LookAt(collisionPoint);

            //Get the updated rotation
            var newRotation = transform.rotation;

            //Interpolate from the previous rotation to the new rotation
            transform.rotation = Quaternion.Lerp(previousRotation, newRotation, RotationSpeed * Time.deltaTime);

            return collisionPoint;
        }

        return transform.position;
    }

    //If the player collides with anything
    private void OnCollisionEnter(Collision collision)
    {
        //Set the on ground variable to true
        OnGround = true;

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

        //If the collided object has a "Harm Character" component
        var harmCharacter = collision.gameObject.GetComponent<HarmCharacter>();
        if (harmCharacter != null)
        {
            //Harm the character
            health.Damage(harmCharacter.Damage);
        }
    }

    //If the player triggers something
    private void OnTriggerEnter(Collider other)
    {
        //If the collided object has a "Harm Character" component
        var harmCharacter = other.gameObject.GetComponent<HarmCharacter>();
        if (harmCharacter != null)
        {
            //Harm the character
            health.Damage(harmCharacter.Damage);
        }
    }

    //Tells the player to equip a weapon
    public void EquipWeapon(Weapon weaponPrefab)
    {
        //If the player is already holding a weapon, then unequip it
        if (EquippedWeapon != null)
        {
            UnequipWeapon();
        }

        //Instantiate the weapon
        EquippedWeapon = GameObject.Instantiate(weaponPrefab);

        //Set the weapon's parent to the weapon container
        EquippedWeapon.transform.SetParent(weaponContainer);
        //Reset the local position and local rotation of the weapon
        EquippedWeapon.transform.localPosition = Vector3.zero;
        EquippedWeapon.transform.localRotation = Quaternion.identity;

        //Tell the gun that it has been equipped
        EquippedWeapon.OnEquip(this);

        //Set the "No Weapon" layer to a weight of 0 to stop playing that layer
        animator.SetLayerWeight(1, 0f);
    }

    //Tells the player to unequip a weapon
    public void UnequipWeapon()
    {
        //If there is a weapon equipped
        if (EquippedWeapon != null)
        {
            //Tell the gun that is is about to be unequipped
            EquippedWeapon.OnUnequip(this);
            //Destroy the weapon
            Destroy(EquippedWeapon.gameObject);
            //Nullify the equipped weapon variable
            EquippedWeapon = null;

            //Set the "No Weapon" layer to a weight of 1 to play the layer
            animator.SetLayerWeight(1, 1f);
        }
    }

    //Called any time the animator is doing inverse kinematics
    private void OnAnimatorIK(int layerIndex)
    {
        //If there is a weapon equipped
        if (EquippedWeapon != null)
        {
            //Use it to setup the character's inverse kinematics
            EquippedWeapon.SetCharacterIK(animator);
        }
        //If there is no equipped weapon
        else
        {
            //Reset Inverse Kinematics
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }
    }


    //Destroys the player. Used for the health OnDeath event
    public void DestroyPlayer()
    {
        Destroy(gameObject);
    }
}*/
