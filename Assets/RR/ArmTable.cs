using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class ArmTable : MonoBehaviour {

    public Transform table;
    public Transform table_switch_ref;
    public Transform table_danger_CCW;
    public Transform table_danger_CW;
    public Transform toggle_ref_on;
    public Transform toggle_ref_off;
    public Transform toggle_imp_on;
    public Transform toggle_imp_off;
    public Transform table_position; // object that defines position and is used to trigger pulses
    public Transform table_rotation_ref;
    public Transform table_position_end;
    public GameObject warningSign;

    float speed_factor;
    int signals;
    Vector3 rotateCW;
    Vector3 vCurr;
    Vector3 vEnd;

    _Communication com;
    bool referenceSwitch, danger_ccw, danger_cw;
    bool forceImpOn=false, forceImpOff=false;
    

    float limits_angle;
    float unit_pulse_angle;
    float curr_angle;
    int pulse_cell_curr;
    int pulse_cell_old;
    float PLC_cycle; // target cycle of the PLC in seconds
    int framesPerUnitAngle = 2; // each frame, move by unit/framesPerUnitDist
    bool pulseState = false;
    bool pulseStateOld = false;
    float timeHigh = 0.0f;
    float timeLow = 0.0f;
    float dt;
    bool allowedToMove = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == table_switch_ref)
        {
            referenceSwitch = true;
            //Debug.Log("Table reference reached.");
        }
        if (other.transform == table_danger_CCW)
        {
            danger_ccw = true;
            //Debug.Log("Reached the CCW limit");
        }
        if (other.transform == table_danger_CW)
        {
            danger_cw = true;
            //Debug.Log("Reached the CW limit");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == table_switch_ref)
        {
            referenceSwitch = false;
        }
        if (other.transform == table_danger_CCW)
        {
            danger_ccw = false;
        }
        if (other.transform == table_danger_CW)
        {
            danger_cw = false;
        }
    }  

    // Use this for initialization
    void Start()
    {  
        com = GameObject.Find("Communication").GetComponent<_Communication>();
        if (!com.paramsRead) {
            com.ReadParams ();
        }

        PLC_cycle = com.PLC_cycle;
        signals = com.table_signals; // number of pulses on the distance between arm and the limit
        speed_factor = com.table_speed;
        // Limit horizontal_speed to 1 (i.e. max speed) and 0.1 (min. speed)
        if (speed_factor > 1.0f){
            speed_factor = 1.0f;
        }
        if (speed_factor < 0.1f) {
            speed_factor = 0.1f;
        }
        
        referenceSwitch = false;
        danger_ccw = false; 
        danger_cw = false;
        //warningSign = GameObject.FindGameObjectWithTag ("Danger_table");
        warningSign.SetActive (false);

        // Angle between the limit and starting position of the table (the smaller angle - ie 90 degrees)
        vCurr = table_position.position - table_rotation_ref.position;
        vEnd = table_position_end.position - table_rotation_ref.position;

        limits_angle = Vector3.Angle(vCurr, vEnd)+180;
        unit_pulse_angle = limits_angle / (signals+1); // length of one pulse cell
        curr_angle = limits_angle; // we are at full distance from limit
        pulse_cell_curr = signals+1;
        pulse_cell_old = pulse_cell_curr;

        // Vector for rotation in y axis
        rotateCW = new Vector3(0, speed_factor * unit_pulse_angle/framesPerUnitAngle, 0);
    }

    void rotate_CW()
    {
        if (!danger_cw) {
            table.Rotate(rotateCW);
        }
    }

    void rotate_CCW()
    {
        if (!danger_ccw)
        {
            table.Rotate(-rotateCW);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // Show/hide danger sign if off limits
        if (danger_cw || danger_ccw) 
        {
            warningSign.SetActive (true);
        }
        else
        {
            warningSign.SetActive (false);
        }
        
        // Consider forced values for reference and impulse
        if (toggle_ref_on.GetComponent<Toggle>().isOn)
            com.table_ref(true);
        else if(toggle_ref_off.GetComponent<Toggle>().isOn)
            com.table_ref(false);
        else
            com.table_ref(referenceSwitch);
        
        forceImpOn = toggle_imp_on.GetComponent<Toggle>().isOn;
        forceImpOff = toggle_imp_off.GetComponent<Toggle>().isOn;
        
        
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
        vCurr = table_position.position - table_rotation_ref.position;
        curr_angle = Vector3.Angle (vCurr, vEnd);
        
        if (vCurr.x > 0)
            curr_angle = 360 - curr_angle;
        // Legal angles lie between 0 and 270. If table turns slightly too much, there is jump 
        // to ~360 degrees. Consider some safe margins
        if (curr_angle > 300)
            curr_angle = 0;
        if (curr_angle > 270 && curr_angle < 280)
            curr_angle = 270;

        //Debug.Log("curr_angle: " + curr_angle);

        pulse_cell_curr = Mathf.FloorToInt(curr_angle / unit_pulse_angle);

        // Detect cell change - the reference object has entered different cell
        if (pulse_cell_curr != pulse_cell_old) {
            
            // Check if pulse was made right for previous cell: pulse has to be low long enough
            if (!pulseState && timeLow >= PLC_cycle) {
                
                // Allow movement
                allowedToMove = true;
                // Update current pulse cell
                pulse_cell_old = pulse_cell_curr;

                // Trigger new pulse
                pulseState = true;

            } 
            else {
                allowedToMove = false;
                
                if (pulseState && timeHigh >= PLC_cycle) {
                    // We have to wait for low still
                    pulseState = false;

                }
            }
        } 
        else {
            // Cell not changed - ensure pulse width
            if(pulseState){
                // pulse high
                if(timeHigh >= PLC_cycle){
                    // set pulse to low
                    pulseState = false;
                }
            }
        }

        // Apply forced/computed values to impulse state
        if (forceImpOn)
            com.table_imp_a(true);
        else if(forceImpOff)
            com.table_imp_a(false);
        else
            com.table_imp_a(pulseState);
        

        if (allowedToMove) {
            if (com.table_run ()) {
                if (com.table_dir ()) {
                    rotate_CCW ();
                } else {
                    rotate_CW ();
                }
            }
        }
    }

}
