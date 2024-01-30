using UnityEngine;
using UnityEngine.UI;

public class ArmTable : MonoBehaviour {

    [Tooltip("A name of tag (defined in config-RR.json)")]
    public string tagSwitchReference = "SwitchReferenceRotate";
    [Tooltip("A name of tag (defined in config-RR.json)")]
    public string tagSwitchStep = "SwitchStepRotate";
    [Tooltip("A name of tag (defined in config-RR.json)")]
    public string tagDirection = "MotorRotateDirection";
    [Tooltip("A name of tag (defined in config-RR.json)")]
    public string tagMovement = "MotorRotateMovement";
    [Tooltip("Key of steps limit (defined in config-RR.json)")]
    public string strStepsLimit = "RotateStepsLimit";

    public Transform arm;
    public Transform switchReference;
    public Transform objWarningCCW;
    public Transform objWarningCW;
    public Transform objPosition; // object that defines position and is used to trigger pulses
    public Transform objRotationRef;
    public Transform objPositionEnd;
    public GameObject warningSign;
    
    int stepsLimit;
    Vector3 rotateCW;
    Vector3 vCurr;
    Vector3 vEnd;

    Communication com;

    bool referenceSwitch, warningCCW, warningCW;
    
    int switchReferenceValue, switchReferenceNewValue;    
    bool switchReferenceForceTrue, switchReferenceForceFalse;
    int switchStepValue, switchStepNewValue;
    bool switchStepForceTrue, switchStepForceFalse;

    float limits_angle;
    float unitPulseAngle;
    float currentAngle;
    int pulseCellCurrent;
    int pulseCellOld;
    float PLCCycle; // target cycle of the PLC in seconds
    readonly float speed_factor = 1.0f; // Horizontal speed between 1 (i.e. max speed) and 0.1 (min. speed)
    readonly int framesPerUnitAngle = 2; // each frame, move by unit/framesPerUnitDist
    bool pulseState = false;
    bool pulseStateOld = false;
    float timeHigh = 0.0f;
    float timeLow = 0.0f;
    float dt;
    bool allowedToMove = true;

    // Initialization
    void Awake()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();
        
        PLCCycle = float.Parse(com.appConfig.TrainingModelSpecific["PLCCycle"]);
        stepsLimit = int.Parse(com.appConfig.TrainingModelSpecific["RotateStepsLimit"]); // number of pulses on the distance between arm and the limit
        
        referenceSwitch = false;
        warningCCW = false;
        warningCW = false;
        warningSign.SetActive(false);

        switchReferenceValue = -1;
        switchReferenceForceTrue = false; 
        switchReferenceForceFalse = false;

        switchStepValue = -1;
        switchStepForceTrue = false;
        switchStepForceFalse = false;

        // Angle between the limit and starting position of the table (the smaller angle - ie 90 degrees)
        vCurr = objPosition.position - objRotationRef.position;
        vEnd = objPositionEnd.position - objRotationRef.position;

        limits_angle = Vector3.Angle(vCurr, vEnd) + 180;
        unitPulseAngle = limits_angle / (stepsLimit + 1); // length of one pulse cell
        currentAngle = limits_angle; // we are at full distance from limit
        pulseCellCurrent = stepsLimit + 1;
        pulseCellOld = pulseCellCurrent;

        // Vector for rotation in y axis
        rotateCW = new Vector3(0, speed_factor * unitPulseAngle / framesPerUnitAngle, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == switchReference)
        {
            referenceSwitch = true;
            Debug.Log("Rotate reference reached.");
        }
        if (other.transform == objWarningCCW)
        {
            warningCCW = true;
            Debug.Log("Reached the CCW limit");
        }
        if (other.transform == objWarningCW)
        {
            warningCW = true;
            Debug.Log("Reached the CW limit");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == switchReference)
        {
            referenceSwitch = false;
        }
        if (other.transform == objWarningCCW)
        {
            warningCCW = false;
        }
        if (other.transform == objWarningCW)
        {
            warningCW = false;
        }
    }  


    void RotateCW()
    {
        if (!warningCW) {
            arm.Rotate(rotateCW);
        }
    }

    void RotateCCW()
    {
        if (!warningCCW)
        {
            arm.Rotate(-rotateCW);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // Show/hide danger sign if off limits
        if (warningCW || warningCCW) 
        {
            warningSign.SetActive (true);
        }
        else
        {
            warningSign.SetActive (false);
        }
        
        // Compute state of step switch (movement impulse)
        // Duration of a previous frame
        dt = Time.deltaTime;

        // Update time counters
        if (pulseState) {
            // positive edge - reset 
            if (!pulseStateOld) {
                timeHigh = 0.0f;
                pulseStateOld = true;
            }
            // pulse is high
            timeHigh += dt;
        } 
        else {
            // negative edge - reset 
            if (pulseStateOld) {
                timeLow = 0.0f;
                pulseStateOld = false;
            }
            // pulse is low
            timeLow += dt;
        }
        
        // Check for pulse triggering
        vCurr = objPosition.position - objRotationRef.position;
        currentAngle = Vector3.Angle (vCurr, vEnd);
        
        if (vCurr.x > 0)
            currentAngle = 360 - currentAngle;
        // Legal angles lie between 0 and 270. If table turns slightly too much, there is jump 
        // to ~360 degrees. Consider some safe margins
        if (currentAngle > 300)
            currentAngle = 0;
        if (currentAngle > 270 && currentAngle < 280)
            currentAngle = 270;

        //Debug.Log("curr_angle: " + curr_angle);

        pulseCellCurrent = Mathf.FloorToInt(currentAngle / unitPulseAngle);

        // Detect cell change - the reference object has entered different cell
        if (pulseCellCurrent != pulseCellOld) {
            
            // Check if pulse was made right for previous cell: pulse has to be low long enough
            if (!pulseState && timeLow >= PLCCycle) {
                
                // Allow movement
                allowedToMove = true;
                // Update current pulse cell
                pulseCellOld = pulseCellCurrent;

                // Trigger new pulse
                pulseState = true;

            } 
            else {
                allowedToMove = false;
                
                if (pulseState && timeHigh >= PLCCycle) {
                    // We have to wait for low still
                    pulseState = false;
                }
            }
        } 
        else {
            // Cell not changed - ensure pulse width
            if(pulseState){
                // pulse high
                if(timeHigh >= PLCCycle){
                    // set pulse to low
                    pulseState = false;
                }
            }
        }

        // Consider forced values for reference switch and impulse
        switchReferenceNewValue = referenceSwitch ? 1 : 0;
        WriteOnChange(tagSwitchReference, switchReferenceValue, switchReferenceNewValue, switchReferenceForceTrue, switchReferenceForceFalse);
        switchReferenceValue = switchReferenceNewValue;

        switchStepNewValue = pulseState ? 1 : 0;
        WriteOnChange(tagSwitchStep, switchStepValue, switchStepNewValue, switchStepForceTrue, switchStepForceFalse);
        switchStepValue = switchStepNewValue;

        // Movement control
        if (allowedToMove) {
            if (com.GetTagValue(tagMovement)) {
                if (com.GetTagValue(tagDirection)) {
                    RotateCCW ();
                } else {
                    RotateCW ();
                }
            }
        }
    }

    void WriteOnChange(string tag, int sensorValue, int newValue, bool forceTrue, bool forceFalse)
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
    public void SwitchReferenceForceTrueOnChange(Toggle change)
    {
        Debug.Log($"{tagSwitchReference}, {change.isOn}, {change.name}, {change.group.name}");

        switchReferenceForceTrue = change.isOn;
        // Write true to PLC if isOn = true
        // Write value to PLC if isOn = false
        int val = 1;
        if (!change.isOn)
        {
            val = switchReferenceValue;
        }
        com.WriteToPlc(tagSwitchReference, val);
    }

    public void SwitchReferenceForceFalseOnChange(Toggle change)
    {
        Debug.Log($"{tagSwitchReference}, {change.isOn}, {change.name}, {change.group.name}");

        switchReferenceForceFalse = change.isOn;
        // Write false to PLC if isOn = true
        // Write value to PLC if isOn = false
        int val = 0;
        if (!change.isOn)
        {
            val = switchReferenceValue;
        }
        com.WriteToPlc(tagSwitchReference, val);
    }

    public void SwitchStepForceTrueOnChange(Toggle change)
    {
        Debug.Log($"{tagSwitchStep}, {change.isOn}, {change.name}, {change.group.name}");

        switchStepForceTrue = change.isOn;
        // Write true to PLC if isOn = true
        // Write value to PLC if isOn = false
        int val = 1;
        if (!change.isOn)
        {
            val = switchStepValue;
        }
        com.WriteToPlc(tagSwitchStep, val);
    }

    public void SwitchStepForceFalseOnChange(Toggle change)
    {
        Debug.Log($"{tagSwitchStep}, {change.isOn}, {change.name}, {change.group.name}");

        switchStepForceFalse = change.isOn;
        // Write false to PLC if isOn = true
        // Write value to PLC if isOn = false
        int val = 0;
        if (!change.isOn)
        {
            val = switchStepValue;
        }
        com.WriteToPlc(tagSwitchStep, val);
    }

}
