using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//스킬슬롯의 쿨타임 모방

public class BuffUI : MonoBehaviour {

    private float s_time;
    private float lifeTime = 99f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //슬롯의 쿨타임 모작,
        this.transform.GetChild(0).GetComponent<Image>().fillAmount =1- (s_time+ lifeTime - Time.time) / lifeTime;
        if (s_time + lifeTime <= Time.time)
            Destroy(gameObject);
	}
    public void InitDuration(float _lifeTime)
    {
        s_time = Time.time;
        lifeTime = _lifeTime;
//        Destroy(gameObject, lifeTime);//수명다되면 삭제 디스트로이로는 지속갱신이안됨

    }
}
