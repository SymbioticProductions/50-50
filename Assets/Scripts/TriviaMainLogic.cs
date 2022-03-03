using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TriviaMainLogic : MonoBehaviour
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
    Timer timer;

    [Header("Slider")]
    [SerializeField] Slider playerSlider;

    int int_Points;

    // Start is called before the first frame update
    void Start() {

        timer = FindObjectOfType<Timer>();
        playerSlider.value = 0;
        StartCoroutine(LateStart(0.5f));

    }

    void Update()
    {
        timerImage.fillAmount = timer.fl_FillFraction;

        //This section keeps looping Sungket, not sure why

        /*if (timer.bool_LoadNextQuestion)
        {
            bool_Answered_Early = false;
            GetNextQuestion();
            timer.bool_LoadNextQuestion = false;
        }
        else if (!bool_Answered_Early && !timer.bool_IsAnsweringQuestion) {
            DisplayResult(-1);
        }*/
    }

    //Added this in here as a delay. For some reason the code this code was executing quicker than the ReadCSV code, so the questions weren't coming up
    //So this just delays this section of the game by 5ms, allowing for all the questions to be loaded.
    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        InitaliseSceneAssets();
      
    }

    //Gets the next question, supposed to be used in the void Update part when the next question is loaded
    void GetNextQuestion() {
        InitaliseSceneAssets();
    }

    //Initalises everything, sets the text for the questions, buttons, points text and sets the slider
    public void InitaliseSceneAssets() {

        getQuestion.SetQuestion();

        int int_AnswerIndex = Random.Range(1, 10);
        questionText.text = currentQuestion.GetQuestion();
        int_Points = currentQuestion.GetPointValue();
        pointsText.text = "This Question is worth: " + currentQuestion.GetPoints() + " points";

        for (int i = 0; i <= go_AnswerButtons.Length; i++)
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
    public void AnswerButtonClick(int index) {

        bool_Answered_Early = true;
        DisplayResult(index);
        timer.CancelTimer();

    }

    //Changes the question text to right or wrong depending on which button was pressed
    public void DisplayResult(int index) {

        if (go_AnswerButtons[index].name.Equals("Correct"))
        {
            questionText.text = "Correct";
            playerSlider.value = playerSlider.value + int_Points;
            Debug.Log("Correct");
        }
        else
        {
            questionText.text = "Wrong";
            Debug.Log("Wrong");
        }

    }

}
