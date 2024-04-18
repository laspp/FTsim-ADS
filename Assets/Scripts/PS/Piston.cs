using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Piston : MonoBehaviour
{
    [Tooltip("A name of tag (defined in config-PS.json)")]
    public string tagValveEntryForward = "ValveEntryForward";
    [Tooltip("A name of tag (defined in config-PS.json)")]
    public string tagValveEntryBackward = "ValveEntryBackward";
    [Tooltip("A name of tag (defined in config-PS.json)")]
    public string tagValveMachine = "ValveMachine";
    [Tooltip("A name of tag (defined in config-PS.json)")]
    public string tagValveExitForward = "ValveExitForward";
    [Tooltip("A name of tag (defined in config-PS.json)")]
    public string tagValveExitBackward = "ValveExitBackward";
    [Tooltip("A name of tag (defined in config-PS.json)")]
    public string MotorCompressor = "MotorCompressor";

    public enum PistonLocation
    {
        Entry,
        Machine,
        Belt
    }

    [Tooltip("Select the piston location")]
    public PistonLocation pistonType;
    string tagForward;
    string tagBackward;

    Vector3 moveVector = new(0, -1, 0);
    public float speed = 0.5f;
    float position;
    float positionStart;
    float positionEnd;

    Communication com;

    // Start is called before the first frame update
    void Start()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();

        position = transform.localPosition.y;

        // Set positionEnd based on the selected piston type
        switch (pistonType)
        {
            case PistonLocation.Entry:
                positionStart = position;
                positionEnd = position - 1.412f;
                tagForward = tagValveEntryForward;
                tagBackward = tagValveEntryBackward;
                break;
            case PistonLocation.Machine:
                positionStart = position;
                positionEnd = position - 1.65f;
                break;
            case PistonLocation.Belt:
                //start and End are reversed because piston is rotated compared to Entry piston
                positionStart = position + 1.412f;
                positionEnd = position;
                tagForward = tagValveExitForward;
                tagBackward = tagValveExitBackward;
                break;
            //default:
            //    positionEnd = positionStart;
            //    break;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        //UnityEngine.Debug.Log($"Compressor on: {com.GetTagValue(MotorCompressor)}");
        // Movement control
        if (com.GetTagValue(MotorCompressor))
        {
            if (com.GetTagValue(tagForward) || com.GetTagValue(tagValveMachine) && pistonType == PistonLocation.Machine)
            {
                MoveForward();
                UnityEngine.Debug.Log($"moving {pistonType} piston forward");
            }
            else if (com.GetTagValue(tagBackward) || !com.GetTagValue(tagValveMachine) && pistonType == PistonLocation.Machine)
            {
                MoveBackward();
                UnityEngine.Debug.Log($"moving {pistonType} piston backward");
            }
        }
    }

    void MoveForward()
    {
        if (position > positionEnd)
        {
            transform.Translate(Time.fixedDeltaTime * speed * moveVector);
            position = transform.localPosition.y;
            //UnityEngine.Debug.Log($"{position}");
        }
    }

    void MoveBackward()
    {
        if (position < positionStart)
        {
            transform.Translate(Time.fixedDeltaTime * speed * -moveVector);
            position = transform.localPosition.y;
            //UnityEngine.Debug.Log($"{position}");
        }
    }

}
