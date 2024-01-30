using UnityEngine;
using System.Collections;

public class VirtualPlatformMove : MonoBehaviour {

	public float speedFull = 1.5f;
	public float speedStep = 0.05f;
	public float moveStep = 0.65f;
	public Transform table_bound;
	public float timeLimitMove = 3.0f;
	float watchDog = 0f;
	Vector3 pos = Vector3.zero;
	Vector3 pos_old = Vector3.zero;
	Vector3 dest = Vector3.zero;
	Vector3 desiredVelocity = Vector3.zero;
	Rigidbody rb;
	float lastSqrtMag, sqrtMag, speedCurr;
	bool isMoving;

	bool hasCollided = false;
	
	void Start () {
		isMoving = false;
		speedCurr = 0;
		rb = GetComponent<Rigidbody> ();
		lastSqrtMag = Mathf.Infinity;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.transform == table_bound) {
			hasCollided = true;
		}

	}
	void OnTriggerStay(Collider other)
	{
		if (other.transform == table_bound) {
			hasCollided = true;
		}

	}


	void Update() {
		pos = rb.position;

		if (!isMoving) {
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				pos_old = pos;
				dest = pos;
				dest.z += moveStep;
				desiredVelocity = (dest - pos).normalized;
				lastSqrtMag = Mathf.Infinity;
				isMoving = true;
				watchDog = 0;

			}
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				pos_old = pos;
				dest = pos;
				dest.z -= moveStep;
				desiredVelocity = (dest - pos).normalized;
				lastSqrtMag = Mathf.Infinity;
				isMoving = true;
				watchDog = 0;

			}
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				pos_old = pos;
				dest = pos;
				dest.x -= moveStep;
				desiredVelocity = (dest - pos).normalized;
				lastSqrtMag = Mathf.Infinity;
				isMoving = true;
				watchDog = 0;

			}
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				pos_old = pos;
				dest = pos;
				dest.x += moveStep;
				desiredVelocity = (dest - pos).normalized;
				lastSqrtMag = Mathf.Infinity;
				isMoving = true;
				watchDog = 0;
			}
			if (Input.GetKeyDown (KeyCode.PageUp)) {
				pos_old = pos;
				dest = pos;
				dest.y += moveStep;
				desiredVelocity = (dest - pos).normalized;
				lastSqrtMag = Mathf.Infinity;
				isMoving = true;
				watchDog = 0;

			}
			if (Input.GetKeyDown (KeyCode.PageDown)) {
				pos_old = pos;
				dest = pos;
				dest.y -= moveStep;
				desiredVelocity = (dest - pos).normalized;
				lastSqrtMag = Mathf.Infinity;
				isMoving = true;
				watchDog = 0;
			}
		} else {
			watchDog += Time.deltaTime;
		}

		sqrtMag = (dest - pos).sqrMagnitude;
		if (sqrtMag > lastSqrtMag) {
			dest = pos;
			desiredVelocity = Vector3.zero;
			isMoving = false;
			speedCurr = 0;
		}
		lastSqrtMag = sqrtMag;

		// collided! move back to old position
		if (hasCollided || watchDog > timeLimitMove) {
			dest = pos_old;
			desiredVelocity = (dest - pos).normalized;
			lastSqrtMag = Mathf.Infinity;
			isMoving = true;
			hasCollided = false;
			watchDog = 0;
			speedCurr = 0;
			sqrtMag = (dest - pos).sqrMagnitude;
		}

	}
	void FixedUpdate(){
		if (isMoving) {
			if (Mathf.Sqrt (sqrtMag) < moveStep * 0.5f) {
				speedCurr -= speedStep;
				speedCurr = Mathf.Max (speedCurr, speedFull*0.1f);
			} else {
				speedCurr += speedStep;
				speedCurr = Mathf.Min (speedCurr, speedFull);
			}


		}
		rb.velocity = desiredVelocity * speedCurr;
		//Debug.Log (speedCurr);
	}
}
