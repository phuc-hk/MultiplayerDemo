using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public TextMeshProUGUI roomName;

    public void JoinRoom()
    {
        GameObject.Find("DemoPhoton").GetComponent<DemoPhoton>().JoinRoomByName(roomName.text);
    }
}
