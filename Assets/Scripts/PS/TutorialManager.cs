using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using TMPro;

[Serializable]
public class TutorialData
{
    public int TutorialStep;
    public string TutorialTitle;
    public string TaskDescription;
    public List<ChatBubble> ChatBubbles;
    public Test Test;
}

[Serializable]
public class ChatBubble
{
    public int Id;
    public string Text;
}

//-----------------Test-----------------
[Serializable]
public class Test
{
    public string TestType;
    public string TestName;
    public string TestDescription;
    public string TestExpected;
    public string TestResult;
    public string TestResultStatus;
}

//-----------------code-----------------
// TutorialLoader with refactored LoadTutorial method
public class TutorialLoader : MonoBehaviour
{
    public TutorialManager tutorialManager; 
    public string tutorialsFolderPath = "Scripts/PS/Tutorials";
    public int currentTutorialIndex = 1; 

    public TutorialData LoadTutorial(int tutorialIndex)
    {
        string fileName = $"Tutorial_{tutorialIndex}_*.json";
        string fullPath = Path.Combine(Application.dataPath, tutorialsFolderPath);
        string[] foundFiles = Directory.GetFiles(fullPath, fileName);
        
        if (foundFiles.Length > 0) 
        {
            string jsonContent = File.ReadAllText(foundFiles[0]); // Directly access the only file
            TutorialData tutorialData = JsonUtility.FromJson<TutorialData>(jsonContent);
            if (tutorialData == null)
            {
                tutorialManager.AddError($"Failed to parse tutorial data from {foundFiles[0]}");
                Debug.LogError($"Failed to parse tutorial data from {foundFiles[0]}");
                return null;
            }
            Debug.Log($"Loaded tutorial: {tutorialData}"); // .TutorialTitle
            return tutorialData;
        }
        else
        {
            tutorialManager.AddError($"Expected file matching pattern '{fileName}' in '{tutorialsFolderPath}', but found {foundFiles.Length}.");
            Debug.LogError($"Expected file matching pattern '{fileName}' in '{tutorialsFolderPath}', but found {foundFiles.Length}.");
            return null;
        }
    }

//RAZMISLI KAKO UPORABITI
    // public void LoadNextTutorial()
    // {
    //     currentTutorialIndex++;
    // }
}

// TutorialManager with state management and display logic
public class TutorialManager : MonoBehaviour
{
    private TutorialLoader tutorialLoader;
    private TutorialData currentTutorialData;
    public GameObject dialogPrefab; // Reference to your dialog prefab
    private HashSet<string> errorMessages = new HashSet<string>();

    void Start()
    {
        tutorialLoader = gameObject.AddComponent<TutorialLoader>();
        tutorialLoader.tutorialManager = this; // create a reference to this TutorialManager
    
        InvokeRepeating(nameof(DisplayErrors), 1f, 1f); // display errors every second

        currentTutorialData = tutorialLoader.LoadTutorial(tutorialLoader.currentTutorialIndex);
        if (currentTutorialData != null)
        {
            DisplayChatBubbles(currentTutorialData.ChatBubbles);
            Debug.Log($"TutorialManager initialized with tutorial index: {tutorialLoader.currentTutorialIndex}");
        } 
        
    }

    // public void OnTutorialCompleted()
    // {
    //     tutorialLoader.LoadNextTutorial();
    //     currentTutorialData = tutorialLoader.LoadTutorial(tutorialLoader.currentTutorialIndex);
    //     // if (currentTutorialData != null)
    //     // {
    //     //     DisplaySpeechBubble(currentTutorialData.SpeechBubble);
    //     // }
    // }

    void DisplayChatBubbles(List<ChatBubble> chatBubbles)
    {
        foreach (var chatBubble in chatBubbles)
        {
            string objectName = $"ChatBubble_{currentTutorialData.TutorialStep}_{chatBubble.Id}";
            GameObject chatBubbleObject = GameObject.Find(objectName);
            if (chatBubbleObject != null)
            {
                TMP_Text bubbleText = chatBubbleObject.GetComponentInChildren<TMP_Text>();
                if (bubbleText != null)
                {
                    bubbleText.text = chatBubble.Text;
                }
                else
                {
                    Debug.LogError($"Text component not found in {objectName}");
                }
            }
            else
            {
                Debug.LogError($"ChatBubble object {objectName} not found.");
            }
        }
    }

//-----------------Error handling-----------------
    public void AddError(string message)
    {
        errorMessages.Add(message);
    }

    public void DisplayErrors()
    {
        if (errorMessages.Count > 0)
        {
            string errorMessage = string.Join("\n", errorMessages);
            ShowDialog(errorMessage);
            errorMessages.Clear(); // Clear after displaying
        }
    }
  
    public void ShowDialog(string message)
    {
        Dialog.MessageBox(
            "Dialog_error_tutorial",
            "Tutorial Error",
            message,
            "Retry",
            () => Start(), 
            widthMax: 300,
            heightMax: 220
        );
    }
}