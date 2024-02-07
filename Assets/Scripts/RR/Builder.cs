using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Builder : MonoBehaviour
{

    public GameObject buttonReset;
    public GameObject buttonSave;
    public GameObject toggleSpawn;
    public GameObject prefabSmartBar;
    public GameObject communication;
    public int gridX = 10;
    public int gridZ = 16;
    public float spacing = 0.65f;
    public int offsetX = -8;
    public int offsetZ = -3;

    Communication com;
    float startX;
    float startZ;
    Button btnSave, btnReset;
    Toggle tglSpawn;

    bool builderMode;
    public bool BuilderMode { get => builderMode; set => builderMode = value; }

    bool spawnMode;
    public bool SpawnMode { get => spawnMode; set => spawnMode = value; }

    // Start is called before the first frame update
    void Awake()
    {
        com = communication.GetComponent<Communication>();

        BuilderMode = false;
        SpawnMode = false;
        btnSave = buttonSave.GetComponent<Button>();
        btnReset = buttonReset.GetComponent<Button>();
        tglSpawn = toggleSpawn.GetComponent<Toggle>();
        btnSave.interactable = false;
        btnReset.interactable = false;
        tglSpawn.interactable = false;

        startX = prefabSmartBar.transform.position.x + offsetX * spacing;
        startZ = prefabSmartBar.transform.position.z + offsetZ * spacing;
    }

    void Start()
    {
        prefabSmartBar.SetActive(false);
        for (int z = 0; z < gridZ; z++)
        {
            for (int x = 0; x < gridX; x++)
            {
                Vector3 pos = new Vector3(startX, 0, startZ) + new Vector3(x, 0, z) * spacing;
                GameObject clone = Instantiate(prefabSmartBar, pos, Quaternion.identity);
                // Put clone in the same group as prefab's
                clone.transform.parent = prefabSmartBar.transform.parent;
                clone.SetActive(true);
            }
        }
    }
        
    public void ResetSmartBarsHeight()
    {
        if (BuilderMode)
        {
            // Set the height of all smart bars (their base parent) to 1
            GameObject[] smartBarBases = GameObject.FindGameObjectsWithTag("SmartBarBase");
            foreach (GameObject sbb in smartBarBases)
            {
                //sbb.transform.localScale = initScale;
                sbb.GetComponent<SmartBarBase>().ResetTargetScale();
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
