using UnityEngine;
using System.Collections;

[System.Serializable]
public class TutorialHighestScore
{
	public int TutorialID;
	public string TutorialName;
	public int PlayerID;
	public int HighestScore;//name of the tutorial

	public TutorialHighestScore(int tutorialID, string tutorialName, int playerID, int highestScore)
	{
		TutorialID = tutorialID;
		TutorialName = tutorialName;
		PlayerID = playerID;
		HighestScore = highestScore;
	
	}

}