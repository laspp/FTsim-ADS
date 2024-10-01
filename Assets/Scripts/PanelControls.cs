using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PanelControls : MonoBehaviour
{
    public Communication communication;
    public GameObject workpiece;
    public Transform spawnPoint;
    public Transform lightRed;
    public Transform lightGreen;
    public Transform lightBlueTop;
    public Transform lightBlueBottom;
       

    void Start()
    {
        
        // Initialize all panel inputs to false (buttons, toggle switch)
        communication.WriteToPlc("ToggleSwitch", false);
        communication.WriteToPlc("ButtonRed", false);
        communication.WriteToPlc("ButtonGreen", false);
        communication.WriteToPlc("ButtonBlackTopLeft", false);
        communication.WriteToPlc("ButtonBlackTopRight", false);
        communication.WriteToPlc("ButtonBlackBottomLeft", false);
        communication.WriteToPlc("ButtonBlackBottomRight", false);

        workpiece.SetActive(false);
    }

    void Update()
    {
        // Control lights
        lightRed.GetComponent<Toggle>().isOn = communication.GetTagValue("LightRed");
        lightGreen.GetComponent<Toggle>().isOn = communication.GetTagValue("LightGreen");
        lightBlueTop.GetComponent<Toggle>().isOn = communication.GetTagValue("LightBlueTop");
        lightBlueBottom.GetComponent<Toggle>().isOn = communication.GetTagValue("LightBlueBottom");
    }

    public void CreateNewWorkpiece()
    {
        Vector3 pos = spawnPoint.position;
        GameObject clone = Instantiate(workpiece, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity);
        clone.tag = "Workpiece";
        clone.SetActive(true);
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

    public void LoadStartMenuScene()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void ToggleSwitchOnChange(Toggle change)
    {
        communication.WriteToPlc("ToggleSwitch", change.isOn);
    }
    public void ButtonRedOnChange(Toggle change)
    {
        communication.WriteToPlc("ButtonRed", change.isOn);
    }
    public void ButtonGreenOnChange(Toggle change)
    {
        communication.WriteToPlc("ButtonGreen", change.isOn);
    }
    public void ButtonBlackTopLeftOnChange(Toggle change)
    {
        communication.WriteToPlc("ButtonBlackTopLeft", change.isOn);
    }
    public void ButtonBlackTopRightOnChange(Toggle change)
    {
        communication.WriteToPlc("ButtonBlackTopRight", change.isOn);
    }
    public void ButtonBlackBottomLeftOnChange(Toggle change)
    {
        communication.WriteToPlc("ButtonBlackBottomLeft", change.isOn);
    }
    public void ButtonBlackBottomRightOnChange(Toggle change)
    {
        communication.WriteToPlc("ButtonBlackBottomRight", change.isOn);
    }
}
