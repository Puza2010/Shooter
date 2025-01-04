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

    private void OnVideoFinished(VideoPlayer vp)
    {
        // Load the next scene once the video is done
        SceneManager.LoadScene(nextSceneName);
    }
}
