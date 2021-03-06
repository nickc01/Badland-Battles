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
				if (spawnedEnemies == null)
				{
					spawnedEnemies.RemoveAt(i);
				}
			}
			return spawnedEnemies.Count;
		}
	}

	List<EnemyController> spawnedEnemies = new List<EnemyController>();
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
		yield return new WaitForSeconds(startDelay);

		for (CurrentWave = startingWave; CurrentWave < Waves.Count; CurrentWave++)
		{
			onWaveStart.Invoke(Waves[CurrentWave].WaveName);

			foreach (var spawn in Waves[CurrentWave].WaveSpawns)
			{
				var amount = spawn.amountOfEnemies;

				if (amount <= 0)
				{
					amount = 1;
				}
				EnemiesLeftToSpawn += amount;
				StartCoroutine(SpawnEnemies(spawn));
			}

			yield return new WaitUntil(() => EnemiesLeftToSpawn == 0);
			yield return new WaitUntil(() => EnemiesLeftAlive == 0);
			yield return new WaitForSeconds(Waves[CurrentWave].EndDelay);
		}

	}

	IEnumerator SpawnEnemies(Spawn spawnInfo)
	{
		yield return new WaitForSeconds(spawnInfo.Delay);

		var amount = spawnInfo.amountOfEnemies;

		if (amount <= 0)
		{
			amount = 1;
		}

		for (int i = 0; i < amount; i++)
		{
			spawnedEnemies.Add(Instantiate(spawnInfo.Enemy,spawnInfo.spawner.transform.position,Quaternion.identity));
			EnemiesLeftToSpawn--;
			yield return new WaitForSeconds(spawnInfo.timeBetweenSpawns);
		}
	}
}
