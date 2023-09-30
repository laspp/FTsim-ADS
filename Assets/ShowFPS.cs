using UnityEngine;
using UnityEngine.UI;

public class ShowFPS : MonoBehaviour {

	public Text fpsText;

	private Communication com;
	private int frameCount;
	private double dt;
	private int fps;
	private double updateRate;
	private bool showFPS = false;

	// Use this for initialization
	void Start () {
		frameCount = 0;
		dt = 0.0;
		fps = 0;
		updateRate = 4.0;  // 4 updates per sec.

		com = GameObject.Find("Communication").GetComponent<Communication>();
		showFPS = com.appConfig.ShowFPS;
		fpsText.text = "";
	}
	
	void Update () {
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



