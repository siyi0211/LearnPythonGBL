using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;//Load scenes
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;


public class NoteDataController : MonoBehaviour
{
    private NoteData[] allNoteData;

    private string noteDataFileName = "NoteData.json";

    public SimpleObjectPool noteButtonObjectPool;
    public Transform noteButtonParent;

    public List<GameObject> noteButtonGameObjects = new List<GameObject>();

    void Start()
    {
        LoadGameData();
        LoadNoteButton();

    }

    public void LoadGameData()
    {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, noteDataFileName);

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            NoteDataList loadedData = JsonUtility.FromJson<NoteDataList>(dataAsJson);

            // Retrieve the allRoundData property of loadedData
            allNoteData = loadedData.allNoteData;
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }

    public void LoadNoteButton()
    {

        if (allNoteData.Length > 0)
        {
            Debug.LogError("all note data lenght > 0");
            for (int i = 0; i < allNoteData.Length; i++)
            {
                GameObject noteButtonGameObject = noteButtonObjectPool.GetObject();
                noteButtonGameObjects.Add(noteButtonGameObject);
                noteButtonGameObject.transform.SetParent(noteButtonParent);

                SelectNoteButton selectNoteButton = noteButtonGameObject.GetComponent<SelectNoteButton>();
                selectNoteButton.Setup(allNoteData[i]);

            }

        }
        else
        {

            Debug.LogError("all tutorial data lenght = 0");
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void NoteButtonClicked(int id)
    {
        Debug.LogError("note chap:" + id);
        NoteChapterNumber.noteChapterNumber = id;
        SceneManager.LoadScene("NoteScene");
    }
}
