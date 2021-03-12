using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public sealed class Wave
{
	[Tooltip("The name of the wave")]
	public string WaveName = "WAVE_NAME";
	[Tooltip("A list of spawn instructions that will dictate how the wave will spawn the enemies")]
	public List<Spawn> WaveSpawns;
	[Tooltip("The delay before the next wave begins")]
	public float EndDelay = 10f;
}

[Serializable]
public sealed class Spawn
{
	[Tooltip("The enemy prefab to instantiate")]
	public EnemyController Enemy;
	[Tooltip("The spawner the enemy will spawn at")]
	public EnemySpawner spawner;
	[Space]
	[Tooltip("The delay before the spawn occurs")]
	public float Delay;
	[Space]
	[Tooltip("How many enemies will spawn")]
	public int amountOfEnemies = 1;
	[Tooltip("If Amount Of Enemies is set to more than one enemy, this will represent the time between each enemy instantiation")]
	public float timeBetweenSpawns = 0.5f;
}


public class EnemyWaveManager : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The delay before the waves start")]
	float startDelay = 10f;
	[SerializeField]
	[Tooltip("The wave the game will start on. 0 represents the first wave")]
	int startingWave = 0;
	[SerializeField]
	[Tooltip("All the possible waves in the game")]
	List<Wave> Waves;
	[SerializeField]
	[Tooltip("Called when a new wave starts")]
	UnityEvent<string> onWaveStart;

	/// <summary>
	/// The current wave the wave manager is on
	/// </summary>
	public int CurrentWave { get; private set; }

	/// <summary>
	/// How many enemies are left to spawn in the wave
	/// </summary>
	public int EnemiesLeftToSpawn { get; private set; }

	/// <summary>
	/// How many enemies are left alive
	/// </summary>
	public int EnemiesLeftAlive
	{
		get
		{
			for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
			{
				if (spawnedEnemies[i] == null)
				{
					spawnedEnemies.RemoveAt(i);
				}
			}
			return spawnedEnemies.Count;
		}
	}

	List<EnemyController> spawnedEnemies = new List<EnemyController>();
	/// <summary>
	/// The list of currently spawned enemies
	/// </summary>
	public IEnumerable<EnemyController> SpawnedEnemies
	{
		get
		{
			foreach (var enemy in spawnedEnemies)
			{
				if (enemy != null)
				{
					yield return enemy;
				}
			}
		}
	}



	/// <summary>
	/// Called when a new wave starts
	/// </summary>
	public event UnityAction<string> OnWaveStart
	{
		add => onWaveStart.AddListener(value);
		remove => onWaveStart.RemoveListener(value);
	}

	private void Start()
	{
		StartCoroutine(WaveRunner());
	}


	IEnumerator WaveRunner()
	{
		//Waits for the starting delay
		yield return new WaitForSeconds(startDelay);

		//Loop over each wave in the wave manager
		for (CurrentWave = startingWave; CurrentWave < Waves.Count; CurrentWave++)
		{
			//Trigger the wave start event
			onWaveStart.Invoke(Waves[CurrentWave].WaveName);

			//Loop over each of the enemy spawns
			foreach (var spawn in Waves[CurrentWave].WaveSpawns)
			{
				//Get the amount of enemies to spawn
				var amount = spawn.amountOfEnemies;

				if (amount <= 0)
				{
					amount = 1;
				}
				EnemiesLeftToSpawn += amount;
				//Start a new routine to spawn the enemies
				StartCoroutine(SpawnEnemies(spawn));
			}

			//Wait until all the enemies have been spawned
			yield return new WaitUntil(() => EnemiesLeftToSpawn == 0);
			//Wait until all the enemies are dead
			yield return new WaitUntil(() => EnemiesLeftAlive == 0);
			//Wait for the ending delay
			yield return new WaitForSeconds(Waves[CurrentWave].EndDelay);
		}
	}

	//Used to spawn the enemies
	IEnumerator SpawnEnemies(Spawn spawnInfo)
	{
		//Wait for the specified delay
		yield return new WaitForSeconds(spawnInfo.Delay);

		//Get the amount of enemies to spawn
		var amount = spawnInfo.amountOfEnemies;

		if (amount <= 0)
		{
			amount = 1;
		}

		//Loop over all the enemies to spawn
		for (int i = 0; i < amount; i++)
		{
			//instantiate the enemy at the specified spawner
			spawnedEnemies.Add(Instantiate(spawnInfo.Enemy,spawnInfo.spawner.transform.position,Quaternion.identity));
			EnemiesLeftToSpawn--;
			//Wait for the time between spawns
			yield return new WaitForSeconds(spawnInfo.timeBetweenSpawns);
		}
	}
}
