using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface Slot 
{
    void SlotUse();
    float ShowCoolTime();//슬롯에서 쿨타임을 보여줄 함수
    //이게 존재하니 아이템에도 스킬쿨타임을 쓰게될것 실제 스킬쿨타임이 도는건 슬롯에서만 하게될거임
    //float ShowLastTime();//마지막 사용시간 << 원래 bool로해서 할려고했으나 타겟같은 스킬의경우 이렇게할경우 반환이 애매하기에 이렇게 처리함
    //ㄴ 어떤점에서? 내가 스킬을 썼을대 그스킬을 누를때까지 기다렸다가 반환해야할텐데 그게 생각보다 쉽지 않을듯
//ㄴ 해결
}
