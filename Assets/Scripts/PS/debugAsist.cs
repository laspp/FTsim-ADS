using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugInputHandler : MonoBehaviour
{
    public  TMP_InputField DBGtxt;// Reference to the InputField
    public TutorialManager tutorialManager;
    private TutorialLoader tutorialLoader;
    void Start()
    {
        //tutorialLoader = tutorialManager.tutorialLoader;
        // Add a listener to handle input field changes
        DBGtxt.onEndEdit.AddListener(OnInputFieldEndEdit);
    }

    void OnInputFieldEndEdit(string input)
    {
        // Parse the input to an integer
        if (int.TryParse(input, out int number))
        {
            //tutorialLoader.currentTutorialIndex = number;
            tutorialManager.StartNewTutorial(number);
            // tutorialManager.currentTutorialData = tutorialLoader.LoadTutorial(number);
            
            // tutorialManager.CheckTutorialData();
        }
        else
        {
            Debug.LogError("Invalid input. Please enter a valid number.");
        }
    }
}