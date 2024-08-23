using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class MatchMakingManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI timerText;
    public Button startButton;
    private float timer = 0f;
    private bool searching = false;

    // Assuming playerLevel is determined elsewhere, for this example, we set it here.
    public int playerLevel = 1; // This should be dynamically set based on the player's actual level
    private TypedLobby customLobby = new TypedLobby("MatchmakingLobby", LobbyType.Default);

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Connect to Photon

        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    void OnStartButtonClicked()
    {
        if (PhotonNetwork.IsConnected)
        { 
            JoinMatchmakingLobby();
        }
    }

    public override void OnConnectedToMaster()
    {
        //JoinMatchmakingLobby();
    }

    // Join a custom matchmaking lobby to start the matchmaking process
    void JoinMatchmakingLobby()
    {
        PhotonNetwork.JoinLobby(customLobby);  // Join the matchmaking lobby
    }

    // Callback when joining the lobby is successful
    public override void OnJoinedLobby()
    {
        StartMatchmaking();
    }

    // Start searching for rooms with the same level
    void StartMatchmaking()
    {
        searching = true;

        // Set the player's level in their custom properties
        Hashtable playerProperties = new Hashtable();
        playerProperties["Level"] = playerLevel;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

        // Search for available rooms with matching properties
        PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "C0.Level = " + playerLevel);
    }

    // Callback when available rooms are received
    public override void OnRoomListUpdate(System.Collections.Generic.List<RoomInfo> roomList)
    {
        RoomInfo bestMatch = null;

        foreach (RoomInfo room in roomList)
        {
            if (room.CustomProperties.ContainsKey("Level") && (int)room.CustomProperties["Level"] == playerLevel && room.PlayerCount < room.MaxPlayers)
            {
                bestMatch = room;
                break;
            }
        }

        // If a matching room is found, join it
        if (bestMatch != null)
        {
            PhotonNetwork.JoinRoom(bestMatch.Name);
        }
        else
        {
            // If no room found, create a new one
            Debug.Log("Not found, creat one");
            CreateNewRoom();
        }
    }

    void CreateNewRoom()
    {
        Hashtable roomProperties = new Hashtable();
        roomProperties["Level"] = playerLevel;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.CustomRoomProperties = roomProperties;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "Level" };

        PhotonNetwork.CreateRoom(null, roomOptions, customLobby);
    }

    public override void OnJoinedRoom()
    {
        searching = false;

        // After joining the room, transfer to the MatchReadyScene
        SceneManager.LoadScene("MatchReadyScene");
    }

    void Update()
    {
        if (searching)
        {
            timer += Time.deltaTime;
            timerText.text = "Searching for a player... " + Mathf.FloorToInt(timer).ToString();
        }
    }
}
