using UnityEngine;
using UnityEngine.UI;

public class SwitchDetection : MonoBehaviour
{
    private Communication com;
    private int sensorValue;
    [Tooltip("String with a tag name. If None, the name of the game object will be used.")]
    public string sensorTag = null;
    private bool forceTrue, forceFalse;
    
    void Awake()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();

        if (string.IsNullOrWhiteSpace(sensorTag))
        {
            sensorTag = this.name;
        }

        sensorValue = -1; // Initial sensor value
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is the one we're interested in
        if (other.gameObject.CompareTag("Workpiece") || other.gameObject.CompareTag("Miza_ograja") )
        {
            sensorValue = 1; // Change sensor value
            com.WriteToPlc(sensorTag, sensorValue); // Write new value to PLC
            UnityEngine.Debug.Log($"swithced pressed {sensorTag}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the object that left the trigger is the one we're interested in
        if (other.gameObject.CompareTag("Workpiece") || other.gameObject.CompareTag("Miza_ograja") )
        {
            sensorValue = 0; // Change sensor value
            com.WriteToPlc(sensorTag, sensorValue); // Write new value to PLC
            UnityEngine.Debug.Log($"swithced UN-pressed {sensorTag}");
        }
    }

    public void SwitchForceTrueOnChange(Toggle change)
    {
        Debug.Log($"{sensorTag}, {change.isOn}, {change.name}, {change.group.name}");

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
    public void SwitchForceFalseOnChange(Toggle change)
    {
        Debug.Log($"{sensorTag}, {change.isOn}, {change.name}, {change.group.name}");

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

