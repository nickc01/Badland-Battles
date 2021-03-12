using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthMarker : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How long the health marker will stay on screen, assuming the health doesn't change")]
    float timeOnScreen = 5f;
    [SerializeField]
    [Tooltip("How fast the color bar will animate")]
    float animationSpeed = 3f;

    //The target that the marker is representing. This will affect where the health marker is located on the screen
    public Transform Target;
    //An offset applied in screen coordinates, if any
    public Vector2 ScreenOffset;

    //The maximum health. If the health is equal this this, then the bar will be filled
    float maxHealth = 100f;
    //The health to be displayed on the marker
    float health = 100f;


    Vector3 lastKnownPosition;

    /// <summary>
    /// The maximum health. If the health is equal this this, then the bar will be filled
    /// </summary>
    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            //Update the maximum health
            maxHealth = value;
            //Reset the marker's timer
            timer = 0f;
        }
    }

    //The health to be displayed on the marker
    public float Health
    {
        get => health;
        set
        {
            //Update the health
            health = value;
            //Update the health text
            healthText.text = value.ToString();
            //Reset the marker's timer
            timer = 0f;
        }
    }

    //The timer that will keep track of how long the marker should display on the screen
    float timer = 0f;


    [Space]
    [Header("Marker Objects")]
    [SerializeField]
    RectTransform bottomBar; //Represents the black background bar of the color bar
    [SerializeField]
    RectTransform topBar; //Represents the red foreground bar of the color bar
    [SerializeField]
    TextMeshProUGUI healthText; //Represents the text that displays the number of HP left

    //The rect transform of the marker
    RectTransform rectTransform;

    private void Awake()
    {
        //Makes the health marker a child of the current game canvas
        transform.SetParent(GameCanvas.Instance.Canvas.transform);

        //Get the rect transform of the marker
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector3 position = lastKnownPosition;

        if (Target != null)
        {
            position = Target.transform.position;
            lastKnownPosition = position;
        }

        //Get the main camera
        var camera = Camera.main;

        //Convert the world-coordinates of the target to screen coordinates
        Vector2 screenCoordinates = (Vector2)camera.WorldToScreenPoint(position) + ScreenOffset;

        //Set the marker to the screen coordinates
        rectTransform.anchoredPosition = screenCoordinates;

        //Retrieve the new width for the color bar
        float colorBarWidth = Mathf.Lerp(0f, bottomBar.rect.width, health / maxHealth);
        //Store the original size of the color bar
        var oldSize = topBar.sizeDelta;
        //Smoothly interpolate to the new width of the color bar
        topBar.sizeDelta = new Vector2(Mathf.Lerp(oldSize.x, colorBarWidth, animationSpeed * Time.deltaTime), oldSize.y);

        if (timer < timeOnScreen)
        {
            //Increase the timer
            timer += Time.deltaTime;
            //If the timer is greater than the specified time on screen
            if (timer >= timeOnScreen)
            {
                //Destroy the marker
                Destroy(gameObject);
            }
        }
    }
}
