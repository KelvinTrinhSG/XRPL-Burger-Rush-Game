using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	public int dir = 1;
	public int speed = 40;
	
	// Simply rotate the object on the given pivot (Z here)
	void Update () {
		transform.rotation = Quaternion.Euler(0, 180, Time.time * speed * dir);
	}
}
