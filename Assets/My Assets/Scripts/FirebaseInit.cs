using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    private FirebaseAuth auth;
    private DatabaseReference databaseReference;

    void Start()
    {
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
                    auth = FirebaseAuth.DefaultInstance;
                    databaseReference = FirebaseDatabase.GetInstance("https://rogue-shoot-em-up-default-rtdb.europe-west1.firebasedatabase.app/").RootReference;
                    SignInAnonymously();
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            }
            else
            {
                Debug.LogError("Failed to check and fix dependencies: " + task.Exception);
            }
        });
    }

    private void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            AuthResult authResult = task.Result;
            FirebaseUser newUser = authResult.User;
            Debug.LogFormat("User signed in anonymously: {0} ({1})", newUser.DisplayName, newUser.UserId);
        });
    }

    public DatabaseReference GetDatabaseReference()
    {
        return databaseReference;
    }
}