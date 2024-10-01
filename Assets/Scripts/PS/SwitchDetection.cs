using UnityEngine;
using UnityEngine.UI;

public class SwitchDetection : MonoBehaviour
{
    private Communication com;
    private int sensorValue, newValue;
    [Tooltip("String with a tag name. If None, the name of the game object will be used.")]
    public string sensorTag = null;
    private bool forceTrue, forceFalse;
    
    void Start()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();

        if (string.IsNullOrWhiteSpace(sensorTag))
        {
            sensorTag = this.name;
        }

        forceTrue = false;
        forceFalse = false;
        sensorValue = 0; // Initial sensor value
        com.WriteToPlc(sensorTag, sensorValue);
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is the one we're interested in
        if (other.gameObject.CompareTag("Workpiece") || other.gameObject.CompareTag("Miza_ograja") )
        {
            newValue = 1;
            // Check for change and forces, then write to PLC
            WriteOnChange(sensorTag, newValue);
            //UnityEngine.Debug.Log($"swithced pressed {sensorTag}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the object that left the trigger is the one we're interested in
        if (other.gameObject.CompareTag("Workpiece") || other.gameObject.CompareTag("Miza_ograja") )
        {
            newValue = 0;
            // Check for change and forces, then write to PLC
            WriteOnChange(sensorTag, newValue);
            //UnityEngine.Debug.Log($"swithced UN-pressed {sensorTag}");
        }
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
    public void SwitchForceTrueOnChange(Toggle change)
    {
        //Debug.Log($"tag: {sensorTag}, sensorValue: {sensorValue}, isOn: {change.isOn}, toggle: {change.name}, group: {change.group.name}");

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
       
        //Debug.Log($"tag: {sensorTag}, sensorValue: {sensorValue}, isOn: {change.isOn}, toggle: {change.name}, group: {change.group.name}");

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

