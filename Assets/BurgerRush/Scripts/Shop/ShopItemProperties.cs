using UnityEngine;
using System.Collections;

public class ShopItemProperties : MonoBehaviour {
	
	/// <summary>
	/// A very simple value holder for different shop items.
	// You can easily add/edit item properties via this controller.
	/// </summary>

	public int itemIndex;			//ID
	public int itemPrice;			//Price
	public GameObject priceTag;		//UI object used to show the price in game

	void Awake (){

		if(PlayerPrefs.GetInt("shopItem-" + itemIndex.ToString()) != 1)
			priceTag.GetComponent<TextMesh>().text = "Price: $" + itemPrice;
		else 
			priceTag.GetComponent<TextMesh>().text = "Purchased";
		
	}
}