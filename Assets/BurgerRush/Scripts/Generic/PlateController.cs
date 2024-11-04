using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlateController : MonoBehaviour {

	/// <summary>
	/// When we click on any ingredients, the game checks if the ID of selected ingredient matches the customer order.
	/// If this is the case, we need to add this ingredient to the server plate object to simulate the shape of customers order.
	/// The fact is that we are not doing anything special. We are just instantiating static ingredients based on the master queue 
	/// array from the MainGameController.
	/// </summary>

	//list of all available ingredients
	public GameObject[] availableIngredients;


	/// <summary>
	/// Build the shape of customer burger by instantiating related ingredients that match customer's order
	/// </summary>
	public void updateDelivery(int ingID) {

		//number of deliveryQueueItems
		int dqi = MainGameController.deliveryQueueItems;

		GameObject newIng = Instantiate (availableIngredients [ingID - 1], 
			transform.position + new Vector3(0, -0.7f + (0.35f * dqi), -0.1f * dqi), 
			Quaternion.Euler (90, 0, 180)) as GameObject;
		
		newIng.transform.parent = transform;
		newIng.name = "DeliveryIng-0" + dqi;
		newIng.tag = "DeliveryIngredient";

	}


	/// <summary>
	/// Destroy the main queue array and all ingredient objects instantiated befroe.
	/// </summary>
	public void deleteDelivery () {

		//get all objects with "DeliveryIngredient" tag
		GameObject[] di = GameObject.FindGameObjectsWithTag("DeliveryIngredient");
		foreach (GameObject go in di) {
			Destroy (go);
		}

		//clear main delivery arrays
		MainGameController.deliveryQueueItems = 0;
		MainGameController.deliveryQueueIsFull = false;
		MainGameController.deliveryQueueItemsContent.Clear();

	}

}