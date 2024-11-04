using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class CareerMapManager : MonoBehaviour {

	/// <summary>
	/// CareerMapManager will load the game scene with parameters set by you
	/// for the selected level. It will saves those values inside playerPrefs and
	/// they will be fetched and applied in the game scene.
	/// </summary>

	static public int userLevelAdvance;			//our current progress in game
	public AudioClip menuTap;					//tap sfx
	private bool canTap;						//tap is allowed just once
	private float buttonAnimationSpeed = 9;

	void Awake (){
		canTap = true; //player can tap on buttons
		
		if(PlayerPrefs.HasKey("userLevelAdvance"))
			userLevelAdvance = PlayerPrefs.GetInt("userLevelAdvance");
		else
			userLevelAdvance = 0; //default. only level 1 in open.

		//cheat - debug
		//userLevelAdvance = 5;
	}


	void Start (){
		//prevent screenDim in handheld devices
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}


	void Update (){
		if(canTap)	
			StartCoroutine(tapManager());
	}


	///***********************************************************************
	/// Process user inputs
	///***********************************************************************
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
			print(objectHit.tag);

			if(objectHit.tag == "levelSelectionItem") {
				canTap = false;
				playSfx(menuTap);
				StartCoroutine(animateButton(objectHit));
				
				//save the game mode
				PlayerPrefs.SetString("gameMode", "CAREER");
				PlayerPrefs.SetInt("careerLevelID", objectHit.GetComponent<CareerLevelSetup>().levelID);

				//save the game time
				PlayerPrefs.SetInt("careerAvailableTime", objectHit.GetComponent<CareerLevelSetup>().levelTime);
								
				//save mission variables
				PlayerPrefs.SetInt("careerMission", objectHit.GetComponent<CareerLevelSetup>().levelMission);

				//save level complexity
				PlayerPrefs.SetInt("levelComplexity", objectHit.GetComponent<CareerLevelSetup>().levelComplexity);

				//set level location
				PlayerPrefs.SetInt("levelLocation", (int)objectHit.GetComponent<CareerLevelSetup>().shopLocation);

				//set available ingredients
				int availableIngredients = objectHit.GetComponent<CareerLevelSetup>().availableIngredients.Length;
				PlayerPrefs.SetInt("availableIngredients", availableIngredients); //save the length of availableIngredients
				for(int i = 0; i < availableIngredients; i++) {
					PlayerPrefs.SetInt(	"careerIngredient_" + i.ToString(), 
						objectHit.GetComponent<CareerLevelSetup>().availableIngredients[i]);
				}				
				
				yield return new WaitForSeconds(0.5f);
				SceneManager.LoadScene("Game");
			}
				
			if(objectHit.name == "BackButton") {
				playSfx(menuTap);
				StartCoroutine(animateButton(objectHit));
				yield return new WaitForSeconds(1.0f);
				SceneManager.LoadScene("Menu");
				yield break;
			}
		}
	}
		

	///***********************************************************************
	/// Animate button by modifying it's scale
	///***********************************************************************
	IEnumerator animateButton ( GameObject _btn  ){
		Vector3 startingScale = _btn.transform.localScale;
		Vector3 destinationScale = startingScale * 0.85f;
		//yield return new WaitForSeconds(0.1f);
		float t = 0.0f; 
		while (t <= 1.0f) {
			t += Time.deltaTime * buttonAnimationSpeed;
			_btn.transform.localScale = new Vector3( Mathf.SmoothStep(startingScale.x, destinationScale.x, t),
			                                        Mathf.SmoothStep(startingScale.y, destinationScale.y, t),
			                                        _btn.transform.localScale.z);
			yield return 0;
		}
		
		float r = 0.0f; 
		if(_btn.transform.localScale.x >= destinationScale.x) {
			while (r <= 1.0f) {
				r += Time.deltaTime * buttonAnimationSpeed;
				_btn.transform.localScale = new Vector3( Mathf.SmoothStep(destinationScale.x, startingScale.x, r),
				                                        Mathf.SmoothStep(destinationScale.y, startingScale.y, r),
				                                        _btn.transform.localScale.z);
				yield return 0;
			}
		}
		
		//if(r >= 1)
			//canTap = true;
	}


	///***********************************************************************
	/// play audio clip
	///***********************************************************************
	void playSfx ( AudioClip _sfx  ){
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Play();
	}


}