using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshCollider))]

public class SmartBar : MonoBehaviour, IPointerClickHandler
{
    public GameObject panelBuilder;
    public Transform workpieceRespawnPoint;
    public Material materialBase;
    public Material materialHover;
    public Material materialRespawn;

    GameObject smartBarBase;
    MeshRenderer objMeshRederer;
    Builder builderScript;
    SmartBarBase smartBarBaseScript;
    readonly string tagRespawn = "SmartBarRespawn";

    void Awake()
    {
        builderScript = panelBuilder.GetComponent<Builder>();
        smartBarBase = transform.parent.gameObject;
        smartBarBaseScript = smartBarBase.GetComponent<SmartBarBase>();
        objMeshRederer = GetComponent<MeshRenderer>();
        materialBase = objMeshRederer.material;
    }

    void OnMouseEnter()
    {
        if (builderScript.BuilderMode)
        {
            if (builderScript.SpawnMode)
            {
                objMeshRederer.material = materialRespawn;
            }
            else
            {
                objMeshRederer.material = materialHover;
            }
        }
    }
    void OnMouseExit()
    {
        if (gameObject.CompareTag(tagRespawn))
        {
            objMeshRederer.material = materialRespawn;
        }
        else
        {
            objMeshRederer.material = materialBase;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (builderScript.BuilderMode)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (builderScript.SpawnMode)
                {
                    SetRespawnPoint();
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

    public void SetRespawnPoint()
    {
        float maxScale = smartBarBaseScript.maxScale;
        // Set spawn point above this bar
        workpieceRespawnPoint.position = new Vector3(transform.position.x, maxScale, transform.position.z);

        // Remove tag from old position
        GameObject go = GameObject.FindGameObjectWithTag(tagRespawn);
        if (go != null)
        {
            go.tag = "Untagged";
            go.GetComponent<MeshRenderer>().material = materialBase;
        }

        // Apply tag to mark this smartbar
        gameObject.tag = tagRespawn;
        objMeshRederer.material = materialRespawn;
    }
}