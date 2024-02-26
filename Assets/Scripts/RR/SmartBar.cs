using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(MeshCollider))]

public class SmartBar : MonoBehaviour, IPointerClickHandler
{
    public GameObject panelBuilder;
    public Transform workpieceRespawnPoint;
    public Material materialBase;
    public Material materialHover;
    public Material materialRespawn;
    public Material materialCollision;
    public GameObject buttonClearCollisions;
    public string tagRespawn = "SmartBarRespawn";
    public string tagPlayer = "Player";

    GameObject smartBarBase;
    MeshRenderer objMeshRederer;
    Builder builderScript;
    SmartBarBase smartBarBaseScript;
    bool collisionDetected;

    void Awake()
    {
        builderScript = panelBuilder.GetComponent<Builder>();
        smartBarBase = transform.parent.gameObject;
        smartBarBaseScript = smartBarBase.GetComponent<SmartBarBase>();
        objMeshRederer = GetComponent<MeshRenderer>();
        materialBase = objMeshRederer.material;
        collisionDetected = false;
    }

    void OnTriggerEnter(Collider collider)
    {
        //Debug.Log($"SmartBar::OnTriggerEnter: {collider} {collider.gameObject.tag}");
        if (collider.gameObject.CompareTag(tagPlayer)) {
            //Debug.Log($"SmartBar::OnTriggerEnter: {collider} {collider.gameObject.tag}");
            objMeshRederer.material = materialCollision;
            collisionDetected = true;
            buttonClearCollisions.GetComponent<Button>().interactable = true;
        }
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
        if (collisionDetected)
        {
            objMeshRederer.material = materialCollision;
        }
        else if (gameObject.CompareTag(tagRespawn))
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
        if (!collisionDetected)
        {
            objMeshRederer.material = materialRespawn;
        }        
    }

    public void ResetCollisionDetected()
    {
        collisionDetected = false;
        OnMouseExit();
        buttonClearCollisions.GetComponent<Button>().interactable = false;
    }
}