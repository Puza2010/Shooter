using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Playniax.Ignition;

public class SpaceAmbientSoundScript : MonoBehaviour
{
    private static SpaceAmbientSoundScript instance;
    private AudioSource audioSource;
    private EasyGameUI easyGameUI;

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

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        easyGameUI = FindObjectOfType<EasyGameUI>();
        
        // Start playing if we're in the main menu
        if (SceneManager.GetActiveScene().name == "Minimalistic")
        {
            audioSource.Play();
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Find EasyGameUI if not already set
        if (easyGameUI == null)
        {
            easyGameUI = FindObjectOfType<EasyGameUI>();
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        // Find EasyGameUI if not set (can happen after scene changes)
        if (easyGameUI == null)
        {
            easyGameUI = FindObjectOfType<EasyGameUI>();
            return;
        }

        // Check UI panel states and handle audio
        bool shouldPlayAudio = false;

        // Check if we're in the main menu scene
        if (SceneManager.GetActiveScene().name == "Minimalistic")
        {
            shouldPlayAudio = true;
        }

        // Check if home panel is active (when returning from game)
        if (easyGameUI.home != null && easyGameUI.home.gameObject.activeInHierarchy)
        {
            shouldPlayAudio = true;
        }

        // Check if game panel is active
        if (easyGameUI.inGame != null && easyGameUI.inGame.gameObject.activeInHierarchy)
        {
            shouldPlayAudio = false;
        }

        // Apply audio state
        if (shouldPlayAudio && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (!shouldPlayAudio && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset EasyGameUI reference on scene change
        easyGameUI = FindObjectOfType<EasyGameUI>();

        // Handle scene changes
        if (scene.name == "TheGame")
        {
            audioSource.Stop();
        }
        else if (scene.name == "Minimalistic")
        {
            audioSource.Play();
        }
    }
}
