using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelHelp : MonoBehaviour
{
    public Transform showHelpStartToggle;
    Toggle toggle;
    public GameObject keyboardShortcuts;
    public StatusBar panelStatusBar;
    public GameObject TextR;
    public GameObject startupBubble;
    TutorialManager tutorialManager;
    int showHelpOnStart;
    bool hintShown = false;
    private bool showOnStartupClickedFirstTime = true;
    void Awake()
    {
        tutorialManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
        if (!PlayerPrefs.HasKey("showHelpOnStart"))
        {
            Debug.Log("PlayerPRefs not found");
            PlayerPrefs.SetInt("showHelpOnStart", 1);
        }
        showHelpOnStart = PlayerPrefs.GetInt("showHelpOnStart");
        //showHelpStartToggle.GetComponent<Toggle>().isOn = showHelpOnStart == 1;
        toggle = showHelpStartToggle.GetComponent<Toggle>();
        toggle.isOn = showHelpOnStart == 1;
        keyboardShortcuts.SetActive(showHelpOnStart == 1);
        gameObject.SetActive(showHelpOnStart == 1);
    }

    public void ToggleVisibility()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            keyboardShortcuts.SetActive(false);
            TextR.SetActive(false); 
            tutorialManager.ClearCurrentChatBubbles();
            if (!hintShown)
            {
                panelStatusBar.SetStatusBarText($"Press F1 to display the help window.");
                hintShown = true;
            }
            
        }            
        else
        {
            gameObject.SetActive(true);
            keyboardShortcuts.SetActive(true);
            TextR.SetActive(true);
        }
            
    }

    public void ToggleButtonOnChange()
    {
        if (toggle.isOn)
        {
            Debug.Log("ENABLE TUTORIAL Show help on start 1");
            PlayerPrefs.SetInt("showHelpOnStart", 1);
        }
        else
        {
            Debug.Log("DISABLE TUTORIAL Show help on start 0");
            PlayerPrefs.SetInt("showHelpOnStart", 0);
        }

        if (showOnStartupClickedFirstTime) 
        {
            showOnStartupClickedFirstTime = false; 
            
            startupBubble.SetActive(true);    
            TMP_Text bubbleText = startupBubble.GetComponentInChildren<TMP_Text>();
            bubbleText.text = "If this checkbox is marked, the tutorial is available.";
        }
    }
}
