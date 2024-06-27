using System;
using UnityEngine;
using System.IO;

[Serializable]
public class TutorialData
{
    //klas v katere je shranjen deselizirazian JSON
    public int TutorialStep;
    public string TutorialTitle;
    public string TaskDescription;
    public SpeechBubble SpeechBubble;
    public Test Test;
    public string nextTutorial;
}

//-----------------SpeechBubble-----------------
[Serializable]
public class SpeechBubble
{
    public string Text;
    public Position Position;
    public Style Style;
}

[Serializable]
public class Position
{
    public string baseComponent;
    public int x;
    public int y;
}

[Serializable]
public class Style
{
    public string BackgroundColor;
    public string TextColor;
    public int BorderRadius;
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

//koda
// TutorialLoader with refactored LoadTutorial method
public class TutorialLoader : MonoBehaviour
{
    public string tutorialsFolderPath = "Scripts/PS/Tutorials";
    public int currentTutorialIndex = 1; // Start with the first tutorial

    public TutorialData LoadTutorial(int tutorialIndex)
    {
        string fileName = $"Tutorial{tutorialIndex}-*.json";
        string fullPath = Path.Combine(Application.dataPath, tutorialsFolderPath);
        string[] files = Directory.GetFiles(fullPath, fileName);

        if (files.Length > 0)
        {
            string jsonContent = File.ReadAllText(files[0]);
            TutorialData tutorialData = JsonUtility.FromJson<TutorialData>(jsonContent);
            Debug.Log($"Loaded tutorial: {tutorialData.TutorialTitle}");
            return tutorialData;
        }
        else
        {
            Debug.LogError($"Tutorial file not found for index: {tutorialIndex}");
            return null;
        }
        
    }

    public void LoadNextTutorial()
    {
        currentTutorialIndex++;
    }
}

// TutorialManager with state management and display logic
public class TutorialManager : MonoBehaviour
{
    private TutorialLoader tutorialLoader;
    private TutorialData currentTutorialData;

    void Start()
    {
        if (tutorialLoader == null)
        {
            tutorialLoader = FindObjectOfType<TutorialLoader>();
        }

        if (tutorialLoader != null)
        {
            
            currentTutorialData = tutorialLoader.LoadTutorial(tutorialLoader.currentTutorialIndex);
            if (currentTutorialData != null)
            {
                DisplaySpeechBubble(currentTutorialData.SpeechBubble);
            }
        }
        else
        {
            Debug.LogError("TutorialLoader not found.");
        }
        Debug.Log($"TutorialManager initialized with tutorial index: {tutorialLoader.currentTutorialIndex}");
    }

    public void OnTutorialCompleted()
    {
        tutorialLoader.LoadNextTutorial();
        currentTutorialData = tutorialLoader.LoadTutorial(tutorialLoader.currentTutorialIndex);
        if (currentTutorialData != null)
        {
            DisplaySpeechBubble(currentTutorialData.SpeechBubble);
        }
    }

    void DisplaySpeechBubble(SpeechBubble speechBubble)
    {
        // Implementation for displaying the speech bubble
    }
}