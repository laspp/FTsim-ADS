using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class _ToggleKeyControlMod : MonoBehaviour
{
	public string key_code;
	bool keyOld = false;

	void Update()
	{
		if (Input.GetKeyDown (key_code) && !keyOld) {
			this.GetComponent<Toggle> ().isOn = true;
		}
		if (Input.GetKeyDown (key_code) && keyOld) {
			this.GetComponent<Toggle> ().isOn = false;
		}
		keyOld = this.GetComponent<Toggle> ().isOn;
	}
}
