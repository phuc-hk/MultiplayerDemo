using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomListManager : MonoBehaviourPunCallbacks
{
    public GameObject roomUIPrefab;

    private void Start()
    {
        // Join the lobby to receive room list updates
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        Debug.Log("Update Room List");
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].IsOpen && roomList[i].IsVisible && roomList[i].PlayerCount >= 1)
            {
                GameObject room = Instantiate(roomUIPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("Content").transform);
                room.GetComponentInChildren<TextMeshProUGUI>().text = roomList[i].Name;
            }
        }
    }
}
