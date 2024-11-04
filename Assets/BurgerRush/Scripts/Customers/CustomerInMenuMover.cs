using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerInMenuMover : MonoBehaviour {

	/// <summary>
	/// Simple customers in menu scene just need to enter from left side and exit from right side.
	/// We are using them to add some life to the menu scene. This is not mandatory at all.
	/// </summary>

	public Material[] availableFaces;					//different textures for spawned customer objects

	internal Vector3 dest = new Vector3 (12, 0, 0);		//destination
	private Vector3 startingPos;						//starting position of this object
	private float speed;								//move speed
	private float timeVariance;							//position on the sine wave


	void Awake () {

		speed = Random.Range (1.5f, 4.5f);
		startingPos = transform.position;
		GetComponent<Renderer>().material = availableFaces[Random.Range(0, availableFaces.Length)];
		StartCoroutine(move ());
		
	}
	

	IEnumerator move (){
		
		timeVariance = Random.value;
		while(transform.position.x < dest.x) {
			
			transform.position = new Vector3(transform.position.x + (Time.deltaTime * speed),
				startingPos.y - 0.25f + (Mathf.Sin((Time.time + timeVariance) * 10) / 8),
				transform.position.z);

			if(transform.position.x >= dest.x) {
				Destroy (gameObject);
			}

			yield return 0;
		}
	}

}
