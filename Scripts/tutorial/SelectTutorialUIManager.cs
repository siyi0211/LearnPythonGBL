using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectTutorialUIManager : MonoBehaviour
{
    private int tutorialSetNumber;
    private TutorialData[] allTutorialData;


    public void Tutorial1()
    {
        tutorialSetNumber = 0;
        SceneManager.LoadScene("TutorialScene");
    }

    public void Tutorial2()
    {
        tutorialSetNumber = 1;
        SceneManager.LoadScene("TutorialScene");
    }

    public int getTutorialSetNumber()
    {
        return tutorialSetNumber;
    }

}
