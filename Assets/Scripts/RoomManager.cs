using Firebase.Auth;
using Firebase.Database;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    //public TextMeshProUGUI roomName;

    //public void JoinRoom()
    //{
    //    GameObject.Find("DemoPhoton").GetComponent<DemoPhoton>().JoinRoomByName(roomName.text);
    //}

    // Firebase database reference
    //public DatabaseManager databaseManager;
    private FirebaseUser currentUser;

    // Button references
    public Button readyStartButton;
    public TextMeshProUGUI buttonText;

    private bool isRoomOwner = false;
    private PhotonView photonView;

    void Awake()
    {
        // Enable automatic scene synchronization for all players in the room
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        // Check if the current player is the room owner
        CheckRoomOwnership();

        // Set up Firebase listener to check player readiness
        MonitorPlayerReadiness();
        photonView = GetComponent<PhotonView>();
    }

    // Called when player clicks the Ready/Start button
    public void OnReadyStartButtonClicked()
    {
        if (isRoomOwner)
        {
            // Owner starts the game
            photonView.RPC("RPC_StartGame", RpcTarget.All);  // Notify all players to start the game
        }
        else
        {
            // Non-owner player presses Ready button
            SetPlayerReadyStatus(AuthenticationManager.Instance.userId, true);
            readyStartButton.interactable = false; // Disable the Ready button after clicking
        }
    }

    [PunRPC]  // Declare this method as an RPC
    private void RPC_StartGame()
    {
        // Logic to start the game for all players
        PhotonNetwork.LoadLevel("5MatchScene");  // Load the match scene for everyone
    }

    // Start the game
    private void StartGame()
    {
        // Logic to start the game
        PhotonNetwork.LoadLevel("5MatchScene"); // Load the match scene
    }

    // Update the player's readiness status in Firebase
    private void SetPlayerReadyStatus(string userId, bool isReady)
    {
        //if (databaseManager != null)
        {
            DatabaseManager.Instance.UpdatePlayerReadyStatus(userId, isReady);
        }
    }


    // Check if the player is the room owner
    private void CheckRoomOwnership()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Player is the room owner
            isRoomOwner = true;
            buttonText.text = "Start";
        }
        else
        {
            // Player is not the room owner
            isRoomOwner = false;
            buttonText.text = "Ready";
        }
    }

    private void MonitorPlayerReadiness()
    {
        Debug.Log("Checking player readiness...");

        DatabaseReference playersRef = FirebaseDatabase.DefaultInstance.GetReference("Players")
                                                    .Child(AuthenticationManager.Instance.userId)
                                                    .Child("IsReady");

        // Listen for changes in the players' readiness status
        playersRef.ValueChanged += (sender, args) =>
        {
            bool newStatus = (bool)args.Snapshot.Value;
            if (newStatus == true)
            {
                Debug.Log("Player " + AuthenticationManager.Instance.userId + " readiness in room has changed to " + newStatus);
                //if (AreAllPlayersReady())
                DatabaseManager.Instance.UpdateRoomReadyStatus(PhotonNetwork.CurrentRoom.Name, true);
                UpdateStartButtonState();
            }
        };

        ListenToRoomStatus();

        // Listen for when a new player joins the room
        PhotonNetwork.NetworkingClient.EventReceived += (eventData) =>
        {
            if (eventData.Code == Photon.Realtime.EventCode.Join)  // When a player joins
            {
                Debug.Log("A new player has joined the room.");
                DatabaseManager.Instance.UpdateRoomReadyStatus(PhotonNetwork.CurrentRoom.Name, false);
                UpdateStartButtonState(); // Update the Start button when a new player joins
            }
        };

        // Initial button state update
        UpdateStartButtonState();
    }

    void ListenToRoomStatus()
    {
        FirebaseDatabase.DefaultInstance.GetReference("Rooms").Child(PhotonNetwork.CurrentRoom.Name).Child("IsReady").ValueChanged += RoomStatusChanged;
    }

    void RoomStatusChanged(object sender, ValueChangedEventArgs args)
    {
       
        if (args.Snapshot.Exists)
        {
            bool newStatus = (bool)args.Snapshot.Value;
            if (newStatus == true)
            {
                UpdateStartButtonState();
                //Debug.Log("Room ready da thay doi roi ne " + newStatus);
            }           
        }
    }

    public void UpdateStartButtonState()
    {
        if (isRoomOwner)
        {
            // If the room owner is the only player in the room, keep the Start button interactable
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                readyStartButton.interactable = true;
                Debug.Log("Only the owner is in the room. Start button is interactable.");
            }
            else
            {
                // Check if all players are ready (wait until all are checked)
                CheckAllPlayersReady((allReady) =>
                {
                    try
                    {
                        readyStartButton.interactable = allReady;
                       
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Exception occurred: " + ex.Message);
                    }
                });
            }
        }
    }

    // Method to check if all players in the room are ready, using a callback
    private void CheckAllPlayersReady(System.Action<bool> callback)
    {
        // List to store the readiness statuses for all players
        List<bool> readinessStatuses = new List<bool>();

        // Get list of players in the room from Photon
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // Fetch the Firebase User ID from custom properties
            if (player.CustomProperties.ContainsKey("FirebaseUserId"))
            {
                string firebaseUserId = player.CustomProperties["FirebaseUserId"] as string;

                // Fetch readiness status from Firebase for each player using Firebase user ID
                DatabaseManager.Instance.GetPlayerReadyStatus(firebaseUserId, (isReadyPlayer) =>
                {
                    // Store the readiness status of the player
                    readinessStatuses.Add(isReadyPlayer);

                    // Check if we've fetched readiness statuses for all players
                    if (readinessStatuses.Count == PhotonNetwork.PlayerList.Length)
                    {
                        // If any player is not ready, the result should be false
                        bool allReady = !readinessStatuses.Contains(false);
                        callback(allReady);  // Invoke the callback with the final result
                    }
                });
            }
        }
    }
}
