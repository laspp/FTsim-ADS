using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustToggleVisibility : MonoBehaviour
{
    public void ToggleVisibility(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(!obj.activeSelf);
        }
        else
        {
            Debug.LogError("GameObject is null.");
        }
    }

}