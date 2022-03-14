using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;

public class MainMenuUIManager : MonoBehaviour
{
	public string Url;

	public Text playerNameText;
	public Text playerLevelText;
	public Text playerCoinText;

	private PlayerData[] allPlayer;
	private PlayerData player;
    private string allPlayerDataFileName = "PlayerData.json";

    private void Start()
    {
		LoadPlayerData();
		
		SetupUI();
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


    public void SetupUI()
    {
        player = GetCurrentPlayerData();

        playerNameText.text = player.name;
        playerLevelText.text = "Lv. " + player.level;
        playerCoinText.text = ""+ player.coin;

    }
    public void TutorialButton()
	{
		SceneManager.LoadScene("SelectTutorialScene");
	}

	public void NoteButton()
	{
		SceneManager.LoadScene("SelectNoteScene");
	}

	public void CommunityButton()
	{
		Application.OpenURL(Url);
	}

}
