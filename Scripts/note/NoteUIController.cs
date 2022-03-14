using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;//Load scenes
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System;

public class NoteUIController : MonoBehaviour
{
    private NoteData[] allNoteData;

    private NoteData noteData;

    private String[] notes;

    private string noteDataFileName = "NoteData.json";

    public Text noteDisplayText;
    public Text chapterNameDisplayText;
    public Text progressDisplayText;
    public GameObject PreviousButton;
    public GameObject NextButton;
    public Text NextButtonText;

    private int notesNum;

    void Start()
    {
        LoadGameData();
        SetUpRound();

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

    public NoteData GetCurrentNoteData()
    {
        Debug.LogError("note set number is: " + NoteChapterNumber.noteChapterNumber);

        for (int i = 0; i < allNoteData.Length; i++)
        {

            if (allNoteData[i].ID == NoteChapterNumber.noteChapterNumber)
            {
                return allNoteData[i];
            }

        }

        return null;

    }

    public void SetUpRound()
    {

        //Once dataController is loaded initialize game variables with the data found
        noteData = GetCurrentNoteData();
        notesNum = 0;
        notes = noteData.Notes;
        chapterNameDisplayText.text = noteData.Name;

        ButtonDisplay();

    }

    public void noteTextDisplay()
    {
        noteDisplayText.text = notes[notesNum];
        
    }

    public void progressDisplay()
    {
        progressDisplayText.text = notesNum + 1 + "/" + notes.Length;

    }

    public void ButtonDisplay()
    {
        if (notesNum == 0)
        {
            PreviousButton.SetActive(false);
        }
        else
        {
            PreviousButton.SetActive(true);
        }

        if (notesNum == notes.Length - 1)
        {
            NextButtonText.text = "Done";
        }
        else 
        {
            NextButtonText.text = "Next";
        }

        progressDisplay();
        noteTextDisplay();
    }

    public void PreviousButtonClick()
    {
        if (notesNum != 0)
        {
            notesNum -= 1;
        }

        ButtonDisplay();
    }

    public void NextButtonClick()
    {
        if (notes.Length > notesNum + 1)
        {
            notesNum++;
            ButtonDisplay();
        }
        else
        {
            SceneManager.LoadScene("SelectNoteScene");
        }
    }

    public void BackButton()
    {
        SceneManager.LoadScene("SelectNoteScene");
    }



}
