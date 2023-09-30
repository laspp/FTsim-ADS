using UnityEngine;
using System.Collections;

public class rotateLight : MonoBehaviour {

	public float speed = 100;
	private Transform obj;
	// Use this for initialization
	void Start () {
		obj = this.transform;
	}

	// Update is called once per frame
	void Update () {
		obj.Rotate (new Vector3 (0, Time.deltaTime * speed, 0));
	}
}
