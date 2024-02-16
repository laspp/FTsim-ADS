using UnityEngine;
using UnityEngine.UI;

public class KeyControlToggle : MonoBehaviour
{
    public string key_code;
    public bool detectEdge = false;
    private bool keyOld = false;

    void Update()
    {
        if (detectEdge)
        {
            if (Input.GetKeyDown(key_code) && !keyOld)
            {
                this.GetComponent<Toggle>().isOn = true;
            }
            if (Input.GetKeyDown(key_code) && keyOld)
            {
                this.GetComponent<Toggle>().isOn = false;
            }
            keyOld = this.GetComponent<Toggle>().isOn;
        }
        else
        {
            if (Input.GetKeyDown(key_code))
            {
                this.GetComponent<Toggle>().isOn = true;
            }
            if (Input.GetKeyUp(key_code))
            {
                this.GetComponent<Toggle>().isOn = false;
            }
        }



    }
}
