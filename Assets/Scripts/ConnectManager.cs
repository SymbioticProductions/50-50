using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ConnectManager : MonoBehaviourPunCallbacks {

[SerializeField]
private GameObject ConnectPanel;

[SerializeField]
private TextMeshProUGUI StatusText;

private bool isConnecting = false; //not trying to connect to server until prompted
private const string gameVersion = "v1";

    void Awake() //ensures host is in control of game scene for all players
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public void Connect()
    {
        isConnecting = true;
        ConnectPanel.SetActive(false);
        ShowStatus("Connecting...");

        if (PhotonNetwork.IsConnected)
        {
            ShowStatus("Joining session...");
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            ShowStatus("Connecting...");
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    private void ShowStatus(string text) //feedback info to users to indicate whether theyre are joining/joined lobby 
    {
        if (StatusText == null)
        {
            // do nothing
            return;
        }

        StatusText.gameObject.SetActive(true);
        StatusText.text = text;
    }

    public override void OnConnectedToMaster() //on connecting to the host's session 
    {
        if (isConnecting)
        {
            ShowStatus("Connected, joining session...");
            PhotonNetwork.JoinRandomRoom();
        }
    }

        public override void OnJoinRandomFailed(short returnCode, string message) //when no host session available, new session will be created as host
    {
        ShowStatus("Creating session...");
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }); //can change max players to four in future.
        //can also make this manual so that host can choose when to start match.
    }

        public override void OnDisconnected(DisconnectCause cause) //resets player's start scene
    {
        isConnecting = false;
        ConnectPanel.SetActive(true);
    }

        public override void OnJoinedRoom() 
    {
        ShowStatus("Joined session - waiting for another player.");
    }

        public override void OnPlayerEnteredRoom(Player newPlayer) //host takes players to the game scene
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }

}
