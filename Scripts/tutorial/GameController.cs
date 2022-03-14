using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;


public class GameController : MonoBehaviour
{
	public Text questionDisplayText;
	public Text questionNumberDisplayText;
	public Text scoreDisplayText;
	public Text timeDisplayText;
	public Text TotalScoreText;
	public Text TotalTimeUsed;
	public SimpleObjectPool answerButtonObjectPool;
	public Transform answerButtonParent;
	public GameObject questionDisplay;
	public GameObject roundEndDisplay;
	public Text highScoreDisplay;

	private DataController dataController;
	private TutorialData tutorialData;
	private QuestionData[] questionPool;

	private QuestionData questionData;
	private AnswerData answerData;


	public AllPlayerData allPlayerData;
	private PlayerData[] allPlayer;
	private PlayerData player;
	private string allPlayerDataFileName = "PlayerData.json";
	private TutorialHighestScore[] tutorialHighestScores;

	private int highScore;
	private int questionNumber;
	private bool isRoundActive;
	private float time;
	private int questionIndex;
	private int playerScore;
	private List<GameObject> answerButtonGameObjects = new List<GameObject>();

	private TutorialData[] allTutorialData;//hold data for each round
	private string gameDataFileName = "TutorialData.json";

	// Use this for initialization
	void Start()
	{
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
		dataController = FindObjectOfType<DataController>();//we can use find because we start with the Persistent scene which has a data controller
		LoadPlayerData();
		LoadGameDataFromFirebase();
		//LoadGameData();
		//SetUpRound();
		
	}

	public void LoadGameDataFromFirebase()
	{
		FirebaseDatabase.DefaultInstance
		.GetReference("allTutorialData").OrderByChild("ID")
		.GetValueAsync().ContinueWithOnMainThread(task => {
			if (task.IsFaulted)
			{
				Debug.LogError("NO DATA");
			}
			else if (task.IsCompleted)
			{
				Debug.LogError("Got DATA");
				DataSnapshot snapshot = task.Result;

				List<TutorialData> tutorials = new List<TutorialData>();

				foreach (var tutorial in snapshot.Children)
				{

					var id = tutorial.Child("ID").Value.ToString();

					int ID = Convert.ToInt32(id);

					var name = tutorial.Child("Name").Value.ToString();

					var isActive = tutorial.Child("Active").Value.ToString();

					bool IsActive = Convert.ToBoolean(isActive);

					var pointsAddedForCorrectAnswer = tutorial.Child("PointsAddedForCorrectAnswer").Value.ToString();

					int Points = Convert.ToInt32(pointsAddedForCorrectAnswer);

					List<QuestionData> questions = new List<QuestionData>();

					foreach (DataSnapshot question in tutorial.Child("Questions").Children)
					{
						var questionName = question.Child("QuestionText").Value.ToString();

						List<AnswerData> answers = new List<AnswerData>();

						foreach (DataSnapshot answer in question.Child("Answers").Children)
						{
							var answertext = answer.Child("AnswerText").Value.ToString();

							var isCorrect = answer.Child("IsCorrect").Value.ToString();

							bool IsCorrect = Convert.ToBoolean(isCorrect);

							answerData = new AnswerData(answertext, IsCorrect);

							answers.Add(answerData);

						}

						AnswerData[] AllAnswerData = answers.ToArray();

						questionData = new QuestionData(questionName, AllAnswerData);

						questions.Add(questionData);

						answers.Clear();

					}

					QuestionData[] AllQuestionData = questions.ToArray();

					tutorialData = new TutorialData(ID, name, IsActive, Points, AllQuestionData);

					tutorials.Add(tutorialData);

					questions.Clear();

				}
				allTutorialData = tutorials.ToArray();
				SetUpRound();
				tutorials.Clear();

			}
		});

	}

	public void LoadGameData()
	{
		// Path.Combine combines strings into a file path
		// Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
		string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);

		if (File.Exists(filePath))
		{
			// Read the json from the file into a string
			string dataAsJson = File.ReadAllText(filePath);
			// Pass the json to JsonUtility, and tell it to create a GameData object from it
			GameData loadedData = JsonUtility.FromJson<GameData>(dataAsJson);

			// Retrieve the allRoundData property of loadedData
			allTutorialData = loadedData.allTutorialData;
		}
		else
		{
			Debug.LogError("Cannot load game data!");
		}
	}
	public TutorialData GetCurrentTutorialSetData()
	{
		Debug.LogError("tutorial set number is: "+TutorialSetNumber.tutorialSetNumber);

		for (int i = 0; i < allTutorialData.Length; i++) {

			if (allTutorialData[i].ID == TutorialSetNumber.tutorialSetNumber)
			{
				return allTutorialData[i];
			}

		}

		return null;

	}
	public void LoadPlayerData()
	{
		// Path.Combine combines strings into a file path
		// Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
		string filePath = Path.Combine(Application.streamingAssetsPath, allPlayerDataFileName);

		if (File.Exists(filePath))
		{
			// Read the json from the file into a string
			string dataAsJson = File.ReadAllText(filePath);
			// Pass the json to JsonUtility, and tell it to create a GameData object from it
			AllPlayerData loadedData = JsonUtility.FromJson<AllPlayerData>(dataAsJson);

			// Retrieve the allRoundData property of loadedData
			allPlayer = loadedData.allPlayerData;
		}
		else
		{
			Debug.LogError("Cannot load game data!");
		}
	}

	public PlayerData GetCurrentPlayerData()
	{

		for (int i = 0; i < allPlayer.Length; i++)
		{

			if (allPlayer[i].id == PlayerID.playerID)
			{
				return allPlayer[i];
			}

		}

		return null;

	}

	public void SetUpRound()
	{
		
		//Once dataController is loaded initialize game variables with the data found
		tutorialData = GetCurrentTutorialSetData();
		player = GetCurrentPlayerData();

		tutorialHighestScores = player.tutorialHighestScoresList;

		questionPool = tutorialData.Questions;

		//find player high score
		highScore = findHighScore();

		//initialize remaining game data and show first question
		playerScore = 0;
		questionIndex = 0;

		ShowQuestionNumber();
		ShowPlayerScore();
		ShowQuestion();
		isRoundActive = true;
	}
	private int findHighScore() 
	{
		if (tutorialHighestScores.Length != 0) {

			for (int i = 0; i < tutorialHighestScores.Length; i++)
			{
				if (tutorialHighestScores[i].TutorialName.Equals(tutorialData.Name))
				{
					return tutorialHighestScores[i].HighestScore; 
				}
			}
		}

		//if cant find, then create new highscore

		int score = 0;
		
		Array.Resize(ref tutorialHighestScores, tutorialHighestScores.Length + 1);
		tutorialHighestScores[tutorialHighestScores.Length - 1] = new TutorialHighestScore(tutorialData.ID, tutorialData.Name, player.id, score);
		player.tutorialHighestScoresList = tutorialHighestScores;
		savePlayerData();

		return tutorialHighestScores[tutorialHighestScores.Length - 1].HighestScore;

	}

	private void ShowPlayerScore()
	{
		scoreDisplayText.text = "Score: " + playerScore.ToString();
	}

	private void ShowQuestionNumber()
	{
		questionNumber = questionIndex + 1;
		questionNumberDisplayText.text = "QUESTION " + questionNumber + "/"+ questionPool.Length;
	}

	private void ShowQuestion()
	{   //Remove old answers get current question and display the text
		RemoveAnswerButtons();
		ShowQuestionNumber();
		QuestionData questionData = questionPool[questionIndex];
		questionDisplayText.text = questionData.QuestionText;

		//Get all answers for the question, create new buttons for each and add them to the answerButtonParent object(AnswerPanel) 
		for (int i = 0; i < questionData.Answers.Length; i++)
		{
			GameObject answerButtonGameObject = answerButtonObjectPool.GetObject();
			answerButtonGameObjects.Add(answerButtonGameObject);
			answerButtonGameObject.transform.SetParent(answerButtonParent);

			//we get a reference to the answer button then use its attatched script to set the answer
			AnswerButton answerButton = answerButtonGameObject.GetComponent<AnswerButton>();//Give the gameobject an answer button component
			answerButton.Setup(questionData.Answers[i]);
		}
	}

	private void RemoveAnswerButtons()
	{//if answer buttons exists remove them , remove the game object and add it to the available object pool
		while (answerButtonGameObjects.Count > 0)
		{
			answerButtonObjectPool.ReturnObject(answerButtonGameObjects[0]);
			answerButtonGameObjects.RemoveAt(0);
		}
	}

	public void AnswerButtonClicked(bool isCorrect)
	{

		//Increase player score if answer is correct and update the display
		if (isCorrect)
		{
			playerScore += tutorialData.PointsAddedForCorrectAnswer;
			scoreDisplayText.text = "Score: " + playerScore.ToString();
		}

		//If we have more questions show the next question otherwise end the round
		if (questionPool.Length > questionIndex + 1)
		{
			questionIndex++;
			ShowQuestion();
		}
		else
		{
			EndRound();
			
		}

	}

	public void EndRound()
	{   // set the round over and turn off question display then turn on round end display panel
		isRoundActive = false;

		//
		compareHighScore();

		TotalScoreText.text = "Total Score: "+ playerScore.ToString() + "/" + questionPool.Length;
		float minutes = Mathf.Round(time / 60);
		float seconds = Mathf.Round(time % 60);
		TotalTimeUsed.text = "Total Time Used: " + string.Format("{0:00}", minutes) + ":" + string.Format("{0:00}", seconds);

		highScoreDisplay.text = "HighScore: "+ highScore;

		questionDisplay.SetActive(false);
		roundEndDisplay.SetActive(true);

	}
	private void compareHighScore() {

		if (playerScore > highScore)
		{
			highScore = playerScore;
			saveHighScore();
		}
	
	}

	private void saveHighScore() 
	{

		for (int i = 0; i < tutorialHighestScores.Length; i++)
		{
			if (tutorialHighestScores[i].TutorialName.Equals(tutorialData.Name))
			{
				tutorialHighestScores[i].HighestScore = highScore;
				player.tutorialHighestScoresList = tutorialHighestScores;
				savePlayerData();
			}
		}

		

	}
	public void savePlayerData()
	{
		string filePath = Path.Combine(Application.streamingAssetsPath, allPlayerDataFileName);

		if (File.Exists(filePath))
		{
			allPlayerData.allPlayerData = allPlayer;


			string playerJSON = JsonUtility.ToJson(allPlayerData);
			Debug.Log("JSON: " + playerJSON);
			File.WriteAllText(filePath, playerJSON);
		}
		else
		{
			Debug.LogError("Cannot find player data to save data!");
		}
	}
	public void ReturnToMenu()
	{
		SceneManager.LoadScene("SelectTutorialScene");
	}

	// Update is called once per frame
	void Update()
	{

		//if round is active decrement time remaining and update display
		if (isRoundActive)
		{
			time += Time.deltaTime;
			UpdateTimeRemainingDisplay();
		}
	}

	private void UpdateTimeRemainingDisplay()
	{
		float minutes = Mathf.Round(time / 60);
		float seconds = Mathf.Round(time % 60);
		//timeDisplayText.text = "Time: " + Mathf.Round(time).ToString();
		timeDisplayText.text = "Time: " + string.Format("{0:00}", minutes) + ":"+ string.Format("{0:00}", seconds) ;
	}
}