using TMPro;
using UnityEngine;



/// <summary>
/// A base class for all UI HUDS that use color scrolling
/// </summary>
public abstract class ColorScrollHUD : MonoBehaviour
{
	[SerializeField]
	[Tooltip("How fast the colored bars will animate towards a new value")]
	protected float colorAnimationSpeed = 4f;
	[SerializeField]
	[Tooltip("Represents the foreground of the color bar")]
	RectTransform colorBarTop;
	[SerializeField]
	[Tooltip("Represents the background of the color bar")]
	RectTransform colorBarBottom;
	[SerializeField]
	[Tooltip("Used to display a number in the color bar")]
	TextMeshProUGUI numberText;

	//Stores the current percentage value of the color bar
	float currentPercentage = 1f;

	public RectTransform ColorBarTop => colorBarTop;
	public RectTransform ColorBarBottom => colorBarBottom;


	protected virtual void Update()
	{
		//Get the new width of the color bar
		float newWidth = -Mathf.Lerp(ColorBarBottom.rect.width, 0f, currentPercentage);

		//Store the old size of the color bar
		var oldSize = ColorBarTop.sizeDelta;

		//Interpolate to the new width of the color bar
		ColorBarTop.sizeDelta = new Vector2(Mathf.Lerp(oldSize.x, newWidth, colorAnimationSpeed * Time.deltaTime), oldSize.y);
	}

	/// <summary>
	/// Sets the color bar progress to a new value. The value is divided by the maxValue to represent a percentage on how filled the color bar will be
	/// </summary>
	/// <param name="value">The numerator of the percentage</param>
	/// <param name="maxValue">The denominator of the percentage</param>
	protected void SetColorProgress(float value, float maxValue)
	{
		//Update the current percentage
		currentPercentage = value / maxValue;
		//Update the number text
		numberText.text = value.ToString();
	}

	/// <summary>
	/// Sets the color bar progress to a new value. The value is divided by the maxValue to represent a percentage on how filled the color bar will be
	/// </summary>
	/// <param name="value">The numerator of the percentage</param>
	/// <param name="maxValue">The denominator of the percentage</param>
	protected void SetColorProgress(int value, int maxValue)
	{
		//Update the current percentage
		currentPercentage = value / (float)maxValue;
		//Update the number text
		numberText.text = value.ToString();
	}

	//Sets the color progress instantly. No animated interpolations are applied
	private void SetColorProgressRaw(float percentage)
	{
		//Update the current percentage
		currentPercentage = percentage;

		//Get the new width of the color bar
		float newWidth = -Mathf.Lerp(ColorBarBottom.rect.width, 0f, percentage);
		//Store the old size of the color bar
		var oldSize = ColorBarTop.sizeDelta;
		//Set the new size of the color bar
		ColorBarTop.sizeDelta = new Vector2(newWidth, oldSize.y);
	}

	/// <summary>
	/// Sets the color progress instantly. No animated interpolations are applied
	/// </summary>
	/// <param name="value">The numerator of the percentage</param>
	/// <param name="maxValue">The denominator of the percentage</param>
	protected void SetColorProgressRaw(float value, float maxValue)
	{
		SetColorProgressRaw(value / maxValue);
		numberText.text = value.ToString();
	}

	/// <summary>
	/// Sets the color progress instantly. No animated interpolations are applied
	/// </summary>
	/// <param name="value">The numerator of the percentage</param>
	/// <param name="maxValue">The denominator of the percentage</param>
	protected void SetColorProgressRaw(int value, int maxValue)
	{
		SetColorProgressRaw(value / (float)maxValue);
		numberText.text = value.ToString();
	}

}
