#pragma warning disable 0414
#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif


public class UnityAdsRewardedButton : MonoBehaviour {

	/// <summary>
	/// We use a videoAd button for two purposes.
	/// 1. We have an instance in the menu scene (without isSaveMe flag) that grants the player a few coins 
	/// after watching a short video.
	/// 2. We have another instance in Game scene (with isSaveMe) which is used to show a video ad when player lost the game,
	/// and needs a revive button with some additional time.
	/// </summary>

	public bool isSaveMe = false;	//determines if we are using this video button to save player or grant simple video reward

	private bool canTap = true;		//just one tap is allowed
	private string zoneId;			//Important. setting this to null plays the default video.
									//if you want to show rewarded video ads, you have to set your
									//own "Placement Id" here. See Unity Ads dashboard for more info.

	private int rewardCoins = 100;	//reward is 100 free coins
	private int extraTime = 30;		//reward is 30 extra seconds
	private bool status;			//is ad ready to show?

	#if UNITY_ADS
	ShowOptions options = new ShowOptions();
	#endif

	private GameObject VideoAdHelperText;		//helper text used in Game scene

	void Start() {
		VideoAdHelperText = GameObject.FindGameObjectWithTag ("VideoAdHelperText");
	}

	void Update () {	

		if (string.IsNullOrEmpty (zoneId)) 
			zoneId = null;

		#if UNITY_ADS
		status = Advertisement.IsReady (zoneId) ? true : false;
		options.resultCallback = HandleShowResult;
		#endif

		//No video button if video ads is not ready to play
		if(status) {
			
			GetComponent<BoxCollider>().enabled = true;
			GetComponent<Renderer>().enabled = true;
			if (VideoAdHelperText)
				VideoAdHelperText.SetActive (true);

		} else {
			
			GetComponent<BoxCollider>().enabled = false;
			GetComponent<Renderer>().enabled = false;
			if (VideoAdHelperText)
				VideoAdHelperText.SetActive (false);

		}

		if(canTap)
			touchManager();
	}

	private RaycastHit hitInfo;
	private Ray ray;
	void touchManager () {
		
		//Mouse of touch?
		if(	Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)  
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
		else if(Input.GetMouseButtonUp(0))
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		else
			return;
		
		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			switch(objectHit.name) {
				
			case "Button-VideoAds":
				canTap = false;
				//hide videoAds button by moving it far away
				transform.position = new Vector3(transform.position.x, transform.position.y, 10000);
				StartCoroutine(reactiveTap());

				#if UNITY_ADS
				Advertisement.Show (zoneId, options);
				#endif

				break;	
			}
		}
	}

	#if UNITY_ADS
	private void HandleShowResult (ShowResult result) {
		
		switch (result) {

		case ShowResult.Finished:

			if (isSaveMe) {

				Debug.Log ("Video completed. Grant player some extra time!");
				MainGameController.startTime = (int)Time.time + extraTime;
				MainGameController.gameIsFinished = false;

				//hide EndGamePlane object to let the player continue his game
				GameObject egp = GameObject.FindGameObjectWithTag ("EndGamePlane");
				if (egp) {
					egp.SetActive (false);
				}

			} else {
				Debug.Log ("Video completed. User rewarded " + rewardCoins + " coins.");
				//add and save reward
				PlayerPrefs.SetInt ("PlayerMoney", PlayerPrefs.GetInt ("PlayerMoney") + rewardCoins);
				SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
			}

			break;

		case ShowResult.Skipped:
			Debug.LogWarning ("Video was skipped.");
			break;

		case ShowResult.Failed:
			Debug.LogError ("Video failed to show.");
			break;
		}

	}
	#endif


	IEnumerator reactiveTap() {
		yield return new WaitForSeconds(1.0f);
		canTap = true;
	}
}