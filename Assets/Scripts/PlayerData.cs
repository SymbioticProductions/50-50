using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerData : MonoBehaviourPunCallbacks
{

    public string str_PlayerName = "Default_PlayerName";
    public int int_PlayerScore = 0, int_TwoPointCheck = 0, int_ThreePointCheck = 0;
    int int_NumberOfPlayers = 0;
    
    public void SetPlayerName(string str_InputName)
    {

        if (string.IsNullOrEmpty(str_InputName))
        {
            Debug.LogError("Player name is empty");
            return;
        }
        PhotonNetwork.NickName = str_InputName;
    }
    public void AddPoints(int int_Points)
    {

        if (photonView.IsMine) {
            int_PlayerScore = int_PlayerScore + int_Points;
        }
        
    }

    public int ReturnScore()
    {
        return int_PlayerScore;
    }

}
