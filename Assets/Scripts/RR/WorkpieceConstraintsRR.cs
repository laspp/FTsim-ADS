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
        GameObject arm = handLeft.gameObject;
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
