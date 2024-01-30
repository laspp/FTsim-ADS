using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ArmLift : MonoBehaviour
{

	public Transform arm;
	public Transform arm_switch_ref;
	public Transform arm_danger_up;
	public Transform arm_danger_down;
	public Transform toggle_ref_on;
	public Transform toggle_ref_off;
	public Transform toggle_imp_on;
	public Transform toggle_imp_off;
	public Transform arm_position;


	_Communication com;
	bool referenceSwitch, danger_up, danger_down;
	bool forceImpOn = false, forceImpOff = false;
	GameObject dangerSign;

	float speed_factor;
	int signals;
	Vector3 downVec;

	float limits_dist;
	float unit_pulse_dist;
	float curr_dist;
	int pulse_cell_curr;
	int pulse_cell_old;
	float PLC_cycle;
	// target cycle of the PLC in seconds
	int framesPerUnitDist = 2;
	// each frame, move by unit/framesPerUnitDist
	bool pulseState = false;
	bool pulseStateOld = false;
	float timeHigh = 0.0f;
	float timeLow = 0.0f;
	float dt;
	bool allowedToMove = true;


	void OnTriggerEnter (Collider other)
	{
		if (other.transform == arm_switch_ref) {
			referenceSwitch = true;
			//Debug.Log("vertical-Reached the switch");
		}
		if (other.transform == arm_danger_up) {
			danger_up = true;
			//Debug.Log("vertical-Danger up");
		}
		if (other.transform == arm_danger_down) {
			danger_down = true;
			//Debug.Log("vertical-Danger bottom");
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.transform == arm_switch_ref) {
			referenceSwitch = false;
		}
		if (other.transform == arm_danger_up) {
			danger_up = false;
		}
		if (other.transform == arm_danger_down) {
			danger_down = false;
		}
	}


	// Use this for initialization
	void Start ()
	{
		com = GameObject.Find ("Communication").GetComponent<_Communication> ();
		if (!com.paramsRead) {
			com.ReadParams ();
		}
		referenceSwitch = false;
		danger_up = false; 
		danger_down = false;
		dangerSign = GameObject.FindGameObjectWithTag ("Danger_lift");

		PLC_cycle = com.PLC_cycle;
		signals = com.vertical_signals; // number of pulses on the distance between arm and the limit
		speed_factor = com.vertical_speed;
		// Limit speed to 1 (i.e. max speed) and 0.1 (min. speed)
		if (speed_factor > 1.0f)
			speed_factor = 1.0f;
		if (speed_factor < 0.1f)
			speed_factor = 0.1f;

		// Distance between the limit and starting position of the arm
		limits_dist = Vector3.Distance (arm_danger_down.position, arm_position.position);
		unit_pulse_dist = limits_dist / (signals + 1); // length of one pulse cell
		curr_dist = limits_dist; // we are at full distance from limit
		pulse_cell_curr = signals + 1; // Mathf.FloorToInt(curr_dist / unit_pulse_dist);
		pulse_cell_old = pulse_cell_curr;

		// Vector for movement - in y axis
		downVec = new Vector3 (0, 0, speed_factor * unit_pulse_dist / framesPerUnitDist);
	}

	void go_down ()
	{
		if (!danger_down) {
			arm.Translate (downVec);
		}
	}

	void go_up ()
	{
		if (!danger_up) {
			arm.Translate (-downVec);
		}
	}



	// Update is called once per frame
	void Update ()
	{
		if (danger_up || danger_down) {
			dangerSign.SetActive (true);
		} else {
			dangerSign.SetActive (false);
		}

		// Consider forced values for reference and impulse
		if (toggle_ref_on.GetComponent<Toggle> ().isOn)
			com.lift_ref (true);
		else if (toggle_ref_off.GetComponent<Toggle> ().isOn)
			com.lift_ref (false);
		else
			com.lift_ref (referenceSwitch);

		forceImpOn = toggle_imp_on.GetComponent<Toggle> ().isOn;
		forceImpOff = toggle_imp_off.GetComponent<Toggle> ().isOn;

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
		} else {
			// negative edge - reset 
			if (pulseStateOld) {
				timeLow = 0.0f;
				pulseStateOld = false;
			}
			// pulse is low
			timeLow += dt;
		}

		// Check for pulse triggering
		curr_dist = Vector3.Distance (arm_position.position, arm_danger_down.position);
		pulse_cell_curr = Mathf.FloorToInt (curr_dist / unit_pulse_dist);

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

			} else {
				allowedToMove = false;
				if (pulseState && timeHigh >= PLC_cycle) {
					// We have to wait for low still
					pulseState = false;
				}
			}
		} else {
			// Cell not changed - ensure pulse width
			if (pulseState) {
				// pulse high
				if (timeHigh >= PLC_cycle) {
					// set pulse to low
					pulseState = false;
				}
			}
		}

		// Apply forced/computed values to impulse state
		if (forceImpOn)
			com.lift_imp_a (true);
		else if (forceImpOff)
			com.lift_imp_a (false);
		else
			com.lift_imp_a (pulseState);

		if (allowedToMove) {
			if (com.lift_run())
			{
				if (com.lift_dir())
				{
					go_down();
				}
				else
				{
					go_up();
				}
			}
		}
	}
}