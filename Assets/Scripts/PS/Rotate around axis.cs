using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatearoundaxis : MonoBehaviour
{
    public float speed = 10f; // rotation speed

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
          // Rotate this object and all its children around their own axes
        // foreach (Transform child in transform)
        // {
        //     child.Rotate(Vector3.up, speed * Time.deltaTime);
        // }

        // // Rotate the parent object around its own axis
        // transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}
