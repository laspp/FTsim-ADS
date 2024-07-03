using UnityEngine;
using UnityEngine.UI;

public class PanelHelp : MonoBehaviour
{
    public Transform showHelpStartToggle;
    public GameObject keyboardShortcuts;
    public StatusBar panelStatusBar;

    int showHelpOnStart;
    bool hintShown = false;

    void Awake()
    {
        // ker je toggle disejblan ga ne upoštevamo, mogoče kasneje uporabi
        // if (!PlayerPrefs.HasKey("showHelpOnStart"))
        // {
        //     PlayerPrefs.SetInt("showHelpOnStart", 1);
        // }
        PlayerPrefs.SetInt("showHelpOnStart", 1); // Force enable on start
        showHelpOnStart = PlayerPrefs.GetInt("showHelpOnStart");
        showHelpStartToggle.GetComponent<Toggle>().isOn = (showHelpOnStart == 1);
        gameObject.SetActive(showHelpOnStart == 1);
        keyboardShortcuts.SetActive(showHelpOnStart == 1);
    }

    public void ToggleVisibility()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            keyboardShortcuts.SetActive(false);
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
        }
            
    }

    public void ToggleButtonOnChange()
    {
        if (showHelpStartToggle.GetComponent<Toggle>().isOn)
        {
            PlayerPrefs.SetInt("showHelpOnStart", 1);
        }
        else
        {
            PlayerPrefs.SetInt("showHelpOnStart", 0);
        }
    }
}
