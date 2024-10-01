using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkpieceMove : MonoBehaviour
{
    [Tooltip("A name of tag (defined in config.json)")]
    public string tagDirection = "Belt#Direction";
    [Tooltip("A name of tag (defined in config.json)")]
    public string tagMovement = "Belt#Movement";
    public float speed = 1.0f;
    public Vector3 direction = new Vector3(0, 0, 1);

    private Communication com;
    private GameObject[] workpieces;
    private Bounds bndWorkpiece;
    private Bounds bndForceField;

    

    // Start is called before the first frame update
    void Start()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();
        bndForceField = transform.GetComponent<Renderer>().bounds;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        workpieces = GameObject.FindGameObjectsWithTag("Workpiece");

        foreach (GameObject workpiece in workpieces)
        {
            bndWorkpiece = workpiece.GetComponent<Renderer>().bounds;

            if (bndWorkpiece.Intersects(bndForceField))
            {
                if (com.GetTagValue(tagMovement))
                {
                    Vector3 movement = Time.fixedDeltaTime * speed * direction;
                    if (com.GetTagValue(tagDirection))
                    {
                        movement = -movement;
                    }
                    workpiece.transform.Translate(movement, Space.World);
                }                
            }
        }
    }
}
