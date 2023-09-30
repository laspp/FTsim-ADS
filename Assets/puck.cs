using UnityEngine;
using System.Collections;

public class puck : MonoBehaviour {

    
	public float speed=1.3f;
	public Transform[] beltColider;
	public Transform pusher1, pusher2;
	   

    Communication com;

    Vector3[] directions = { new Vector3(0,0,1), new Vector3(-1,0,0) , new Vector3(1,0,0) , new Vector3(0,0,1) };
	    
    // Use this for initialization
    void Start () {
        com = GameObject.Find("Communication").GetComponent<Communication>();
    }
	
	// Update is called once per frame
	void Update () {

		// Pusher zone - disable rotations to stabilize movement
		if (transform.GetComponent<Renderer> ().bounds.Intersects (pusher1.GetComponent<Renderer> ().bounds) || transform.GetComponent<Renderer> ().bounds.Intersects (pusher2.GetComponent<Renderer> ().bounds)) {
			transform.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation;
		} else {
			transform.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationY;
		}

		// Belts - forces
        for (int i=0; i < 4; i++)
        {
			if (transform.GetComponent<Renderer>().bounds.Intersects(beltColider[i].GetComponent<Renderer>().bounds) && com.belt_run(i))
            {
                if (com.belt_direction(i))
                {
					transform.Translate(Time.deltaTime * speed * (- directions[i]));
                }
                else
                {
					transform.Translate (Time.deltaTime * speed * directions[i]);

                }
            }
        }
    }
}
