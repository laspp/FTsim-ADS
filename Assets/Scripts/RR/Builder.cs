using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Builder : MonoBehaviour
{

    public GameObject buttonReset;
    public GameObject buttonSave;
    public GameObject toggleSpawn;

    Vector3 initScale = new Vector3(1,1,1);
    Button btnSave, btnReset;
    Toggle tglSpawn;

    bool builderMode;
    public bool BuilderMode { get => builderMode; set => builderMode = value; }

    bool spawnMode;
    public bool SpawnMode { get => spawnMode; set => spawnMode = value; }

    // Start is called before the first frame update
    void Awake()
    {
        BuilderMode = false;
        SpawnMode = false;
        btnSave = buttonSave.GetComponent<Button>();
        btnReset = buttonReset.GetComponent<Button>();
        tglSpawn = toggleSpawn.GetComponent<Toggle>();
        btnSave.interactable = false;
        btnReset.interactable = false;
        tglSpawn.interactable = false;
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

    public void ToggleModeOnChange()
    {
        BuilderMode = !BuilderMode;
        if (BuilderMode)
        {
            btnSave.interactable = true;
            btnReset.interactable = true;
            tglSpawn.interactable = true;
        }
        else
        {
            btnSave.interactable = false;
            btnReset.interactable = false;
            tglSpawn.interactable = false;
        }
    }
    public void ToggleSpawnOnChange()
    {
        SpawnMode = !SpawnMode;
    }
}
