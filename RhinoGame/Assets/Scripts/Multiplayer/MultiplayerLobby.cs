using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerLobby : MonoBehaviourPunCallbacks
{
    public Transform Panels;

    [Header("Login Panel")]
    public InputField PlayerName;

    [Header("Create Room Panel")]
    
    public InputField RoomName;

    [Header("Inside Room Panel")]
    public GameObject TextPrefab;
    public Transform InsideRoomContent;

    private void Start()
    {
        PlayerName.text = "Player" + Random.Range(1, 10000);
    }

    #region Auxilliary
    public void ActivatePanel(string panelName)
    {
        DeactivatePanels();
        foreach (Transform panel in Panels)
        {
            if (panel.name == panelName)
                panel.gameObject.SetActive(true);
        }
    }

    private void DeactivatePanels()
    {
        foreach (Transform panel in Panels)
        {
            panel.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Button Clicks
    public void LoginButtonClicked()
    {
        PhotonNetwork.LocalPlayer.NickName = PlayerName.text;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoomClicked()
    {
        if (string.IsNullOrEmpty(RoomName.text))
        {
            Debug.Log("Room name cannot be empty!");
        }
        else
        {
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            PhotonNetwork.CreateRoom(RoomName.text, roomOptions);
        }
        
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        ActivatePanel("Login");
    }

    #endregion

    #region PUN Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!");
        ActivatePanel("Selection");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected!");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined!");
        ActivatePanel("InsideRoom");

        foreach (var player in PhotonNetwork.PlayerList)
        {
            var newPlayerRoomEntry = Instantiate(TextPrefab, InsideRoomContent);
            newPlayerRoomEntry.GetComponent<Text>().text = player.NickName;
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Creation Failed!");
    }

    #endregion
}
