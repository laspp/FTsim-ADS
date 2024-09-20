using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Piston : MonoBehaviour
{
    Communication com;
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
    string tagForward;
    string tagBackward;

    public enum PistonLocation
    {
        Entry,
        Machine,
        Belt
    }
    [Tooltip("Select the piston location")]
    public PistonLocation pistonType;

    Vector3 moveVector = new(0, -0.5f, 0);
    public float speed = 0.01f;
    float position;
    float positionStart;
    float positionEnd;

    private bool callEventForward = true;
    private bool callEventBackward = true;

    private AirPressureController airPressureController;

    // Start is called before the first frame update
    void Start()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();
        airPressureController = GameObject.Find("AirPressure").GetComponent<AirPressureController>();

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
                positionEnd = position - 1.80f;
                break;
            case PistonLocation.Belt:
                //start and End are reversed because piston is rotated compared to Entry piston
                positionStart = position + 1.422f;
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
        // if (com.GetTagValue(MotorCompressor))
        if (airPressureController.GetAirPressureLevel() > 0)
        {
            if (pistonType == PistonLocation.Machine) {
                if (com.GetTagValue(tagValveMachine)){
                    MoveForward();
                    UnityEngine.Debug.Log($"moving {pistonType} piston forward");
                } else {
                    MoveBackward();
                    UnityEngine.Debug.Log($"moving {pistonType} piston backward");
                }
            }
            else if (com.GetTagValue(tagForward))
            {
                MoveForward();
                UnityEngine.Debug.Log($"moving {pistonType} piston forward");
            }
            else if (com.GetTagValue(tagBackward))
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
            if (callEventForward)
            {
                callEventForward = false;
                //OnPistonMove?.Invoke(2);
                airPressureController.DecrementAirPressureLevel();
                UnityEngine.Debug.Log("cam forward event ");
            }
            //UnityEngine.Debug.Log($"{position}");
        } else if (Math.Abs(position - positionEnd) < 0.02f) {
            callEventForward = true;
        }
    }

    void MoveBackward()
    {
        if (position < positionStart)
        {
            transform.Translate(Time.fixedDeltaTime * speed * -moveVector);
            position = transform.localPosition.y;
            if (callEventBackward)
            {
                callEventBackward = false;
                //OnPistonMove?.Invoke(2);
                if (pistonType != PistonLocation.Machine){
                    airPressureController.DecrementAirPressureLevel();
                }
                UnityEngine.Debug.Log("cam backward  event");
            }
            //UnityEngine.Debug.Log($"{position}");
        } else if ( Math.Abs(position - positionStart) < 0.02f )
        {
            callEventBackward = true;
        }
    }



}
