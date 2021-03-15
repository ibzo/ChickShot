using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviour
{
    public Text RoomText;
    [HideInInspector]
    public string RoomName;
    
    public void JoinRoom()
    {
        if(PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();

        PhotonNetwork.JoinRoom(RoomName);
    }
}
