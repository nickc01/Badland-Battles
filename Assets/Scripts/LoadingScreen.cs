using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The curve for determining how smoothly the loading screen fades")]
	AnimationCurve fadeCurve;
	[SerializeField]
	[Tooltip("How fast the loading screen fades in or fades out")]
	float fadeTime = 0.5f;

	[SerializeField]
	[Tooltip("The black background of the loading screen")]
	RawImage loadingScreenBackground;
	[SerializeField]
	[Tooltip("The loading text of the loading screen")]
	TextMeshProUGUI loadingScreenText;

	//The original color of the background
	Color backgroundColor;
	//The original color of the loading text
	Color textColor;

	private void Awake()
	{
		//Store the original color of the background
		backgroundColor = loadingScreenBackground.color;
		//Make the background transparent
		loadingScreenBackground.color = new Color(backgroundColor.r,backgroundColor.g,backgroundColor.g,0f);
		//Enable the background
		loadingScreenBackground.enabled = true;

		//Store the original color of the loading text
		textColor = loadingScreenText.color;
		//Make the loading text transparent
		loadingScreenText.color = new Color(textColor.r, textColor.g, textColor.g, 0f);
		//Enable the loading text
		loadingScreenText.enabled = true;

		//Prevent the loading screen from being destroyed on a new scene being loaded
		DontDestroyOnLoad(gameObject);

		//Start the fade in routine
		StartCoroutine(FadeIn());

		//Hook into the on game load event
		GameLoader.OnGameLoad += OnGameLoad;
	}

	//Called when the game scene has been loaded
	private void OnGameLoad(UnityEngine.SceneManagement.Scene obj)
	{
		//Stop any fade routines if there are any running
		StopAllCoroutines();
		//Start the fade out routine
		StartCoroutine(FadeOut());
		//Unhook from the on game load routine
		GameLoader.OnGameLoad -= OnGameLoad;
	}

	//Fades the loading screen in to view
	IEnumerator FadeIn()
	{
		//Wait a starting delay
		yield return new WaitForSeconds(0.1f);

		//Interpolate from the transparent colors to the original colors so the background and loading text fade into view
		var oldBackgroundColor = loadingScreenBackground.color;
		var oldTextColor = loadingScreenText.color;

		for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
		{
			loadingScreenBackground.color = Color.Lerp(oldBackgroundColor, backgroundColor,fadeCurve.Evaluate(t / fadeTime));
			loadingScreenText.color = Color.Lerp(oldTextColor, textColor, fadeCurve.Evaluate(t / fadeTime));

			yield return null;
		}

		loadingScreenBackground.color = backgroundColor;
		loadingScreenText.color = textColor;
	}

	//Fades the loading screen out of view
	IEnumerator FadeOut()
	{
		//Store the old color of the loading screen
		var oldBackgroundColor = loadingScreenBackground.color;
		var oldTextColor = loadingScreenText.color;

		//Store the transparent versions of the loading screen
		var newBackgroundColor = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.g, 0f);
		var newTextColor = new Color(textColor.r, textColor.g, textColor.g, 0f);

		//Fade the background and loading text from the old colors to the transparent ones
		for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
		{
			loadingScreenBackground.color = Color.Lerp(oldBackgroundColor, newBackgroundColor, fadeCurve.Evaluate(t / fadeTime));
			loadingScreenText.color = Color.Lerp(oldTextColor, newTextColor, fadeCurve.Evaluate(t / fadeTime));

			yield return null;
		}

		loadingScreenBackground.color = newBackgroundColor;
		loadingScreenText.color = newTextColor;

		//Disable the loading background and text
		loadingScreenText.enabled = false;
		loadingScreenBackground.enabled = false;
		//Destroy the entire loading screen
		Destroy(gameObject);
	}
}
