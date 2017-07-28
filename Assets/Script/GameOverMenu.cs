using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour {

	// Use this for initialization
    public void Retry()
    {
        SceneManager.LoadScene("Playing");
        SceneManager.UnloadSceneAsync("GameOver");
    }
    public void Exit()
    {
        Application.Quit();
    }

    public void Menu()
    {
        SceneManager.LoadScene("Start");
        SceneManager.UnloadSceneAsync("GameOver");
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
