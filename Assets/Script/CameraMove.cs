using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {
    GameObject viewTarget;
	// Use this for initialization
	void Start () {
        viewTarget = GameObject.Find("Player").gameObject;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        this.transform.position = new Vector3(
            viewTarget.transform.position.x,
            viewTarget.transform.position.y + 12,
            viewTarget.transform.position.z);
	}
}
