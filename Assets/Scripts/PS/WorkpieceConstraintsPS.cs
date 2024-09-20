//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

////kpija kode iz L2N
//public class WorkpieceConstraints : MonoBehaviour
//{
//    public Transform pusher1Zone;
//    public Transform pusher2Zone;

//    private Communication com;
//    private Bounds bndWorkpiece;
//    private Bounds bndPusher1Zone;
//    private Bounds bndPusher2Zone;
//    private RigidbodyConstraints oldConstraints;
//    private bool isFirstTime = true;

//    // Start is called before the first frame update
//    void Start()
//    {
//        com = GameObject.Find("Communication").GetComponent<Communication>();

//        bndPusher1Zone = pusher1Zone.GetComponent<Renderer>().bounds;
//        bndPusher2Zone = pusher2Zone.GetComponent<Renderer>().bounds;

//        oldConstraints = transform.GetComponent<Rigidbody>().constraints;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        bndWorkpiece = transform.GetComponent<Renderer>().bounds;

//        // Pusher freeze zone - disable rotations to stabilize movement
//        if (bndWorkpiece.Intersects(bndPusher1Zone) || bndWorkpiece.Intersects(bndPusher2Zone))
//        {
//            if (isFirstTime)
//            {
//                oldConstraints = transform.GetComponent<Rigidbody>().constraints;
//                isFirstTime = false;
//            }            
//            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
//        }
//        else
//        {
//            transform.GetComponent<Rigidbody>().constraints = oldConstraints;
//            isFirstTime = true;
//        }

//    }
//}
