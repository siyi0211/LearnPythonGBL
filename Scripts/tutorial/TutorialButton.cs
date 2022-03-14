using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour
{
	public Text tutorialNameText;//Text to display
	private DataController dataController;//reference to data controller

	private TutorialData tutorialData;//store the answer instance


	// Use this for initialization
	void Start()
	{
		dataController = FindObjectOfType<DataController>(); // find the data controller
	}

	public void Setup(TutorialData data)//pass in answer data and set up for display
	{
		tutorialData = data;
		tutorialNameText.text = tutorialData.Name;
	}


	void Update()
	{

	}

	//handle the button click 
	public void HandleClick()
	{
		dataController.TutorialButtonClicked(tutorialData.ID);
	}
}
