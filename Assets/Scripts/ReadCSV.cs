using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ReadCSV : MonoBehaviour
{

    List<Questions> questions = new List<Questions>();
    [SerializeField] TextAsset ta_QuestionInformation;
    [SerializeField] QuestionScriptObject q1, q2, q3, q4, q5;
    int int_NumberOfQuestions;

    // On start, load the CSV file and read it
    void Start() {

        string[] str_QuestionData = ta_QuestionInformation.text.Split(new char[] {'\n'});
        int_NumberOfQuestions = str_QuestionData.Length;

        for(int i = 0; i < int_NumberOfQuestions - 1; i++) {
            
            string[] row = str_QuestionData[i].Split(new char[] {','});
            Questions q = new Questions();
            q.str_Question = row[1];
            q.str_RightAnswer = row[2];
            q.str_WrongAnswer = row[3];

            questions.Add(q);

        }

        for (int j = 1; j <= 5; j++) {

            int index = Random.Range(1, int_NumberOfQuestions);

            if (j == 1)
            {
                q1.str_Question = questions[index].str_Question;
                q1.str_RightAnswer = questions[index].str_RightAnswer;
                q1.str_WrongAnswer = questions[index].str_WrongAnswer;
            }
            else if (j == 2)
            {
                q2.str_Question = questions[index].str_Question;
                q2.str_RightAnswer = questions[index].str_RightAnswer;
                q2.str_WrongAnswer = questions[index].str_WrongAnswer;
            }
            else if (j == 3)
            {
                q3.str_Question = questions[index].str_Question;
                q3.str_RightAnswer = questions[index].str_RightAnswer;
                q3.str_WrongAnswer = questions[index].str_WrongAnswer;
            }
            else if (j == 4)
            {
                q4.str_Question = questions[index].str_Question;
                q4.str_RightAnswer = questions[index].str_RightAnswer;
                q4.str_WrongAnswer = questions[index].str_WrongAnswer;
            }
            else if (j == 5)
            {
                q5.str_Question = questions[index].str_Question;
                q5.str_RightAnswer = questions[index].str_RightAnswer;
                q5.str_WrongAnswer = questions[index].str_WrongAnswer;
            }


        }

    }

}
