using UnityEngine;

public class PanelToggleActive : MonoBehaviour
{

    public void ToggleVisibility()
    {
        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
}
