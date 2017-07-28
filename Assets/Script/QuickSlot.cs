using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//http://le0zh.github.io/2016/04/13/skill_cooldown_in_unity/
//ㄴ이걸 보고 퀵슬롯 스킬 쿨타임 돌림
public class QuickSlot : MonoBehaviour {
    public char CallKey;//private로하면 안됨? 특정이름값에서 어떤값을빼는걸 하던 qwer을 직접써야할듯
                        //ㄴ 퀵슬롯을 하나의 거대한 무언가가관리?
                        //스크립트 너무많아질듯 그냥 업데이트에서 그냥 내가입력한키를 CallKey와같으면 하도록

    //슬롯엔 아이템만 넣을거임
    //1번으로 무한복붙하기로.. Item필요없다고생각함
    public Slot slot; // 이건 자기자신의 슬롯
    private float printo;
    //퀵슬롯쓰면 쿨타임도 있어야하지않을까? <<추후에.. 아니면 전체 공통으로 쿨타임 던지던가

	// Use this for initialization
	void Start () {
        slot = this.GetComponent<Slot>();
        //스킬슬롯도 여기에 추가하고, 스킬 넣게되면 Item을 밖으로 넘김
	}
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(CallKey.ToString()) && Time.timeScale != 0 )//inputString == CallKey.ToString() )
        {
                        slot.SlotUse();
        }

        printo = this.transform.GetChild(0).GetChild(1).GetComponent<Image>().fillAmount = slot.ShowCoolTime();
    }
}
/* 스킬 쿨타임 대로 쭉돌게하기 해결완료
 * 수학어려움ㅎ 어쨋든 간단하게 쓰자면 스킬쓰고지난시간+쿨타임 - 현재시간 /쿨타임으로했음
 * 확실히 퀵슬롯같은 받아서 호출하는 애가 아니라 스킬슬롯에서 메커니즘 되도록하니깐 지난금요일보다 훨씬 빠르게됨
 * 
 * 문제있음 < 스킬 바꿔치기하면 해당스킬의 쿨이라고 해야하나 그게 덜도는기분 <<정확히는 current가 좀 삐걱거림
 * //ㄴ쿨타임부분자체를 스킬쪽해서 하게하는건?
 * 쿨타임자체를 스킬쪽에서 하게하고 방식을 약간바꿈
 * 스킬을마지막쏜시간 + 쿨타임 에서 스킬현재시간을빼도록한것
 * 이렇게하니깐 스킬도잘돌고 슬롯에서 스킬바꿔치기해도 잘됨
 */