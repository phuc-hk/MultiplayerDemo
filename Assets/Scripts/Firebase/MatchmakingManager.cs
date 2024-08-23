using Firebase.Database;
using Photon.Pun;
using UnityEngine;
using System.Collections;

public class MatchmakingManager : MonoBehaviour
{
    private DatabaseReference dbReference;
    private bool searchingForOpponent = false;
    string userId = "Phuc";

    void Start()
    {
        // Initialize Firebase database reference
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Add the player to the matchmaking queue
    public void AddPlayerToQueue()
    {
        string key = dbReference.Child("queue").Push().Key;
        dbReference.Child("queue").Child(key).SetValueAsync(userId).ContinueWith(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Player added to queue");
                StartSearchingForOpponent(); // Start searching after adding to queue
            }
        });
    }

    // Start continuously searching for an opponent
    public void StartSearchingForOpponent()
    {
        if (!searchingForOpponent)
        {
            searchingForOpponent = true;
            StartCoroutine(SearchForOpponentCoroutine());
        }
    }

    // Coroutine to search for an opponent at regular intervals
    private IEnumerator SearchForOpponentCoroutine()
    {
        while (searchingForOpponent)
        {
            FindOpponent(); // Call method to find an opponent
            yield return new WaitForSeconds(5); // Wait for 5 seconds before checking again
        }
    }

    // Stop searching for an opponent if needed
    public void StopSearchingForOpponent()
    {
        searchingForOpponent = false;
        StopCoroutine(SearchForOpponentCoroutine());
    }

    // Find an opponent from the Firebase queue
    public void FindOpponent()
    {
        dbReference.Child("queue").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && task.Result.ChildrenCount > 1) // Check if there are at least two players
            {
                DataSnapshot snapshot = task.Result;
                string currentPlayerId = PhotonNetwork.NickName; // Assume Photon username is the userId
                Debug.Log("Find opponent");
                foreach (var player in snapshot.Children)
                {
                    string playerId = player.Value.ToString();
                    if (playerId != currentPlayerId) // Make sure we're not matching the same player
                    {
                        // Start match with opponent
                        StartMatchWithPlayer(playerId);
                        dbReference.Child("queue").Child(player.Key).RemoveValueAsync(); // Remove the matched player from queue
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("No opponent found yet.");
            }
        });
    }

    // Start match with an opponent by creating/joining a room in Photon
    public void StartMatchWithPlayer(string opponentId)
    {
        string roomName = "Room_" + opponentId + "_" + PhotonNetwork.NickName; // Create unique room name

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinOrCreateRoom(roomName, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);
        }
    }
}