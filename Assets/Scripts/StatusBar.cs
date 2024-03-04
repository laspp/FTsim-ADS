using TMPro;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
    public TextMeshProUGUI statusBarText;
    bool init = true;

    // Start is called before the first frame update
    void Awake()
    {
        // First, hide panel that is a parent of a text object.
        // It will become visible, when text becomes not empty.
        if (init)
        {
            ClearStatusBarText();
            init = false;
        }
        
    }

    public void SetStatusBarText(string t)
    {
        statusBarText.text = t;

        gameObject.SetActive(true);
    }

    public void ClearStatusBarText()
    {
        statusBarText.text = "";
        gameObject.SetActive(false);
    }
}
