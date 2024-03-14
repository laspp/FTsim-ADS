using System.Collections;
using TMPro;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
    public TextMeshProUGUI statusBarText;
    public float timeToDisappear;
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

        // Cancel any existing coroutine for hiding status bar text
        StopCoroutine("HideStatusBarText");
        
        // Start coroutine to hide status bar text after some time
        StartCoroutine(HideStatusBarText());
    }

    IEnumerator HideStatusBarText()
    {
        yield return new WaitForSeconds(timeToDisappear);

        // Clear the status bar text after waiting
        ClearStatusBarText();
    }

    public void ClearStatusBarText()
    {
        statusBarText.text = "";
        gameObject.SetActive(false);
    }
}
