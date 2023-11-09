using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{

    public string sceneName;
    public void OpenScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    
}
