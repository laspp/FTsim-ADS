using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Transform dropDown2;
    public string tagTableDirection = "MotorTableDirection";
    public string tagTableMovement = "MotorTableMovement";
    public string tagTablePosSwitch = "SwitchTablePosition";

    Rigidbody table; //če je object rigitbody je to simulirano "fizikalno" telo na katerega delujejo sile v unitiyju
    Vector3 rotateCW = new Vector3(0, 9, 0); //smaler the number, slower rotation
    Communication com;
    float rotation = 0;
    Quaternion deltaRotation;

    // Start is called before the first frame update
    void Start()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();
        table = GetComponent<Rigidbody>();

    }

    void rotate_CW()
    {
        rotation += Time.fixedDeltaTime * rotateCW.y;
        deltaRotation = Quaternion.Euler(rotateCW * Time.fixedDeltaTime);
        table.MoveRotation(table.rotation * deltaRotation);
    }

    void rotate_CCW()
    {
        rotation -= Time.fixedDeltaTime * rotateCW.y;
        deltaRotation = Quaternion.Euler(-rotateCW * Time.fixedDeltaTime);
        table.MoveRotation(table.rotation * deltaRotation);
    }

    //private float timer = 0.0f;
    //private float waitTime = 0.2f; // Time to wait in seconds

    // Update is called once per frame
    void Update()
    {
        //timer += Time.deltaTime;

        //if (timer > waitTime)
        //{
            //--
            if (com.GetTagValue(tagTableMovement))
            {
                Debug.Log($"Table moventent");
                if (com.GetTagValue(tagTableDirection))
                {
                    Debug.Log("rotate CCW");
                    rotate_CCW();
                }
                else
                {
                    rotate_CW();
                    Debug.Log("rotate CW");
                }
            }

            //update the table position switch value
            //com.WriteToPlc(tagTablePosSwitch, Mathf.Abs(rotation) % 90 < 4);
            //--
            //Debug.Log($"rotation {rotation}");
        //    timer = 0.0f; // Reset the timer
        //}
    }
}
