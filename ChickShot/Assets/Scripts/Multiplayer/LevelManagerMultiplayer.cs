using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManagerMultiplayer : MonoBehaviourPunCallbacks
{
    public int MaxKills = 3;
    int highScore = 0;
    public GameObject GameOverPopUp;
    public Text WinnerText;
    public Text EndGameStatementText;
    public Text TimerText;

    float startTime = 60f;
    [HideInInspector]
    public float timer;
    bool timeIsRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("LevelManagerMultiplayer");
        PhotonNetwork.Instantiate("PlayerMultiplayer", new Vector3(0,0,0), Quaternion.identity);
        timer = startTime;
        timeIsRunning = true;
    }

    public void Update()
    {
        if (timeIsRunning)
        {
            timer -= Time.deltaTime;
            Debug.Log("Timer: " + timer);

            int timerInteger = (int)timer;
            string timerString = timerInteger.ToString();
            TimerText.GetComponent<Text>().text = timerString;

            if(timer <= 0)
            {
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    var kills = player.GetScore();
                    if(kills > highScore)
                    {
                        highScore = kills;
                        WinnerText.text = player.NickName;
                    }
                }

                Debug.Log("Time Up!");
                timeIsRunning = false;
                EndGameStatementText.text = "Is The Winner!";
                GameOverPopUp.SetActive(true);

            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("OnPlayerPropertiesUpdate - LevelManager!");
        Debug.Log("targetPlayer.GetScore(): " + targetPlayer.GetScore());


        if (targetPlayer.GetScore() == MaxKills && timer > 0)
        {
            GameOver(targetPlayer);
        }
    }

    private void GameOver(Photon.Realtime.Player targetPlayer)
    {
        WinnerText.text = targetPlayer.NickName;
        GameOverPopUp.SetActive(true);
        StorePersonalBest();
    }

    //sets new personal best if greater than previous best score
    private void StorePersonalBest()
    {
        var username = PhotonNetwork.LocalPlayer.NickName;
        var score = PhotonNetwork.LocalPlayer.GetScore();
        var totalPlayersInGame = PhotonNetwork.CurrentRoom.PlayerCount;
        var roomName = PhotonNetwork.CurrentRoom.Name;

        var playerData = GameManager.Instance.playerData;
        if(score >= playerData.bestScore)
        {
            //set new best
            playerData.username = username;
            playerData.bestScore = score;
            playerData.date = DateTime.UtcNow;
            playerData.totalPlayersInTheGame = totalPlayersInGame;
            playerData.roomName = roomName;
            playerData.isPlayerDataUpdated = true;

            GameManager.Instance.SavePlayerData();
        }
    }

    public void LeaveGameClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("MultiplayerLobby");
    }
}
