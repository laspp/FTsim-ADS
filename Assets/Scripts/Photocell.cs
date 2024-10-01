using UnityEngine;
using UnityEngine.UI;

public class Photocell : MonoBehaviour
{
    [Tooltip("Detector plate A (start) of photocell. If None, child 'DetectorA' will be searched for.")]
    public Transform detectorA = null;
    [Tooltip("Detector plate B (start) of photocell. If None, child 'DetectorB' will be searched for.")]
    public Transform detectorB = null;
    [Tooltip("String with a tag name. If None, the name of the game object will be used.")]
    public string sensorTag = null;

    private Communication com;
    private GameObject[] workpieces;
    private Bounds bndWorkpiece;
    private Bounds boundsDetectorA, boundsDetectorB;
    private int sensorValue, newValue;
    private bool forceTrue, forceFalse;


    // Start is called before the first frame update
    void Awake()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();

        if (detectorA == null)
        {
            detectorA = this.transform.Find("DetectorA");
            if (detectorA == null)
            {
                Debug.LogError($"Photocell: error - parameter 'detectorA' is empty but cannot find child with name 'DetectorA'.");
            }
        }

        if (detectorB == null)
        {
            detectorB = this.transform.Find("DetectorB");
            if (detectorB == null)
            {
                Debug.LogError($"Photocell: error - parameter 'detectorB' is empty but cannot find child with name 'DetectorB'.");
            }
        }

        if (string.IsNullOrWhiteSpace(sensorTag))
        {
            sensorTag = this.name;
        }

        sensorValue = -1;
        forceTrue = false;
        forceFalse = false;
        boundsDetectorA = detectorA.GetComponent<Renderer>().bounds;
        boundsDetectorB = detectorB.GetComponent<Renderer>().bounds;

    }

    // Update is called once per frame
    void Update()
    {
        // Handle interactions between workpieces and photocells
        workpieces = GameObject.FindGameObjectsWithTag("Workpiece");

        newValue = 1;

        foreach (GameObject workpiece in workpieces)
        {
            bndWorkpiece = workpiece.GetComponent<Renderer>().bounds;

            if (bndWorkpiece.Intersects(boundsDetectorA) && bndWorkpiece.Intersects(boundsDetectorB))
            {
                newValue = 0;
            }
        }
        // Check for change and forces, then write to PLC
        WriteOnChange(sensorTag, newValue);
    }

    private void WriteOnChange(string tag, int newValue)
    {
        if (sensorValue != newValue)
        {
            //  If both forces are inactive, write to PLC
            if (!(forceFalse || forceTrue))
            {
                com.WriteToPlc(tag, newValue);
            }
            sensorValue = newValue;
        }
    }

    public void ForceTrueOnChange(Toggle change)
    {
        //Debug.Log($"{sensorTag}, {change.isOn}, {change.name}, {change.group.name}");

        forceTrue = change.isOn;
        // Write true to PLC if isOn = true
        // Write value to PLC if isOn = false
        int val = 1;
        if (!change.isOn)
        {
            val = sensorValue;
        }
        com.WriteToPlc(sensorTag, val);

    }
    public void ForceFalseOnChange(Toggle change)
    {
        //Debug.Log($"{sensorTag}, {change.isOn}, {change.name}, {change.group.name}");

        forceFalse = change.isOn;
        // Write false to PLC if isOn = true
        // Write value to PLC if isOn = false
        int val = 0;
        if (!change.isOn)
        {
            val = sensorValue;
        }
        com.WriteToPlc(sensorTag, val);
    }

}
