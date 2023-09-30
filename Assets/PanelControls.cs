using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PanelControls : MonoBehaviour
{
    
    public Transform newWorkpiece;
    public Transform lightRed;
    public Transform lightGreen;
    public Transform lightBlueTop;
    public Transform lightBlueBottom;


    private Communication com;

    void Awake()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();
    }

    void Update()
    {
        // Control lights
        lightRed.GetComponent<Button>().interactable = com.GetTagValue("LightRed");
        lightGreen.GetComponent<Button>().interactable = com.GetTagValue("LightGreen");
        lightBlueTop.GetComponent<Button>().interactable = com.GetTagValue("LightBlueTop");
        lightBlueBottom.GetComponent<Button>().interactable = com.GetTagValue("LightBlueBottom");
    }

    public void CreateNewWorkpiece()
    {
        newWorkpiece.tag = "Workpiece";
        Instantiate(newWorkpiece, new Vector3(-2.05f, 8, -0.3f), Quaternion.identity);
        newWorkpiece.tag = "Untagged";
    }
    public void RemoveWorkpiece()
    {
        GameObject[] workPiece = GameObject.FindGameObjectsWithTag("Workpiece");
        if (workPiece != null && workPiece.Length > 0)
        {
            Destroy(workPiece[0]);
        }
    }

    public void ResetScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ToggleSwitchOnChange(Toggle change)
    {
        Debug.Log("ToggleSwitch change: " + change.isOn);
        com.WriteToPlc("ToggleSwitch", change.isOn);
    }
    public void ButtonRedOnChange(Toggle change)
    {
        com.WriteToPlc("ButtonRed", change.isOn);
    }
    public void ButtonGreenOnChange(Toggle change)
    {
        com.WriteToPlc("ButtonGreen", change.isOn);
    }
    public void ButtonBlackTopLeftOnChange(Toggle change)
    {
        com.WriteToPlc("ButtonBlackTopLeft", change.isOn);
    }
    public void ButtonBlackTopRightOnChange(Toggle change)
    {
        com.WriteToPlc("ButtonBlackTopRight", change.isOn);
    }
    public void ButtonBlackBottomLeftOnChange(Toggle change)
    {
        com.WriteToPlc("ButtonBlackBottomLeft", change.isOn);
    }
    public void ButtonBlackBottomRightOnChange(Toggle change)
    {
        com.WriteToPlc("ButtonBlackBottomRight", change.isOn);
    }

}
