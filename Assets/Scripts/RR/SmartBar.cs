using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshCollider))]

public class SmartBar : MonoBehaviour, IPointerClickHandler
{
    public GameObject panelBuilder;
    public Transform workpieceSpawnPoint;

    GameObject smartBarBase;
    MeshRenderer objMeshRederer;
    Color origColor;
    Builder builderScript;
    SmartBarBase smartBarBaseScript;


    void Awake()
    {
        builderScript = panelBuilder.GetComponent<Builder>();
        smartBarBase = transform.parent.gameObject;
        smartBarBaseScript = smartBarBase.GetComponent<SmartBarBase>();
        objMeshRederer = GetComponent<MeshRenderer>();
        origColor = objMeshRederer.material.color;
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
        objMeshRederer.material.color = origColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (builderScript.BuilderMode)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (builderScript.SpawnMode)
                {
                    float maxScale = smartBarBaseScript.maxScale;
                    // Set spawn point above this bar
                    workpieceSpawnPoint.position = new Vector3(transform.position.x, maxScale, transform.position.z);
                }
                else
                {
                    // Let's scale the cube up by growBy
                    // To scale in only one direction, we actually scale its parent (empty gameObject placed at the base of a bar)
                    smartBarBaseScript.ScaleUp();
                }
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                smartBarBaseScript.ScaleDown();
            }
        }
    }
}