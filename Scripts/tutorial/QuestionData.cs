using UnityEngine;
using System.Collections;

[System.Serializable]
public class QuestionData
{
	public string QuestionText;
	public AnswerData[] Answers;//List of our answer class

	public QuestionData(string questionText, AnswerData[] answers)
	{
		QuestionText = questionText;
		Answers = answers;
	}
}