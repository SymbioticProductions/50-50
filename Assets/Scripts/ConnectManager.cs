using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;
using TMPro;

public class ConnectManager : MonoBehaviourPunCallbacks {

    [SerializeField]
    private GameObject ConnectPanel;

    [SerializeField]
    private TextMeshProUGUI StatusText;

    [SerializeField]
    private GameObject StartButton;

    [SerializeField]
    private GameObject BackButton;

    //ref player data through Dictionary 

    private bool isConnecting = false; //not trying to connect to server until prompted
    private const string gameVersion = "v1";

    void Awake() //ensures host is in control of game scene for all players
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public void Connect()
    {
        if (string.IsNullOrEmpty(PhotonNetwork.NickName)) {

            ShowStatus("Please enter your name");
            isConnecting = false;
        }
        else
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

    public void Quit()  //added to the quit game button
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); 
#endif
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
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 4 });
        //can also make this manual so that host can choose when to start match.
    }

    public override void OnDisconnected(DisconnectCause cause) //resets player's start scene
    {
        isConnecting = false;
        ConnectPanel.SetActive(true);
        StartButton.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(false);

    }

    public override void OnJoinedRoom()
    {
        ShowStatus("Joined session: " + PhotonNetwork.CurrentRoom.PlayerCount + " player(s) in lobby"); //displays the amount of players in lobby

        if (PhotonNetwork.IsMasterClient)        //the start button will only show for the host, as they are the only one to load game
            StartButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }

    public void LoadLevel() // made the load scene manual, so host can wait for 1-3 more players, or play solo
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("Game");
            ShowLobby();

    }

    public void DisconnectPlayer() //disconnects player from session and returns to start menu
    {
        StartCoroutine(Disconnect());
    }
    IEnumerator Disconnect()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        PhotonNetwork.LoadLevel("Start");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log(newPlayer.NickName + " has joined");

        ShowStatus("Joined session: " + PhotonNetwork.CurrentRoom.PlayerCount + " player(s) in lobby"); //displays the amount of players in lobby        
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount + " players in lobby");
        Debug.Log(otherPlayer.NickName + " has left");

        ShowStatus("Joined session: " + PhotonNetwork.CurrentRoom.PlayerCount + " player(s) in lobby"); //displays the amount of players in lobby 

        if (PhotonNetwork.IsMasterClient) //allows the new host to start the match if original host leaves
        {
            StartButton.SetActive(true);
        }
    }
    public void ShowLobby()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log(player.NickName + " is player " + player.ActorNumber);
        }
    }



}
