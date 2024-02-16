using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class KeyControlButton : MonoBehaviour
{

	public KeyCode key;

	public Button ButtonObj { get; private set; }

	Graphic targetGraphic;

	void Awake()
	{
		ButtonObj = GetComponent<Button>();
		targetGraphic = GetComponent<Graphic>();
	}

	void Start()
	{
		Up();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(key))
		{
			Down();
		}
		else if (Input.GetKeyUp(key))
		{
			Up();
		}
	}

	void Up()
	{
		StartColorTween(ButtonObj.colors.normalColor, false);
	}

	void Down()
	{
		StartColorTween(ButtonObj.colors.pressedColor, false);
		ButtonObj.onClick.Invoke();
	}

	void StartColorTween(Color targetColor, bool instant)
	{
		if (targetGraphic == null)
			return;

		targetGraphic.CrossFadeColor(targetColor, instant ? 0f : ButtonObj.colors.fadeDuration, true, true);
	}

	void OnApplicationFocus(bool focus)
	{
		Up();
	}
}
