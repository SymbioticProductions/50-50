using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trivia Question", fileName = "New Question")]
public class QuestionScriptObject : ScriptableObject {

    [TextArea(2, 6)] //Adjust and control textbox (Minimum of 2 lines, max of 6)

    public string str_Question;
    public string str_RightAnswer;
    public string str_WrongAnswer;
    int previousValue;

    public string GetQuestion() { 
        return str_Question;
    }

    public string GetRightAnswer() {
        return str_RightAnswer;
    }

    public string GetWrongAnswer() {
        return str_WrongAnswer;
    }

    public string GetCorrectAnswer() {
        return str_RightAnswer;
    }
}
