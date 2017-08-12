using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { NONE, HEAL, HASTE, POWER };
//버프타입은 크게 3가지 초당회복, 가속, 공/방업
//버프타입으로 치부해야 디버프와헷갈리지 않을꺼같은데

public class PeaceSkill : SkillObject
{

    public bool houseOnly;//집전용스킬, 오브젝트마다 다름
    public BuffType buffType = BuffType.NONE;
    public float lifeTime = 0f;//버프계열을 넣기때문에 이렇게함ㅡ
    //ㄴ여기 있을필요 없을지도


    public override void Init()
    {
        etc += addEtc;
   //사이즈는안쓰네
    }

    public override void Activity()
    {
        //데미지는 이름상 나에게 액션을가하는거라 버프를 이쪽으로 옮길수도 있겠음
        if( (this.GetComponent<ParticleSystem>() == null ) ||
            ( this.GetComponent<ParticleSystem>().isPlaying == false) )
            Destroy(gameObject);
        //버프방식은 디버프방식과 일치함
    }
    protected override void Damage(Status target)
    {//이게 하우스인지 아닌지 체크는 스킬 쏠때 이미 하도록했으니깐 거기까지 신경은 안씀

        //여기서 UI로 이미지를 하나 만든걸 보낸다그리고 그거의 제한수명도 여기서보냄
        //그냥 버프쪽에서함 그저 이 이미지만 보내는걸로도 충분하도록
        if (buffType != BuffType.NONE)
        {
            BuffDebuff temp = MakeComponent(target.gameObject, (DebuffType)buffType);
            switch (buffType)
            {//Init는 기본, 디버프를 기본으로 했기에 전부 음수로하는게 best <<하드코딩?>
                case BuffType.HEAL: temp.Init((float)etc + addEtc, 1, -( (int)damage), 0);
                    break;
                case BuffType.HASTE:temp.Init((float)etc+addEtc, 0, -(float)(damage/100) );
                    break;
                case BuffType.POWER: temp.Init((float)etc + addEtc, 0, -(int)damage/3, -(int)damage/4);
                    break;
            }
        }

        else if (houseOnly == true)//만약 하우스 전용일경우
        {
            target.virtualStatus.hp += (int)damage + 3;
            //체력소모던 마력소모던 그냥 집만 수리할듯
        }
        else
        {
            if (costMp == true) target.virtualStatus.hp += (int)damage + 5;
            else target.virtualStatus.mp += (int)damage + 5;
        }
        target.CheckOverHpMp();

        //디버프와 버프의 열거형을 똑같이 했으니 될것ㅎ..

        Destroy(this.GetComponent<Rigidbody>());
        //오브젝트를 지우면 이펙트가 남으니 리짓바디or 콜라이더 둘중하나를 지워버림ㅎ
        //꼼수로 들어갔다 나갔다 하면서 회복한다거나 그런걸 방지


    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
