using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using Playniax.Pyro;

public class FirebaseScoreUpdater : MonoBehaviour
{
    public static FirebaseScoreUpdater Instance;

    private DatabaseReference databaseReference;
    private string userId;
    private string currentRunId;
    private int currentScore;
    private float elapsedTime;
    private int previousRank = -1;
    private bool isFirstMessage = true;

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
            StartNewRun();
            StartCoroutine(UpdateScoreRoutine());
        });
    }

    private void StartNewRun()
    {
        currentRunId = Guid.NewGuid().ToString(); // Unique identifier for each run
        elapsedTime = 0f;
        previousRank = -1; // Reset previous rank at the start of a new run
        isFirstMessage = true; // Mark the first message flag as true
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
            elapsedTime += 10f;
            if (!string.IsNullOrEmpty(userId))
            {
                SaveScore(currentScore, userId, currentRunId, Mathf.FloorToInt(elapsedTime));
            }
        }
    }

    private void SaveScore(int score, string userId, string runId, int time)
    {
        var scoreData = new Dictionary<string, object>
        {
            ["score"] = score,
            ["timestamp"] = DateTime.UtcNow.ToString("o")
        };

        databaseReference.Child("users").Child(userId).Child("runs").Child(runId).Child(time + "s").SetValueAsync(scoreData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                CompareScores(score, time);
            }
            else
            {
                Debug.LogError($"Failed to save score: {task.Exception}");
            }
        });
    }

    private void CompareScores(int currentScore, int time)
    {
        databaseReference.Child("users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<int> scoresAtTime = new List<int>();

                foreach (DataSnapshot userSnapshot in snapshot.Children)
                {
                    DataSnapshot runsSnapshot = userSnapshot.Child("runs");

                    foreach (DataSnapshot runSnapshot in runsSnapshot.Children)
                    {
                        DataSnapshot timeSnapshot = runSnapshot.Child(time + "s");
                        if (timeSnapshot.Exists)
                        {
                            int previousScore = int.Parse(timeSnapshot.Child("score").Value.ToString());
                            scoresAtTime.Add(previousScore);
                        }
                    }
                }

                if (scoresAtTime.Count == 0)
                {
                    Debug.Log("No scores found for this time interval.");
                    return;
                }

                // Sort scores in descending order (highest scores first)
                scoresAtTime.Sort((a, b) => b.CompareTo(a));

                // Find the rank of the current score
                int currentRank = scoresAtTime.FindIndex(s => s <= currentScore) + 1;

                Debug.Log($"Current score: {currentScore} is at position {currentRank} at {time}s among previous runs.");

                // Determine the correct suffix for the rank
                string rankSuffix = GetRankSuffix(currentRank);

                // Display message using Messenger with appropriate color
                string message = $"{currentRank}{rankSuffix}";
                Color messageColor = Color.green;

                if (!isFirstMessage)
                {
                    if (currentRank < previousRank)
                    {
                        messageColor = Color.green; // Improved rank
                    }
                    else if (currentRank > previousRank)
                    {
                        messageColor = Color.red; // Worsened rank
                    }
                }
                else
                {
                    isFirstMessage = false; // Set to false after the first message
                }

                // Update previous rank for next comparison
                previousRank = currentRank;

                // Display the message with the appropriate color
                Messenger.MessageSettings messageSettings = Messenger.instance.Get("Score");
                if (messageSettings != null)
                {
                    messageSettings.color = messageColor;
                }

                // Convert screen space position to world space
                Vector3 screenPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.8f, Camera.main.nearClipPlane + 10f));
                Messenger.instance.Create("Score", message, screenPosition);
            }
            else
            {
                Debug.LogError($"Failed to retrieve scores: {task.Exception}");
            }
        });
    }

    private string GetRankSuffix(int rank)
    {
        if (rank % 100 >= 11 && rank % 100 <= 13)
        {
            return "th";
        }

        switch (rank % 10)
        {
            case 1:
                return "st";
            case 2:
                return "nd";
            case 3:
                return "rd";
            default:
                return "th";
        }
    }
}
