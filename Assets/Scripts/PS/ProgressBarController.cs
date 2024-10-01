using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    [Tooltip("A name of tag (defined in config-PS.json)")]
    public string MotorCompressor = "MotorCompressor";
    Communication com;

    private Image[] squares; // Assign the 10 squares in the Unity Inspector
    private int currentSquare = 0;
    private bool isRunning = false;

    private void Start()
    {
        com = GameObject.Find("Communication").GetComponent<Communication>();

        squares = GetComponentsInChildren<Image>();

        // Initially set all squares to be non-visible
        foreach (var square in squares)
        {
            square.enabled = false; // Disable the Image component to make non-visible
        }

        isRunning = true;
        StartCoroutine(UpdateProgressBar());
}

    void Update()
    {
        //StartCoroutine(UpdateProgressBar());
    }

    private IEnumerator UpdateProgressBar()
    {
        // BUG THAT BLOCK WHOLE UNITY!!!
        while (isRunning) // Keep running the coroutine
        {
            //Debug.Log($"isrunning");
            if (com.GetTagValue(MotorCompressor))
            {
                
                // If there are missing squares, add one
                if (currentSquare < squares.Length && squares[currentSquare].enabled == false)
                {
                    //UnityEngine.Debug.Log($"sqare enabled");
                    squares[currentSquare].enabled = true; // Enable the Image component to make visible
                    currentSquare++;
                }
                yield return new WaitForSeconds(1); // Wait for 1 second
            }
        }
    }

    //subscribe to the OnPistonMove event
    // private void OnEnable()
    // {
    //     Piston.OnPistonMove += RemoveSquares;
    // }

    // //unsubscribe to the OnPistonMove event
    // private void OnDisable()
    // {
    //     Piston.OnPistonMove -= RemoveSquares;
    // }

    private void RemoveSquares(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (currentSquare > 0)
            {
                //Debug.Log($"sqare removed");
                currentSquare--;
                squares[currentSquare].enabled = false; // Disable the Image component to make non-visible
            }
        }
    }
}