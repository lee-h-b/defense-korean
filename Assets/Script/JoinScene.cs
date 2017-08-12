using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class JoinScene : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && GameManager.inst.InBattle == false)
        {
            other.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            Time.timeScale = 0;
            SceneManager.LoadScene("Upgrade", LoadSceneMode.Additive);
            other.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;//활성을 거짓으로 함으로써 멈추게만들고
                                                                             //신을 부르고 다시 참으로 돌려서 움직이기가 되게함
        }
    }
}
