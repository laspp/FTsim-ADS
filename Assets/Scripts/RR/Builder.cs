using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Builder : MonoBehaviour
{
    public Communication communication;
    public StatusBar panelStatusBar;
    public GameObject buttonReset;
    public GameObject buttonSave;
    public GameObject toggleRespawn;
    public GameObject prefabSmartBar;    
    public int gridX = 10;
    public int gridZ = 16;
    public float spacing = 0.65f;
    public int offsetX = -8;
    public int offsetZ = -3;
        
    float startX;
    float startZ;
    Button btnSave, btnReset;
    Toggle tglRespawn;
    List<int[]> smartBarGrid;
    bool isSaved = false;
    int sX, sZ, sScale = 1;

    bool builderMode;
    public bool BuilderMode { get => builderMode; set => builderMode = value; }

    bool spawnMode;
    public bool RespawnMode { get => spawnMode; set => spawnMode = value; }

    // Start is called before the first frame update
    void Awake()
    {
        smartBarGrid = communication.appConfig.SmartBarGrid;

        BuilderMode = false;
        RespawnMode = false;
        btnSave = buttonSave.GetComponent<Button>();
        btnReset = buttonReset.GetComponent<Button>();
        tglRespawn = toggleRespawn.GetComponent<Toggle>();
        btnSave.interactable = false;
        btnReset.interactable = false;
        tglRespawn.interactable = false;

        startX = prefabSmartBar.transform.position.x + offsetX * spacing;
        startZ = prefabSmartBar.transform.position.z + offsetZ * spacing;
    }

    void Start()
    {
        prefabSmartBar.SetActive(false);
        // Create a copy of smartBarGrid (we will remove items from it)
        List<int[]> smartBarGridCopy = DeepCopy(smartBarGrid);

        for (int z = 0; z < gridZ; z++)
        {
            for (int x = 0; x < gridX; x++)
            {
                if (smartBarGridCopy != null)
                {
                    // Check if index exists in smartBarGrid and use its props
                    isSaved = false;
                    foreach (var savedItem in smartBarGridCopy)
                    {
                        sX = savedItem[0];
                        sZ = savedItem[1];
                        sScale = savedItem[2];
                        if (x == sX && z == sZ)
                        {
                            // Remove item from the list
                            smartBarGridCopy.Remove(savedItem);
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

                // Is a default respawn point?
                if (x == -offsetX && z == -offsetZ) {
                    Transform child = clone.transform.GetChild(0);
                    child.gameObject.GetComponent<SmartBar>().SetRespawnPoint();
                }
            }
        }
    }
    static List<int[]> DeepCopy(List<int[]> original)
    {
        List<int[]> copy = new List<int[]>(original.Count);
        foreach (var array in original)
        {
            int[] newArray = new int[array.Length];
            System.Array.Copy(array, newArray, array.Length);
            copy.Add(newArray);
        }
        return copy;
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

    public void ClearSmartBarCollisions()
    {
        GameObject[] smartBarBases = GameObject.FindGameObjectsWithTag("SmartBarBase");
        Transform child;
        foreach (GameObject sbb in smartBarBases)
        {
            child = sbb.transform.GetChild(0);
            child.gameObject.GetComponent<SmartBar>().ResetCollisionDetected();
        }
    }

    public void SaveSmartBarGrid()
    {
        // Clear the list and  populate it based on current state of smartBars.
        smartBarGrid.Clear();

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

                AddIfNotExists(smartBarGrid, coord);
            }
        }

        // Write the list to JSON config file
        communication.ConfigFileSave();

    }
    // Method to add a new item to the list only if it doesn't already exist
    static void AddIfNotExists(List<int[]> list, int[] newItem)
    {
        // Check if the new item already exists in the list
        if (!list.Any(item => item.SequenceEqual(newItem)))
        {
            list.Add(newItem);
        }
    }

    public void ToggleModeOnChange()
    {
        BuilderMode = !BuilderMode;
        if (BuilderMode)
        {
            btnSave.interactable = true;
            btnReset.interactable = true;
            tglRespawn.interactable = true;
            panelStatusBar.SetStatusBarText("Utilize the left and right mouse buttons to manipulate the terrain.");
        }
        else
        {
            btnSave.interactable = false;
            btnReset.interactable = false;
            tglRespawn.interactable = false;
        }
    }
    public void ToggleRepawnOnChange()
    {
        RespawnMode = !RespawnMode;

        if (RespawnMode)
        {
            panelStatusBar.SetStatusBarText("Click on a terrain block to set it as the respawn point for workpieces.");
        }
    }
}
