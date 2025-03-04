using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PanelControls : MonoBehaviour
{
    public Communication communication;
    public GameObject workpiece;
    public Transform spawnPoint;
    public Transform LEDRed;
    public Transform LEDGreen;
    public Transform LEDBlueUp;
    public Transform LEDBlueDown;
       
    private int workpieceCounter = 0; // Counter for the number of workpieces
    private const int maxWorkpieces = 50; // Maximum number of workpieces allowed

    void Start()
    {
        
        // Initialize all panel inputs to false (buttons, toggle switch)
        communication.WriteToPlc("ToggleSwitch", false);
        communication.WriteToPlc("ButtonRed", false);
        communication.WriteToPlc("ButtonGreen", false);
        communication.WriteToPlc("ButtonBlackLeftUp", false);
        communication.WriteToPlc("ButtonBlackRightUp", false);
        communication.WriteToPlc("ButtonBlackLeftDown", false);
        communication.WriteToPlc("ButtonBlackRightDown", false);

        workpiece.SetActive(false);
    }

    void Update()
    {
        // Control lights
        LEDRed.GetComponent<Toggle>().isOn = communication.GetTagValue("LEDRed");
        LEDGreen.GetComponent<Toggle>().isOn = communication.GetTagValue("LEDGreen");
        LEDBlueUp.GetComponent<Toggle>().isOn = communication.GetTagValue("LEDBlueUp");
        LEDBlueDown.GetComponent<Toggle>().isOn = communication.GetTagValue("LEDBlueDown");
    }

    public void CreateNewWorkpiece()
    {
        if (workpieceCounter <= maxWorkpieces)
        {
            Vector3 pos = spawnPoint.position;
            GameObject clone = Instantiate(workpiece, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity);
            clone.tag = "Workpiece";
            clone.SetActive(true);
            workpieceCounter++;
        }
        else
        {
            Debug.LogWarning("Maximum number of workpieces reached.");

            if (GameObject.FindWithTag("Dialog_error_max_workpieces") == null)
            {
                Dialog.MessageBox(
                    "Dialog_error_max_workpieces",
                    "Runtime Error",
                    $"Maximum number of workpieces reached.\nClicking DEL will remove workpiece from scene, that was generated first.",
                    "DEL", () => { RemoveWorkpiece(); }, widthMax: 300, heightMax: 120
                    );
            }
        }
    }
    public void RemoveWorkpiece()
    {
        GameObject[] workPiece = GameObject.FindGameObjectsWithTag("Workpiece");
        if (workPiece != null && workPiece.Length > 0)
        {
            Destroy(workPiece[0]);
            workpieceCounter--;
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
    public void ButtonBlackLeftUpOnChange(Toggle change)
    {
        communication.WriteToPlc("ButtonBlackLeftUp", change.isOn);
    }
    public void ButtonBlackRightUpOnChange(Toggle change)
    {
        communication.WriteToPlc("ButtonBlackRightUp", change.isOn);
    }
    public void ButtonBlackLeftDownOnChange(Toggle change)
    {
        communication.WriteToPlc("ButtonBlackLeftDown", change.isOn);
    }
    public void ButtonBlackRightDownOnChange(Toggle change)
    {
        communication.WriteToPlc("ButtonBlackRightDown", change.isOn);
    }
}
