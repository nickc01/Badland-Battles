using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
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

    int currentAmmo;

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

    public int Ammo
    {
        get => currentAmmo;
        set
        {
            currentAmmo = Mathf.Clamp(value, 0, maximumAmmo);
            AmmoHUD.Instance.UpdateAmmo(currentAmmo);
        }
    }


    void Awake()
    {
        //Get the character's animator component
        animator = GetComponent<Animator>();
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

        currentAmmo = startingAmmo;
        AmmoHUD.Instance.SetMaxAmmo(maximumAmmo);
        AmmoHUD.Instance.UpdateAmmoRaw(startingAmmo);
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
        //If the character is to face the mouse pointer
        if (turnTowardsMouse)
        {
            //Look at the mouse pointer
            LookAtMouse();
        }

        //If the character is jumping in the air and the character is falling down
        if (Jumping && body.velocity.y <= 0f)
        {
            //The character is no longer jumping and is switching to the falling state
            Jumping = false;
        }

        //If we are pressing space or pressing the fire button and the character is on the ground
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1")) && OnGround)
        {
            animator.Play("Jumping");
            //Jump
            Jumping = true;
            //The character is no longer on the ground
            OnGround = false;
            //Add a jump force
            body.AddRelativeForce(0f, jumpForce, 0f, ForceMode.Impulse);
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

    //This function rotates the character to face whereever the mouse pointer is pointing at
    void LookAtMouse()
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
        }
    }

    //If the player collides 
    private void OnCollisionEnter(Collision collision)
    {
        OnGround = true;

        var pickup = collision.gameObject.GetComponent<Pickup>();
        if (pickup != null)
        {
            Debug.Log("Touched Pickup = " + pickup.gameObject.name);
            pickup.OnPickup(this);
            if (pickup.DestroyOnPickup)
            {
                Destroy(pickup.gameObject);
            }
        }
    }
}
