using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class KeyControlToggle : MonoBehaviour
{
    public KeyCode keyCode;
    public bool detectEdge = false;
    private bool keyOld = false;

    void Update()
    {
        if (detectEdge)
        {
            if (Input.GetKeyDown(keyCode) && !keyOld)
            {
                this.GetComponent<Toggle>().isOn = true;
            }
            if (Input.GetKeyDown(keyCode) && keyOld)
            {
                this.GetComponent<Toggle>().isOn = false;
            }
            keyOld = this.GetComponent<Toggle>().isOn;
        }
        else
        {
            if (Input.GetKeyDown(keyCode))
            {
                this.GetComponent<Toggle>().isOn = true;
            }
            if (Input.GetKeyUp(keyCode))
            {
                this.GetComponent<Toggle>().isOn = false;
            }
        }



    }
}
