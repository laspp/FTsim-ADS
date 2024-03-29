using System.Collections;
using System.Collections.Generic;
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
    public string SwitchCompressor = "SwitchCompressor";

    public enum PistonLocation
    {
        Entry,
        Machine,
        Belt
    }

    [Tooltip("Select the type of piston")]
    public PistonLocation pistonType;
    string tagForward;
    string tagBackward;

    Vector3 moveVector = new(1, 0, 0);
    float speed = 0.5f;
    float position;
    float positionStart;
    float positionEnd;

    Communication com;

    // Start is called before the first frame update
    void Start()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();

        // Set positionEnd based on the selected piston type
        switch (pistonType)
        {
            case PistonLocation.Machine:
                positionEnd = positionStart + 1.65f;
                tagForward = tagValveMachine;
                break;
            case PistonLocation.Entry:
                positionEnd = positionStart + 1.412f;
                tagForward = tagValveEntryForward;
                tagBackward = tagValveEntryBackward;
                break;
            case PistonLocation.Belt:
                positionEnd = positionStart + 1.412f;
                tagForward = tagValveExitForward;
                tagBackward = tagValveExitBackward;
                break;
            //default:
            //    positionEnd = positionStart;
            //    break;
        }

        positionStart = transform.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log($"Compressor on: {com.GetTagValue(SwitchCompressor)}");
        // Movement control
        if (com.GetTagValue(SwitchCompressor))
        {
            if (com.GetTagValue(tagForward))
            {
                MoveForward();
            }
            else if (com.GetTagValue(tagBackward))
            {
                MoveBackward();
            }
        } //vedno je false
    }

    void MoveForward()
    {
        if (position < positionEnd)
        {
            transform.Translate(Time.fixedDeltaTime * speed * moveVector);
            position = transform.position.y;
        }
    }

    void MoveBackward()
    {
        if (position > positionStart)
        {
            transform.Translate(Time.fixedDeltaTime * speed * -moveVector);
            position = transform.position.y;
        }
    }

}
