using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRay : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The color to start on")]
	Color startingColor;
	[SerializeField]
	[Tooltip("The color to end on")]
	Color endColor;
	[SerializeField]
	[Tooltip("How long it will take to transition from the starting color to the end color")]
	float colorTime;

	//The internal timer
	float timer = 0f;

	//The property block used to set the color
	MaterialPropertyBlock materialBlock;
	//The renderer of the ray
	Renderer rayRenderer;

	private void Awake()
	{
		//Get the renderer and setup the property block
		rayRenderer = GetComponent<Renderer>();
		materialBlock = new MaterialPropertyBlock();

		//Get the renderer info
		rayRenderer.GetPropertyBlock(materialBlock);
		//Set the property block's color
		materialBlock.SetColor("_MainColor", startingColor);
		//Set the renderer info
		rayRenderer.SetPropertyBlock(materialBlock);
	}

	private void Update()
	{
		//Increase the timer per frame
		timer += Time.deltaTime;

		//if the timer is greater than or equal to the color time
		if (timer >= colorTime)
		{
			//Destroy the object
			Destroy(gameObject);
		}
		else
		{
			//Get the renderer info
			rayRenderer.GetPropertyBlock(materialBlock);
			//Set the property block's color. The color is interpolated between the starting color and end color
			materialBlock.SetColor("_MainColor", Color.Lerp(startingColor,endColor,timer / colorTime));
			//Set the renderer info
			rayRenderer.SetPropertyBlock(materialBlock);
		}
	}
}
