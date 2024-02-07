using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PanelControls : MonoBehaviour
{
    public GameObject communication;
    public GameObject workpiece;
    public Transform spawnPoint;
    public Transform lightRed;
    public Transform lightGreen;
    public Transform lightBlueTop;
    public Transform lightBlueBottom;

    Communication comScript;

    void Awake()
    {
        comScript = communication.GetComponent<Communication>();

        // Initialize all panel inputs to false (buttons, toggle switch)
        comScript.WriteToPlc("ToggleSwitch", false);
        comScript.WriteToPlc("ButtonRed", false);
        comScript.WriteToPlc("ButtonGreen", false);
        comScript.WriteToPlc("ButtonBlackTopLeft", false);
        comScript.WriteToPlc("ButtonBlackTopRight", false);
        comScript.WriteToPlc("ButtonBlackBottomLeft", false);
        comScript.WriteToPlc("ButtonBlackBottomRight", false);

        workpiece.SetActive(false);
    }

    void Update()
    {
        // Control lights
        lightRed.GetComponent<Toggle>().isOn = comScript.GetTagValue("LightRed");
        lightGreen.GetComponent<Toggle>().isOn = comScript.GetTagValue("LightGreen");
        lightBlueTop.GetComponent<Toggle>().isOn = comScript.GetTagValue("LightBlueTop");
        lightBlueBottom.GetComponent<Toggle>().isOn = comScript.GetTagValue("LightBlueBottom");
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
        comScript.WriteToPlc("ToggleSwitch", change.isOn);
    }
    public void ButtonRedOnChange(Toggle change)
    {
        comScript.WriteToPlc("ButtonRed", change.isOn);
    }
    public void ButtonGreenOnChange(Toggle change)
    {
        comScript.WriteToPlc("ButtonGreen", change.isOn);
    }
    public void ButtonBlackTopLeftOnChange(Toggle change)
    {
        comScript.WriteToPlc("ButtonBlackTopLeft", change.isOn);
    }
    public void ButtonBlackTopRightOnChange(Toggle change)
    {
        comScript.WriteToPlc("ButtonBlackTopRight", change.isOn);
    }
    public void ButtonBlackBottomLeftOnChange(Toggle change)
    {
        comScript.WriteToPlc("ButtonBlackBottomLeft", change.isOn);
    }
    public void ButtonBlackBottomRightOnChange(Toggle change)
    {
        comScript.WriteToPlc("ButtonBlackBottomRight", change.isOn);
    }

}
