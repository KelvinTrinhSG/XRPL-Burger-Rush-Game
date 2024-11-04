using UnityEngine;
using System.Collections;

public class IngredientsController : MonoBehaviour {

	/// <summary>
	/// Main class for Handling all things related to ingredients
	/// when we click on an ingredient, it should match the exact id of the product the current customer wants.
	/// So we first need to check if the player is tapping on the right ingredient.
	/// If tap is correct, we will add the ingredient to the delivery queue
	/// if not, we delete the current delivery, and player needs to start from scratch
	/// </summary>

	//Public ID of this ingredient. (used to build up the delivery queue based on customers orders)
	public int factoryID;

	//Private flags
	private float delayTime;				//after this delay, we let player to be able to choose another ingredient again
	private bool canTap;					//cutome flag to prevent double picking
	private bool isLocked;					//check if this ingrediennt is locked or available

	//Reference to game objects
	private GameObject deliveryPlate;
	private GameObject currentCustomer;

	//prefabs
	public GameObject lockGo;				//lock prefab

	//Audio
	public AudioClip wrongPick;
	public AudioClip correctPick;
	public AudioClip successfulDelivery;


	/// <summary>
	/// Init
	/// </summary>
	void Awake (){
		
		delayTime = 0.15f;
		canTap = true;
		isLocked = true;
		deliveryPlate = GameObject.FindGameObjectWithTag ("serverPlate");

	}


	void Start (){

		//check if this ingredient is available for this level
		int ai = PlayerPrefs.GetInt("availableIngredients");
		for (int i = 0; i < ai; i++) {
			if (factoryID == PlayerPrefs.GetInt ("careerIngredient_" + i)) {
				isLocked = false;
				break;
			}
		}

		//check if this is locked on open
		if (isLocked) {
			GetComponent<BoxCollider> ().enabled = false;
			GetComponent<Renderer> ().material.color = new Color (1, 1, 1, 0.5f);
			GameObject lck = Instantiate (lockGo, transform.position + new Vector3 (0, 0, -0.1f), Quaternion.Euler (0, 180, 0)) as GameObject;
			lck.name = "Lock";
		} else {
			GetComponent<BoxCollider> ().enabled = true;
			GetComponent<Renderer> ().material.color = new Color (1, 1, 1, 1);
		}
	}


	/// <summary>
	/// FSM
	/// </summary>
	void Update (){

		if(canTap && !MainGameController.deliveryQueueIsFull && customerIsAvailable())
			monitorTap();

	}


	/// <summary>
	/// Process player inputs.
	/// </summary>
	private RaycastHit hitInfo;
	private Ray ray;
	void monitorTap (){
		
		//Mouse of touch?
		if(	Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Moved)  
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
		else if(Input.GetMouseButtonDown(0))
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		else
			return;
			
		if(Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			if(objectHit.tag == "ingredient" && objectHit.name == gameObject.name) {
				StartCoroutine(updateOrderQuoue(gameObject));
			}
		}

	}


	/// <summary>
	/// when we click on an ingredient, it should match the exact id of the product the current customer wants.
	/// So we first need to check if the player is tapping on the right ingredient.
	/// If tap is correct, we will add the ingredient to the delivery quoue
	/// if not, we delete the current delivery, and player needs to start from scratch
	/// </summary>
	IEnumerator updateOrderQuoue (GameObject selectedIngredient){
		if(!MainGameController.gameIsFinished && !MainGameController.deliveryQueueIsFull) {

			//prevent double tap
			canTap = false;
			StartCoroutine(reactivate());

			//check tapped ingredient with customer's order
			int dqi = MainGameController.deliveryQueueItems;

			//prevent array size error
			if (dqi >= CustomerController.orderIngredientsIDs.Length) {
				MainGameController.deliveryQueueIsFull = true;
				print ("deliveryQueueIsFull");
				yield break;
			}

			if (factoryID != CustomerController.orderIngredientsIDs [dqi]) {

				//player tapped on a wrong ingredient.
				//So...

				//play wrong pick sfx
				playSfx(wrongPick);

				//delete delivery items on the plater
				deliveryPlate.GetComponent<PlateController> ().deleteDelivery ();

				//clear main delivery arrays
				MainGameController.deliveryQueueItems = 0;
				MainGameController.deliveryQueueIsFull = false;
				MainGameController.deliveryQueueItemsContent.Clear();

				yield break;
			}

			//add this ingredient to delivery quoue
			MainGameController.deliveryQueueItems++;
			MainGameController.deliveryQueueItemsContent.Add(factoryID);

			//tell PlaterController to update the delivery image on plate
			deliveryPlate.GetComponent<PlateController>().updateDelivery(factoryID);

			//play ingredient pick sound
			playSfx (correctPick);

			//check if order is finished and completed
			if (MainGameController.deliveryQueueItems == CustomerController.orderIngredientsIDs.Length) {
				//order is complete!
				print ("Order is done!");
				//wait
				yield return new WaitForSeconds (0.2f);
				playSfx (successfulDelivery);
				//tell customer to settle and leave
				GameObject c = GameObject.FindGameObjectWithTag ("customer");
				c.GetComponent<CustomerController> ().settle ();
			}

			StartCoroutine(reactivate());

			//debug
			/*
			print ("Delivery size: " + MainGameController.deliveryQueueItems);
			for (int i = 0; i < MainGameController.deliveryQueueItemsContent.Count; i++) {
				print ("Ing[" + i + "]: " + MainGameController.deliveryQueueItemsContent [i]);
			}
			*/

		}
	}


	/// <summary>
	/// Make this ingredient clickable again
	/// </summary>
	IEnumerator reactivate (){
		yield return new WaitForSeconds(delayTime);
		canTap = true;
	}
		

	/// <summary>
	/// Check if there is any customer inside the shop (which is ready to order)
	/// </summary>
	/// <returns><c>true</c>, if there is a customer in shop<c>false</c> otherwise.</returns>
	bool customerIsAvailable() {
		GameObject c = GameObject.FindGameObjectWithTag ("customer");
		if (c != null) {
			if(CustomerController.isCustomerReady)
				return true;
			else
				return false;
		} else
			return false;
	}


	/// <summary>
	/// Play AudioClips
	/// </summary>
	/// <param name="_sfx">Sfx.</param>
	void playSfx ( AudioClip _sfx  ){
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}

}