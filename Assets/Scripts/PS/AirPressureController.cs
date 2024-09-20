using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AirPressureController : MonoBehaviour
{
    private int airPressureLevel = 0;
    private int maxAirPressureLevel = 10;
    private object lockObject = new object();
    private Communication com;

    [Tooltip("A name of tag (defined in config-PS.json)")]
    public string MotorCompressor = "MotorCompressor";

    [Tooltip("Name of gameObject for air tank reference. If null, \"zrak\" will be used")]
    public string airTankStr = "zrak";
    private GameObject airTank;

    [Tooltip("Name of gameObject for progres bar reference. If null, \"PressureProgressBar\" will be used")]
    public string progressBarStr = "PressureProgressBar";
    private GameObject progressBar;

    void Start()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();
        airTank = GameObject.Find(airTankStr);
        progressBar = GameObject.Find(progressBarStr);
        // Debug.Log($"airTank: {airTank}");
        // Debug.Log($"PressureProgressBar: {progressBar}");

        //start with empty tank
        foreach (var air in airTank.GetComponentsInChildren<Image>())
        {
            air.enabled = false;
        }
        
        //and empty progress bar
        foreach (var sqare in progressBar.GetComponentsInChildren<Image>())
        {
            sqare.enabled = false;
        }

        StartCoroutine(IncrementPressure());
    }
    //korutina za povecanje pritiska
    IEnumerator IncrementPressure()
    {
        while (true)
        {
            if (com.GetTagValue(MotorCompressor))
            {
                IncrementAirPressureLevel();
            }
            yield return new WaitForSeconds(5);
        }
    }

    void Update()
    {
        // Update the child objects of the air tank and progress bar
        for (int i = 0; i < maxAirPressureLevel; i++)
        {
            bool isActive = i < GetAirPressureLevel();
            airTank.transform.GetChild(i).gameObject.SetActive(isActive);
            
            Image childImage = progressBar.transform.GetChild(i).GetComponent<Image>();
            childImage.enabled = isActive;
            //Debug.Log($"airTank: {childImage} for i={i} is {isActive}");
        }
    }

    public int GetAirPressureLevel()
    {
        lock (lockObject)
        {
            return airPressureLevel;
        }
    }

    public void IncrementAirPressureLevel()
    {
        if (airPressureLevel < maxAirPressureLevel){
            lock (lockObject)
            {
                airPressureLevel++;
            }
        }
    }

    public void DecrementAirPressureLevel()
    {
        Debug.Log($"DecrementAirPressureLevel from {airPressureLevel} to {airPressureLevel - 1}");
        if (airPressureLevel > 0){
            lock (lockObject)
            {
                airPressureLevel--;
            }
        }
    }


}