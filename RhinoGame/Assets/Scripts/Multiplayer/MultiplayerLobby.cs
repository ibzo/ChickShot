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
    public GameObject StartGameButton;

    [Header("List Rooms Panel")]
    public GameObject RoomEntryPrefab;
    public Transform ListRoomsContent;

    Dictionary<string, RoomInfo> cachedRoomList;

    private void Awake()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

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

    private void DeleteChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    #endregion

    #region Button Clicks
    public void LoginButtonClicked()
    {
        PhotonNetwork.LocalPlayer.NickName = PlayerName.text;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void JoinRandomRoomClicked()
    {
        PhotonNetwork.JoinRandomRoom();
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

    public void StartGameClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("Multiplayer");
    }

    public void LeaveRoomClicked()
    {
        PhotonNetwork.LeaveRoom();
        DeleteChildren(InsideRoomContent);
        ActivatePanel("Selection");
    }

    public void ListRoomsClicked()
    {
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();

        ActivatePanel("ListRooms");
    }

    public void LeaveLobbyClicked()
    {
        if (PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();

        ActivatePanel("Selection");
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
        StartGameButton.SetActive(PhotonNetwork.IsMasterClient);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            var newPlayerRoomEntry = Instantiate(TextPrefab, InsideRoomContent);
            newPlayerRoomEntry.name = player.NickName;
            newPlayerRoomEntry.GetComponent<Text>().text = player.NickName;
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Creation Failed!");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby!");
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Left Lobby!");
        cachedRoomList.Clear();
        DeleteChildren(ListRoomsContent);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("Player Entered Room!");

        var newPlayerRoomEntry = Instantiate(TextPrefab, InsideRoomContent);
        newPlayerRoomEntry.name = newPlayer.NickName;
        newPlayerRoomEntry.GetComponent<Text>().text = newPlayer.NickName;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Player Left Room!");
        foreach (Transform child in InsideRoomContent)
        {
            if (child.name == otherPlayer.NickName)
                Destroy(child.gameObject);
        }
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        StartGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate: " + roomList.Count);

        DeleteChildren(ListRoomsContent);

        UpdateCacheRoomList(roomList);

        //Update UI
        foreach (var room in cachedRoomList)
        {
            var newRoomEntry = Instantiate(RoomEntryPrefab, ListRoomsContent);
            var roomEntryScript = newRoomEntry.GetComponent<RoomEntry>();
            roomEntryScript.RoomName = room.Key;
            roomEntryScript.RoomText.text = $"[{room.Key}] - ({room.Value} / {room.Value.MaxPlayers})";
            //roomEntryScript.RoomText.text = "[" + room.Name + " - (" + room.PlayerCount + " / " + room.MaxPlayers + ")";
        }
    }

    private void UpdateCacheRoomList(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            //Remove from cache
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(room.Name))
                    cachedRoomList.Remove(room.Name);

                continue;
            }

            if (cachedRoomList.ContainsKey(room.Name))
                cachedRoomList[room.Name] = room;
            else
                cachedRoomList.Add(room.Name, room);
        }
    }

    #endregion
}
