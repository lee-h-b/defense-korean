using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive_Money : PassiveSkill {

    //머니는 데미지필요없음
    //etc를 통한 확률 감산만 함 < 이수치는 고정적임 < 아마 패시브의 모든수치는 고정적일것 << 스킬오브젝트 잘못만듬, 인터페이스 ㅇㄷ

    // Use this for initialization
    public float per;
    public float addper;//임시 아마 per만 쓸거같음 <<레벨당 5%씩 늘어날테니 쓸모없을지도

    //ETC가확률 <버프형의경우 damage로만

    public override void Init()
    {// 업할때마다 캐스터의 배율을 직접 건드리도록 함
        caster.goldRate += per / 100;
    }
    //공격은 데미지, 확률이 늘어야하지만 얘는 퍼센트만 늘면되니깐
    public override void bonusUp(double val1 = 0, double val2 = 0)
    {
        per += (float)val1;
        caster.goldRate += (float)val1 /100;//그냥 단순대입 할까 싶었는데 만약 아이템으로 골드레이트를 변경한다면?
    }

    public override float[] GetAllValues()
    {
        float[] temp = new float[3] { per,0,0 };//값이 총3개일리는없지만
        //인포박스에서 설명으로 낼때 {0}%로 {1}의 데미지를 준다 이런식으로 하고싶음
        //하지만 이렇게 할려면 모두가 반환이 2개이상은되야함, 그래서 이런식으로 per만 정상,나머진 쓰레기를 줬음
        //배열채로 몇번째인지를 설정 못하니깐ㅡ
        return temp;
    }

}
