//using UnityEngine;

//public class WorkpieceConstraintsRR : MonoBehaviour
//{

//    public Transform handLeft;
//    public Transform handLeft_grab_area;
//    public Transform handRight_grab_area;
//    public Transform holder;

//    bool leftTouching, rightTouching, grabbed;

//    void OnTriggerEnter(Collider other)
//    {
//        if (other.transform == handRight_grab_area)
//        {
//            rightTouching = true;
//            //Debug.Log("rightTouching.");
//        }
//        if (other.transform == handLeft_grab_area)
//        {
//            leftTouching = true;
//            //Debug.Log("leftTouching.");
//        }
//    }
//    void OnTriggerExit(Collider other)
//    {
//        if (other.transform == handRight_grab_area)
//        {
//            rightTouching = false;
//            //Debug.Log("not rightTouching.");
//        }
//        if (other.transform == handLeft_grab_area)
//        {
//            leftTouching = false;
//            //Debug.Log("not leftTouching.");
//        }
//    }


//    void imGrabbed()
//    {
//        //GameObject a = GameObject.FindWithTag("hnd");
//        handLeft.GetComponent<ArmGrip>().UpdateGripState(true);
//    }

//    void imFree()
//    {
//        handLeft.GetComponent<ArmGrip>().UpdateGripState(false);
//    }

//    // Use this for initialization
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//        if (leftTouching && rightTouching)
//        {
//            transform.GetComponent<Rigidbody>().isKinematic = true;
//            transform.GetComponent<Rigidbody>().useGravity = false;
//            handLeft.GetComponent<ArmGrip>().objGripped = transform;
//            imGrabbed();
//        }


//        if (handLeft.GetComponent<ArmGrip>().objGripped == transform)
//        {
//            transform.position = holder.position;
//        }
//        else
//        {
//            if (transform.GetComponent<Rigidbody>().isKinematic)
//                imFree();
//            transform.GetComponent<Rigidbody>().useGravity = true;
//            transform.GetComponent<Rigidbody>().isKinematic = false;
//            transform.parent = null;
//        }

//    }
//}


using UnityEngine;

public class WorkpieceConstraintsRR : MonoBehaviour
{
    public Transform handLeft;
    public Transform handLeftGripArea;
    public Transform handRightGripArea;
    public Transform holder;

    bool leftTouching, rightTouching;

    private void OnTriggerEnter(Collider other)
    {
        UpdateTouchingState(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        UpdateTouchingState(other, false);
    }

    void UpdateTouchingState(Collider other, bool isTouching)
    {
        if (other.transform == handRightGripArea)
        {
            rightTouching = isTouching;
        }
        if (other.transform == handLeftGripArea)
        {
            leftTouching = isTouching;
        }
    }

    void UpdateArmGripState(bool isGripped)
    {
        GameObject arm = GameObject.FindWithTag("hnd");
        var armGripComponent = arm.GetComponent<ArmGrip>();
        if (armGripComponent != null)
        {
            armGripComponent.UpdateGripState(isGripped);
        }
    }

    void FixedUpdate()
    {
        if (leftTouching && rightTouching)
        {
            HandleGrab();
        }
        else
        {
            HandleRelease();
        }
    }

    void HandleGrab()
    {
        MakeImmobile();
        UpdateArmGripState(true);
        handLeft.GetComponent<ArmGrip>().objGripped = transform;
        
        if (IsBeingHeldByLeftHand())
        {
            MoveToHolderPosition();
        }
    }

    void MakeImmobile()
    {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
    }

    bool IsBeingHeldByLeftHand()
    {
        var armGripComponent = handLeft.GetComponent<ArmGrip>();
        return armGripComponent != null && armGripComponent.objGripped == transform;
    }

    void MoveToHolderPosition()
    {
        transform.position = holder.position;
    }

    void HandleRelease()
    {
        if (IsImmobile())
        {
            UpdateArmGripState(false);
        }

        EnableGravityAndKinematics();        
    }

    bool IsImmobile()
    {
        return GetComponent<Rigidbody>().isKinematic;
    }

    void EnableGravityAndKinematics()
    {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;
    }
}
