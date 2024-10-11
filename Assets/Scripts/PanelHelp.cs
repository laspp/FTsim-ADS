using UnityEngine;
using UnityEngine.UI;

public class PanelHelp : MonoBehaviour
{
    public Transform showHelpStartToggle;
    public GameObject keyboardShortcuts;
    public StatusBar panelStatusBar;
    GameObject TextR;
    TutorialManager tutorialManager;
    int showHelpOnStart;
    bool hintShown = false;

    //pojedi cež celo TOGGLE procedura, kje se forca 1?
    void Awake()
    {
        TextR = GameObject.Find("TextR");
        tutorialManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
        if (!PlayerPrefs.HasKey("showHelpOnStart"))
        {
            Debug.Log("PlayerPRefs not found");
            PlayerPrefs.SetInt("showHelpOnStart", 1);
        }
        showHelpOnStart = PlayerPrefs.GetInt("showHelpOnStart");
        //showHelpStartToggle.GetComponent<Toggle>().isOn = showHelpOnStart == 1;
        Toggle toggle = showHelpStartToggle.GetComponent<Toggle>();
        toggle.isOn = showHelpOnStart == 1;
        keyboardShortcuts.SetActive(showHelpOnStart == 1);

        //toggle.onValueChanged.AddListener(delegate { ToggleButtonOnChange(); });
    }

    public void ToggleVisibility()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            keyboardShortcuts.SetActive(false);
            TextR.SetActive(false); //JustToggleVisibility.ToggleVisibility();
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
        Toggle toggle = showHelpStartToggle.GetComponent<Toggle>();
            Debug.Log("ToggleButtonOnChange called. Toggle isOn: " + toggle.isOn);
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
    }
}
