using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectNoteButton : MonoBehaviour
{
	public Text noteNameText;//Text to display
	private NoteDataController noteDataController;//reference to data controller

	private NoteData noteData;//store the answer instance


	// Use this for initialization
	void Start()
	{
		noteDataController = FindObjectOfType<NoteDataController>(); // find the data controller
	}

	public void Setup(NoteData data)//pass in answer data and set up for display
	{
		noteData = data;
		noteNameText.text = noteData.Name;
	}


	void Update()
	{

	}

	//handle the button click 
	public void HandleClick()
	{
		noteDataController.NoteButtonClicked(noteData.ID);
	}
}
