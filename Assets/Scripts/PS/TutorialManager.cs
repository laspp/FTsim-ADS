using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;

[Serializable]
public class TutorialData
{
    public int TutorialStep;
    public string TutorialTitle;
    public string TaskDescription;
    public List<ChatBubble> ChatBubbles;
    public List<Test> Tests;
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
    public string Tag;
    public bool Val;
    public float Time;
}

//-----------------code-----------------
// TutorialLoader with refactored LoadTutorial method
public class TutorialLoader : MonoBehaviour
{
    public TutorialManager tutorialManager; 
    public string tutorialsFolderPath = "Scripts/PS/Tutorials";
    public int currentTutorialIndex = 0; 

    public TutorialData LoadTutorial(int tutorialIndex)
    {
        string fileName = $"Tutorial_{tutorialIndex}_*.json";
        string fullPath = Path.Combine(Application.dataPath, tutorialsFolderPath);
        string[] foundFiles = Directory.GetFiles(fullPath, fileName);
        
        if (foundFiles.Length > 0) 
        {
            string jsonContent = File.ReadAllText(foundFiles[0]); // Directly access the only file
            try
            {
                TutorialData tutorialData = JsonUtility.FromJson<TutorialData>(jsonContent);
                if (tutorialData == null)
                {
                    tutorialManager.AddError($"File {foundFiles[0]} is empty or not a valid JSON.");
                    Debug.LogError($"File {foundFiles[0]} is empty or not a valid JSON.");
                    return null;
                }
                //Debug.Log($"Loaded tutorial: {tutorialData}"); // .TutorialTitle
                Debug.Log($"Loaded tutorial: {JsonUtility.ToJson(tutorialData, true)}");
                return tutorialData;
            }
            catch (Exception e)
            {
                tutorialManager.AddError($"Failed to parse tutorial data from {foundFiles[0]}. Error: {e.Message}");
                Debug.LogError($"Failed to parse tutorial data from {foundFiles[0]}. Error: {e.Message}");
                return null;
            }
        }
        else
        {
            tutorialManager.AddError($"Expected file matching pattern '{fileName}' in '{tutorialsFolderPath}', but found {foundFiles.Length}.");
            Debug.LogError($"Expected file matching pattern '{fileName}' in '{tutorialsFolderPath}', but found {foundFiles.Length}.");
            return null;
        }
    }


    public TutorialData LoadNextTutorial()
    {
        currentTutorialIndex++;
        return LoadTutorial(currentTutorialIndex);
    }
}

// TutorialManager with state management and display logic
public class TutorialManager : MonoBehaviour
{
    public enum ButtonState
    {
        Read,
        Test,
        Next
    }
    private TutorialLoader tutorialLoader;
    private TutorialData currentTutorialData;
    public GameObject dialogPrefab; // Reference to your dialog prefab
    private HashSet<string> errorMessages = new HashSet<string>();
   // private List<Test> tests = new List<Test>();
    Communication com;
    Button testButton;
    TMP_Text testButtonText;

    private ButtonState testButtonState = ButtonState.Next;
    Color lightGray = new Color(0.7f, 0.7f, 0.7f);
    //Color lighterlightGray = new Color(0.85f, 0.85f, 0.85f);
    private List<GameObject> allChatBubbles = new List<GameObject>();
    private List<GameObject> currentlyOpenChatBubbles = new List<GameObject>();
    public GameObject chatBubblesParent;
    private int chatBubbleIndex;

    Button arrPrevButton;
    Button arrNextButton;

    void Start()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();
        GameObject testButtonGameObj = GameObject.Find("ButtonTest");
        testButton = testButtonGameObj.GetComponent<Button>();
        testButtonText = testButton.GetComponentInChildren<TMP_Text>();
        tutorialLoader = gameObject.AddComponent<TutorialLoader>();
        tutorialLoader.tutorialManager = this; // create a reference to this TutorialManager
        chatBubblesParent = GameObject.Find("ChatBubblesParent");
        Debug.Log($"ChatBubblesParent: {chatBubblesParent}");
        arrPrevButton = GameObject.Find("ButtonPrev").GetComponent<Button>();
        arrNextButton = GameObject.Find("ButtonNext").GetComponent<Button>();
        
        InvokeRepeating(nameof(DisplayErrors), 1f, 1f); // display errors every second

        HideAllChatBubbles();

        SetButtonColor(arrNextButton, lightGray);
        arrNextButton.interactable = false;

        currentTutorialData = tutorialLoader.LoadTutorial(tutorialLoader.currentTutorialIndex);
        CheckTutorialData();
    }

    void CheckTutorialData()
    {
        //testButton.interactable = true;
        chatBubbleIndex = 0;

        if (currentTutorialData != null)
        {
            //Tudi če je prazno polje, pobriše prejšni text
            DisplayTaskInPanel(currentTutorialData.TutorialTitle, currentTutorialData.TaskDescription);
            
            if(currentTutorialData.ChatBubbles == null || currentTutorialData.ChatBubbles.Count == 0){
                //nič ni za prebreati
                Debug.Log("No chat bubbles to display.");
                testButtonText.text = "Test";
                testButtonState = ButtonState.Test;
            } else {
                Debug.Log($"ChatBubbles count: {currentTutorialData.ChatBubbles.Count}");
                DisplayChatBubbles(currentTutorialData.ChatBubbles, chatBubbleIndex);
            }

            if(currentTutorialData.Tests == null || currentTutorialData.Tests.Count == 0){ 
            // if there are tests enable the test button
                Debug.Log("No tests to run.");
                testButtonText.text = "Next";
                testButtonState = ButtonState.Next;
            } else {
                Debug.Log($"Tests count: {currentTutorialData.Tests.Count}");
                
                testButtonText.text = "Read";
                testButtonState = ButtonState.Read;
                testButton.interactable = false;

                arrNextButton.interactable = true;
                //SetButtonColor(arrNextButton, Color.white);
            }
            Debug.Log($"TutorialManager initialized with tutorial index: {tutorialLoader.currentTutorialIndex}");
        } 
        else
        {
            Debug.LogError("TutorialData is missing.");
        }
        
    }

    void DisplayTaskInPanel(string tutorialTitle, string taskDescription)
    {
        try{
            GameObject tutorialPanel = GameObject.Find("TutorialPanel");
            TMP_Text titleText = tutorialPanel.transform.Find("TextHeader").GetComponent<TMP_Text>();
            titleText.text = tutorialTitle;

            GameObject textMainField = GameObject.Find("TextMainField");
            TMP_Text descriptionText = textMainField.GetComponentInChildren<TMP_Text>();
            descriptionText.text = taskDescription;
        }
        catch (Exception e)
        {
            Debug.LogError("TutorialPanel object not found; " + e.Message);
        }
    }

    void HideAllChatBubbles()
    {
        allChatBubbles.Clear();
        
        if (chatBubblesParent != null)
        {
            foreach (Transform child in chatBubblesParent.transform)
            {
                if (child.gameObject.name.StartsWith("ChatBubble"))
                {
                    child.gameObject.SetActive(false);
                    allChatBubbles.Add(child.gameObject);
                }
            }
        }
        else
        {
            Debug.LogError("ChatBubblesParent is not assigned.");
        }
    }

    void ClearCurrentChatBubbles()
    {
        foreach (GameObject cb in currentlyOpenChatBubbles)
        {
            cb.SetActive(false);
        }
        currentlyOpenChatBubbles.Clear();
    }

    void DisplayChatBubbles(List<ChatBubble> chatBubbles, int chatBubbleId = 0)
    {
        
        if (chatBubbleId < 0 || chatBubbleId >= chatBubbles.Count)
        {
            Debug.LogError($"Invalid chatBubbleId: {chatBubbleId}");
            return;
        }
        ChatBubble chatBubble = chatBubbles[chatBubbleId];
        string objectName = $"ChatBubble_{currentTutorialData.TutorialStep}_{chatBubbleId.ToString()}";

        foreach (GameObject obj in allChatBubbles)
        {
            if (obj.name == objectName)//display the chat bubble
            {
                obj.SetActive(true);
                
                TMP_Text bubbleText = obj.GetComponentInChildren<TMP_Text>();
                if (bubbleText != null)
                {
                    bubbleText.text = chatBubble.Text;
                    Debug.Log($"Displaying chat bubble: {objectName}");
                }
                else
                {
                    Debug.LogError($"Text component not found in {objectName}");
                }
                currentlyOpenChatBubbles.Add(obj);
                Debug.Log($"added {obj.name} to currentlyOpenChatBubbles");
                return;
            }
        }
        Debug.LogError($"ChatBubble object {objectName} not found.");
    }

    public void ButtonArrPrev()
    {
        
    }

    public void ButtonArrNext()
    {       

        chatBubbleIndex++;
        Debug.Log($"chatBubbleIndex: {chatBubbleIndex}");
        DisplayChatBubbles(currentTutorialData.ChatBubbles, chatBubbleIndex);

        //disable previous one?

        if(chatBubbleIndex == currentTutorialData.ChatBubbles.Count - 1){
            testButtonText.text = "Test";
            testButtonState = ButtonState.Test;
            testButton.interactable = true;
            SetButtonColor(testButton, lightGray);

            //SetButtonColor(arrNextButton, lightGray);
            arrNextButton.interactable = false;
        }
    }

//----------------------Test----------------------


    public async void ButtonTest()
    {
        var testTasks = new List<Task<bool>>();
        
        testButton.interactable = false;
     

        switch (testButtonState)
        {
            case ButtonState.Read:
            // uninteractive in this state
            // ko pridemo do zadnjega oblačka na pučici desno gremo v testing, če obstaja
                
                break;
            case ButtonState.Test:
            //implementacija testiranja
                foreach (var test in currentTutorialData.Tests)
                    {
                        testTasks.Add(RunTest(test));   
                    }

                    bool[] results = await Task.WhenAll(testTasks);
                    bool allTrue = results.All(result => result);

                if (allTrue)
                    {
                        Debug.Log("All tests succeeded.");
                        SetButtonColor(testButton, Color.green*0.85f);
                        testButtonText.text = "Next";
                        testButtonState = ButtonState.Next;
                    }
                else
                    {
                        Debug.Log("Some tests failed.");
                        SetButtonColor(testButton, Color.red*0.85f);
                    }

                
                testButton.interactable = true;
                break;
            case ButtonState.Next:
            //preklop na naslednji tutorial
                ClearCurrentChatBubbles();
                
                currentTutorialData = tutorialLoader.LoadNextTutorial();
                CheckTutorialData();

  

               
                break;
        }

        
    }

    private async Task<bool> RunTest(Test test)
    {
        bool curVal;
        float curTime = 0;

        while (curTime <= test.Time)
        {
            curVal = com.GetTagValue(test.Tag);
            if (curVal == test.Val)
            {
                curTime += 0.5f;
                await Task.Delay(500); // Wait for 0.5 seconds
            }
            else
            {
                Debug.Log($"Test failed: {test.Tag} is not {test.Val}");
                return false;
            }
        }
        return true;
    }

    private void SetButtonColor(Button button, Color color)
    {
        color.a = 1;
        //color = color*0.85f;
        var colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = color*1.4f;
        colors.pressedColor = color;
        colors.selectedColor = color*1.2f;
        button.colors = colors;
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