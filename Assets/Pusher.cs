using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum PositionAxis
{
    x,
    y,
    z
}

public class Pusher : MonoBehaviour
{
    [Tooltip("A name of tag (defined in config.json)")]
    public string tagSwitchStart = "SwitchPusher#Start";
    [Tooltip("A name of tag (defined in config.json)")]
    public string tagSwitchEnd = "SwitchPusher#End";
    [Tooltip("A name of tag (defined in config.json)")]
    public string tagDirection = "Pusher#Direction";
    [Tooltip("A name of tag (defined in config.json)")]
    public string tagMovement = "Pusher#Movement";
    [Tooltip("How should direction be treated. If false, pusher moves forward when direction is 1.")]
    public bool reversePolarity = false;
    public float speed = 0.5f;
    [Tooltip("How much distance can pusher travel.")]
    public float travelDistance = 0.9f;
    [Tooltip("Do not show danger sign if that far away from end switch.")]
    public float dangerMargin = 0.1f; 
    [Tooltip("Stop movement if that far from end switch. Should be larger than danger margin.")]
    public float dangerOffset = 0.5f;
    [Tooltip("Object with danger sign. If None, child 'DangerSign' will be searched for.")]
    public GameObject dangerSign;

    public PositionAxis selectPositionAxis = new PositionAxis();
    public bool inverseDirection = false;

    Vector3 moveVector = new(1, 0, 0);

    Communication com;

    private float position;
    private float positionStart;
    private float positionEnd;
    
    private bool isOnStart = false;
    private bool isOnEnd = false;
    private bool dangerousPosition = false;

    private int switchStartValue, switchStartNewValue;
    private int switchEndValue, switchEndNewValue;
    private bool switchStartForceTrue, switchStartForceFalse;
    private bool switchEndForceTrue, switchEndForceFalse;

    // Use this for initialization
    void Awake()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();
        if (dangerSign == null)
        {
            dangerSign = this.transform.Find("DangerSign").gameObject;
            if (dangerSign == null)
            {
                Debug.LogError($"Pusher: error - parameter 'Danger Sign' is empty but cannot find child with name 'DangerSign'.");
            }
        }
        switchStartValue = -1; // It means "uninitialized"
        switchStartForceTrue = false;
        switchStartForceFalse = false;
        switchEndValue = -1;
        switchEndForceTrue = false;
        switchEndForceFalse = false;

        // Initial absolute position x of the object
        
        position = GetPosition();
        // Positions of start and end switches (imaginary)
        positionStart = position;
        positionEnd = positionStart + travelDistance;

    }
    float GetPosition() {

        switch (selectPositionAxis)
        {
            case PositionAxis.x:
                return transform.position.x * (inverseDirection ? -1 : 1);                
            case PositionAxis.y:
                return transform.position.y * (inverseDirection ? -1 : 1);                
            case PositionAxis.z:
                return transform.position.z * (inverseDirection ? -1 : 1);                
            default:
                return 0;
        }       

    }
    void MoveForward()
    {
        if (position < (positionEnd + dangerOffset))
        {
            transform.Translate(Time.fixedDeltaTime * speed * moveVector);
            position = GetPosition();
        }
    }

    void MoveBackward()
    {
        if (position > (positionStart - dangerOffset))
        {
            transform.Translate(Time.fixedDeltaTime * speed * -moveVector);
            position = GetPosition();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        isOnStart = position <= positionStart;
        isOnEnd = position >= positionEnd;
        dangerousPosition = (position < positionStart - dangerMargin) || (position > positionEnd + dangerMargin);

        switchStartNewValue = isOnStart ? 1 : 0;
        WriteOnChange(tagSwitchStart, switchStartValue, switchStartNewValue, switchStartForceTrue, switchStartForceFalse);
        switchStartValue = switchStartNewValue;

        switchEndNewValue = isOnEnd ? 1 : 0;
        WriteOnChange(tagSwitchEnd, switchEndValue, switchEndNewValue, switchEndForceTrue, switchEndForceFalse);
        switchEndValue = switchEndNewValue;

        if (dangerousPosition)
        {
            dangerSign.SetActive(true);
        }
        else
        {
            dangerSign.SetActive(false);
        }

        // Movement control
        if (com.GetTagValue(tagMovement))
        {
            if (com.GetTagValue(tagDirection) ^ reversePolarity)
            {
                MoveBackward();
            }
            else
            {
                MoveForward();
            }
        }
    }

    private void WriteOnChange(string tag, int sensorValue, int newValue, bool forceTrue, bool forceFalse)
    {
        if (sensorValue != newValue)
        {
            //  If both forces are inactive, write to PLC
            if (!(forceFalse || forceTrue))
            {
                com.WriteToPlc(tag, newValue);
            }
        }
    }
    public void SwitchStartForceTrueOnChange(Toggle change)
    {
        Debug.Log($"{tagSwitchStart}, {change.isOn}, {change.name}, {change.group.name}");

        switchStartForceTrue = change.isOn;
        // Write true to PLC if isOn = true
        // Write value to PLC if isOn = false
        int val = 1;
        if (!change.isOn)
        {
            val = switchStartValue;
        }
        com.WriteToPlc(tagSwitchStart, val);

    }
    public void SwitchStartForceFalseOnChange(Toggle change)
    {
        Debug.Log($"{tagSwitchStart}, {change.isOn}, {change.name}, {change.group.name}");

        switchStartForceFalse = change.isOn;
        // Write false to PLC if isOn = true
        // Write value to PLC if isOn = false
        int val = 0;
        if (!change.isOn)
        {
            val = switchStartValue;
        }
        com.WriteToPlc(tagSwitchStart, val);
    }
    public void SwitchEndForceTrueOnChange(Toggle change)
    {
        Debug.Log($"{tagSwitchEnd}, {change.isOn}, {change.name}, {change.group.name}");

        switchEndForceTrue = change.isOn;
        // Write true to PLC if isOn = true
        // Write value to PLC if isOn = false
        int val = 1;
        if (!change.isOn)
        {
            val = switchEndValue;
        }
        com.WriteToPlc(tagSwitchEnd, val);

    }
    public void SwitchEndForceFalseOnChange(Toggle change)
    {
        Debug.Log($"{tagSwitchEnd}, {change.isOn}, {change.name}, {change.group.name}");

        switchEndForceFalse = change.isOn;
        // Write false to PLC if isOn = true
        // Write value to PLC if isOn = false
        int val = 0;
        if (!change.isOn)
        {
            val = switchEndValue;
        }
        com.WriteToPlc(tagSwitchEnd, val);
    }
}