using UnityEngine;
using System.Collections;

public class TextMeshController : MonoBehaviour {
	
	/// <summary>
	/// When we touch on player money to collect it, we need a UI text object to show us the amount of money
	/// received. This class will create a UI text object and animates it for a few seconds to show the player
	/// how much money he has received.
	/// </summary>

	internal Vector3 startingSize;		//initial object size
	internal string moneyAmount;		//money amount


	/// <summary>
	/// Init
	/// </summary>
	void Start (){
		
		//start with default scale.
		startingSize = transform.localScale;

		//animate the UI text object.
		StartCoroutine(scaleUp());
	}


	/// <summary>
	/// Animate by moving and resizing...
	/// </summary>
	IEnumerator scaleUp (){

		GetComponent<TextMesh>().text = moneyAmount;
		while(transform.localScale.x < 2) {
			transform.localScale = new Vector3(transform.localScale.x + 0.045f,
			                                   transform.localScale.y + 0.045f,
			                                   transform.localScale.z);
			transform.position = new Vector3(transform.position.x,
			                                 transform.position.y + 0.025f,
			                                 transform.position.z);
			yield return 0;
		}

		float t = 2;
		while(t > 0) {
			t -= Time.deltaTime;	
			transform.position = new Vector3(transform.position.x,
			                                 transform.position.y + 0.01f,
			                                 transform.position.z);
			if(t <= 0)
				Destroy(gameObject);
			
			yield return 0;
		}
	}

}