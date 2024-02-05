using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshCollider))]

public class SmartBar : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Amount of growing/shrinking when clicked.")]
    public float growBy = 1.0f;
    public float maxHeight = 5.0f;
    public GameObject panelBuilder;

    MeshRenderer objMeshRederer;
    Color origColor;
    Vector3 growVector;
    float origHeight;
    Builder builderScript;


    void Awake()
    {
        origHeight = transform.parent.localScale.y;
        objMeshRederer = GetComponent<MeshRenderer>();
        origColor = objMeshRederer.material.color;
        growVector = new Vector3(0, growBy, 0);
        builderScript = panelBuilder.GetComponent<Builder>();
    }

    void OnMouseEnter()
    {
        if (builderScript.BuilderMode) {
            objMeshRederer.material.color = Color.green;
        }
    }
    void OnMouseExit()
    {
        if (builderScript.BuilderMode)
        {
            objMeshRederer.material.color = origColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (builderScript.BuilderMode)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // Let's scale the cube up by growBy
                // To scale in only one direction, we actually scale its parent (empty gameObject placed at the base of a bar)
                if (transform.parent.localScale.y < maxHeight)
                {
                    transform.parent.localScale += growVector;
                }

            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Let's scale the cube down by growBy
                if (transform.parent.localScale.y > origHeight)
                {
                    transform.parent.localScale -= growVector;
                }
            }
        }
    }
}