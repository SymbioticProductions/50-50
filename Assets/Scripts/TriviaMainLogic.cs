using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/*Issue with syncing hosts bar, also issue when leaving the timer to run out it is not incrementing the turn value, needs to trigger that*/

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
    [SerializeField] public PlayerData player;
    [SerializeField] Slider slider1, slider2, slider3, slider4;

    public PhotonView PV2;
    public PhotonView PV1;

    public const byte EVENT_TIMER = 1;
    public const byte EVENT_TURN = 2;
    public const byte EVENT_DISPLAY = 3;
    public const byte EVENT_SLIDER = 4;

    public int turnNumber = 1;
    public int int_Points;
    bool bool_moveToNextQuestion = false;
    public bool isPlayerTurn;

    public string button1Text, button2Text;

    private int PlayerNumber = PhotonNetwork.LocalPlayer.ActorNumber;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LateStart(1.75f));
        timer = FindObjectOfType<Timer>();
        PV2 = GameObject.Find("GameManager").GetComponent<PhotonView>();
        PV1 = GameObject.Find("TimerManager").GetComponent<PhotonView>();
        ToggleButtons(0);
        questionText.text = "Initalising...";

        if (PV2.IsMine)
        {
            PV2.RPC("InitalisePlayers", RpcTarget.All);
            //PV2.RPC("StarterTurn", RpcTarget.All);

        }
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
        CheckTurn();
        InitaliseSceneAssets();
    }

    [PunRPC]
    void InitalisePlayers()
    {

        Vector2 spawnPos = new Vector2(0, 0);
        PhotonNetwork.Instantiate(player.name, spawnPos, Quaternion.identity);
        player.str_PlayerName = PhotonNetwork.NickName;
        Debug.Log("Player: " + player.str_PlayerName + " has joined!");
        Debug.Log("PlayerNumber = " + PlayerNumber);

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

            if (i == 0)
            {
                button1Text = buttonText.text;
            }
            else {
                button2Text = buttonText.text;
            }

        }

    }

    //This is assigned to the buttons, says that if either answer button is clicked, it displays cancels the timer and displays the result
    public void AnswerButtonClick(int index)
    {
        
        if (PV1.IsMine)  // if the player is host, calls for all clients
        {
            PV1.RPC("CancelTimer", RpcTarget.All, 0);
            ToggleButtons(0);
            bool_Answered_Early = true;
            PV2.RPC("DisplayResult", RpcTarget.All, index);
            PV2.RPC("IncrementTurn", RpcTarget.All, 1);//For loop, cycles through to next player in index and sets turn to true.

        }
        else //if the player is a client, requests host to do methods
        {
            int[] reset = new int[] { 0 };
            int[] turn = new int[] { 1 };
            int[] display = new int[] {index};
            
            PhotonNetwork.RaiseEvent(EVENT_TIMER, reset, RaiseEventOptions.Default, SendOptions.SendReliable);
            ToggleButtons(0);
            PhotonNetwork.RaiseEvent(EVENT_DISPLAY, display, RaiseEventOptions.Default, SendOptions.SendReliable);
            PhotonNetwork.RaiseEvent(EVENT_TURN, turn, RaiseEventOptions.Default, SendOptions.SendReliable);

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
        Debug.Log("Result Index:" + index);
        
        if (go_AnswerButtons[index].name.Equals("Correct"))
        {
            questionText.text = "Correct";
            Debug.Log("Answer is Correct");
            bool_moveToNextQuestion = true;
            int[] slider = new int[] { int_Points };

            if (PV1.IsMine)
            {
                PV2.RPC("SliderEvent", RpcTarget.All, int_Points);
            }
            else {
                PhotonNetwork.RaiseEvent(EVENT_SLIDER, slider, RaiseEventOptions.Default, SendOptions.SendReliable);
            }
                
        }
        else
        {
            questionText.text = "Wrong";
            Debug.Log("Answer is Wrong");
        }

    }

    [PunRPC]
    public void SliderEvent(int index) {

        if (index == null || index == 0) {
            index = int_Points;
        }

        if (turnNumber == 1)
        {
            if (photonView.IsMine)
            this.slider1.value = slider1.value + index;
        }
        else if (turnNumber == 2)
        {
            slider2.value = slider2.value + index;
        }
        else if (turnNumber == 3)
        {
            slider3.value = slider3.value + index;
        }
        else {
            slider4.value = slider4.value + index;
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

                case EVENT_SLIDER:
                    int[] slider = (int[])photonEvent.CustomData;

                    PV2.RPC("SliderEvent", RpcTarget.All, int_Points);

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

                if (i == 0)
                {
                    stream.SendNext(this.button1Text);
                }
                else {
                    stream.SendNext(this.button2Text);
                }

            }

            stream.SendNext(this.slider1);
            stream.SendNext(this.slider1.value);
            stream.SendNext(this.slider2);
            stream.SendNext(this.slider2.value);
            stream.SendNext(this.slider3);
            stream.SendNext(this.slider3.value);
            stream.SendNext(this.slider4);
            stream.SendNext(this.slider4.value);
        }
        else
        {
            this.getQuestion = (ReadCSV)stream.ReceiveNext();
            this.currentQuestion = (QuestionScriptObject)stream.ReceiveNext();
            this.questionText.text = (string)stream.ReceiveNext();
            this.pointsText.text = (string)stream.ReceiveNext();
            this.button1Text = (string)stream.ReceiveNext();
            this.button2Text = (string)stream.ReceiveNext();

            for (int i = 0; i < go_AnswerButtons.Length; i++)
            {

                buttonText = go_AnswerButtons[i].GetComponentInChildren<TextMeshProUGUI>();

                if (i == 0)
                {

                    this.buttonText.text = button1Text;

                }
                else {

                    this.buttonText.text = button2Text;

                }
                
            }

            this.slider1 = (Slider)stream.ReceiveNext();
            this.slider1.value = (float)stream.ReceiveNext();
            this.slider2 = (Slider)stream.ReceiveNext();
            this.slider2.value = (float)stream.ReceiveNext();
            this.slider3 = (Slider)stream.ReceiveNext();
            this.slider3.value = (float)stream.ReceiveNext();
            this.slider4 = (Slider)stream.ReceiveNext();
            this.slider4.value = (float)stream.ReceiveNext();
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
            Debug.Log("turnNumber = " + turnNumber);
        }
        Debug.Log("Player " + turnNumber + "'s turn");
    }

    public void ToggleButtons(int index) {

        if (index == 0)
        {

            go_AnswerButtons[0].SetActive(false);
            go_AnswerButtons[1].SetActive(false);

        }
        else {

            go_AnswerButtons[0].SetActive(true);
            go_AnswerButtons[1].SetActive(true);

        }
        
    }

    void StarterTurn() //disables all clients answer buttons for the first turn
    {
        if(PlayerNumber != turnNumber)
        {
            isPlayerTurn = false;
            ToggleButtons(0);

        }
    }

    public void CheckTurn() //disables the answer buttons if its not the player's turn (not working properly)
    {

        Debug.Log("Check turn run");
        Debug.Log("PlayerNumber = " + PlayerNumber);
        Debug.Log("TurnNumber = " + turnNumber);

        if (PlayerNumber == turnNumber)
        {

            isPlayerTurn = true;
            ToggleButtons(1);

        }
        else
        {

            isPlayerTurn = false;
            ToggleButtons(0);

        }
    }
}
