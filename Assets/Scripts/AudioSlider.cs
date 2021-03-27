using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
	//The currently stored audio mixer data
	static AudioMixerTable _mixerData;
	static AudioMixerTable MixerData
	{
		get
		{
			//Load the data if it's not loaded already
			LoadAudioData();
			return _mixerData;
		}
	}

	[Serializable]
	class AudioMixerTable
	{
		//A list of all the gorup data
		public List<GroupInfo> GroupData = new List<GroupInfo>();

		//Adds an audio mixer group to the list. This is used to store the data in PlayerPrefs
		public void AddGroup(AudioMixerGroup group)
		{
			//If the group is not already added
			if (!GroupData.Any(gi => gi.Group == group))
			{
				//Add the new group data
				GroupData.Add(new GroupInfo
				{
					Group = group,
					Volume = 0f
				});
			}
		}

		//Gets the stored volume of a mixer group
		public float GetGroupVolume(AudioMixerGroup group)
		{
			var groupInfo = GroupData.FirstOrDefault(gi => gi.Group == group);
			//If the group is in the list
			if (groupInfo != null)
			{
				return groupInfo.Volume;
			}
			//If the group is not in the list
			else
			{
				return 0f;
			}
		}

		//Sets the stored volume for a mixer group
		public void SetGroupVolume(AudioMixerGroup group, float volume)
		{
			var groupInfo = GroupData.FirstOrDefault(gi => gi.Group == group);
			//If the group is not in the list
			if (groupInfo == null)
			{
				GroupData.Add(new GroupInfo
				{
					Group = group,
					Volume = volume
				});
			}
			//If the group is in the list
			else
			{
				groupInfo.Volume = volume;
			}
			//Set the mixer group's volume
			group.audioMixer.SetFloat(group.name + " Volume", volume);
		}
	}

	[Serializable]
	class GroupInfo
	{
		//The group that is being stored
		public AudioMixerGroup Group;
		//The volume for that group
		public float Volume;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	//When the game starts
	static void LoadAudioData()
	{
		//If the mixer data is not loaded yet
		if (_mixerData == null)
		{
			//If there is a key for it in playerprefs
			if (PlayerPrefs.HasKey("AudioMixerData"))
			{
				//Load from playerprefs and convert it from json to the object
				_mixerData = JsonUtility.FromJson<AudioMixerTable>(PlayerPrefs.GetString("AudioMixerData"));
				//Run the startup routine
				UnboundRoutine.StartUnboundRoutine(StartupRoutine());
			}
			//If there is no key for it in playerprefs
			else
			{
				//Create a new object
				_mixerData = new AudioMixerTable();
			}
		}
	}

	/// <summary>
	/// There is a bug where attempting to set the mixer volume during Awake() fails, So the setting of the volume has to be delayed using a coroutine
	/// </summary>
	/// <returns></returns>
	static IEnumerator StartupRoutine()
	{
		//Wait a frame
		yield return null;
		//Loop over each group in the data set
		foreach (var data in MixerData.GroupData)
		{
			//Set the volume of each group to their stored volume levels
			data.Group.audioMixer.SetFloat(data.Group.name + " Volume", data.Volume);
		}
	}

	//Saves the Audio data to playerprefs
	static void SaveAudioData()
	{
		PlayerPrefs.SetString("AudioMixerData", JsonUtility.ToJson(MixerData));
		PlayerPrefs.Save();
	}

	[SerializeField]
	[Tooltip("The mixer group the slider is bound to")]
	AudioMixerGroup mixerGroup;

	//The slider component
	Slider slider;

	private void Awake()
	{
		//Get the slider component
		slider = GetComponentInChildren<Slider>();
		//Adds the configured mixer group to the global audio data
		MixerData.AddGroup(mixerGroup);

		//Configure the slider values
		slider.minValue = -80f;
		slider.maxValue = 0f;
		slider.value = MixerData.GetGroupVolume(mixerGroup);

		slider.onValueChanged.AddListener(OnSliderValueChange);
	}

	//Called when the slider value changes
	void OnSliderValueChange(float value)
	{
		//Set the group volume to the new value
		MixerData.SetGroupVolume(mixerGroup, value);
		//Save the audio data
		SaveAudioData();
	}


}
