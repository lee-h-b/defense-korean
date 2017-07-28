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

        if (buffType != BuffType.NONE)
        {
            BuffDebuff temp = MakeComponent(target.gameObject, (DebuffType)buffType);
            switch (buffType)
            {//Init는 기본, 디버프를 기본으로 했기에 전부 음수로하는게 best <<하드코딩?>
                case BuffType.HEAL: temp.Init((float)damage / 2, 1, -(1+ (int)etc), 0);
                    break;
                case BuffType.HASTE:temp.Init((float)damage, 0, -(float)(etc/100) );
                    break;
                case BuffType.POWER: temp.Init((float)damage, 0, -(int)etc, -(int)etc/2);
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
        //디버프와 버프의 열거형을 똑같이 했으니 될것ㅎ..

        Destroy(this.GetComponent<Rigidbody>());
        //오브젝트를 지우면 이펙트가 남으니 리짓바디or 콜라이더 둘중하나를 지워버림ㅎ
        //꼼수로 들어갔다 나갔다 하면서 회복한다거나 그런걸 방지


    }

    /*
    private BuffDebuff MakeComponent(GameObject target)
        //버프는 1버프당 1스킬로 할것, 2단계스킬이라고 스까거나 그런거 절대없음
        //사실상 플레이스 스킬과유사함 그런데 왜 따로 두냐면 셀프스킬라인은 단일로할것이기에
        //ㄴ 그냥 타겟스킬에다가 값 몇개 추가하면되지않을까
        //ㄴ 차라리 메이크컴포넌트 구문을 스킬오브젝트에 넣는게 나을듯
    {
        BuffDebuff temp;
        GameObject objectTemp;
    } 스킬오브젝트에 넣었기에 이걸 안하게될지도모름
    */
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
