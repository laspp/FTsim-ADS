using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PanelControls : MonoBehaviour
{

    public GameObject workpiece;
    public Transform spawnPoint;
    public Transform lightRed;
    public Transform lightGreen;
    public Transform lightBlueTop;
    public Transform lightBlueBottom;


    Communication com;

    void Awake()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();

        // Initialize all panel inputs to false (buttons, toggle switch)
        com.WriteToPlc("ToggleSwitch", false);
        com.WriteToPlc("ButtonRed", false);
        com.WriteToPlc("ButtonGreen", false);
        com.WriteToPlc("ButtonBlackTopLeft", false);
        com.WriteToPlc("ButtonBlackTopRight", false);
        com.WriteToPlc("ButtonBlackBottomLeft", false);
        com.WriteToPlc("ButtonBlackBottomRight", false);

        workpiece.SetActive(false);
    }

    void Update()
    {
        // Control lights
        lightRed.GetComponent<Toggle>().isOn = com.GetTagValue("LightRed");
        lightGreen.GetComponent<Toggle>().isOn = com.GetTagValue("LightGreen");
        lightBlueTop.GetComponent<Toggle>().isOn = com.GetTagValue("LightBlueTop");
        lightBlueBottom.GetComponent<Toggle>().isOn = com.GetTagValue("LightBlueBottom");
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
