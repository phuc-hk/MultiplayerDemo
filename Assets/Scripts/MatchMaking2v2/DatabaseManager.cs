using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    // Firebase Realtime Database reference
    private DatabaseReference dbReference;
    //private AuthenticationManager authenticationManager;

    void Awake()
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
    }

    void Start()
    {
        // Initialize Firebase Database
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        //AuthenticationManager.Instance.OnSignIn.AddListener(AddPlayerToDatabase);
    }

    #region PLAYERS
    // Method to add a player to the "Players" node in Firebase
    public void AddPlayerToDatabase(string userId)
    {
        if (userId == null)
        {
            Debug.LogError("User is null. Cannot add player to database.");
            return;
        }

        //string userId = user.UserId;

        // Creating a player object to store player data
        PlayerData playerData = new PlayerData("Anonymous_Player", false, "NoRoom");

        // Add player data to the "Players" node in the Firebase Database
        dbReference.Child("Players").Child(userId).SetRawJsonValueAsync(JsonUtility.ToJson(playerData)).ContinueWith(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Player data added to Firebase database for User ID: " + userId);
            }
            else
            {
                Debug.LogError("Failed to add player to Firebase database: " + task.Exception);
            }
        });

        //SceneManager.LoadSceneAsync("2MainScene");
    }

    // Method to update the player's RoomID after creating or joining a room
    public void UpdatePlayerRoomID(string userId, string roomId)
    {
        // Update the player's RoomID in the "Players" node
        dbReference.Child("Players").Child(userId).Child("RoomID").SetValueAsync(roomId).ContinueWith(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Player's RoomID updated to: " + roomId);
            }
            else
            {
                Debug.LogError("Failed to update player's RoomID: " + task.Exception);
            }
        });
    }

    // Method to update the player's ready status in Firebase
    public void UpdatePlayerReadyStatus(string userId, bool isReady)
    {
        dbReference.Child("Players").Child(userId).Child("IsReady").SetValueAsync(isReady).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log(userId + " ready status updated: " + isReady);
            }
            else
            {
                Debug.LogError("Failed to update ready status: " + task.Exception);
            }
        });
    }

    // Method to retrieve player's ready status from Firebase
    //public bool GetPlayerReadyStatus(string userId)
    //{
    //    bool isReady = false;

    //    dbReference.Child("Players").Child(userId).Child("IsReady").GetValueAsync().ContinueWith(task =>
    //    {
    //        if (task.IsCompleted)
    //        {
    //            isReady = (bool)task.Result.Value;
    //            Debug.Log(userId + " Player ready status fetched: " + isReady);
               
    //        }
    //        else
    //        {
    //            Debug.LogError("Failed to fetch ready status: " + task.Exception);
    //        }
    //    });

    //    return isReady;
    //}

    // Method to retrieve player's ready status using a callback
    public void GetPlayerReadyStatus(string userId, System.Action<bool> callback)
    {
        dbReference.Child("Players").Child(userId).Child("IsReady").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                bool isReady = task.Result.Exists && (bool)task.Result.Value;
                Debug.Log(userId + " Player ready status fetched: " + isReady);
                callback(isReady);  // Invoke the callback with the result
            }
            else
            {
                Debug.LogError("Failed to fetch ready status: " + task.Exception);
                callback(false);  // Return false in case of failure
            }
        });
    }
    #endregion

    #region ROOMS
    // Method to add a room to the "Rooms" node in Firebase
    public void AddRoomToDatabase(string roomId, string ownerId)
    {
        // Create room data
        RoomData roomData = new RoomData(roomId, ownerId);

        // Add room data to the "Rooms" node in Firebase Database
        dbReference.Child("Rooms").Child(roomId).SetRawJsonValueAsync(JsonUtility.ToJson(roomData)).ContinueWith(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Room added to Firebase: " + roomId);
            }
            else
            {
                Debug.LogError("Failed to add room to Firebase: " + task.Exception);
            }
        });
    }

    // Method to update the room's ready status in Firebase
    public void UpdateRoomReadyStatus(string roomId, bool isReady)
    {
        dbReference.Child("Rooms").Child(roomId).Child("IsReady").SetValueAsync(isReady).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Player ready status updated: " + isReady);
            }
            else
            {
                Debug.LogError("Failed to update ready status: " + task.Exception);
            }
        });
    }

    // Add player to Firebase room
    public void AddPlayerToRoom(string roomId, string userId)
    {
        dbReference.Child("Rooms").Child(roomId).Child("Players").Child(userId).SetValueAsync(true).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Player added to room: " + userId);
            }
            else
            {
                Debug.LogError("Failed to add player to room: " + task.Exception);
            }
        });
    }

    #endregion

    // Player data model class
    public class PlayerData
    {
        public string PlayerName;
        public bool IsReady;
        public string RoomID;

        public PlayerData(string playerName, bool isReady, string roomID)
        {
            this.PlayerName = playerName;
            this.IsReady = isReady;
            this.RoomID = roomID;
        }
    }

    // Room data model class
    public class RoomData
    {
        public string RoomID;
        public string OwnerID;

        public RoomData(string roomID, string ownerID)
        {
            this.RoomID = roomID;
            this.OwnerID = ownerID;
        }
    }
}
