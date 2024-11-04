using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customerMoneyController : MonoBehaviour {

	/// <summary>
	/// When customer receives the order, it leaves the money on the table.
	/// We use this controller on the money object to let it be collectable by player.
	/// </summary>

	public int moneyAmount;			//How much money inside this object?
	public AudioClip coinTap;		//Tap sfx
	public GameObject moneyText;	//UI object used to show the amount of money when player collects the money
	private bool canTap;			//tap flag
	private GameObject GC;			//reference to main game controller


	void Awake () {

		//find and cache gameController
		GC = GameObject.FindGameObjectWithTag ("GameController");
	}


	void Start () {
		canTap = true;
		transform.position = new Vector3 (transform.position.x + UnityEngine.Random.Range(-1.5f, 1.0f), 
			transform.position.y, 
			transform.position.z - (UnityEngine.Random.value) );
	}
	

	void OnMouseDown() {

		if (!canTap)
			return;

		canTap = false;
		playSFX (coinTap);
		collectMoney ();
		StartCoroutine (reactiveTap ());

	}


	/// <summary>
	/// Collecting the money:
	/// 1. First we need to add the money to gameController
	/// 2. Then we need to update level mission
	/// 3. Then we need to create a UI element to show the collected money 
	/// 4. The final step is destroying this instance.
	/// </summary>
	void collectMoney() {

		MainGameController.totalMoneyMade += moneyAmount;
		GC.GetComponent<MainGameController> ().updateLevelMission ();

		//money text object on UI
		GameObject mt = Instantiate(moneyText, 
			transform.position, 
			Quaternion.Euler(0, 0, 0)) as GameObject;
		mt.name = "MoneyText";
		mt.GetComponent<TextMeshController> ().moneyAmount = "+$" + moneyAmount.ToString ();

		GetComponent<Renderer> ().enabled = false;
		GetComponent<BoxCollider> ().enabled = false;
		Destroy (gameObject, 1.0f);


	}


	IEnumerator reactiveTap() {
		yield return new WaitForSeconds (0.5f);
		canTap = true;
	}


	void playSFX ( AudioClip _sfx  ){
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}
}
