using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
	[SerializeField]
	[Tooltip("How long the music will take to fade in")]
	float fadeInTime = 0.5f;

	[SerializeField]
	[Tooltip("How long the music will take to fade out")]
	float fadeOutTime = 0.3f;

	[SerializeField]
	[Tooltip("The volume of the music player")]
	float volume = 1f;

	[SerializeField]
	[Tooltip("Whether the music player should fade in on awake")]
	bool fadeInOnAwake = true;

	//The audio source
	new AudioSource audio;

	private void Awake()
	{
		//Get the audio source
		audio = GetComponent<AudioSource>();
		//If the player is fading on awake
		if (fadeInOnAwake)
		{
			//Mute the volume
			audio.volume = 0;
			//Fade the music in
			FadeInMusic();
		}
		else
		{
			//If not fading in on awake, then set the volume to the configured value
			audio.volume = volume;
		}
	}

	/// <summary>
	/// Fades the music in
	/// </summary>
	public void FadeInMusic()
	{
		//Stop any running coroutines
		StopAllCoroutines();
		//Start the fade in routine
		StartCoroutine(FadeInRoutine());
	}

	//Fades in the music
	IEnumerator FadeInRoutine()
	{
		//Store the old volume
		float oldVolume = audio.volume;
		//Interpolate to the new volume level
		for (float i = 0; i < fadeInTime; i += Time.deltaTime)
		{
			audio.volume = Mathf.Lerp(oldVolume,volume,i / fadeInTime);
			yield return null;
		}
		audio.volume = volume;
	}

	/// <summary>
	/// Fades the music out
	/// </summary>
	public void FadeOutMusic()
	{
		//Stop any running coroutines
		StopAllCoroutines();
		//Start the fade out routine
		StartCoroutine(FadeOutRoutine());
	}

	/// <summary>
	/// Fades the music out after a set amount of time
	/// </summary>
	/// <param name="time">The delay before fading out</param>
	public void FadeOutMusicAfterTime(float time)
	{
		//Stop any running coroutines
		StopAllCoroutines();
		//Start the fade out after time routine
		StartCoroutine(FadeAfterTimeRoutine(time));
	}

	//Fades the music out after a set amount of time
	IEnumerator FadeAfterTimeRoutine(float time)
	{
		//Wait the specified delay
		yield return new WaitForSeconds(time);
		//Fade the music out
		yield return FadeOutRoutine();
	}

	//Fades the music out
	IEnumerator FadeOutRoutine()
	{
		//Store the old volume
		float oldVolume = audio.volume;
		//Interpolate to the new volume level
		for (float i = 0; i < fadeInTime; i += Time.deltaTime)
		{
			audio.volume = Mathf.Lerp(oldVolume, 0, i / fadeInTime);
			yield return null;
		}
		audio.volume = 0;
	}
}
