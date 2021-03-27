using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSystem : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Determines how smooth the movement will be between slides")]
	AnimationCurve slidingCurve;
	[SerializeField]
	[Tooltip("How long it will take to move to a new slide")]
	float slideTime = 1f;
	[SerializeField]
	List<Slide> Slides;


	Slide CurrentSlide;
	RectTransform rTransform;
	//The event system that is used for player input
	EventSystem eventSystem;

	private void Awake()
	{
		eventSystem = GameObject.FindObjectOfType<EventSystem>();
		rTransform = GetComponent<RectTransform>();
		//Find the slide that is closest to the position 0,0. Use that as the current slide
		float distanceToCenter = float.PositiveInfinity;
		foreach (var slide in Slides)
		{
			var slideDistance = Vector3.Distance(slide.transform.localPosition,Vector3.zero);
			if (slideDistance < distanceToCenter)
			{
				distanceToCenter = slideDistance;
				CurrentSlide = slide;
			}
		}
	}

	//Slides from one slide to another
	void Slide(Slide from, Slide to)
	{
		//Make sure the slides are defined
		if (from == null || to == null || from != CurrentSlide || to == CurrentSlide)
		{
			return;
		}

		//Stop any sliding routines if any are running
		StopAllCoroutines();

		CurrentSlide = to;

		//Disable the event system so the player doesn't click on anything during the transition
		if (eventSystem != null)
		{
			eventSystem.enabled = false;
		}

		//Start the sliding routine
		StartCoroutine(SliderRoutine(to));
	}

	//Slide to the destination slide
	public void SlideTo(Slide to)
	{
		Slide(CurrentSlide, to);
	}

	//Slides to the slide with the specified name
	public void SlideTo(string slideName)
	{
		Slide(CurrentSlide, Slides.FirstOrDefault(s => s.SlideName == slideName));
	}


	IEnumerator SliderRoutine(Slide destinationSlide)
	{
		//Get the transform of the destination slide
		var destTransform = destinationSlide.GetComponent<RectTransform>();

		//Store the current position the menu system is located at
		var currentPosition = rTransform.anchoredPosition;
		//Store the destination position the menu system will move towards
		var destinationPosition = -destTransform.anchoredPosition;

		//Interpolate from the current position to the destination position
		for (float t = 0; t < slideTime; t += Time.unscaledDeltaTime)
		{
			rTransform.anchoredPosition = Vector3.Lerp(currentPosition, destinationPosition, slidingCurve.Evaluate(t / slideTime));

			yield return null;
		}

		rTransform.anchoredPosition = destinationPosition;
		//Enable the event system so the user can click on buttons
		if (eventSystem != null)
		{
			eventSystem.enabled = true;
		}

		yield break;
	}

	//Called from the "Play" button to load the specified scene
	public void LoadScene(string sceneName)
	{
		//Load the game scene
		GameLoader.LoadScene(sceneName);
	}

	//Called from the "Quit" button to quit the game
	public void QuitGame()
	{
#if UNITY_EDITOR
		if (UnityEditor.EditorApplication.isPlaying)
		{
			UnityEditor.EditorApplication.ExitPlaymode();
		}
#else
		Application.Quit();
#endif
	}

	//Plays a UI sound effect
	public void PlaySoundEffect(AudioClip clip)
	{
		//Play a sound effect that is not used in 3D space
		GameAudioSource.PlayGlobalSound(clip);
	}
}
