using UnityEngine;
using UnityEngine.UI;

public class ToggleVisibilityBtn : MonoBehaviour
{
    public GameObject targetObject; // Reference to the object to be toggled

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(!targetObject.activeSelf);
        }
        else
        {
            Debug.LogError("Target object is not assigned.");
        }
    }
}