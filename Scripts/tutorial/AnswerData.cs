using UnityEngine;
using System;

//Need serializable for access to class in the editor
[Serializable]
public class AnswerData
{
	public string AnswerText;
	public bool IsCorrect;

	public AnswerData(string answerText, bool isCorrect)
	{
		AnswerText = answerText;
		IsCorrect = isCorrect;
	}
}