using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonUIControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateRoom()
    {
        PhotonManager.Instance.CreateRoom();
    }

    public void JoinRoom()
    {
        string roomName = GetComponentInChildren<TextMeshProUGUI>().text;
        PhotonManager.Instance.JoinRoomByName(roomName);
    }
}
