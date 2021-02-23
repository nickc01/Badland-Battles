using TMPro;
using UnityEngine;



/// <summary>
/// A base class for all UI HUDS that use color scrolling
/// </summary>
public abstract class ColorScrollHUD : MonoBehaviour
{
	[SerializeField]
	protected float colorAnimationSpeed = 4f;
	[SerializeField]
	RectTransform colorBarTop;
	[SerializeField]
	RectTransform colorBarBottom;
	[SerializeField]
	TextMeshProUGUI numberText;

	float currentPercentage = 1f;

	public RectTransform ColorBarTop => colorBarTop;
	public RectTransform ColorBarBottom => colorBarBottom;

	/*public RectTransform ColorBarTop
	{
		get
		{
			if (colorBarTop == null)
			{
				colorBarTop = ColorBarBottom.transform.Find("Health Bar Top").GetComponent<RectTransform>();
			}
			return colorBarTop;
		}
	}

	public RectTransform ColorBarBottom
	{
		get
		{
			if (colorBarBottom == null)
			{
				colorBarBottom = transform.Find("Health Bar Bottom").GetComponent<RectTransform>();
			}
			return colorBarBottom;
		}
	}*/

	protected virtual void Update()
	{
		//SetColorProgress(currentPercentage);
		float newWidth = -Mathf.Lerp(ColorBarBottom.rect.width, 0f, currentPercentage);

		var oldSize = ColorBarTop.sizeDelta;

		ColorBarTop.sizeDelta = new Vector2(Mathf.Lerp(oldSize.x, newWidth, colorAnimationSpeed * Time.deltaTime), oldSize.y);
	}


	protected void SetColorProgress(float value, float maxValue)
	{
		currentPercentage = value / maxValue;
		numberText.text = value.ToString();
	}

	protected void SetColorProgress(int value, int maxValue)
	{
		currentPercentage = value / (float)maxValue;
		numberText.text = value.ToString();
	}


	private void SetColorProgressRaw(float percentage)
	{
		currentPercentage = percentage;

		float newWidth = -Mathf.Lerp(ColorBarBottom.rect.width, 0f, percentage);

		var oldSize = ColorBarTop.sizeDelta;

		ColorBarTop.sizeDelta = new Vector2(newWidth, oldSize.y);
	}

	protected void SetColorProgressRaw(float value, float maxValue)
	{
		SetColorProgressRaw(value / maxValue);
		numberText.text = value.ToString();
	}

	protected void SetColorProgressRaw(int value, int maxValue)
	{
		SetColorProgressRaw(value / (float)maxValue);
		numberText.text = value.ToString();
	}

}
