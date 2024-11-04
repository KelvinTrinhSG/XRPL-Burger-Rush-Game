using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
	
	/// <summary>
	/// This class handles all touch events on buttons, and also updates the 
	/// player status (available-money) on screen.
	/// </summary>

	//This string is used for share & rate button and should match the exact bundle name you set in BuildSettings
	public static string gameBundleName = "com.yourcompany.yourgamename";
	public static string gameName = "gameName"; //custom game name used when player wants to share your game via social media apps

	private float buttonAnimationSpeed = 9;		//speed on animation effect when tapped on button
	private bool canTap = true;					//flag to prevent double tap

	public GameObject playerMoney;				//Reference to UI 3d text
	private int availableMoney;					//saved player money

	public AudioClip tapSfx;					//tap sound for buttons click


	/// <summary>
	/// Init. Updates the 3d texts with saved values fetched from playerprefs.
	/// </summary>
	void Awake (){
		
		//PlayerPrefs.DeleteAll();
		Time.timeScale = 1.0f;

		//Updates 3d text with saved values fetched from playerprefs
		availableMoney = PlayerPrefs.GetInt("PlayerMoney");
		playerMoney.GetComponent<TextMesh>().text = "Coins: " + availableMoney;
	}


	/// <summary>
	/// FSM
	/// </summary>
	void Update (){	
		if(canTap) {
			StartCoroutine(tapManager());
		}
	}


	/// <summary>
	/// Process player inputs
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
			switch(objectHit.name) {
			
				//Game Modes
				case "Button-01":
					playSfx(tapSfx);								//play touch sound
					StartCoroutine(animateButton(objectHit));		//touch animation effect
					yield return new WaitForSeconds(1.0f);			//Wait for the animation to end
					SceneManager.LoadScene("LevelSelection");		//Load the next scene
					break;	

				case "Button-02":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(1.0f);
					SceneManager.LoadScene("ShopAndPlay");
					break;

				case "Button-03":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(1.0f);
					Application.OpenURL("market://details?id=" + gameBundleName);
					break;

				//This button has its own controller
				case "Button-VideoAds":
					playSfx(tapSfx);
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(1.0f);
					break;

				//This button has its own controller
				case "Button-Share":
					StartCoroutine(animateButton(objectHit));
					yield return new WaitForSeconds(1.0f);
					Application.OpenURL("https://emc.network/");
					break;

			}	
		}
	}


	/// <summary>
	/// This function animates a button by modifying it's scales on x-y plane.
	// can be used on any element to simulate the tap effect.
	/// </summary>
	IEnumerator animateButton ( GameObject _btn  ){
		
		canTap = false;
		Vector3 startingScale = _btn.transform.localScale;		//initial scale	
		Vector3 destinationScale = startingScale * 0.85f;		//target scale
		
		//Scale up
		float t = 0.0f; 
		while (t <= 1.0f) {
			t += Time.deltaTime * buttonAnimationSpeed;
			_btn.transform.localScale = new Vector3( Mathf.SmoothStep(startingScale.x, destinationScale.x, t),
			                                      	 Mathf.SmoothStep(startingScale.y, destinationScale.y, t),
			                                        _btn.transform.localScale.z);
			yield return 0;
		}
		
		//Scale down
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
		
		if(r >= 1)
			canTap = true;
	}


	/// <summary>
	/// Play Audio clips
	/// </summary>
	/// <param name="_clip">Clip.</param>
	void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}

}