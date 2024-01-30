using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class _ToggleKeyControl : MonoBehaviour
{
	public string key_code;

	void Update()
	{
		if (Input.GetKeyDown (key_code)) {
			this.GetComponent<Toggle> ().isOn = true;
		}
		if (Input.GetKeyUp (key_code)) {
			this.GetComponent<Toggle> ().isOn = false;
		}
	}
}
