using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerData : MonoBehaviour
{

    public void SetPlayerName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("Player name is empty");
            return;
        }
        PhotonNetwork.NickName = name;
    }







    public int int_PlayerScore;
    public bool bool_IsPlayerTurn;  //if yes enable all buttons, if no disable buttons

    /*
     What I would suggest is syncing this with the networking side. So it grabs the players local score, and then syncs that across the server,
    and uses that score to update the client side sliders. For example, if player2's total score is 5, it will show their slider having moved 5 spaces up on player1's game.
    Try asking Renato, show him the code, talk it through to him and ask him what his thoughts are. I'll add comments where I can to explain everything as well.
     */

}
