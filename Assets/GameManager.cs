using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	//The singleton for accessing the game manager anywhere
	public static GameManager Instance { get; private set; }

	[Header("Weapon System")]
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

	[Space]
	[Header("Health System")]
	[SerializeField]
	[Tooltip("The health marker prefab")]
	HealthMarker markerPrefab;
	public HealthMarker MarkerPrefab => markerPrefab;


	private void Awake()
	{
		Instance = this;
	}
}
