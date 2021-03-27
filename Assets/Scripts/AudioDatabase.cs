using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "Audio Database")]
public class AudioDatabase : ScriptableObject
{
	//The singleton for accessing the database anywhere
	public static AudioDatabase Instance;

	//The list of all pistol sounds
	public List<AudioClip> PistolSounds;
	//The list of all shotgun sounds
	public List<AudioClip> ShotgunSounds;
	//The list of all death sounds
	public List<AudioClip> DeathSounds;
	//The sound for reloading
	public AudioClip ReloadSound;

	//The master audio group
	public AudioMixerGroup MasterAudioGroup;
	//The sound effects audio group
	public AudioMixerGroup SFXAudioGroup;
	//The music audio group
	public AudioMixerGroup MusicAudioGroup;

	//The prefab for game audio sources
	public GameAudioSource AudioPrefab;


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	//When the game starts
	static void LoadDatabase()
	{
		//Load the audio database from the resources folder
		Instance = Resources.Load<AudioDatabase>("Game Audio Database");
	}
}

