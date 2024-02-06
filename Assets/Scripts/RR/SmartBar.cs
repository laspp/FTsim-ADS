using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshCollider))]

public class SmartBar : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Amount of growing/shrinking when clicked. 1 means the height of a workpiece.")]
    public float scaleStep = 1.0f;
    public float scaleSpeed = 1.0f;
    public float maxScale = 5.0f;
    public GameObject panelBuilder;
    public Transform workpieceSpawnPoint;

    MeshRenderer objMeshRederer;
    Color origColor;
    Vector3 growVector;
    float origScale;
    float currentScale;
    float targetScale;
    float scaleEps = 0.01f;
    Builder builderScript;


    void Awake()
    {
        origScale = transform.parent.localScale.y;
        currentScale = origScale;
        targetScale = currentScale;
        objMeshRederer = GetComponent<MeshRenderer>();
        origColor = objMeshRederer.material.color;
        growVector = new Vector3(0, scaleStep, 0);
        builderScript = panelBuilder.GetComponent<Builder>();
    }

    void FixedUpdate()
    {
        // Get the current scale in the Y direction
        currentScale = transform.parent.localScale.y;
        float diff = targetScale - currentScale;

        // Check if the current scale is not approximately equal to the target scale
        if (Mathf.Abs(diff) > scaleEps)
        {
            // Determine whether to increase or decrease the scale based on the difference between current and target scales
            float scaleChange = Mathf.Sign(diff) * scaleSpeed * Time.fixedDeltaTime;

            // Apply the scale change
            transform.parent.localScale += new Vector3(0f, scaleChange, 0f);
        }
    }



    void OnMouseEnter()
    {
        if (builderScript.BuilderMode)
        {
            if (builderScript.SpawnMode)
            {
                objMeshRederer.material.color = Color.yellow;
            }
            else
            {
                objMeshRederer.material.color = Color.green;
            }
        }
    }
    void OnMouseExit()
    {
        //if (builderScript.BuilderMode)
        //{
        objMeshRederer.material.color = origColor;
        //}
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (builderScript.BuilderMode)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (builderScript.SpawnMode)
                {
                    // Set spawn point above this bar
                    workpieceSpawnPoint.position = new Vector3(transform.position.x, maxScale, transform.position.z);
                }
                else
                {
                    // Let's scale the cube up by growBy
                    // To scale in only one direction, we actually scale its parent (empty gameObject placed at the base of a bar)
                    if (transform.parent.localScale.y < maxScale)
                    {
                        //transform.parent.localScale += growVector;
                        targetScale += scaleStep;
                    }
                }

            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Let's scale the cube down by growBy
                if (transform.parent.localScale.y > origScale)
                {
                    //transform.parent.localScale -= growVector;
                    targetScale -= scaleStep;
                }
            }
        }
    }
}