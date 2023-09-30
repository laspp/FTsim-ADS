using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelHelp : MonoBehaviour
{
    public Transform showHelpStartToggle;

    private int showHelpOnStart;
    private GameObject helpWindow;

    void Awake()
    {
        helpWindow = GameObject.FindGameObjectWithTag("UI_help_window");

        if (!PlayerPrefs.HasKey("showHelpOnStart"))
        {
            PlayerPrefs.SetInt("showHelpOnStart", 1);
        }
        showHelpOnStart = PlayerPrefs.GetInt("showHelpOnStart");
        showHelpStartToggle.GetComponent<Toggle>().isOn = (showHelpOnStart == 1);
        helpWindow.SetActive(showHelpOnStart == 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleVisibility()
    {
        if (helpWindow.activeInHierarchy)
            helpWindow.SetActive(false);
        else
            helpWindow.SetActive(true);
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
