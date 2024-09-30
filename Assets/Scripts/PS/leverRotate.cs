using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class leverRotate : MonoBehaviour
{
    private float rotationX = 0f;
    private bool isAnimating = false;
    public int speed = 80;
    public Communication com;
    public string SwitchCompressorTag = "SwitchCompressor";
    private int sensorValue;
    //private bool forceTrue, forceFalse;
    
    void Start()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();
        com.WriteToPlc(SwitchCompressorTag, false);
    }

    public void SwitchCompressorChange(Toggle change)
    {
        Debug.Log("SwitchCompressorChange: " + change.isOn);
        com.WriteToPlc(SwitchCompressorTag, change.isOn);
        if (change && !isAnimating)
        {
            StartCoroutine(RotateLever(change.isOn));
        }
    }

    IEnumerator RotateLever(bool rotateUp)
    {
        Debug.Log("RotateLever: " + rotateUp);
        isAnimating = true;
        if (rotateUp)
        {
            while (rotationX < 130f)
            {
                rotationX += Time.deltaTime * speed;
                rotationX = Mathf.Clamp(rotationX, 0f, 130f);
                transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                yield return null;
            }
        }
        else
        {
            while (rotationX > 0f)
            {
                rotationX -= Time.deltaTime * speed;
                rotationX = Mathf.Clamp(rotationX, 0f, 130f);
                transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                yield return null;
            }
        }
        isAnimating = false;
    }

    public void SwitchForceTrueOnChange(Toggle change)
    {
        Debug.Log($"{SwitchCompressorTag}, {change.isOn}, {change.name}, {change.group.name}");

        //forceTrue = change.isOn;
        // Write true to PLC if isOn = true
        // Write value to PLC if isOn = false
        int val = 1;
        if (!change.isOn)
        {
            val = sensorValue;
        }
        com.WriteToPlc(SwitchCompressorTag, val);

    }
    public void SwitchForceFalseOnChange(Toggle change)
    {
        Debug.Log($"{SwitchCompressorTag}, {change.isOn}, {change.name}, {change.group.name}");

        //forceFalse = change.isOn;
        // Write false to PLC if isOn = true
        // Write value to PLC if isOn = false
        int val = 0;
        if (!change.isOn)
        {
            val = sensorValue;
        }
        com.WriteToPlc(SwitchCompressorTag, val);
    }
}