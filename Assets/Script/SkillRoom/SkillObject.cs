using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//스킬을쓰면 얘가 복제되서 날라감

public class SkillObject : MonoBehaviour,ISkill {
    public double etc = 7;//범위면 etc가 수명시간이 될거고 논타겟,타겟이면 속도가될거임
//    public float size = 0;
//    public double lifeTime = 0f;//라이프타임은 범위,셀프기술만사용
    public float damage;
    public float addSize;
    public float addEtc;
    //아마 플레이어만 스킬쓸거같긴함
    public GameObject caster;
//    public GameObject target;
    public SKILLTYPE type;
    public bool costMp = true;//코스트가 체력인지 마력인지


    //    public bool guided = false;//유도여부 이건 스킬에서 못정하고 스킬오브젝트가 정하는거임
    //\트루면 view를 대입하고 view가 Target을 찾게될거같음 아니면 find하거나
    //    private GameObject sight;

    //타겟은 그냥 유도미사일 형태는 재미없으니 지정하는식이될거같음
    //ㄴ 유도미사일형태는 오브젝트 옵션에서 추가 하게되겠음
    //////////////////////주석된 자료값들은전부 하위클래스가 지니게될 얘들임    

    //타겟이면? 쟤자리에서 파티클만하고 시간지나면 destroy self함

    public virtual void Activity()
    { }
    public virtual void Init()//하위가만들어냄 여기선 기본 Init's <<할게있긴함?
    {
        if ( this.transform.parent != null &&this.transform.parent.GetComponent<SkillObject>())//예상 - 만약 부모가 스킬오브젝트를 가지고 있다면 ?
               //ㄴ 원래 논타겟스킬에 있었는데 이렇게 두는게 좀더 나아보임
               //ㄴ스킬에 데미지 가하는식으로 이미 있는거같은데
        {
            SkillObject parent = this.transform.parent.GetComponent<SkillObject>();
            this.caster = parent.caster;
            this.damage = parent.damage*0.5f;//자식의 데미지는 부모의 절반
            this.addSize = parent.addSize;
            this.addEtc = parent.addEtc;
        }
            
    }

    public void bonusUp(double size, double etc)
    {
        //++식이아니라 곱셈하고,대입시킴 대입아니면 안되더라 연산하면 영구로쭉감
        addSize += (float)size;
        addEtc += (float)etc;
    }
    public void ResetObject(SKILLTYPE t)//add series는 만국공통이니깐 이렇게 내비둠
    {
        type = t;
        addSize = 0;
        addEtc = 0;
    }

    protected virtual BuffDebuff MakeComponent(GameObject target, DebuffType type)
    {//이름 이상한듯 디버프나 버프주는거 아닌가?
        BuffDebuff temp;
        GameObject objectTemp;
        //우선 버프용 오브젝트로 하나만들고, 거기서 리프레쉬하자
        if (target.transform.Find("Buff").Find(this.name) == null)
        {
            objectTemp = new GameObject();
            objectTemp.transform.parent = target.transform.Find("Buff");
            objectTemp.name = this.name;//안될지도?
            //                Instantiate(null, target.transform.FindChild("Buff"));
            //만약에 디버프,버프에 이오브젝트가 없다면 하나 만들어줌
            //없으면 이름까지 바꾸면서 만듬
        }
        else//있다면
        {
            objectTemp = target.transform.Find("Buff").Find(this.name).gameObject;
        }

        if (type == DebuffType.None)//값이 없을경우에
        {//None이면 무조건 포이즌인게 좀문제? 도트데미지로 할꺼라서 그렇게 한건데
            if (objectTemp == null)//디버프이름은? 스킬이름 그대로.
            {
                temp = objectTemp.AddComponent<Poison>();
            }
            temp = target.GetComponent<Poison>();
            return temp;

        }

        else if (objectTemp.GetComponent(type.ToString()) == null)//만약 타입이 있는데 그타입과 같은 컴포넌트를 안가지고있다면
         //target.GetComponent(debuffType[num].ToString()) == null
         //ㄴ 여기에서 바뀌어야함  현재는 타겟의 컴포넌트에서 찾았음
        {//컴포넌트생성
            temp = objectTemp.AddComponent(Type.GetType(type.ToString())) as BuffDebuff; //오류가능성이있음
                                                                                                    //최소 이거까지는 여러번 쓰이게 되는거니 좀 능동적? 유연? 해져야댐
                                                                                                    //이 Type은 C#에서 나온것

            //       temp.Init((float)etc / 2, 1, (int)damage, (float)((size + addSize) * 3 * 100 + etc));
            //ㄴ포이즌인경우 << 상태이상데미지를 주는구문,이건 외부에서함
        }
        temp = objectTemp.GetComponent(type.ToString()) as BuffDebuff;
        temp.Refresh();//리프레쉬해서 시작시간갱신을 통해 지속시간 초기화를 꾀함
                       //버그. 리프레쉬가 에러가남 < 아마 오브젝트의 문제
                       //ㄴ추가가 안되는걸로 추정
                       //초기화도 해주고 반환까지해줌
        return temp;

    }

    protected virtual void Damage(Status target)
    {
        
        Status attacker = caster.GetComponent<Status>();

        attacker.MagicAttack(target, (int)damage);
        //반올림시킬까?
        //ㄴ데미지자체는 나눌필요는 ㅇ벗다고생각함
        //이렇게하면 여러가지로 데미지 공식 재정의 할순있을것


        //170516 프로텍티드, 가상화로변경 왜냐면 체력회복은 이 데미지를 다르게 받아갈것임
        //걔하나를 위해 바꿈ㅎ
    }


    // Use this for initialization


    void Start () { Init();  }//Init으로 초기화함


	void FixedUpdate () {//걍 하고싶길래 했음
        Activity();//start에 Init과 비슷해질것
    }
    private void OnTriggerEnter(Collider other)
    {//온트리거 효과는 모두가? //사실상타겟/논타겟만 쓰고있긴한대

        //물체와 부딪히면 무조건소멸함
        //시전자의 태그가 플레이어일 경우 << 케이스에 따라 다를듯
        switch (type)
        {
            case SKILLTYPE.NONTARGET:
            case SKILLTYPE.GUIDED:
                if (caster.tag == "Player")//소환자가 플레이어
                {//소환자가 적인경우는 안쓴거임
                    if (other.tag == "Enemy")//적이맞았을경우에
                    {
                        Damage(other.GetComponent<Status>());

                            Destroy(gameObject);//필요에따라 여기에다가 파티클집어넣을수도 있음
                    }
                }
                if (other.tag == "Untagged")
                {
                    Destroy(gameObject);
                }
            break;

            case SKILLTYPE.TARGET:
                if(other.tag == "Enemy")
                Damage(other.GetComponent<Status>());//충돌한것중 적만
                break;
            case SKILLTYPE.PEACE:
                if (other.tag != "Enemy" && other.GetComponent<Status>() != null)
                    Damage(other.GetComponent<Status>());//에너미가 아닌것을 받도록함
                //언태그드같은 애들한테도 전부 확인을할테니 null이 아닌경우를 체크해가도록
                //쨋든 에너미가 아니면서 status가 있으면 일단 Damage를 수행
//                Destroy(gameObject);//그뒤 자신오브젝트를삭제 스킬 발동위치는 집이나 플레이어일테니
//ㄴ삭제안함 이펙트까지사라지니깐.. 차라리 해당오브젝트 자체를 지우면 모르겠다
                //무조건 실행은 될거임
                break;
        }
    }

    
}
