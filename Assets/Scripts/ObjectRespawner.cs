using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RespawnInfo
{
	public string Name; //The name of the original object
	public GameObject Copy; //The copy that the respawned object will be instantiated from
	public Vector3 Position; //The position of the original object
	public Quaternion Rotation; //The rotation of the original object
	public Vector3 Size; //The size of the original object
	public Transform Parent; //The parent of the original object
	public bool EnabledState; //The active state of the original object
}



public class ObjectRespawner : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The amount of time an object takes to respawn. Note that this is approximate, not exact")]
	float respawnTime = 5f;
	public float RespawnTime => respawnTime;


	[SerializeField]
	[Tooltip("A list of objects to respawn")]
	List<GameObject> ObjectsToRespawn;

	//A list that contains all the necessary information for respawning objects
	List<RespawnInfo> RespawnList;

	private void Awake()
	{
		RespawnList = new List<RespawnInfo>();

		foreach (var obj in ObjectsToRespawn)
		{
			PrepareObject(obj);
		}
		StartCoroutine(RespawnChecker());
	}

	//Prepares an object for respawning
	void PrepareObject(GameObject obj)
	{
		//Spawn the copy with the same information as the original
		var copy = GameObject.Instantiate(obj, obj.transform.position, obj.transform.rotation, obj.transform.parent);

		//Disable the copy
		copy.SetActive(false);
		//Remove the copy from the original parent
		copy.transform.SetParent(null);
		//Hide the copy in from view in the hierarchy, since it doesn't need to be seen
		copy.hideFlags = HideFlags.HideInHierarchy;

		//Compile all this information together so that the object can be respawned later
		RespawnList.Add(new RespawnInfo
		{
			Name = obj.name,
			Copy = copy,
			EnabledState = obj.activeSelf,
			Parent = obj.transform.parent,
			Position = obj.transform.position,
			Rotation = obj.transform.rotation,
			Size = obj.transform.localScale
		});
	}

	/// <summary>
	/// Makes an object respawnable. When it is destroyed, it will automatically be respawned after a specific amount of time, specified by <see cref="RespawnTime"/>
	/// </summary>
	/// <param name="obj">The object to make respawnable</param>
	public void MakeObjectRespawnable(GameObject obj)
	{
		PrepareObject(obj);
	}

	//Continously checks of objects need to be respawned
	IEnumerator RespawnChecker()
	{
		while (true)
		{
			//Check if any objects have been destroyed every 0.5 seconds
			yield return new WaitForSeconds(0.5f);

			//Loop over each object in the ObjectsToRespawn list. Loop backwards in case objects get removed during looping
			for (int i = ObjectsToRespawn.Count - 1; i >= 0; i--)
			{
				//If the object has been destroyed
				if (ObjectsToRespawn[i] == null)
				{
					//Respawn the object
					StartCoroutine(RespawnObject(RespawnList[i], respawnTime));

					//Remove the original and the copy
					ObjectsToRespawn.RemoveAt(i);
					RespawnList.RemoveAt(i);
				}
			}
		}
	}

	IEnumerator RespawnObject(RespawnInfo respawnInfo, float time)
	{
		//Wait for the respawn time to occur
		yield return new WaitForSeconds(time);

		//Spawn the instance
		var instance = GameObject.Instantiate(respawnInfo.Copy, respawnInfo.Position, respawnInfo.Rotation, respawnInfo.Parent);
		//Reset it's name
		instance.name = respawnInfo.Name;
		//Set it to the same enable state as the original
		instance.SetActive(respawnInfo.EnabledState);
		//Set the scale to the original
		instance.transform.localScale = respawnInfo.Size;

		//Add the new instance to the respawn list so that it can respawn again
		ObjectsToRespawn.Add(instance);
		RespawnList.Add(respawnInfo);
	}
}
