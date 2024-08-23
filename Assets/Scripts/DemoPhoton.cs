using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoPhoton : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI textState;
    public TMP_InputField inputRoomName;
    public Button buttonCreateaRoom;
    public Button buttonJoinRoom;

    public static DemoPhoton Instance { get; private set; }

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

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
        textState.text = "Loading...";
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        textState.text = "Connected";
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(inputRoomName.text, roomOptions);
        //PhotonNetwork.JoinOrCreateRoom()
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(inputRoomName.text);
    }

    public void JoinRoomByName(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void CreateOrJoinRoom(string roomName)
    {
        RoomOptions roomOptions = new();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.LoadLevel("PlayScene");
        //PhotonNetwork.LoadLevel("MainScene");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Create room succesfuly");
        //textState.text = "Create room succesfuly";
        //PhotonNetwork.LoadLevel("PlayScene");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("Create room fail " + message);
        //textState.text = message;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("Join room fail " + message);
        //textState.text = "Failed to join room: " + message;
    }

}
