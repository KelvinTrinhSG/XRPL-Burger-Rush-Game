using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBoost : MonoBehaviour
{
    // Singleton instance
    public static ResourceBoost Instance { get; private set; }

    // Variable to store the resource boost value
    public int money = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetBoostValue()
    {
        money = 0;
    }
}