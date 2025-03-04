using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.ComponentModel;

[Serializable]
public class TutorialData
{
    public int TutorialStep;
    public string TutorialTitle;
    public string TaskDescription;
    public List<ChatBubble> ChatBubbles;
    public List<Test> Tests;
    public List<Detector> Detectors;
}

[Serializable]
public class ChatBubble
{
    public int Id;
    public string Text;
}

[Serializable]
public class Detector
{
    public string Tag;
    public bool Val;
    public string CheckAt;
}

//-----------------Test-----------------
[Serializable]
public class Test
{
    public string Tag;
    public bool Val;
    public float TestRunTime;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    [DefaultValue(0)]
    public float StartTestDelay;
}

//-----------------code-----------------
// TutorialLoader with refactored LoadTutorial method
public class TutorialLoader : MonoBehaviour
{
    public TutorialManager tutorialManager; 
    public string tutorialsFolderPath = "StreamingAssets/Tutorials-PS";
    public int currentTutorialIndex; 

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
                //Debug.Log($"Loaded tutorial: {JsonUtility.ToJson(tutorialData, true)}");
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
        //save the current tutorial index, so we can continue from the same tutorial next time
        PlayerPrefs.SetInt("tutorialIndex", currentTutorialIndex);
        
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
    public GameObject dialogPrefab; 
    private HashSet<string> errorMessages = new HashSet<string>();
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
    private bool tutorialActive;
    public Button buttonPrevious;
    public Button buttonNext;
    public GridLayoutGroup gridToggles;
    public GameObject TestOutputConsoleParent;
    private TMP_Text testOutputConsole;    
    public ScrollRect tutorialPanelScrollRect;

    void Start()
    {
        tutorialLoader = gameObject.AddComponent<TutorialLoader>();
        tutorialLoader.tutorialManager = this; 

        com = GameObject.Find("Communication").GetComponent<Communication>();

        if (!PlayerPrefs.HasKey("showHelpOnStart"))
        {
            Debug.Log("PlayerPrefs showHelpOnStart key not found");
            PlayerPrefs.SetInt("showHelpOnStart", 1);
        }
        tutorialActive = PlayerPrefs.GetInt("showHelpOnStart") == 1;
      
        InvokeRepeating(nameof(DisplayErrors), 1f, 1f); // display errors every second

        HideAllChatBubbles();

        if (tutorialActive){
            initTutotialManager();
        }
    }

    public void initTutotialManager()
    {
        GameObject testButtonGameObj = GameObject.Find("ButtonTest");
        testButton = testButtonGameObj.GetComponent<Button>();
        testButtonText = testButton.GetComponentInChildren<TMP_Text>();
        buttonPrevious = GameObject.Find("ButtonPrev").GetComponent<Button>();
        buttonNext = GameObject.Find("ButtonNext").GetComponent<Button>();
        buttonPrevious.interactable = false;
        buttonNext.interactable = false;
        testOutputConsole = TestOutputConsoleParent.GetComponentInChildren<TMP_Text>();

        int i;
        //get the last turorial index from playerPrefs if exists
        if (!PlayerPrefs.HasKey("tutorialIndex"))
        {
            Debug.Log("Created tutorialIndex with value 0 in PlayerPrefs.");
            i = 0;	
            PlayerPrefs.SetInt("tutorialIndex", 0);
        } else {
            i = PlayerPrefs.GetInt("tutorialIndex");
        }

        StartNewTutorial(i);
    }

    public void StartNewTutorial(int index)
    { 
        TestOutputConsoleParent.SetActive(false);
        ClearCurrentChatBubbles();
        tutorialLoader.currentTutorialIndex = index;
        buttonNext.interactable = false;
        buttonPrevious.interactable = false;
        currentTutorialData = tutorialLoader.LoadTutorial(index);
        tutorialPanelScrollRect.verticalNormalizedPosition = 1;
        CheckTutorialData();
    }
    public void CheckTutorialData()
    {
        //testButton.interactable = true;
        chatBubbleIndex = 0;

        if (currentTutorialData != null)
        {
            //even if field is empty, we still display the title and description
            DisplayTaskInPanel(currentTutorialData.TutorialTitle, currentTutorialData.TaskDescription);
            
            bool oblacki = currentTutorialData.ChatBubbles != null && currentTutorialData.ChatBubbles.Count > 0;
            bool testi = currentTutorialData.Tests != null && currentTutorialData.Tests.Count > 0;

            if(oblacki && testi){
                Debug.Log($"O_T test count:{currentTutorialData.Tests.Count}, ChatBubbles count: {currentTutorialData.ChatBubbles.Count}");
                DisplayChatBubbles(currentTutorialData.ChatBubbles, chatBubbleIndex);

                if(currentTutorialData.ChatBubbles.Count > 1){
                    ChangeStateToRead();
                } else {
                    ChangeStateToTest();
                }
            } else if (oblacki && !testi){
                Debug.Log($"O_!T No tests, ChatBubbles count: {currentTutorialData.ChatBubbles.Count}");
                DisplayChatBubbles(currentTutorialData.ChatBubbles, chatBubbleIndex);

                if(currentTutorialData.ChatBubbles.Count > 1){
                    ChangeStateToRead(); //read v next, ker ni testov
                } else {
                    ChangeStateToNext();
                }
            } else if (!oblacki && testi){
                // buttonNext.interactable = false;
                // buttonPrevious.interactable = false;

                Debug.Log($"!O_T test count:{currentTutorialData.Tests.Count}");

                ChangeStateToTest();
            } else {
                Debug.Log("!O_!T No tests and no chat bubbles");

                ChangeStateToNext();
            }
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

    public void ClearCurrentChatBubbles()
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

                return;
            }
        }
        Debug.LogError($"ChatBubble object {objectName} not found.");
    }

    public void ButtonArrPrev()
    {
        if(chatBubbleIndex > 0){
            chatBubbleIndex--;
        }
        Debug.Log($"chatBubbleIndex: {chatBubbleIndex}");
        DisplayChatBubbles(currentTutorialData.ChatBubbles, chatBubbleIndex);

        //če bi hotel dobiti nazaj prvi oblaček ko je smao
        // if(currentTutorialData.ChatBubbles.Count > 1){
        //     buttonNext.interactable = true;
        // }

        if(chatBubbleIndex <= 0){
            buttonPrevious.interactable = false;
        }
        buttonNext.interactable = true;
    }

    public void ButtonArrNext()
    {       
        chatBubbleIndex++;
        Debug.Log($"chatBubbleIndex: {chatBubbleIndex}");
        DisplayChatBubbles(currentTutorialData.ChatBubbles, chatBubbleIndex);

        if(chatBubbleIndex >= 1){
            buttonPrevious.interactable = true;
        }
        //disable previous one?

        if(chatBubbleIndex == currentTutorialData.ChatBubbles.Count - 1){
            buttonNext.interactable = false;
            if(currentTutorialData.Tests != null || currentTutorialData.Tests.Count > 0){
                ChangeStateToTest();
            } else {
                ChangeStateToNext();
            }
        }
    }

    void ChangeStateToTest(){
        testButtonText.text = "Test";
        testButtonState = ButtonState.Test;
        testButton.interactable = true;
        SetButtonColor(testButton, Color.white);
    }

    void ChangeStateToNext(){
        testButtonText.text = "Next";
        testButtonState = ButtonState.Next;
        testButton.interactable = true;
    }

    void ChangeStateToRead(){
        testButtonText.text = "Read";
        testButtonState = ButtonState.Read;
        testButton.interactable = false;

        if(tutorialLoader.currentTutorialIndex < 17){
            buttonNext.interactable = true;
        }
    }

//----------------------Test----------------------
    public async void ButtonTest()
    {
        testButton.interactable = false;

        switch (testButtonState)
        {
            case ButtonState.Read:
            // uninteractive in this state
            // ko pridemo do zadnjega oblačka na pučici desno gremo v testing, če obstaja
                
                break;
            case ButtonState.Test:
            // test implementation
                var testTasks = new List<Task<bool>>();

                //clear and disable force table
                DisableAllToggles();

                testOutputConsole.text = "";
                TestOutputConsoleParent.SetActive(true);

                bool allTrue = false;
                //check detectors for stating positions
                if(CheckDetectors(currentTutorialData.Detectors, "Start"))
                {
                    foreach (var test in currentTutorialData.Tests)
                    {
                        testTasks.Add(RunTest(test));   
                    }

                    bool[] results = await Task.WhenAll(testTasks);
                    allTrue = results.All(result => result);
                } 

                if (allTrue && CheckDetectors(currentTutorialData.Detectors, "End"))
                {
                    Debug.Log("All tests passed!");
                    testOutputConsole.text += "All tests passed! :-)";
                    SetButtonColor(testButton, Color.green*0.85f);
                    ChangeStateToNext();
                }
                else
                {
                    SetButtonColor(testButton, Color.red*0.85f);
                    testButton.interactable = true;
                    
                }

                EnableAllToggles();
                break;
            case ButtonState.Next:
            //switch to next tutorial
                ClearCurrentChatBubbles();
                TestOutputConsoleParent.SetActive(false);

                currentTutorialData = tutorialLoader.LoadNextTutorial();
                CheckTutorialData();
                break;
        }

        
    }

    private async Task<bool> RunTest(Test test)
    {
        bool curVal;
        float curTime = 0;
        float totalTime = test.StartTestDelay + test.TestRunTime;
        float refreshRate = 0.2f; //change ONLY this value to set refresh rate in seconds
        int refreshRateMiliseconds = (int)(refreshRate * 1000);

        Communication.TagLocation location = com.CheckTagLocation(test.Tag);

        //Debug.Log($"TT{test.StartTestDelay} _ {test.TestRunTime}");
        switch(location)
        {  
            case Communication.TagLocation.Output:
                while (curTime < totalTime)
                {
                    //Debug.Log($"TIME: {curTime}");
                    if (test.StartTestDelay <= curTime)
                    {

                        curVal = com.GetTagValue(test.Tag);
                        if (curVal != test.Val)
                        {
                            Debug.Log($"Test failed: {test.Tag} is not {test.Val}");
                            testOutputConsole.text += $"X  Test failed: {test.Tag} is not {test.Val}\n";
                            return false;
                        }
                    }
                    curTime += refreshRate;
                    await Task.Delay(refreshRateMiliseconds);
                }
                return true;
            case Communication.TagLocation.Input:
                //totalTime in case of an input represends timeout
                while (curTime < totalTime)
                {
                    //Debug.Log($"TIME: {curTime}");
                    if (test.StartTestDelay <= curTime)
                    {
                        curVal = com.GetInputTagValue(test.Tag);
                        if (curVal == test.Val)
                        {
                            Debug.Log($"INPUT Test succeeded: {test.Tag} is {test.Val}");
                            testOutputConsole.text += $"OK Test of INPUT {test.Tag} passed\n";
                            return true;
                        }
                    }
                    curTime += refreshRate;
                    await Task.Delay(refreshRateMiliseconds);
                }
                Debug.Log($"Test failed: {test.Tag} is not {test.Val}");
                testOutputConsole.text += $"X  Test failed: {test.Tag} is not {test.Val}\n";
                return false;
            case Communication.TagLocation.None:

                AddError($"Cannot get location and value of tag '{test.Tag}'.");
                return false;
        }
        return false;
    }

    private bool CheckDetectors(List<Detector> detectors, string calledAt)
    {
        if(detectors != null && detectors.Count > 0) {
            foreach (var detector in detectors)
            {
                if (detector.CheckAt == calledAt)
                {
                    if (com.GetDetectorValue(detector.Tag) != detector.Val)
                    {
                        Debug.Log($"Detector {detector.Tag} is not in correct state. Called at: {calledAt}");
                        testOutputConsole.text += $"Detector {detector.Tag} is not in correct state upon {calledAt}\n";
                        return false;
                    }
                }
            }
            Debug.Log("All detectors are in correct state.");
            testOutputConsole.text += $"OK All detectors are in correct state on {calledAt} :-)\n";
        }
        return true;
    }

    private void SetButtonColor(Button button, Color color)
    {
        color.a = 1;
        var colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = color*1.4f;
        colors.pressedColor = color;
        colors.selectedColor = color*1.2f;
        button.colors = colors;
    }

    public void DisableAllToggles()
    {
        foreach (Transform child in gridToggles.transform)
        {
            Toggle toggle = child.GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = false;
                toggle.interactable = false;
            }
        }
    }

    public void EnableAllToggles()
    {
        foreach (Transform child in gridToggles.transform)
        {
            Toggle toggle = child.GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.interactable = true;
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