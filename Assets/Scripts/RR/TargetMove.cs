using UnityEngine;
using System.Collections;

public class TargetMove : MonoBehaviour {

	Vector3 posHolder, pos;
	private GameObject holder;
	// Use this for initialization
	void Start () {
		holder = GameObject.FindGameObjectWithTag ("Holder");
		pos = transform.position;
		posHolder = holder.transform.position;
	}

	// Update is called once per frame
	void Update () {
		posHolder = holder.transform.position;
		//pos.x = posHolder.x;
		//pos.z = posHolder.z;
		//transform.position = pos;
		transform.position = posHolder;
	}
}
