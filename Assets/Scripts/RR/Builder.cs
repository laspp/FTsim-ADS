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
    List<int[]> smartBarGrid;
    bool isSaved = false;
    int sX, sZ, sScale = 1;

    bool builderMode;
    public bool BuilderMode { get => builderMode; set => builderMode = value; }

    bool spawnMode;
    public bool SpawnMode { get => spawnMode; set => spawnMode = value; }

    // Start is called before the first frame update
    void Awake()
    {
        com = communication.GetComponent<Communication>();
        smartBarGrid = com.appConfig.SmartBarGrid;

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
                if (smartBarGrid != null)
                {
                    // Check if index exists in smartBarGrid and use its props
                    isSaved = false;
                    foreach (var savedItem in smartBarGrid)
                    {
                        sX = savedItem[0];
                        sZ = savedItem[1];
                        sScale = savedItem[2];
                        if (x == sX && z == sZ)
                        {
                            // Remove item from the list
                            smartBarGrid.Remove(savedItem);
                            isSaved = true;
                            break;
                        }
                    }
                }
                // Instantiate a clone
                Vector3 pos = new Vector3(startX, 0, startZ) + new Vector3(x, 0, z) * spacing;
                GameObject clone = Instantiate(prefabSmartBar, pos, Quaternion.identity);
                // Put clone in the same group as prefab's
                clone.transform.parent = prefabSmartBar.transform.parent;
                clone.SetActive(true);

                // Set x and z to object for further reference
                clone.GetComponent<SmartBarBase>().GridIndexX = x;
                clone.GetComponent<SmartBarBase>().GridIndexZ = z;

                // Apply saved scale if it exists
                if (isSaved)
                {
                    clone.GetComponent<SmartBarBase>().TargetScale = sScale;
                }
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

    public void SaveSmartBarGrid()
    {
        if (smartBarGrid == null)
        {
            smartBarGrid = new List<int[]>();
        }
        
        GameObject[] smartBarBases = GameObject.FindGameObjectsWithTag("SmartBarBase");
        foreach (GameObject sbb in smartBarBases)
        {

            float targetScale = sbb.GetComponent<SmartBarBase>().TargetScale;
            float origScale = sbb.GetComponent<SmartBarBase>().OrigScale;
            
            if (targetScale > origScale)
            {
                int x = sbb.GetComponent<SmartBarBase>().GridIndexX;
                int z = sbb.GetComponent<SmartBarBase>().GridIndexZ;
                int[] coord = new int[] { x, z, (int)targetScale };
                if (!smartBarGrid.Contains(coord))
                {
                    smartBarGrid.Add(coord);
                }
            }
        }


        // TODO: write list to JSON


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
