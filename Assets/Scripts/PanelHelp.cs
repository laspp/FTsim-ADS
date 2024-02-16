using UnityEngine;
using UnityEngine.UI;

public class PanelHelp : MonoBehaviour
{
    public Transform showHelpStartToggle;

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
    }

    public void ToggleVisibility()
    {
        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
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
