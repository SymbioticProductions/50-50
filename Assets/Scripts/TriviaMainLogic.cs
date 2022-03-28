using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class TriviaMainLogic : MonoBehaviourPunCallbacks, IPunObservable, IOnEventCallback
{
    [Header("Questions")]
    [SerializeField] ReadCSV getQuestion;
    [SerializeField] QuestionScriptObject currentQuestion;
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI pointsText;

    [Header("Buttons")]
    [SerializeField] GameObject[] go_AnswerButtons;
    TextMeshProUGUI buttonText;
    bool bool_Answered_Early;

    [Header("Timer")]
    [SerializeField] Image timerImage;
    [PunRPC] Timer timer;

    [Header("Player")]
    [SerializeField] public GameObject player;
    [SerializeField] GameObject slider1, slider2, slider3, slider4;

    public PhotonView PV2;
    public PhotonView PV1;

    public const byte EVENT_TIMER = 1;
    public const byte EVENT_TURN = 2;
    public const byte EVENT_DISPLAY = 3;

    public int turnNumber = 1;
    public int int_Points;
    bool bool_moveToNextQuestion = false;
    public bool isPlayerTurn;

    private int PlayerNumber = PhotonNetwork.LocalPlayer.ActorNumber;

    // Start is called before the first frame update
    void Start()
    {
        PV2 = GameObject.Find("GameManager").GetComponent<PhotonView>();
        PV1 = GameObject.Find("TimerManager").GetComponent<PhotonView>();

        if (PV2.IsMine)
        {
            PV2.RPC("InitalisePlayers", RpcTarget.All);
            //PV2.RPC("StarterTurn", RpcTarget.All);

        }
            timer = FindObjectOfType<Timer>();
            StartCoroutine(LateStart(0.5f));

    }

    void Update()
    {
        timerImage.fillAmount = timer.fl_FillFraction;
        if (PV2.IsMine)
        {
            if (timer.bool_LoadNextQuestion)
            {
                bool_Answered_Early = false;

                PV2.RPC("GetNextQuestion", RpcTarget.All);

                timer.bool_LoadNextQuestion = false;
            }
            else if (!bool_Answered_Early && !timer.bool_IsAnsweringQuestion)
            {
                PV2.RPC("DisplayResult", RpcTarget.All,-1);
            }        
        }

    }

    //Added this in here as a delay. For some reason the code this code was executing quicker than the ReadCSV code, so the questions weren't coming up
    //So this just delays this section of the game by 5ms, allowing for all the questions to be loaded.
    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (PV2.IsMine)
        {
            PV2.RPC("InitaliseSceneAssets", RpcTarget.All);
        }


    }

    //Gets the next question, supposed to be used in the void Update part when the next question is loaded
    [PunRPC]
    void GetNextQuestion()
    {
        InitaliseSceneAssets();
    }

    [PunRPC]
    void InitalisePlayers()
    {

        Vector2 spawnPos = new Vector2(0, 0);
        PhotonNetwork.Instantiate(player.name, spawnPos, Quaternion.identity);
        player.name = PhotonNetwork.NickName;
        Debug.Log("Player: " + player.name + " has joined!");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {

            slider1.gameObject.SetActive(true);

        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {

            slider1.gameObject.SetActive(true);
            slider2.gameObject.SetActive(true);

        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 3)
        {

            slider1.gameObject.SetActive(true);
            slider2.gameObject.SetActive(true);
            slider3.gameObject.SetActive(true);

        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 4)
        {

            slider1.gameObject.SetActive(true);
            slider2.gameObject.SetActive(true);
            slider3.gameObject.SetActive(true);
            slider4.gameObject.SetActive(true);

        }

    }

    //Initalises everything, sets the text for the questions, buttons, points text and sets the slider
    [PunRPC]
    void InitaliseSceneAssets()
    {

        getQuestion.SetQuestion();

        int int_AnswerIndex = Random.Range(1, 10);
        questionText.text = currentQuestion.GetQuestion();
        int_Points = currentQuestion.GetPointValue();
        pointsText.text = "This Question is worth: " + currentQuestion.GetPoints() + " points";

        for (int i = 0; i < go_AnswerButtons.Length; i++)
        {

            buttonText = go_AnswerButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            //Randomly selects which button displays the write answer and which on displays the wrong answer. If even then it shows the right, else it shows the wrong answer
            if (int_AnswerIndex % 2 == 0)
            {
                buttonText.text = currentQuestion.GetRightAnswer();
                go_AnswerButtons[i].name = "Correct";
                int_AnswerIndex = 5;
            }
            else
            {
                buttonText.text = currentQuestion.GetWrongAnswer();
                go_AnswerButtons[i].name = "Wrong";
                int_AnswerIndex = 2;
            }

        }

    }

    //This is assigned to the buttons, says that if either answer button is clicked, it displays cancels the timer and displays the result
    public void AnswerButtonClick(int index)
    {
        //CheckTurn(); //need this to occur locally when turnnumber changes
        if (PV1.IsMine)  // if the player is host, calls for all clients
        {
            PV1.RPC("CancelTimer", RpcTarget.All, 0);

            if (questionText.text == "Correct" || questionText.text == "Wrong")
            {
                PV2.RPC("IncrementTurn", RpcTarget.All, 1);//For loop, cycles through to next player in index and sets turn to true.
            }

            bool_Answered_Early = true;
            PV2.RPC("DisplayResult", RpcTarget.All, index);
            

        }
        else //if the player is a client, requests host to do methods
        {
            int[] reset = new int[] { 0 };
            int[] turn = new int[] { 1 };
            int[] display = new int[] {index}; 

            PhotonNetwork.RaiseEvent(EVENT_TIMER, reset, RaiseEventOptions.Default, SendOptions.SendReliable);

            if (questionText.text == "Correct" || questionText.text == "Wrong")
            {
                PhotonNetwork.RaiseEvent(EVENT_TURN, turn, RaiseEventOptions.Default, SendOptions.SendReliable);
            }
            PhotonNetwork.RaiseEvent(EVENT_DISPLAY, display, RaiseEventOptions.Default, SendOptions.SendReliable);
        }            
    }

    //Changes the question text to right or wrong depending on which button was pressed
    [PunRPC]
    public void DisplayResult(int index)
    {
        if (index < 0)
        {
            return;
        }
        Debug.Log("Index:" + index);
        if (go_AnswerButtons[index].name.Equals("Correct"))
        {
            questionText.text = "Correct";

            Debug.Log("Correct");
            bool_moveToNextQuestion = true;
        }
        else
        {
            questionText.text = "Wrong";
            Debug.Log("Wrong");
        }

    }
    public void OnEvent(EventData photonEvent) //what the host does upon receiving request from client 
    {
        if(PV2.IsMine && PV1.IsMine)
        {
            switch (photonEvent.Code)
            {
                case EVENT_TIMER:
                    int[] reset = (int[])photonEvent.CustomData;

                    //timer.CancelTimer(reset[0]);
                    PV1.RPC("CancelTimer", RpcTarget.All, reset[0]);
                    bool_Answered_Early = true;
                    break;

                case EVENT_TURN:
                    int[] turn = (int[])photonEvent.CustomData;

                    PV2.RPC("IncrementTurn", RpcTarget.All, turn[0]);

                    break;

               case EVENT_DISPLAY:
                    int[] display = (int[])photonEvent.CustomData;
                    int ind = display[0];

                        DisplayResult(ind);

                    break;
            }
        }
    }
    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //syncs everything on the host's screen with all other clients
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.getQuestion);
            stream.SendNext(this.currentQuestion);
            stream.SendNext(this.questionText.text);
            stream.SendNext(this.pointsText.text);

            for (int i = 0; i < go_AnswerButtons.Length; i++)
            {
                stream.SendNext(go_AnswerButtons[i]); //this here isn't set to the correct object for the button i believe
                //stream.SendNext(buttonText.text);
            }

            stream.SendNext(this.slider1);
            stream.SendNext(this.slider2);
            stream.SendNext(this.slider3);
            stream.SendNext(this.slider4);
        }
        else
        {
            this.getQuestion = (ReadCSV)stream.ReceiveNext();
            this.currentQuestion = (QuestionScriptObject)stream.ReceiveNext();
            this.questionText.text = (string)stream.ReceiveNext();
            this.pointsText.text = (string)stream.ReceiveNext();

            for (int i = 0; i < go_AnswerButtons.Length; i++)
            {
                go_AnswerButtons[i] = (GameObject)stream.ReceiveNext(); //this here isn't set to the correct object for the button i believe
                //buttonText.text = (string)stream.ReceiveNext();
            }

            this.slider1 = (GameObject)stream.ReceiveNext();
            this.slider2 = (GameObject)stream.ReceiveNext();
            this.slider3 = (GameObject)stream.ReceiveNext();
            this.slider4 = (GameObject)stream.ReceiveNext();
        }
    }
    

    [PunRPC]
    public void IncrementTurn(int next) //receives the next turnnumber
    {
        next = 1;
        if (turnNumber == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            turnNumber = next;
        }
        else
        {
            turnNumber += next;
        }
        Debug.Log("Player " + turnNumber + "'s turn");
    }
    
    //void GetNextTurn()
    //{
        //IncrementTurn(1);
        //CheckTurn();
    //}

    void StarterTurn() //disables all clients answer buttons for the first turn
    {
        if(PlayerNumber != turnNumber)
        {
            isPlayerTurn = false;
            go_AnswerButtons[0].SetActive(false);
            go_AnswerButtons[1].SetActive(false);

        }
    }

    public void CheckTurn() //disables the answer buttons if its not the player's turn (not working properly)
    {
        if (PlayerNumber == turnNumber)
        {
            isPlayerTurn = true;
            go_AnswerButtons[0].SetActive(true);
            go_AnswerButtons[1].SetActive(true);

        }
        else
        {
            isPlayerTurn = false;
            go_AnswerButtons[0].SetActive(false);
            go_AnswerButtons[1].SetActive(false);

        }
    }
}
