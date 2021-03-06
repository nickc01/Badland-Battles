using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyHealthDisplay : MonoBehaviour
{
	/*[SerializeField]
	[Tooltip("The health marker prefab")]
	HealthMarker markerPrefab;*/

	//The health component of the enemy
	Health health;
	//The marker instance that is currently being used to display the health
	HealthMarker marker;


	private void Awake()
	{
		//Get the health component
		health = GetComponent<Health>();

		//Hook into the onDamage and onHeal events
		health.OnDamage += UpdateHealthDisplay;
		health.OnHeal += UpdateHealthDisplay;
	}


	//Called when the health of the enemy is damaged or healed, and is used to update the health display
	void UpdateHealthDisplay(int newHealth)
	{
		//If no marker is currently displaying
		if (marker == null)
		{
			//Create a new marker
			marker = GameObject.Instantiate(GameManager.Instance.MarkerPrefab);
			//Set the target to this enemy
			marker.Target = transform;
			//Set it's max health value to the enemy's max health
			marker.MaxHealth = health.MaxHealth;
			//Set the health value to the enemy's current health
			marker.Health = newHealth;
		}
		//If there is a marker present
		else
		{
			//Update the marker's health
			marker.Health = newHealth;
		}
	}
}
