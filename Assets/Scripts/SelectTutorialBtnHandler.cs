using UnityEngine;
using UnityEngine.UI;

public class SelectTutorialBtnHandler : MonoBehaviour
{
    public TutorialManager tutorialManager; 
    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick); // add a listener to handle clicks
    }

    void OnButtonClick()
    {
        string buttonName = gameObject.name;

        if (int.TryParse(buttonName, out int tutorialIndex))
        {
            tutorialManager.StartNewTutorial(tutorialIndex);
        }
        else
        {
            Debug.LogError("Failed to extract tutorial index from button name: " + buttonName);
        }
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}