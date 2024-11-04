using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {

	/// <summary>
	/// This class manages pause and unpause states.
	/// </summary>

	public static bool isPaused;
	private float savedTimeScale;
	public GameObject pausePlane;

	enum Page { PLAY, PAUSE }
	private Page currentPage = Page.PLAY;


	void Awake (){		

		isPaused = false;		
		Time.timeScale = 1.0f;
		
		if(pausePlane)
	    	pausePlane.SetActive(false); 
	}


	void Update (){

		touchManager();
		
		//optional pause
		if(Input.GetKeyDown(KeyCode.P) || Input.GetKeyUp(KeyCode.Escape)) {

			//pause is not allowed when game is finished or has not started
			if(MainGameController.gameIsFinished || !MainGameController.gameIsStarted)
				return;

			//PAUSE THE GAME
			switch (currentPage) {
	            case Page.PLAY: 
					PauseGame(); 
					break;
	            case Page.PAUSE: 
					UnPauseGame(); 
					break;
	            default: 
					currentPage = Page.PLAY;
					break;
	        }
		}

		//debug restart
		if(Input.GetKeyDown(KeyCode.R)) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}


	void touchManager (){
		if(Input.GetMouseButtonUp(0)) {
			RaycastHit hitInfo;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hitInfo)) {
				string objectHitName = hitInfo.transform.gameObject.name;
				switch(objectHitName) {
					case "PauseBtn":
					
						//pause is not allowed when game is finished or has not started
						if(MainGameController.gameIsFinished || !MainGameController.gameIsStarted)
							return;
							
						switch (currentPage) {
				            case Page.PLAY: 
								PauseGame(); 
								break;
				            case Page.PAUSE: 
								UnPauseGame(); 
								break;
				            default: 
								currentPage = Page.PLAY;
								break;
				        }
						break;
					
					case "Btn-Resume":
						switch (currentPage) {
				            case Page.PLAY: 
								PauseGame(); 
								break;
				            case Page.PAUSE: 
								UnPauseGame(); 
								break;
				            default: 
								currentPage = Page.PLAY;
								break;
				        }
						break;
						
					case "Btn-Menu":
						UnPauseGame();
						SceneManager.LoadScene("Menu");
						break;
						
					case "Btn-Restart":
						UnPauseGame();
						SceneManager.LoadScene(SceneManager.GetActiveScene().name);
						break;
						
					case "End-Menu":
						SceneManager.LoadScene("Menu");
						break;

					case "End-Next":
						SceneManager.LoadScene("LevelSelection");
						break;
						
					case "End-Restart":
						SceneManager.LoadScene(SceneManager.GetActiveScene().name);
						break;
				}
			}
		}
	}


	void PauseGame (){
		print("Game in Paused...");
		isPaused = true;
		savedTimeScale = Time.timeScale;
	    Time.timeScale = 0;
	    AudioListener.volume = 0;
	    if(pausePlane)
	    	pausePlane.SetActive(true);
	    currentPage = Page.PAUSE;
	}


	void UnPauseGame (){
		print("Unpause");
	    isPaused = false;
	    Time.timeScale = savedTimeScale;
	    AudioListener.volume = 1.0f;
		if(pausePlane)
	    	pausePlane.SetActive(false);   
	    currentPage = Page.PLAY;
	}

}