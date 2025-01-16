using UnityEngine;
using UnityEngine.Video;   // Make sure to include this namespace
using UnityEngine.SceneManagement; // Needed for scene loading

public class IntroVideoManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string nextSceneName = "Minimalistic";

    void Start()
    {
        // Subscribe to the event that is triggered when the video finishes.
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void Update()
    {
        // Check if Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Stop the video
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
            }
            
            // Load the next scene
            LoadNextScene();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
