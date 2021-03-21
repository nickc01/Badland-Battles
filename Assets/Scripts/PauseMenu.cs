using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The position the menu will be positioned when unpaused")]
	RectTransform unpausedPosition;
	[SerializeField]
	[Tooltip("The position the menu will be positioned when paused")]
	RectTransform pausedPosition;
	[SerializeField]
	[Tooltip("Determines how the pause menu will move")]
	AnimationCurve pauseMovementCurve;
	[SerializeField]
	[Tooltip("How long the pause menu takes to move around")]
	float pauseMovementTime = 0.5f;

	//The transform of the pause menu
	RectTransform rTransform;
	//The transform of the target position
	RectTransform target;

	private void Awake()
	{
		//Get the pause menu's rect transform
		rTransform = GetComponent<RectTransform>();
		target = unpausedPosition;
	}

	private void Update()
	{
		//If a target defined
		if (target != null)
		{
			//Set the pause menu's position to the target
			rTransform.localPosition = target.localPosition;
		}
	}

	/// <summary>
	/// Shows the pause menu
	/// </summary>
	public void ShowPauseMenu()
	{
		//Stop any routines if any are running
		StopAllCoroutines();
		//Do not set it's position to a target
		target = null;

		StartCoroutine(LerpRoutine(unpausedPosition,pausedPosition));
	}

	/// <summary>
	/// Hides the pause menu
	/// </summary>
	public void HidePauseMenu()
	{
		//Stop any routines if any are running
		StopAllCoroutines();
		//Do not set it's position to a target
		target = null;

		StartCoroutine(LerpRoutine(pausedPosition,unpausedPosition));
	}

	IEnumerator LerpRoutine(RectTransform from, RectTransform to)
	{
		//Interpolate from the "from" position to the "to" position
		for (float t = 0; t < pauseMovementTime; t += Time.unscaledDeltaTime)
		{
			transform.localPosition = Vector3.Lerp(from.localPosition,to.localPosition, pauseMovementCurve.Evaluate(t / pauseMovementTime));
			yield return null;
		}

		//Set the target to the destination transform
		target = to;
	}
}
