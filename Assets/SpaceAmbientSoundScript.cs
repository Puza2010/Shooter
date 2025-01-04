using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceAmbientSoundScript : MonoBehaviour
{
    private static SpaceAmbientSoundScript instance;

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
