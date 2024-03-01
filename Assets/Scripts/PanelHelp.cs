using UnityEngine;
using UnityEngine.UI;

public class PanelHelp : MonoBehaviour
{
    public Transform showHelpStartToggle;
    public GameObject keyboardShortcuts;

    private int showHelpOnStart;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("showHelpOnStart"))
        {
            PlayerPrefs.SetInt("showHelpOnStart", 1);
        }
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
