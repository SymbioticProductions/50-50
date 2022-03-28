using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Timer : MonoBehaviourPun
{

    [SerializeField] float fl_TimeToCompleteQuestion = 10f;
    [SerializeField] float fl_TimeToShowCorrectAnswer = 5f;

    public bool bool_LoadNextQuestion = false;
    public float fl_FillFraction;

    public bool bool_IsAnsweringQuestion = true;
    public float fl_TimerValue;

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
    }
    [PunRPC]
    public void CancelTimer(int value) {

        value = 0;
        fl_TimerValue = value;
    }

    void UpdateTimer() {
        fl_TimerValue -= Time.deltaTime;

        if (bool_IsAnsweringQuestion)
        {
            if (fl_TimerValue > 0)
            {

                fl_FillFraction = fl_TimerValue / fl_TimeToCompleteQuestion;
            }
            else
            {

                bool_IsAnsweringQuestion = false;
                fl_TimerValue = fl_TimeToShowCorrectAnswer;
                
            }
        }
        else
        {

            if (fl_TimerValue > 0)
            {

                fl_FillFraction = fl_TimerValue / fl_TimeToShowCorrectAnswer;
            }
            else
            {

                bool_IsAnsweringQuestion = true;
                fl_TimerValue = fl_TimeToCompleteQuestion;
                bool_LoadNextQuestion = true;

            }

        }
    }
}
