using UnityEngine;

public class SmartBarBase : MonoBehaviour
{
    [Tooltip("Amount of growing/shrinking when clicked. 1 means the height of a workpiece.")]
    public float scaleStep = 1.0f;
    public float scaleSpeed = 1.0f;
    public float maxScale = 5.0f;

    float origScale;
    float currentScale;
    float targetScale;
    readonly float scaleEps = 0.01f;
    bool isScaling;
    int gridIndexX;
    int gridIndexZ;

    public float TargetScale { get => targetScale; set => targetScale = value; }
    public float OrigScale { get => origScale; set => origScale = value; }
    public int GridIndexX { get => gridIndexX; set => gridIndexX = value; }
    public int GridIndexZ { get => gridIndexZ; set => gridIndexZ = value; }

    void Awake()
    {
        OrigScale = transform.localScale.y;
        currentScale = OrigScale;
        TargetScale = currentScale;
        isScaling = false;
    }

    void FixedUpdate()
    {
        // Get the current scale in the Y direction
        currentScale = transform.localScale.y;
        float diff = TargetScale - currentScale;

        // Check if the current scale is not approximately equal to the target scale
        if (Mathf.Abs(diff) > scaleEps)
        {
            isScaling = true;
            // Determine whether to increase or decrease the scale based on the difference between current and target scales
            float scaleChange = Mathf.Sign(diff) * scaleSpeed * Time.fixedDeltaTime;

            // Apply the scale change
            transform.localScale += new Vector3(0f, scaleChange, 0f);
        }
        else
        {
            isScaling = false;
        }
    }

    public void ScaleUp()
    {
        if (!isScaling && transform.localScale.y < maxScale)
        {
            TargetScale += scaleStep;
        }
    }

    public void ScaleDown()
    {
        if (!isScaling && transform.localScale.y > OrigScale)
        {
            TargetScale -= scaleStep;
        }
    }

    public void SetTargetScale (float newTarget)
    {
        TargetScale = newTarget;
    }

    public void ResetTargetScale()
    {
        TargetScale = OrigScale;
    }
}