using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuScript : MonoBehaviour {
    public Transform mainMenu;
    public Transform howToPlay;
    public void Play()
    {
        SceneManager.LoadScene("Playing");
        SceneManager.UnloadSceneAsync("Start");

    }
    public void Exit()
    {
        Application.Quit();
    }
    public void HowToPlay()
    {
        mainMenu.gameObject.SetActive(false);
        howToPlay.gameObject.SetActive(true);
    }
    public void ReturnMenu()
    {
        mainMenu.gameObject.SetActive(true);
        howToPlay.gameObject.SetActive(false);
    }
}
