using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ReadCSV : MonoBehaviour
{
    List<int> int_Used_Index = new List<int>();
    List<Questions> questions = new List<Questions>();
    [SerializeField] TextAsset ta_Question_Information;
    [SerializeField] QuestionScriptObject q;
    int int_Number_Of_Questions;

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

        while (int_Used_Index.Contains(int_Question_Index)) {
            Debug.Log("Question rerolled");

            if (int_Used_Index.Count == 149)
            {
                int_Used_Index.Clear();
            }
            else {
                int_Question_Index = Random.Range(1, 150);
            }
            
        }

        q.str_Question = questions[int_Question_Index].str_Question;
        q.str_RightAnswer = questions[int_Question_Index].str_RightAnswer;
        q.str_WrongAnswer = questions[int_Question_Index].str_WrongAnswer;
        q.int_Points = 1;
            
        int_Used_Index.Add(int_Question_Index);
    
    }

}
