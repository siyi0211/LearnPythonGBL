using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerData
{
	public int id;
	public string name;
	public string email;
	
	public int coin;
	public int level;
	public int exp; //experience point
	
	public int[] characterOwned; // array of owned character id
	public int characterUsed;

	public string icon; //icon image path

	public TutorialHighestScore[] tutorialHighestScoresList;

	public PlayerData(int ID, string Name, string Email, int Coin, int Level, int EXP, int[] CharOwned, int CharUsed, string Icon, TutorialHighestScore[] TutorialHighestScoresList)
	{
		id = ID;
		name = Name;
		email = Email;
		coin = Coin;
		level = Level;
		exp = EXP;
		characterOwned = CharOwned;
		characterUsed = CharUsed;
		icon = Icon;
		tutorialHighestScoresList = TutorialHighestScoresList;
	}

}