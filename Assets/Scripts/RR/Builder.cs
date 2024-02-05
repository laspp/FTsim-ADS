using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Builder : MonoBehaviour
{

    public GameObject buttonReset;
    public GameObject buttonSave;

    Vector3 initScale = new Vector3(1,1,1);
    Button btnSave, btnReset;

    bool builderMode;
    public bool BuilderMode { get => builderMode; set => builderMode = value; }

    // Start is called before the first frame update
    void Awake()
    {
        BuilderMode = false;
        btnSave = buttonSave.GetComponent<Button>();
        btnReset = buttonReset.GetComponent<Button>();
        btnSave.interactable = false;
        btnReset.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetSmartBarsHeight()
    {
        if (BuilderMode)
        {
            // Set the height of all smart bars (their base parent) to 1
            GameObject[] smartBarBases = GameObject.FindGameObjectsWithTag("SmartBarBase");
            foreach (GameObject sbb in smartBarBases)
            {
                sbb.transform.localScale = initScale;
            }
        }
    }

    public void ToggleButtonOnChange()
    {
        BuilderMode = !BuilderMode;
        if (BuilderMode)
        {
            btnSave.interactable = true;
            btnReset.interactable = true;
        }
        else
        {
            btnSave.interactable = false;
            btnReset.interactable = false;
        }
    }
}
