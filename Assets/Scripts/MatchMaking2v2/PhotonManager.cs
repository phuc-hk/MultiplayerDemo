using Firebase.Auth;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance { get; private set; }
    public TextMeshProUGUI textState;
    private string roomName;
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
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
        //textState.text = "Loading...";
        Debug.Log("Photon connected to master");
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        //textState.text = "Connected";
        Debug.Log("Photon joined lobby");
    }

    public void CreateOrJoinRoom(string roomName)
    {
        RoomOptions roomOptions = new();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new();
        roomOptions.MaxPlayers = 2;
        roomName = "Room " + Random.Range(1, 10);
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        //PhotonNetwork.JoinOrCreateRoom()
    }

    public void JoinRoom()
    {
        //PhotonNetwork.JoinRoom(inputRoomName.text);
    }

    public void JoinRoomByName(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Create room succesfuly");
        //string roomOwner = AuthenticationManager.Instance.userId;
        DatabaseManager.Instance.AddRoomToDatabase(roomName, AuthenticationManager.Instance.userId);
        DatabaseManager.Instance.UpdatePlayerRoomID(AuthenticationManager.Instance.userId, roomName);
        DatabaseManager.Instance.UpdatePlayerReadyStatus(AuthenticationManager.Instance.userId, true);
        //textState.text = "Create room succesfuly";
        //PhotonNetwork.LoadLevel("PlayScene");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        string roomId = PhotonNetwork.CurrentRoom.Name;  // Use Photon room name as Firebase room ID
        DatabaseManager.Instance.AddPlayerToRoom(roomId, AuthenticationManager.Instance.userId);
        //DatabaseManager.Instance.UpdatePlayerReadyStatus(AuthenticationManager.Instance.userId, true);
        DatabaseManager.Instance.UpdateRoomReadyStatus(roomId, false);
        PhotonNetwork.LoadLevel("3RoomScene");
        //PhotonNetwork.LoadLevel("MainScene");
    }

    // Set the current user (from AuthenticationManager)
    public void SetCurrentUser(string userId)
    {
        //currentUser = user;

        // Set Firebase user ID as a custom property in Photon
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["FirebaseUserId"] = userId; //currentUser.UserId; // Add Firebase User ID as custom property
        PhotonNetwork.SetPlayerCustomProperties(customProperties);
    }

    public void NavigateBackToRoom()
    {
        // Load the room scene (replace "RoomScene" with the actual name of your room scene)
        //PhotonNetwork.LoadLevel("3RoomScene");
        UnityEngine.SceneManagement.SceneManager.LoadScene("3RoomScene");
    }
}
