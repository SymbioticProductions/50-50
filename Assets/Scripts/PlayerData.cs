using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] Slider playerSlider;

    public string str_PlayerName;
    public int int_PlayerScore;
    int int_NumberOfPlayers = 0;
    public bool bool_IsPlayerTurn;

    public void Start() {

    //    playerSlider.gameObject.SetActive(true);
    
    }

    public void SetPlayerName(string str_InputName)
    {;

        if (string.IsNullOrEmpty(str_InputName))
        {
            Debug.LogError("Player name is empty");
            return;
        }
        PhotonNetwork.NickName = str_InputName;
    }

    public void AddPoints(int int_Points)
    {

        int_PlayerScore = int_PlayerScore + int_Points;

    }

    public void SetPlayerTurn(bool state)
    {

        bool_IsPlayerTurn = state;
    }

    public bool GetPlayerTurn()
    {
        return bool_IsPlayerTurn;
    }

    public int ReturnScore()
    {
        return int_PlayerScore;
    }

    public string ReturnName()
    {
        return str_PlayerName;
    }

}
