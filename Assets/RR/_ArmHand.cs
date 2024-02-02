using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class _ArmHand : MonoBehaviour
{

	public Transform[] cylinders;
	public Transform toggle_ref_on;
	public Transform toggle_ref_off;
	public Transform toggle_imp_on;
	public Transform toggle_imp_off;
	public Transform hand_switch_ref;
	public Transform hand_danger_open;
	public Transform handLeft;
	public Transform handRight;
	public Transform handCenter;
	public Transform hand_position;
	public Transform grabbed = null;

	_Communication com;
	Collider other;
	bool referenceSwitch, isPuckGrabbed, danger_close, danger_open;
	bool forceImpOn = false, forceImpOff = false;
	GameObject dangerSign;

	float speed_factor;
	int signals;
	Vector3 close_vec;
	Vector3 handLeftEdge;

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

	public void puckGrabbed ()
	{
		isPuckGrabbed = true;
		//Debug.Log("Puck is Grabbed.");
	}

	public void puckFree ()
	{
		isPuckGrabbed = false;
		//Debug.Log("Puck is not grabbed anymore.");
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.transform == hand_switch_ref) {
			referenceSwitch = true;
			//Debug.Log("hand-Reached the switch");
		}
		if (other.transform == hand_danger_open) {
			danger_open = true;
			//Debug.Log("Danger - fully opened");
		}	
	}

	void OnTriggerExit (Collider other)
	{
		if (other.transform == hand_switch_ref) {
			referenceSwitch = false;
		}
		if (other.transform == hand_danger_open) {
			danger_open = false;
		}
	}

	void OnCollisionEnter (Collision collision)
	{
		
		if (collision.transform == handRight) {
			danger_close = true;
			//Debug.Log("danger - hands together");
		}
		if (collision.gameObject.tag == "Player") {
			this.other = collision.collider;
		}
	}

	void OnCollisionExit (Collision collision)
	{		
		if (collision.transform == handRight) {
			danger_close = false;
		}
		if (collision.gameObject.tag == "Player") {
			this.other = null;
		}

	}

	// Use this for initialization
	void Start ()
	{
		danger_close = false;
		danger_open = false;
		referenceSwitch = false;
		isPuckGrabbed = false;

		com = GameObject.Find ("Communication").GetComponent<_Communication> ();
		dangerSign = GameObject.FindGameObjectWithTag ("Danger_hand");
		if (!com.paramsRead) {
			com.ReadParams ();
		}

		speed_factor = com.hand_speed;
		signals = com.hand_signals;
		PLC_cycle = com.PLC_cycle;
		// Limit speed_factor to 1 (i.e. max speed) and 0.1 (min. speed)
		if (speed_factor > 1.0f)
			speed_factor = 1.0f;
		if (speed_factor < 0.1f)
			speed_factor = 0.1f;

		
		// Distance between the limit and starting position of the arm
		limits_dist = Vector3.Distance (handCenter.position, hand_position.position);
		unit_pulse_dist = limits_dist / (signals + 3); // length of one pulse cell; add 3 to finetune target value
		curr_dist = limits_dist; // we are at full distance from limit
		pulse_cell_curr = signals + 3; // Mathf.FloorToInt(curr_dist / unit_pulse_dist);
		pulse_cell_old = pulse_cell_curr;

		// Vector for movement
		close_vec = new Vector3 (speed_factor * unit_pulse_dist / framesPerUnitDist, 0, 0);
	}

	void hand_close ()
	{
		if (!(danger_close || isPuckGrabbed)) {
			handRight.Translate (close_vec);
			handLeft.Translate (close_vec);
		}
	}

	void hand_open ()
	{
		if (!danger_open) {
			grabbed = null;
			handRight.Translate (-close_vec);
			handLeft.Translate (-close_vec);
		}
	}



	// Update is called once per frame
	void Update ()
	{
		if (!other) {
			isPuckGrabbed = false;
		}

		if (danger_open || danger_close) {
			dangerSign.SetActive (true);
		} else {
			dangerSign.SetActive (false);	
		}

		// Consider forced values for reference and impulse
		if (toggle_ref_on.GetComponent<Toggle> ().isOn)
			com.hand_ref (true);
		else if (toggle_ref_off.GetComponent<Toggle> ().isOn)
			com.hand_ref (false);
		else
			com.hand_ref (referenceSwitch);

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
		curr_dist = Vector3.Distance (handCenter.position, hand_position.position);
		pulse_cell_curr = Mathf.FloorToInt (curr_dist / unit_pulse_dist);
		//Debug.Log (pulse_cell_curr);
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
			com.hand_imp (true);
		else if (forceImpOff)
			com.hand_imp (false);
		else
			com.hand_imp (pulseState);

		if (allowedToMove) {
			if (com.hand_run ()) {
				if (com.hand_dir ()) {
					hand_close ();
				} else {					
					hand_open ();
				}
			}
		}
	}
}