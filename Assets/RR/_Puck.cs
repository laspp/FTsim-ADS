using UnityEngine;
using System.Collections;

public class _Puck : MonoBehaviour {

	public Transform handLeft;
    public Transform handLeft_grab_area;
    public Transform handRight_grab_area;
    public Transform holder;

    bool leftTouching, rightTouching, grabbed;

	void OnTriggerEnter(Collider other){
		if (other.transform == handRight_grab_area)
		{
			rightTouching = true;
			//Debug.Log("rightTouching.");
		}
		if (other.transform == handLeft_grab_area)
		{
			leftTouching = true;
			//Debug.Log("leftTouching.");
		}
	}
	void OnTriggerExit(Collider other){
		if (other.transform == handRight_grab_area)
		{
			rightTouching = false;
			//Debug.Log("not rightTouching.");
		}
		if (other.transform == handLeft_grab_area)
		{
			leftTouching = false;
			//Debug.Log("not leftTouching.");
		}
	}

    	
	void imGrabbed(){
		GameObject a = GameObject.FindWithTag ("hnd");
		a.GetComponent<ArmHand>().puckGrabbed();
	}
	
	void imFree(){
		GameObject a = GameObject.FindWithTag ("hnd");
		a.GetComponent<ArmHand>().puckFree();
	}

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (leftTouching && rightTouching)
        {
            transform.GetComponent<Rigidbody>().isKinematic = true;
            transform.GetComponent<Rigidbody>().useGravity = false;
            handLeft.GetComponent<ArmHand>().grabbed = transform;
			imGrabbed();
        }


        if (handLeft.GetComponent<ArmHand>().grabbed == transform)
        {
            transform.position = holder.position;
        }
        else
        {
			if(transform.GetComponent<Rigidbody>().isKinematic)
				imFree();
            transform.GetComponent<Rigidbody>().useGravity = true;
            transform.GetComponent<Rigidbody>().isKinematic = false;
            transform.parent = null;
        }

    }
}
