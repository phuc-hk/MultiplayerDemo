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
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
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
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(inputRoomName.text, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        textState.text = "Create room succesfuly";
        PhotonNetwork.LoadLevel("PlayScene");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        textState.text = message;
    }
}
