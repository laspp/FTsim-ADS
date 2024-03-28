using TMPro;
using UnityEngine;

public class ShowFPS : MonoBehaviour
{
    public Communication communication;
    public TextMeshProUGUI fpsText;

    private int frameCount;
    private double dt;
    private int fps;
    private double updateRate;
    private bool showFPS = false;

    // Use this for initialization
    void Start()
    {
        frameCount = 0;
        dt = 0.0;
        fps = 0;
        updateRate = 4.0;  // 4 updates per sec.

        showFPS = communication.appConfig.ShowFPS;
        fpsText.text = "";
        // Hide panel that is a parent of a text object
        if (!showFPS)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (showFPS)
        {
            frameCount++;
            dt += Time.deltaTime;
            if (dt > 1.0 / updateRate)
            {
                fps = Mathf.FloorToInt((float)(frameCount / dt));
                frameCount = 0;
                dt -= 1.0 / updateRate;
                fpsText.text = "FPS: " + fps.ToString();
            }
        }

    }
}



