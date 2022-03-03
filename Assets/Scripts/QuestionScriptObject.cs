using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionScriptObject : ScriptableObject {

    /*
     Creates the scriptable object framework that can be pulled by the main logic
     */

    [TextArea(2, 6)] //Adjust and control textbox (Minimum of 2 lines, max of 6)

    public string str_Question;
    public string str_RightAnswer;
    public string str_WrongAnswer;
    public int int_Points;
    public string str_Points;

    public string GetQuestion() { 
        return str_Question;
    }

    public string GetRightAnswer() {
        return str_RightAnswer;
    }

    public string GetWrongAnswer() {
        return str_WrongAnswer;
    }

    public int GetPointValue() {
        return int_Points;
    }

    public string GetPoints() {
        
        str_Points = int_Points.ToString();

        return str_Points;
    }
}
