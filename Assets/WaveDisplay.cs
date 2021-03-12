using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class WaveDisplay : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The object used to display the wave name")]
	TextMeshProUGUI waveTextObject;
	[SerializeField]
	[Tooltip("The movement curve that determines how smooth the movement of the UI is")]
	AnimationCurve movementCurve;
	[SerializeField]
	[Tooltip("How fast the movement will play")]
	float movementSpeed = 1f;
	[SerializeField]
	[Tooltip("How long the text will display on screen before it scrolls back up")]
	float screenTime = 5f;

	float _timer = 0f;

	public float TextPivot
	{
		get => waveTextObject.rectTransform.pivot.y;
		set
		{
			var oldPivot = waveTextObject.rectTransform.pivot;
			waveTextObject.rectTransform.pivot = new Vector2(oldPivot.x,value);
		}
	}

	public void DisplayNewWave(string waveName)
	{
		StopAllCoroutines();
		waveTextObject.text = waveName;
		StartCoroutine(Movement());
	}


	IEnumerator Movement()
	{
		waveTextObject.gameObject.SetActive(true);
		while (_timer < 1f)
		{
			_timer += movementSpeed * Time.deltaTime;

			TextPivot = movementCurve.Evaluate(_timer);

			yield return null;
		}

		_timer = 1f;
		TextPivot = movementCurve.Evaluate(_timer);

		yield return new WaitForSeconds(screenTime);

		while (_timer > 0f)
		{
			_timer -= movementSpeed * Time.deltaTime;

			TextPivot = movementCurve.Evaluate(_timer);

			yield return null;
		}

		_timer = 0f;
		waveTextObject.gameObject.SetActive(false);
	}
}
