using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//[RequireComponent(typeof(Animator))]
public abstract class WeaponUser : MonoBehaviour
{
    [Space]
    [Header("Weapon User Properties")]
    [SerializeField]
    [Tooltip("The container object used for holding the gun")]
    Transform weaponContainer;
    [SerializeField]
    [Tooltip("The amount of ammo the character will start with")]
    int ammo;
    [SerializeField]
    [Tooltip("The maximum amount of ammo the character can hold")]
    int maxAmmo;
    [SerializeField]
    [Tooltip("If set to true, then all guns that get equipped will require ammo in order to be shot")]
    bool gunsRequireAmmo = true;
    [SerializeField]
    [Tooltip("If true, the gun will be rotated to point towards a specific target")]
    bool gunPointsTowardsPoint = true;
    //[SerializeField]
    //[Tooltip("The target the gun should point towards, assuming \"Gun Points Towards Point\" is set to true")]
    Vector3 weaponTarget;
    [SerializeField]
    [Tooltip("How fast the gun should rotate towards the target")]
    float gunRotationSpeed = 2f;
    [SerializeField]
    [Tooltip("If set to true, the gun rotation will not tilt up and down")]
    bool keepGunLevel = true;
    [SerializeField]
    [Tooltip("These are layers that the weapon user cannot shoot through. Used in the InLineOfSight Function")]
    LayerMask weaponObstacleLayers;

    public bool GunsRequireAmmo => gunsRequireAmmo;
    public bool GunPointsTowardsPoint
    {
        get => gunPointsTowardsPoint;
        set => gunPointsTowardsPoint = value;
    }
    public float GunRotationSpeed => gunRotationSpeed;
    public bool KeepGunLevel
    {
        get => keepGunLevel;
        set => keepGunLevel = value;
    }
    public int MaxAmmo => maxAmmo;

    public Vector3 WeaponTarget
    {
        get => weaponTarget;
        set => weaponTarget = value;
    }

    public int Ammo
    {
        get => ammo;
        set
        {
            var newValue = Mathf.Clamp(value,0,maxAmmo);
            if (ammo != newValue)
            {
                ammo = newValue;
                OnAmmoChange(ammo);
            }
        }
    }

    Animator _animator;
    public Animator animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            return _animator;
        }
    }


    public Transform WeaponContainer => weaponContainer;

    public bool CanFireWeapon => EquippedWeapon != null && EquippedWeapon.CanFire && (!GunsRequireAmmo || Ammo >= EquippedWeapon.AmmoPerShot);

    /// <summary>
    /// The currently equipped weapon
    /// </summary>
    public Weapon EquippedWeapon { get; private set; }

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

        OnEquip();
    }

    //Tells the player to unequip a weapon
    public void UnequipWeapon()
    {
        //If there is a weapon equipped
        if (EquippedWeapon != null)
        {
            OnUnequip();
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

    protected virtual void Update()
    {
        if (EquippedWeapon != null)
        {
            var oldGunRotation = EquippedWeapon.transform.rotation;

            EquippedWeapon.transform.LookAt(weaponTarget);

            Quaternion newGunRotation = Quaternion.identity;

            if (gunPointsTowardsPoint)
            {
                newGunRotation = EquippedWeapon.transform.rotation;
            }
            else
            {
                newGunRotation = EquippedWeapon.transform.parent.rotation;
            }

            //Interpolate the gun's rotation to look at the collision point
            EquippedWeapon.transform.rotation = Quaternion.Lerp(oldGunRotation, newGunRotation, gunRotationSpeed * Time.deltaTime);


            //Zero out any x-rotations on the gun. This ensures that the gun doesn't point towards the ground or at the sky
            if (keepGunLevel)
            {
                var anglesDegrees = EquippedWeapon.transform.eulerAngles;
                EquippedWeapon.transform.eulerAngles = new Vector3(0f, anglesDegrees.y, anglesDegrees.z);
            }
        }
    }

    /// <summary>
    /// Called whenever a gun is equipped
    /// </summary>
    protected virtual void OnEquip()
    {

    }

    /// <summary>
    /// Called whenever a gun is unequipped
    /// </summary>
    protected virtual void OnUnequip()
    {

    }

    /// <summary>
    /// Called whenever the amount of ammo changes
    /// </summary>
    /// <param name="ammo">The new amount of ammo</param>
    protected virtual void OnAmmoChange(int ammo)
    {

    }

    //Called any time the animator is doing inverse kinematics
    //Make it virtual in case derived classes need it
    protected virtual void OnAnimatorIK(int layerIndex)
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

    public bool InLineOfSight(Vector3 target)
    {
        if (EquippedWeapon == null)
        {
            return false;
        }

        var muzzle = EquippedWeapon.transform.Find("Muzzle");
        //if no muzzle exists, then throw an exception
        if (muzzle == null)
        {
            throw new System.Exception("Unable to find the Muzzle for the gun " + EquippedWeapon.gameObject.name);
        }

        var ray = new Ray(muzzle.transform.position, (target - muzzle.transform.position).normalized);

        if (Physics.Raycast(ray,Vector3.Distance(target,muzzle.transform.position), weaponObstacleLayers.value))
        {
            //Debug.Log("In line of sight " + true.ToString());
            return false;
        }
        else
        {
            //Debug.Log("In line of sight " + false.ToString());
            return true;
        }
    }

    public void FireWeapon(Vector3 target)
    {
        if (CanFireWeapon)
        {
            var muzzle = EquippedWeapon.transform.Find("Muzzle");
            //if no muzzle exists, then throw an exception
            if (muzzle == null)
            {
                throw new System.Exception("Unable to find the Muzzle for the gun " + EquippedWeapon.gameObject.name);
            }

            //Tell the weapon to shoot a bullet.
            EquippedWeapon.Shoot(muzzle.transform.position, target);

            //Decrease the character's ammo
            if (GunsRequireAmmo)
            {
                Ammo -= EquippedWeapon.AmmoPerShot;
            }
        }
    }
}

