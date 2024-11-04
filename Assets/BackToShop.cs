using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToShop : MonoBehaviour
{
    public void BackToShopAction() {
        SceneManager.LoadScene("ShopAndPlay");
    }
}
