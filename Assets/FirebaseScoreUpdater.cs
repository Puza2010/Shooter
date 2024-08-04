using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseScoreUpdater : MonoBehaviour
{
    public static FirebaseScoreUpdater Instance;

    private DatabaseReference databaseReference;
    private string userId;
    private int currentScore;

    private void Awake()
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

        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    FirebaseApp app = FirebaseApp.DefaultInstance;
                    databaseReference = FirebaseDatabase.GetInstance("https://rogue-shoot-em-up-default-rtdb.europe-west1.firebasedatabase.app/").RootReference;
                    SignInAnonymously();
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            }
            else
            {
                Debug.LogError($"Failed to check and fix dependencies: {task.Exception}");
            }
        });
    }

    private void SignInAnonymously()
    {
        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"SignInAnonymouslyAsync encountered an error: {task.Exception}");
                return;
            }

            AuthResult authResult = task.Result;
            userId = authResult.User.UserId;
            Debug.LogFormat("User signed in anonymously: {0} ({1})", authResult.User.DisplayName, userId);
            StartCoroutine(UpdateScoreRoutine());
        });
    }

    public void UpdateScore(int score)
    {
        currentScore = score;
    }

    private IEnumerator UpdateScoreRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10);
            if (!string.IsNullOrEmpty(userId))
            {
                SaveScore(currentScore, userId);
            }
        }
    }

    private void SaveScore(int score, string userId)
    {
        databaseReference.Child("scores").Child(userId).SetValueAsync(score);
    }
}
