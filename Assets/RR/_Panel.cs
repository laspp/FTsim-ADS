using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class _Panel : MonoBehaviour
{

    public Transform key;
    public Transform button1;
    public Transform button2;
    public Transform button3;
    public Transform button4;
    public Transform green;
    public Transform red_left;
    public Transform red_right;
    public Transform yellow_right;
    public Transform yellow_left;
	public Transform prefab;
	public Transform spawnPoint;
	public Transform showHelpStartToggle;

    _Communication com;
	GameObject helpWindow;
	int showHelpOnStart;

    // Use this for initialization
    void Start()
    {
        com = GameObject.Find("Communication").GetComponent<_Communication>();
		helpWindow = GameObject.FindGameObjectWithTag ("UI_help_window");

		if ( !PlayerPrefs.HasKey ("showHelpOnStart")) {
			PlayerPrefs.SetInt ("showHelpOnStart", 1);
		}
		showHelpOnStart = PlayerPrefs.GetInt ("showHelpOnStart");
		showHelpStartToggle.GetComponent<Toggle> ().isOn = (showHelpOnStart==1);
		helpWindow.SetActive (showHelpOnStart == 1);

    }

    public void reset()
    {
		Scene scene = SceneManager.GetActiveScene(); 
		SceneManager.LoadScene(scene.name);
    }

	public void help_toggle_visibility(){
		if (helpWindow.activeInHierarchy)
			helpWindow.SetActive (false);
		else
			helpWindow.SetActive (true);
	}

	public void help_show_on_start_change(){
		if (showHelpStartToggle.GetComponent<Toggle> ().isOn) {
			PlayerPrefs.SetInt ("showHelpOnStart", 1);
		} else {
			PlayerPrefs.SetInt ("showHelpOnStart", 0);
		}
	}

    public void newPart()
    {
		// Get position of Odlagalisce
		Vector3 pos = spawnPoint.position;
        prefab.tag = "Player";
		Instantiate(prefab, new Vector3(pos.x, 8f, pos.z), Quaternion.identity);
        prefab.tag = "Untagged";
    }
	public void removePart()
	{
		GameObject [] ply = GameObject.FindGameObjectsWithTag("Player");
		if (ply != null && ply.Length > 0) {
			GameObject.Destroy(ply[0]);
		}
	}


    // Update is called once per frame
    void Update()
    {
        //update lights
        red_left.GetComponent<Button>().interactable = com.red_left();
        red_right.GetComponent<Button>().interactable = com.red_right();
        yellow_left.GetComponent<Button>().interactable = com.yellow_left();
        yellow_right.GetComponent<Button>().interactable = com.yellow_right();

        com.black_l_u(button1.GetComponent<Toggle>().isOn);
        com.black_r_u(button2.GetComponent<Toggle>().isOn);
        com.black_r_d(button3.GetComponent<Toggle>().isOn);
        com.black_l_d(button4.GetComponent<Toggle>().isOn);
        com.green(green.GetComponent<Toggle>().isOn);
        com.key(key.GetComponent<Toggle>().isOn);

    }
}
