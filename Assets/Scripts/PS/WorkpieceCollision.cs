using UnityEngine;

public class WorkpieceCollision : MonoBehaviour
{
    private Communication com;
    //private bool isWorkpieceOnTable = false;
    public string detectorTag = null;
    void Start()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();

        if (!string.IsNullOrEmpty(detectorTag))
        {
            Communication com = FindObjectOfType<Communication>();
            if (com != null)
            {
                com.RegisterDetector(detectorTag, this);
            }
            else
            {
                Debug.LogError("Communication script not found in the scene.");
            }
        }
        else
        {
            Debug.LogError("Detector tag is not set for " + gameObject.name);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collision detected with: " + other.gameObject.name);

        if (other.gameObject.CompareTag("Workpiece"))
        {
            //isWorkpieceOnTable = true;
            //Communication communication = FindObjectOfType<Communication>();
            if (com != null)
            {
                com.UpdateDetectorValue(detectorTag, true);
            }
            Debug.Log("A Workpiece clone is placed on top of the table.");
        }

    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Workpiece"))
        {
            //isWorkpieceOnTable = false;
            //Communication communication = FindObjectOfType<Communication>();
            if (com != null)
            {
                com.UpdateDetectorValue(detectorTag, false);
            }
            Debug.Log($"A Workpiece clone has been removed from detector {detectorTag}.");
        }
    }

    // public bool IsWorkpieceOnTable()
    // {
    //     return isWorkpieceOnTable;
    // }
}
