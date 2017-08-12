using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDestroyer : MonoBehaviour {
    //이름한번 멋지게 해보고 싶었음
    public float startTime;
    private float lifeTime = 1.4f; //짜피 텍스트인데 수명이 중요할까
	// Use this for initialization
	void Start () {
        startTime = Time.realtimeSinceStartup;
	}
	
	// Update is called once per frame
	void Update () {
        this.GetComponent<TextMesh>().transform.position -= new Vector3(0,0,0.1f );
        if (lifeTime + startTime <= Time.realtimeSinceStartup)
            Destroy(gameObject);
	}
}
