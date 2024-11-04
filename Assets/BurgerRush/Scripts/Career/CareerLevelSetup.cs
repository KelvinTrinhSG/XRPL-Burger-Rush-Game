using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CareerLevelSetup : MonoBehaviour {
	
	/// <summary>
	/// Use this class to set different missions for each level.
	/// when you click/tap on any level button, these values automatically gets saved 
	/// inside playerPrefs and then gets read when the game starts.
	/// </summary>

	[Header("Child Objects")]
	public GameObject label;				//reference to child gameObject

	[Header("Level Settings")]
	public int levelID;						//unique level identifier. Starts from 1.
	public int levelTime;					//available time to beat the level
	[Range(4, 9)]
	public int levelComplexity = 4;			//indicates the complexity of the orders by customers in this level.
											//this indicated the maximum number of ingredients an order can hold.

	/* Indexes
	 * Beach = 0
	 * Restaurant = 1
	 * ...
	 * */
	public enum environments { Beach = 0, Restaurant = 1 }
	public environments shopLocation = environments.Beach;

	[Header("Mission & Stars settings")]
	//for winning 1, 2 or 3 stars
	//Important: beating default levelMission grants 1 star.
	//for 2 stars, player needs to reach 25% more of the initial levelMission.
	//for 3 stars, player needs to reach 50% more of the initial levelMission.
	//You just need to set mission 1. Missions 2 & 3 will be calcualted automatically.
	public int levelMission;

	[Header("Available Ingredients in this level (Ing IDs)")]
	//Important:
	//Ingredient IDs 1 & 2 are used for the bottom & top bread, and should always be available if you want
	//your final burgers to look good.
	public int[] availableIngredients;			//array of indexes of available ingredients. starts from 1.

	[Header("Game Objects & Materials")]
	//star rating for levels
	// Unlocked levels will always show 0-star image.
	public GameObject levelStarsGo;	//reference to child game object
	public Material[] starMats;		//avilable star materials
	public Material[] openStatus;	//change texture if this level is opened or not
	private int levelStars;			//saved stars for this level

	/// <summary>
	/// Init
	/// </summary>
	void Start (){

		//set level number
		label.GetComponent<TextMesh>().text = levelID.ToString();
		
		if(CareerMapManager.userLevelAdvance >= levelID - 1) {
			
			//this level is open
			GetComponent<BoxCollider>().enabled = true;
			GetComponent<Renderer>().material = openStatus[1];

			//grant a few stars
			levelStars = PlayerPrefs.GetInt("Level-" + levelID.ToString() + "-Stars");
			if (levelStars == 3) {
				//3-star
				levelStarsGo.GetComponent<Renderer>().material = starMats[3];
			} else if (levelStars == 2) {
				//2-star
				levelStarsGo.GetComponent<Renderer>().material = starMats[2];
			} else if (levelStars == 1) {
				//1-star
				levelStarsGo.GetComponent<Renderer>().material = starMats[1];
			} else if (levelStars == 0) {
				//0-star (only occures if this is the first time we want to play this level)
				levelStarsGo.GetComponent<Renderer>().material = starMats[0];
			}

			//use animation for the last opened level
			if(CareerMapManager.userLevelAdvance == levelID - 1)
				GetComponent<HeartBeatAnimationEffect>().enabled = true;

		} else {
			
			//level is locked
			GetComponent<BoxCollider>().enabled = false;
			GetComponent<Renderer>().material = openStatus[0];

			//set 0-star image
			levelStarsGo.GetComponent<Renderer>().material = starMats[0];

			//set heartbeat animation to inactive
			GetComponent<HeartBeatAnimationEffect>().enabled = false;
		}
	}
}