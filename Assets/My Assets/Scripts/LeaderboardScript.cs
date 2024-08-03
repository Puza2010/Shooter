using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    public Text leaderboardText;
    private DatabaseReference databaseReference;

    void Start()
    {
        databaseReference = FindObjectOfType<FirebaseInitializer>().GetDatabaseReference();
    }

    public void DisplayLeaderboard(string country, int stage)
    {
        string path = $"scores/{country}/{stage}";
        databaseReference.Child(path).OrderByValue().LimitToLast(10).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<KeyValuePair<string, int>> scores = new List<KeyValuePair<string, int>>();

                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    string playerID = childSnapshot.Key;
                    int score = int.Parse(childSnapshot.Value.ToString());
                    scores.Add(new KeyValuePair<string, int>(playerID, score));
                }

                scores.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

                string leaderboard = "Leaderboard:\n";
                for (int i = 0; i < scores.Count; i++)
                {
                    leaderboard += $"{i + 1}. {scores[i].Key}: {scores[i].Value}\n";
                }

                leaderboardText.text = leaderboard;
            }
            else
            {
                Debug.LogError("Failed to retrieve leaderboard.");
            }
        });
    }
}