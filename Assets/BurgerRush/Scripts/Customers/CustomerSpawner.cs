using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour {

	/// <summary>
	/// Simple spawner class used to spawn customer objects in the menu scene.
	/// This is totally optional and you can remove this class from the project
	///if you don't need walking customers in menu scene.
	/// </summary>

	public GameObject customerGo;			//Prefab
	private bool canSpawnNewCustomer;		//Spawn flag
	private int spawnDir;					//Spawn direction. Not implemented. They only spawn from left, for now.


	void Start () {
		spawnDir = -1;
		canSpawnNewCustomer = true;
	}
	
	void Update () {

		if (canSpawnNewCustomer) {

			canSpawnNewCustomer = false;
			StartCoroutine (reactiveSpawn ());

			GameObject c = Instantiate (customerGo, new Vector3 (11 * spawnDir, 0, 0), Quaternion.Euler (0, 180, 0)) as GameObject;
			c.name = "CustomerInMenuScene";
			c.GetComponent<CustomerInMenuMover> ().dest = new Vector3 (11 * spawnDir * -1, 0, 0);
		}
		
	}


	IEnumerator reactiveSpawn() {
		yield return new WaitForSeconds (2.5f);
		canSpawnNewCustomer = true;
	}
}
