using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshCollider))]

public class SmartBar : MonoBehaviour, IPointerClickHandler
{
    public GameObject panelBuilder;
    public Transform workpieceSpawnPoint;
    public Material materialBase;
    public Material materialHover;
    public Material materialSpawn;

    GameObject smartBarBase;
    MeshRenderer objMeshRederer;
    Builder builderScript;
    SmartBarBase smartBarBaseScript;


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
                objMeshRederer.material = materialSpawn;
            }
            else
            {
                objMeshRederer.material = materialHover;
            }
        }
    }
    void OnMouseExit()
    {
        objMeshRederer.material = materialBase;
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