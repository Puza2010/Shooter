using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuVideoPlayerScript : MonoBehaviour
{
    private static MainMenuVideoPlayerScript instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}