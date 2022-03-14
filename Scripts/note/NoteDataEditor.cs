using UnityEngine;
using UnityEditor;
using System.IO;

public class NoteDataEditor : EditorWindow
{

    public NoteDataList noteDataList;

    private string noteDataListProjectFilePath = "/StreamingAssets/NoteData.json";

    [MenuItem("Window/Note Data Editor")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(NoteDataEditor)).Show();
    }

    void OnGUI()
    {
        if (noteDataList != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("noteDataList");
            EditorGUILayout.PropertyField(serializedProperty, true);

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Save data"))
            {
                SaveNoteDataList();
            }
        }

        if (GUILayout.Button("Load data"))
        {
            LoadNoteDataList();
        }
    }

    private void LoadNoteDataList()
    {
        string filePath = Application.dataPath + noteDataListProjectFilePath;

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            noteDataList = JsonUtility.FromJson<NoteDataList>(dataAsJson);

            if (noteDataList == null)
            {
                noteDataList = new NoteDataList();
            }
        }
        else
        {
            noteDataList = new NoteDataList();
        }
    }

    private void SaveNoteDataList()
    {

        string dataAsJson = JsonUtility.ToJson(noteDataList);

        string filePath = Application.dataPath + noteDataListProjectFilePath;
        File.WriteAllText(filePath, dataAsJson);

    }
}