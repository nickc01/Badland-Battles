using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameAudioSource : MonoBehaviour
{
	/// <summary>
	/// Plays a sound that is not heard in 3D space
	/// </summary>
	/// <param name="clip">The clip to play</param>
	/// <param name="volume">The volume of the clip</param>
	/// <returns>Returns the instance to the audio source. The audio source is automatically deleted</returns>
	public static GameAudioSource PlayGlobalSound(AudioClip clip, float volume = 1)
	{
		return PlayGlobalSound(clip, AudioDatabase.Instance.SFXAudioGroup, volume);
	}

	/// <summary>
	/// Plays a sound that is not heard in 3D space
	/// </summary>
	/// <param name="clip">The clip to play</param>
	/// <param name="audioGroup">The group to play the audio under</param>
	/// <param name="volume">The volume of the clip</param>
	/// <returns>Returns the instance to the audio source. The audio source is automatically deleted</returns>
	public static GameAudioSource PlayGlobalSound(AudioClip clip, AudioMixerGroup audioGroup, float volume = 1)
	{
		var instance = PlayAudioOnce(clip, Camera.main.transform.position,audioGroup,volume);
		instance.transform.SetParent(Camera.main.transform,true);
		instance.GetComponent<AudioSource>().spatialBlend = 0f;
		return instance;
	}

	/// <summary>
	/// Plays the audio clip at a specified point
	/// </summary>
	/// <param name="clip">The clip to play</param>
	/// <param name="position">Where the audio source is located</param>
	/// <param name="volume">The volume of the clip</param>
	/// <returns>Returns the instance to the audio source. The audio source is automatically deleted</returns>
	public static GameAudioSource PlayAudioOnce(AudioClip clip, Vector3 position, float volume = 1)
	{
		return PlayAudioOnce(clip, position, AudioDatabase.Instance.SFXAudioGroup, volume);
	}

	/// <summary>
	/// Plays the audio clip at a specified point
	/// </summary>
	/// <param name="clip">The clip to play</param>
	/// <param name="position">Where the audio source is located</param>
	/// <param name="audioGroup">The group to play the audio under</param>
	/// <param name="volume">The volume of the clip</param>
	/// <returns>Returns the instance to the audio source. The audio source is automatically deleted</returns>
	public static GameAudioSource PlayAudioOnce(AudioClip clip, Vector3 position, AudioMixerGroup audioGroup, float volume = 1)
	{
		//Create the instance
		var audioInstance = GameObject.Instantiate(AudioDatabase.Instance.AudioPrefab, position, Quaternion.identity);
		var audioSource = audioInstance.GetComponent<AudioSource>();

		//Set the clip, set the volume, set the mixer group, play the audio, and destroy it after a set time
		audioSource.clip = clip;
		audioSource.volume = volume;
		audioSource.outputAudioMixerGroup = audioGroup;
		audioSource.Play();
		Destroy(audioInstance.gameObject, clip.length);
		return audioInstance;
	}
}
