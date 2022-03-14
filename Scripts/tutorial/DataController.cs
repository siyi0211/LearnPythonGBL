using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;//Load scenes
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class DataController : MonoBehaviour
{
    private TutorialData[] allTutorialData;//hold data for each round

    private TutorialData tutorialData;
    private QuestionData questionData;
    private AnswerData answerData;

    private string gameDataFileName = "TutorialData.json";

    public SimpleObjectPool tutorialButtonObjectPool;
    public Transform tutorialButtonParent;

    public List<GameObject> tutorialButtonGameObjects = new List<GameObject>();


    // Use this for initialization
    void Start()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        LoadGameDataFromFirebase();
    }

    public void LoadTutorialButton()
    {
        if (allTutorialData.Length > 0)
        {
            Debug.LogError("all tutorial data lenght > 0");
            for (int i = 0; i < allTutorialData.Length; i++)
            {
                Debug.LogError(allTutorialData[i].Name);
                if(allTutorialData[i].Active == true)
                {
                    
                    GameObject tutorialButtonGameObject = tutorialButtonObjectPool.GetObject();
                    tutorialButtonGameObjects.Add(tutorialButtonGameObject);
                    tutorialButtonGameObject.transform.SetParent(tutorialButtonParent);

                    TutorialButton tutorialButton = tutorialButtonGameObject.GetComponent<TutorialButton>();
                    tutorialButton.Setup(allTutorialData[i]);
                }
            }

        }
        else
        {

            Debug.LogError("all tutorial data lenght = 0");
        }
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

                tutorials.Clear();
                LoadTutorialButton();
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

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void TutorialButtonClicked(int id)
    {
        Debug.LogError("button click:" + id);
        TutorialSetNumber.tutorialSetNumber = id;
        SceneManager.LoadScene("TutorialScene");
    }
}