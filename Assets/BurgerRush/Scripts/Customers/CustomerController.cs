using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerController : MonoBehaviour {
	
	/// <summary>
	/// This class manages all thing related to a customer, including the entrance, order, patience, settle and animations.
	/// Basically this is how a customer object works:
	/// 1. This instance has been created by MainGameController class.
	/// 2. Customers make his order by creating a burger with ingredients available for this level (refer to CareerLevelSetup class)
	/// 3. Customer enters the scene, and the order is shown on the board.
	/// 4. Customer carefully inspect all your taps on ingredients until you serve the right burger or ran out of time.
	/// 5. If the order is served, customer will pay you with coin, and leaves with a happy face.
	/// 6. If you ran out of time, customer gets angry and leaves the shop with no pay.
	/// </summary>

	public float customerPatience = 10.0f; 				//in seconds (default = 10)
	public static int customerOrderSize;				//the size and complexity of the order (indicates the total ingredients used inside the burger)
	public static int[] orderIngredientsIDs;			//IDs of the ingredients used inside the order

	//Available items to order.
	//Notice: customer can order just 1 item. A burger (with many ingredients) or a single drink
	public static int[] availableIngredients;			//List of all available ingredients in this level

	// Audio Clips
	public AudioClip orderIsOkSfx;	
	public AudioClip orderIsNotOkSfx;

	//*** Customer Moods ***//
	//We use different materials for each mood
	/* Currently we have 4 moods: 
	 [0]Defalut
	 [1]Bored 
	 [2]Satisfied
	 [3]Angry
	 we change to appropriate material whenever needed. 
	*/
	public Material[] customerMoods; 
	private int moodIndex;

	//Private variables
	internal Vector3 destination;				//destination in scene
	private GameObject gameController; 			//reference to GC object
	private GameObject serverPlate; 			//reference to plate game ovject
	public static bool isCustomerReady;			//flag used to indicate this customer is inside the shop and wants to order something

	public GameObject[] allIngredients;			//list of all ingredients defined for the game
	public int availableDrinks = 3;				//total number of all available drinks 

	private string customerName;				//random name
	private float currentCustomerPatience;		//current patience of the customer
	private bool isOnSeat;						//is customer on his seat?
	private Vector3 startingPosition;

	//Patience bar GUI items and vars
	private Vector3 patienceBarStartingScale;
	public GameObject patienceBarFG;			
	public GameObject patienceBarBG;			
	private bool  patienceBarSliderFlag;	

	//Time variables
	internal float leaveTime;					
	private float creationTime;				
	private bool  isLeaving;				

	//Link to prefabs
	public GameObject moneyGO;					//money (object) the customer pays after receiving the order


	/// <summary>
	/// Init
	/// </summary>
	void Awake (){
		
		patienceBarFG.SetActive(false);
		patienceBarBG.SetActive(false);
		patienceBarStartingScale = patienceBarFG.transform.localScale;

		//set the size of the new order (total ingredients inside the burger)
		customerOrderSize = Random.Range(3, PlayerPrefs.GetInt ("levelComplexity") + 1);
		orderIngredientsIDs = new int[customerOrderSize];
		availableIngredients = new int[0];

		isOnSeat = false;
		isCustomerReady = false;
		currentCustomerPatience = customerPatience;
		moodIndex = 0;
		leaveTime = 0;
		isLeaving = false;
		creationTime = Time.time;
		startingPosition = transform.position;
		gameController = GameObject.FindGameObjectWithTag("GameController");
		serverPlate = GameObject.FindGameObjectWithTag("serverPlate");

		Init();
		StartCoroutine(goToSeat());
	}


	/// <summary>
	/// Init all variables
	/// </summary>
	private GameObject productImage;
	private GameObject[] helperIngredients;
	private GameObject sideReq;
	void Init (){
		
		//we give this customer a nice name
		customerName = "Customer_" + Random.Range(100, 10000);
		gameObject.name = customerName;

		//check if this customer only wants a drink or we need to prepare for a complex order
		if (Random.value > 0.8f) {
			//we only want a drink. drink IDs start from 101.
			int drinkID = Random.Range(0, availableDrinks) + 101;
			//tell MainGameController to update order helper status
			gameController.GetComponent<MainGameController>().updateDrinkHelper(drinkID);

			customerOrderSize = 1; 	//because we need 1 drink
			orderIngredientsIDs = new int[customerOrderSize];
			orderIngredientsIDs[0] = drinkID;

			return;
		}


		//get the list of all available ingredients for this level
		int ai = PlayerPrefs.GetInt("availableIngredients");
		availableIngredients = new int[ai];
		//print ("availableIngredients: " + ai);
		for (int a = 0; a < ai; a++) {
			availableIngredients [a] = PlayerPrefs.GetInt ("careerIngredient_" + a);
			//print ("ingredient " + a + " = " + availableIngredients [a]);
		}
	
		//We need to make an order for this customer at runtime!
		for (int n = 0; n < customerOrderSize; n++) {

			//set starting and end index with the IDs of bottom & top bread
			if (n == 0) {
				orderIngredientsIDs [0] = 1;
			} else if (n == customerOrderSize - 1) {
				orderIngredientsIDs [n] = 2;
			} else {
				//otherwise, set random ingredients inside the order array
				orderIngredientsIDs [n] = availableIngredients [Random.Range (2, availableIngredients.Length)];
			}
		}

		//Uncomment this to show debug informations
		/*productIngredients = customerOrderSize;
		productIngredientsIDs = new int[productIngredients];
		for(int i = 0; i < productIngredients; i++) {
			productIngredientsIDs[i] = orderIngredientsIDs [i];
			print("My order ingredients ID[" + i + "] is: " + productIngredientsIDs[i]);
		}*/
			
		//tell MainGameController to update order helper status
		gameController.GetComponent<MainGameController>().updateOrderHelpers(orderIngredientsIDs);

	}


	/// <summary>
	/// After this customer has been instantiated by MainGameController,
	// it starts somewhere outside game scene and then go to it's position (seat)
	// with a sine animation. The order will be shown afterwards.
	/// </summary>
	private float speed = 6.0f;
	private float timeVariance;
	IEnumerator goToSeat () {

		//Y position on sine wave
		timeVariance = Random.value;

		//move
		while(!isOnSeat) {
			transform.position = new Vector3(transform.position.x + (Time.deltaTime * speed),
			                                 startingPosition.y - 0.25f + (Mathf.Sin((Time.time + timeVariance) * 10) / 8),
			                                 transform.position.z);
				
			//if we have reached to our seat...
			if(transform.position.x >= destination.x) {
				isOnSeat = true;
				isCustomerReady = true;
				patienceBarSliderFlag = true; //start the patience bar

				patienceBarFG.SetActive(true);
				patienceBarBG.SetActive(true);

				//if we only want a drink
				if (customerOrderSize == 1) {
					StartCoroutine (gameController.GetComponent<MainGameController> ().showDrinkHelper ());
				} else {
					//tell game controller to show order helpers
					StartCoroutine (gameController.GetComponent<MainGameController> ().showOrderHelpers ());
				}

				yield break;
			}
			yield return 0;
		}
	}

	
	/// <summary>
	/// FSM
	/// </summary>
	void Update (){
		
		if(patienceBarSliderFlag)
			StartCoroutine(patienceBar());
		
		//Manage customer's mood
		updateCustomerMood();
	}
		

	/// <summary>
	/// Make the customer react to events by switching between available face textures
	/// </summary>
	void updateCustomerMood (){
		//if customer has waited for 1/3 or 2/3 of its patience, make him/her bored and angry.
		if(!isLeaving) {
			if(currentCustomerPatience <= customerPatience / 1.5f && currentCustomerPatience > customerPatience / 2.5f)
				moodIndex = 1;
			else if(currentCustomerPatience <= customerPatience / 2.5f)
				moodIndex = 3;
			else
				moodIndex = 0;
		}
			
		GetComponent<Renderer>().material = customerMoods[moodIndex];
	}
		

	/// <summary>
	/// Show and animate customer's patience bar.
	/// </summary>
	IEnumerator patienceBar (){
		patienceBarSliderFlag = false;
		while(currentCustomerPatience > 0) {
			
			currentCustomerPatience -= Time.deltaTime * Application.targetFrameRate * 0.02f;
			//print ("currentCustomerPatience: " + currentCustomerPatience);
			patienceBarFG.transform.localScale = new Vector3 (
				(currentCustomerPatience / customerPatience) * patienceBarStartingScale.x, 
				patienceBarStartingScale.y, 
				patienceBarStartingScale.z);

			yield return 0;
		}

		if(currentCustomerPatience <= 0) {

			patienceBarFG.SetActive (false);
			patienceBarBG.SetActive (false);

			//customer is angry and will leave with no food received.
			StartCoroutine(leave());
		}

	}


	/// <summary>
	/// Customer should pay and leave the restaurant.
	/// </summary>
	public void settle (){
		
		moodIndex = 2;	//make him/her happy :)
		
		//give cash, money, bonus, etc, here.
		float leaveTime = Time.time;
		int remainedPatienceBonus = (int)Mathf.Round(customerPatience - (leaveTime - creationTime));
		
		//if we have purchased additional items for our restaurant, we should receive more tips
		int tips = 0;
		if(PlayerPrefs.GetInt("shopItem-1") == 1) tips += 1;	//if we have seats
		if(PlayerPrefs.GetInt("shopItem-2") == 1) tips += 2;	//if we have music player
		if(PlayerPrefs.GetInt("shopItem-3") == 1) tips += 3;	//if we have flowers

		//productPrice is the sum of all ingredients price
		int productPrice = 0;
		for (int i = 0; i < orderIngredientsIDs.Length; i++) {
			productPrice += allIngredients [i].GetComponent<IngredientManager> ().price;
		}

		int finalMoney = 	productPrice +
							remainedPatienceBonus + 
							tips;	
		
		GameObject m = Instantiate(	moneyGO, 
			new Vector3(2, -1, -1), 
			Quaternion.Euler(0, 180, 0)) as GameObject;
		m.name = "CustomerMoney-" + finalMoney.ToString() + "-" + Random.value;
		m.GetComponent<customerMoneyController>().moneyAmount = finalMoney;

		playSfx(orderIsOkSfx);
		StartCoroutine(leave());
	}


	/// <summary>
	/// Simple leave routine.
	/// </summary>
	public IEnumerator leave (){
		
		//prevent double run
		if(isLeaving)
			yield break;

		//set the leave flag to prevent multiple calls to this function
		isLeaving = true;
		isCustomerReady = false;

		//destroy delivery 
		serverPlate.GetComponent<PlateController>().deleteDelivery();

		//reset order helpers
		gameController.GetComponent<MainGameController>().resetOrderHelpers();
		
		//hide patienceBar
		patienceBarBG.SetActive(false);
		patienceBarFG.SetActive (false);
		yield return new WaitForSeconds(0.3f);
		
		//animate customer (move it to right, then destroy it)
		while(transform.position.x < 10) {
			transform.position = new Vector3(transform.position.x + (Time.deltaTime * speed * 1.25f),
			                                 startingPosition.y - 0.25f + (Mathf.Sin(Time.time * 10) / 8),
			                                 transform.position.z);
			
			if(transform.position.x >= 10) {
				MainGameController.canCreateNewCustomer = true;
				Destroy(gameObject);
				yield break;
			}
			yield return 0;
		}
	}


	/// <summary>
	/// Play AudioClips
	/// </summary>
	void playSfx ( AudioClip _sfx  ){
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}

}