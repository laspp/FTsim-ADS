using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAxis : MonoBehaviour
{

    [Tooltip("A name of tag (defined in config.json)")]
    public string tagDirection = "Belt#Direction";
    [Tooltip("A name of tag (defined in config.json)")]
    public string tagMovement = "Belt#Movement";
    public float motorAxisSpeed = 155;


    private Communication com;

    // Start is called before the first frame update
    void Start()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();

    }

    // Update is called once per frame
    void Update()
    {
        if (com.GetTagValue(tagMovement))
        {
            if (com.GetTagValue(tagDirection))
            {
                transform.Rotate(new Vector3(0, 0, Time.deltaTime * motorAxisSpeed));
            }
            else
            {
                transform.Rotate(new Vector3(0, 0, -Time.deltaTime * motorAxisSpeed));
            }
        }
    }
}
