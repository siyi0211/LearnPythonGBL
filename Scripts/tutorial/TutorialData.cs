using UnityEngine;
using System.Collections;

[System.Serializable]
public class TutorialData
{
	public int ID;
	public string Name;//name of the tutorial
	public bool Active;
	public int PointsAddedForCorrectAnswer;//points per correct answer
	public QuestionData[] Questions;//list of questions for the round

	public TutorialData(int id, string name,bool active, int pointsAddedForCorrectAnswer, QuestionData[] questions)
	{
		ID = id;
		Name = name;
		Active = active;
		PointsAddedForCorrectAnswer = pointsAddedForCorrectAnswer;
		Questions = questions;
	}
}