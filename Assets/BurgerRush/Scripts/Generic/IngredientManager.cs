using UnityEngine;
using System.Collections;

public class IngredientManager : MonoBehaviour {

	/// <summary>
	/// Simple class used to hold ID and Price of an ingredient.
	/// Notice that when a customer wants to settle, the game calculates the sum of price of all ingredients 
	/// used inside the ordered burger. So you need to carefully set a price for these ingredients.
	/// </summary>

	//ingredient's price
	public int price = 4;	//4 is the default price
	public int ingID;


}