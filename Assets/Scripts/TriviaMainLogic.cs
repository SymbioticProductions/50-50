using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TriviaMainLogic : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] QuestionScriptObject question;
    [SerializeField] TextMeshProUGUI questionText;
    [Header("Buttons")]
    [SerializeField] GameObject[] go_AnswerButtons;
    TextMeshProUGUI buttonText;
    [Header("Timer")]
    [SerializeField] Image timerImage;
    Timer timer;

    // Start is called before the first frame update
    void Start() {

        timer = FindObjectOfType<Timer>();
        StartCoroutine(LateStart(0.5f));

    }

    void Update()
    {
        timerImage.fillAmount = timer.fl_FillFraction;

        if (timer.bool_LoadNextQuestion) {
            GetNextQuestion();
            timer.bool_LoadNextQuestion = false;
        }
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        InitaliseSceneAssets();
      
    }

    void GetNextQuestion() {
        InitaliseSceneAssets();
    }

    public void InitaliseSceneAssets() {

        int int_AnswerIndex = Random.Range(1, 10);
        questionText.text = question.GetQuestion();
        Debug.Log("This is int answer index: " + int_AnswerIndex);

        for (int i = 0; i <= go_AnswerButtons.Length; i++)
        {

            buttonText = go_AnswerButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            if (int_AnswerIndex % 2 == 0)
            {
                Debug.Log("This is int answer index for main: " + int_AnswerIndex);
                buttonText.text = question.GetRightAnswer();
                go_AnswerButtons[i].name = "Correct";
                int_AnswerIndex = 5;
            }
            else
            {
                Debug.Log("This is int answer index for else: " + int_AnswerIndex);
                buttonText.text = question.GetWrongAnswer();
                go_AnswerButtons[i].name = "Wrong";
                int_AnswerIndex = 2;
            }

        }

    }

    public void AnswerButtonClick(int index) {

        if (go_AnswerButtons[index].name.Equals("Correct")) { 
            questionText.text = "Correct";
        }
        else {
            questionText.text = "Wrong";
        }

        timer.CancelTimer();

    }

}
