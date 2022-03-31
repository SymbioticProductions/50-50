using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ReadCSV : MonoBehaviour
{

    List<Questions> questions = new List<Questions>();
    [SerializeField] TextAsset ta_Question_Information;
    [SerializeField] QuestionScriptObject q;
    int int_Number_Of_Questions, int_Points;

    // On start, load the CSV file and read it. Assigns each question to an array separating the right and wrong answers
    void Start() {

        string[] str_QuestionData = ta_Question_Information.text.Split(new char[] {'\n'});
        int_Number_Of_Questions = str_QuestionData.Length;

        for(int i = 0; i < int_Number_Of_Questions - 1; i++) {
            
            string[] row = str_QuestionData[i].Split(new char[] {','});
            Questions q = new Questions();
            q.str_Question = row[1];
            q.str_RightAnswer = row[2];
            q.str_WrongAnswer = row[3];

            questions.Add(q);

        }

    }

    //Chooses a randome number from 1 - 150, then based on the value gets that question and assigns it to the scriptable object
    public void SetQuestion() {

        Debug.Log("This Works");

        int int_Question_Index = Random.Range(1, 150);

        int[] int_Used_Index = new int[20];

        for (int i = 0; i <= int_Used_Index.Length - 1; i++) {

            if (int_Used_Index[i] != int_Question_Index)
            {
                q.str_Question = questions[int_Question_Index].str_Question;
                q.str_RightAnswer = questions[int_Question_Index].str_RightAnswer;
                q.str_WrongAnswer = questions[int_Question_Index].str_WrongAnswer;
                q.int_Points = Random.Range(1, 3);
                int_Used_Index[i] = int_Question_Index;
                break;
            }
            else {
                int_Question_Index = Random.Range(1, 150);
            }
        
        }
    
    }

}
