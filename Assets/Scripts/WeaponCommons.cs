using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/*[CreateAssetMenu(fileName = "WeaponCommons",menuName = "Weapon Commons")]
public sealed class WeaponCommons : ScriptableObject
{
	[SerializeField]
	[Tooltip("The prefab that is instantiated at the nuzzle of the gun to replicate a gun flash")]
	GameObject gunFirePrefab;
	public GameObject GunFirePrefab => gunFirePrefab;
	[SerializeField]
	[Tooltip("The prefab that is instantiated at the target that replicates a bullet hit")]
	GameObject gunHitPrefab;
	public GameObject GunHitPrefab => gunHitPrefab;
	[SerializeField]
	[Tooltip("The prefab that is used to represent the path of the bullet from the source to the target. This can be used to create a ray that shows where the bullet traveled")]
	GameObject gunRayPrefab;
	public GameObject GunRayPrefab => gunRayPrefab;

	//The singleton for accessing the common weapon properties anywhere
	public static WeaponCommons Instance { get; private set; }

	//Called when the game starts. This isn't always called in the editor, so OnEnable is also used
	private void Awake()
	{
		//Set the singleton
		Instance = this;
	}

	//Called when the scriptable object is enabled
	private void OnEnable()
	{
		//Set the singleton
		Instance = this;
	}
}*/

