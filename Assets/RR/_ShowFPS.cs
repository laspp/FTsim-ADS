using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class _ShowFPS : MonoBehaviour {

	public Text fpsText;
	private int frameCount;
	private double dt;
	private int fps;
	private double updateRate;

	_Communication com;
	int isShowFPS = 0;

	// Use this for initialization
	void Start () {
		frameCount = 0;
		dt = 0.0;
		fps = 0;
		updateRate = 4.0;  // 4 updates per sec.

		com = GameObject.Find("Communication").GetComponent<_Communication>();
		isShowFPS = com.isShowFPS;
		fpsText.text = "";
	}
	
	// Update is called once per frame
	void Update () {
        if (isShowFPS==1)
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



