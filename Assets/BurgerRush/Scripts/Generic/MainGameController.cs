using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class MainGameController : MonoBehaviour {
	
	/// <summary>
	/// This class is the main controller of the game and manages customer creation,
	/// time management, money management, game state , win/lose state, showing information on UI,
	/// and showing/hiding/clearing orders in realtime.
	/// </summary>

	public static int availableTime; 		//Time available for this level (set in LevelSelection scene)
	public static int remainingTime;		//how much time has left
	public static int startTime;			//The exact time we click on start button and begin playing
	private int seconds;					//Used to show time in UI
	private int minutes;					//Used to show time in UI

	/// <summary>
	/// We set these values in LevelSelection scene!
	/// </summary>
	static public int levelMission1;		//to beat the level with 1 star
	static public int levelMission2;		//to beat the level with 2 star
	static public int levelMission3;		//to beat the level with 3 star
	static public int requiredBalance;		//dynamiclly changes based on current user money
	static public int acquiredStars;		//how many stars player received in this level

	[Header("Public Game Objects")]
	public GameObject[] customers;			//list of all available customers (different patience and textures)
	public GameObject[] orderHelpers;		//array containing all helper objects for the active order
	public GameObject drinkHelper;			//The object used to show customer drink orders on UI
	public GameObject[] additionalItems;	//Items that can be purchased via in-game shop.
	public GameObject[] missionStarsUI;
	public GameObject moneyBar;
	public GameObject moneyText;
	public GameObject timeText;

	[Header("Level Objective Settings")]
	public GameObject UiLevelObjectivesPanel;
	public GameObject UiLevelNumber;
	public GameObject UiLevelMission1;
	public GameObject UiLevelMission2;
	public GameObject UiLevelMission3;
	public GameObject UiLevelTime;

	[Header("End Game Panel settings")]
	public GameObject UiEndGamePanel;
	public GameObject UiNextButton;			//next button loads the next available level when player beats a level
	public GameObject UiEndGameStars;
	public Material[] UiEndGameStarMats;
	public GameObject UiEndGameMoneyMade;
	public GameObject UiEndGameStatus;		//gameobject which shows the texture of win/lose states
	public Texture2D[] UiEndGameTextures;	//textures for win/lose states
	public GameObject UiEndGameVideoBtn;

	[Header("List of available items to order")]
	public Material[] availableIngredients;	//array containing all available ingredients in game
	public Material[] availableDrinks;		//array containing all available drinks in game

	//static order arrays. Do not edit.
	static public List<int> currentActiveOrder = new List<int>();			//current active order we need to serve
	static public bool deliveryQueueIsFull;									//delivery queue can accept 9 ingredients. more is not acceptable.
	static public int deliveryQueueItems;									//number of items in delivery queue
	static public List<int> deliveryQueueItemsContent = new List<int>();	//conents of delivery queue
	public static bool canCreateNewCustomer;	//flag to prevent double calls to spawn function
	static public int maxSlotState = 9;			//maximum available slots in delivery queue (set in init)

	//Money and GameState
	static public int totalMoneyMade;
	static public bool gameIsFinished;			//Flag
	static public bool gameIsStarted;			//Flag

	[Header("Audio")]
	public AudioClip timeEndSfx;
	public AudioClip starWinSfx;
	public AudioClip winSfx;
	public AudioClip loseSfx;
	public AudioClip tapSfx;


	public void Awake (){
		Init();
	}


	/// <summary>
	/// Init
	/// </summary>
	void Init (){

		Application.targetFrameRate = 50; 	//Optional based on the target platform

		//clear all arrays
		deliveryQueueIsFull = false;
		deliveryQueueItems = 0;
		deliveryQueueItemsContent.Clear();
		currentActiveOrder.Clear();
		totalMoneyMade = 0;
		acquiredStars = 0;
		startTime = 0;
		gameIsFinished = false;
		gameIsStarted = false;
		moneyBar.transform.localScale = new Vector3 (0, 1, 1);

		UiNextButton.SetActive (false);	//only shows when we finish a level in career mode with success.
		
		seconds = 0;
		minutes = 0;
		
		canCreateNewCustomer = false;

		//hide all order helper objects
		hideOrderHelpers();
		
		//check if player previously purchased these items..
		//ShopItem index starts from 1.
		for(int j = 0; j < additionalItems.Length; j++) {
			//format the correct string we use to store purchased items into playerprefs
			string shopItemName = "shopItem-" + (j+1).ToString();;
			if(PlayerPrefs.GetInt(shopItemName) == 1) {
				//we already purchased this item
				additionalItems[j].SetActive(true);
			} else {
				additionalItems[j].SetActive(false);
			}
		}

		//calculate and set level missions
		levelMission1 = PlayerPrefs.GetInt("careerMission");
		levelMission2 = levelMission1 + (int)(levelMission1 * 0.25f);
		levelMission3 = levelMission1 + (int)(levelMission1 * 0.5f);
		requiredBalance = levelMission1; 	//default
		availableTime = PlayerPrefs.GetInt("careerAvailableTime");

		//debug
		print("LevelMissions: " + levelMission1 + " - " + levelMission2 + " - " + levelMission3);
	}


	/// <summary>
	/// Start the game after showing the objectives panel to player
	/// </summary>
	IEnumerator Start () {
	
		yield return new WaitForSeconds(0.15f);
		//set objectives
		UiLevelNumber.GetComponent<TextMesh>().text = "Level " + PlayerPrefs.GetInt("careerLevelID").ToString();
		UiLevelMission1.GetComponent<TextMesh> ().text = "$" + levelMission1.ToString();
		UiLevelMission2.GetComponent<TextMesh> ().text = "$" + levelMission2.ToString();
		UiLevelMission3.GetComponent<TextMesh> ().text = "$" + levelMission3.ToString();
		UiLevelTime.GetComponent<TextMesh> ().text = string.Format("{0:00}' : {1:00}''", availableTime / 60, availableTime % 60);

		UiLevelObjectivesPanel.SetActive (true);
		UiEndGamePanel.SetActive (false);
		StartCoroutine(moveToPosition (UiLevelObjectivesPanel, new Vector3(20, 0, -3) , new Vector3 (0, 0, -3), 0, 1.7f));

	}


	/// <summary>
	/// Moves to object from a to b, by the given delay and speed
	/// </summary>
	/// <returns>The to position.</returns>
	/// <param name="g">The green component.</param>
	/// <param name="from">From.</param>
	/// <param name="to">To.</param>
	/// <param name="d">D.</param>
	/// <param name="s">S.</param>
	IEnumerator moveToPosition(GameObject g, Vector3 from, Vector3 to, float d, float s) {

		yield return new WaitForSeconds(d);
		float t = 0;
		while (t < 1) {
			t += Time.deltaTime * s;
			g.transform.localPosition = new Vector3 (Mathf.SmoothStep (from.x, to.x, t),
				Mathf.SmoothStep (from.y, to.y, t),
				Mathf.SmoothStep (from.z, to.z, t));
			yield return 0;
		}
	}


	/// <summary>
	/// Hides the order helper objects
	/// </summary>
	void hideOrderHelpers() {

		//hide main order
		for (int i = 0; i < orderHelpers.Length; i++)
			orderHelpers [i].GetComponent<Renderer> ().enabled = false;

		//hide drinks
		drinkHelper.GetComponent<Renderer>().enabled = false;

	}


	/// <summary>
	/// FSM
	/// </summary>
	void Update (){

		//no more ingredient can be picked if the queue is full
		if(deliveryQueueItems >= maxSlotState)
			deliveryQueueIsFull = true;
		else	
			deliveryQueueIsFull = false;
		
		if(!gameIsFinished && gameIsStarted) {
			manageClock();
			manageUI();
			StartCoroutine(checkGameWinState());
		}
			
		//create a new customer if there is no customer in scene and game is not finished yet
		if(canCreateNewCustomer && !gameIsFinished && gameIsStarted) {
			createCustomer();
		}

		StartCoroutine(tapManager());

	}


	/// <summary>
	/// Manage player inputs
	/// </summary>
	private RaycastHit hitInfo;
	private Ray ray;
	IEnumerator tapManager (){

		//Mouse of touch?
		if(	Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)  
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
		else if(Input.GetMouseButtonUp(0))
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		else
			yield break;

		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			if(objectHit.name == "Objectives-StartGame") {

				playSfx (tapSfx); 
				StartCoroutine(animateButton (objectHit));
				yield return new WaitForSeconds (0.2f);

				StartCoroutine(moveToPosition (UiLevelObjectivesPanel, new Vector3(0, 0, -3) , new Vector3 (20, 0, -3), 0, 1.7f));
				canCreateNewCustomer = true;

				yield return new WaitForSeconds (0.3f);
				gameIsStarted = true;
				startTime = (int)Time.timeSinceLevelLoad;
				print ("startTime " + startTime + "  -Since: " + Time.timeSinceLevelLoad);

				yield break;
			}

		}
	}


	/// <summary>
	/// Updates the level mission everytime player collects money.
	/// This is where we set the amount of stars player receives after finishing the level.
	/// </summary>
	public void updateLevelMission() {

		if (totalMoneyMade < levelMission1) {
			
			requiredBalance = levelMission1;
			acquiredStars = 0;

		} else if (totalMoneyMade >= levelMission1 && totalMoneyMade < levelMission2) {
			
			requiredBalance = levelMission2;
			if(acquiredStars == 0)
				playSfx (starWinSfx);
			acquiredStars = 1;

		} else if (totalMoneyMade >= levelMission2 && totalMoneyMade < levelMission3) {
			
			requiredBalance = levelMission3;
			if(acquiredStars == 1)
				playSfx (starWinSfx);
			acquiredStars = 2;

		} else if (totalMoneyMade > levelMission3) {

			if(acquiredStars == 2)
				playSfx (starWinSfx);
			acquiredStars = 3;

		}

	}
		

	/// <summary>
	/// Spawn a new customer object
	/// We can only create new customer when no other customer exists in the scene.
	/// </summary>
	public void createCustomer (){

		//set flag to prevent double calls 
		canCreateNewCustomer = false;

		//which customer?
		GameObject tmpCustomer = customers[Random.Range(0, customers.Length)];
		
		//target position in scene
		Vector3 tp = new Vector3(-1.0f, 1.0f, 0.2f);
		
		//create customer
		int offset = -11;
		GameObject newCustomer = Instantiate(tmpCustomer, new Vector3(offset, 1.0f, 0.2f), Quaternion.Euler(0, 180, 0)) as GameObject;

		//set customer's destination
		newCustomer.GetComponent<CustomerController>().destination = tp;
	}


	/// <summary>
	/// Show player money & mission bar on UI
	/// </summary>
	void manageUI (){

		//show money and mission (text) on UI
		moneyText.GetComponent<TextMesh> ().text = "$" + totalMoneyMade.ToString () + "/" + "" + requiredBalance.ToString ();

		//update money length bar on UI
		float barScale = totalMoneyMade / (levelMission3 * 0.99f);
		if (barScale > 1)
			barScale = 1;
		moneyBar.transform.localScale = new Vector3 (barScale , 1, 1);

	}

		
	/// <summary>
	/// Manage game clock and timers and show it on UI
	/// </summary>
	void manageClock (){

		if (gameIsFinished) {
			timeText.GetComponent<TextMesh>().text = "00 : 00";
			return;
		}
		
		remainingTime = (int)(availableTime + startTime - Time.timeSinceLevelLoad);
		seconds = Mathf.CeilToInt(availableTime + startTime - Time.timeSinceLevelLoad) % 60;
		minutes = Mathf.CeilToInt(availableTime + startTime - Time.timeSinceLevelLoad) / 60; 
		timeText.GetComponent<TextMesh>().text = string.Format("{0:00} : {1:00}", minutes, seconds);

	}


	/// <summary>
	/// PlayOneShot the given audioclip 
	/// </summary>
	/// <param name="_sfx">Sfx.</param>
	void playSfx ( AudioClip _sfx  ){
		GetComponent<AudioSource>().PlayOneShot(_sfx);
	}
		

	/// <summary>
	/// Monitor player Win/Lose states all the time
	/// </summary>
	/// <returns>The game window state.</returns>
	IEnumerator checkGameWinState (){
		
		if(gameIsFinished)
			yield break;

		//set UI informations
		UiEndGameMoneyMade.GetComponent<TextMesh>().text = "$" + totalMoneyMade + " / $" + requiredBalance;
		UiEndGameStars.GetComponent<Renderer>().material = UiEndGameStarMats[acquiredStars];

		//if time runs out and we have not reached to the initial 1-star mission, game is over. [LOSE]
		if(remainingTime <= 0 && totalMoneyMade < levelMission1) {
		
			print("Time is up! You have failed :(");	//debug the result
			gameIsFinished = true;						//announce the new status to other classes
			UiEndGamePanel.SetActive(true);				//show the endGame plane
			StartCoroutine(moveToPosition (UiEndGamePanel, new Vector3(-20, 0, -3) , new Vector3 (0, 0, -3), 0, 1.7f));
			UiEndGameStatus.GetComponent<Renderer>().material.mainTexture = UiEndGameTextures[1];	//show the correct texture for result
			playNormalSfx(timeEndSfx);
			yield return new WaitForSeconds(2.0f);
			playNormalSfx(loseSfx);
			
		} else if( (remainingTime <= 0 && totalMoneyMade >= levelMission1) || (remainingTime > 0 && totalMoneyMade >= levelMission3) ) {

			//if time is up, but we have beat at least the first mission,
			//or if we still have time, but beat the 3-star mission, the game is over. [WIN]

			//save career progress & level stars
			saveCareerProgress();

			//no need for videoAds button when we won the level
			if(UiEndGameVideoBtn)
				UiEndGameVideoBtn.SetActive(false);
			
			//grant the prize
			int currentMoney = PlayerPrefs.GetInt("PlayerMoney");
			currentMoney += totalMoneyMade;
			PlayerPrefs.SetInt("PlayerMoney", currentMoney);

			print("Wow, You beat the level! :)");
			gameIsFinished = true;
			UiEndGamePanel.SetActive(true);
			StartCoroutine(moveToPosition (UiEndGamePanel, new Vector3(-20, 0, -3) , new Vector3 (0, 0, -3), 0, 1.7f));
			UiEndGameStatus.GetComponent<Renderer>().material.mainTexture = UiEndGameTextures[0];
			playNormalSfx(winSfx);

			//show next level button
			UiNextButton.SetActive (true);
			
		}
	}

	
	/// <summary>
	/// Save player progress.
	/// </summary>
	void saveCareerProgress (){
		
		int currentLevelID = PlayerPrefs.GetInt("careerLevelID");
		int userLevelAdvance = PlayerPrefs.GetInt("userLevelAdvance");
		
		//if this is the first time we are beating this level...
		if(userLevelAdvance < currentLevelID) {
			userLevelAdvance++;
			PlayerPrefs.SetInt("userLevelAdvance", userLevelAdvance);
		}

		//check if we have played this level before
		int savedStarsForThisLevel = PlayerPrefs.GetInt ("Level-" + PlayerPrefs.GetInt("careerLevelID").ToString() + "-Stars");

		//only save new stars if we beat the level again with more stars than our previous game
		if(acquiredStars > savedStarsForThisLevel)
			PlayerPrefs.SetInt ("Level-" + PlayerPrefs.GetInt("careerLevelID").ToString() + "-Stars", acquiredStars);
	}


	/// <summary>
	/// This is used if customer is ordering just one drink.
	/// We then set the correct texture for the drink helper game object.
	/// </summary>
	/// <param name="drinkID">Drink ID</param>
	public void updateDrinkHelper(int drinkID) {
		drinkHelper.GetComponent<Renderer>().material = availableDrinks[drinkID - 101];
	}


	/// <summary>
	/// Shows the drink helper on UI
	/// </summary>
	/// <returns>The drink helper.</returns>
	public IEnumerator showDrinkHelper() {
		
		Vector3 drinkStartingScale = drinkHelper.transform.localScale;
		drinkHelper.GetComponent<Renderer> ().enabled = true;
		float t = 0;
		while (t < 1) {
			t += Time.deltaTime * 3.0f;
			drinkHelper.transform.localScale = new Vector3 (Mathf.SmoothStep (0.1f, drinkStartingScale.x, t),
				Mathf.SmoothStep (0.1f, drinkStartingScale.y, t),
				0.01f);
			yield return 0;
		}

	}


	/// <summary>
	/// Updates the material of ingredients inside customer order to show them on UI.
	/// </summary>
	private int productIngredients;
	public void updateOrderHelpers(int[] orderIngredients) {
		productIngredients = orderIngredients.Length;
		for(int i = 0; i < productIngredients; i++) {
			orderHelpers[i].GetComponent<Renderer>().material = availableIngredients[ orderIngredients[i]-1 ];
		}
	}


	/// <summary>
	/// Shows all the ingredients that are used inside the order.
	/// </summary>
	public IEnumerator showOrderHelpers() {
		Vector3 helperStartingScale = orderHelpers[0].transform.localScale;	//all helpers share the same scale
		for (int i = 0; i < productIngredients; i++) {
			orderHelpers [i].GetComponent<Renderer> ().enabled = true;
			float t = 0;
			while (t < 1) {
				t += Time.deltaTime * 3.0f;
				orderHelpers [i].transform.localScale = new Vector3 (Mathf.SmoothStep (0.1f, helperStartingScale.x, t),
					Mathf.SmoothStep (0.1f, helperStartingScale.y, t),
					0.01f);
				yield return 0;
			}

			//yield return new WaitForSeconds (0.15f);
		}
	}


	/// <summary>
	/// Hide all helpers when customer leaves or player ran out of time.
	/// </summary>
	public void resetOrderHelpers() {

		hideOrderHelpers ();
		
	}


	/// <summary>
	/// Simple scale animator functon.
	/// </summary>
	IEnumerator animateButton ( GameObject _btn  ){
		
		Vector3 startingScale = _btn.transform.localScale;		//initial scale	
		Vector3 destinationScale = startingScale * 0.85f;		//target scale

		//Scale up
		float t = 0.0f; 
		while (t <= 1.0f) {
			t += Time.deltaTime * 9;
			_btn.transform.localScale = new Vector3( Mathf.SmoothStep(startingScale.x, destinationScale.x, t),
				Mathf.SmoothStep(startingScale.y, destinationScale.y, t),
				_btn.transform.localScale.z);
			yield return 0;
		}

		//Scale down
		float r = 0.0f; 
		if(_btn.transform.localScale.x >= destinationScale.x) {
			while (r <= 1.0f) {
				r += Time.deltaTime * 9;
				_btn.transform.localScale = new Vector3( Mathf.SmoothStep(destinationScale.x, startingScale.x, r),
					Mathf.SmoothStep(destinationScale.y, startingScale.y, r),
					_btn.transform.localScale.z);
				yield return 0;
			}
		}
	}


	/// <summary>
	/// Play audioclips
	/// </summary>
	/// <param name="_sfx">Sfx.</param>
	void playNormalSfx ( AudioClip _sfx  ){
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}
}