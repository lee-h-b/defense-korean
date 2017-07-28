using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//디버프를 여러개 가지는 스킬도 있음해서 디버프스킬을 배열식으로함

public class PlaceSkill : SkillObject
{

    public DebuffType[] debuffType;
    public float lifeTime = 0f;

    public override void Init()
    {
        lifeTime = (float)(Time.time + etc);
        GetComponent<SphereCollider>().radius += addSize;

    }
    public override void Activity()
    {
        if (Time.time > lifeTime)//삶이 다될때까지 보는것 삶이 다되면 소멸맨
            Destroy(gameObject);
    }
    //수정 해야댐< 컴포넌트 종류로만 하니깐 이름이 중복되면 시간갱신만함, 차라리 더쌘걸로 뒤엎으면 뒤엎지 갱신만하니 문제
    //컴포넌트의 이름이 존재하는지 확인하고 그이름을넣기


        //디버프를 있나없나 체크하고, 없다면 그것과 같은 이름으로 오브젝트를 만들고, 컴포넌트를 추가\
        //조건을 디버프있나 없나가 아닌 타겟에서 컴포넌트 자식을 찾기가 되겠음
      
    public void DebuffEnable(GameObject target)
    {

        //ㄴ 문자 대소문자고쳐서 해결

        for (int i = 0; i < debuffType.Length; i++)//한번 괄호 안넣음
        {

            BuffDebuff temp = MakeComponent(target, debuffType[i]);//없으면 컴포넌트를 만들고, 있ㅎ으면 그걸 가져옴 << 값을 할당해줘야지
            
            switch (debuffType[i])
            {
                //none이면 걍 데미지를주는데 포이즌형태로줌

                //컴포넌트값은 위에서만들었고, 이제 종류에따라다른 업그레이드를 행함
                case DebuffType.None:
                    {
                        if (target.GetComponent<Status>() != null)
                            Damage(target.GetComponent<Status>());
                        break;   
                    }
                case DebuffType.Poison: temp.Init((float)etc / 2, 1, (int)damage, (float)((addSize) * 3 * 10 + etc)); break;
                case DebuffType.Slow: temp.Init((float)etc / 2, 0, (damage /100)); break;//더추가하고싶지만 순수하게 데미지만으로 감속
                case DebuffType.Bruise: temp.Init((float)etc / 2, 0, (int)damage, (int)damage / 2); break;//독이랑 비슷한 처지라 이건 인트다 하고 못박아야함
            }
        }


    }
    
    private void OnTriggerStay(Collider other)//내생각엔 이걸 지속능력치 갱신으로 바꿀듯함
    {
        //디버프처럼 하고 만약 걔한테 자식중에 디버프가 없다면 버프를주는 확률이 있음

        //일단은 간단하게 포이즌만무조건 적용되는 가정하에
        //        Physics.OverlapSphere
        if (other.tag == "Enemy")//장판기를 나만하겠다는 심보
        {

            //이 컴포넌트를 이 오브젝트가 소유하는식이아니라 이 클래스에서 타입을 정하고 그타입에따라 case를통해서 other에게 컴포넌트를 전달해주는식으로함
            //ㄴ아마 컴포넌트를 자식으로 소유해서 보내기만하는거면 간결해질거임
            //ㄴ걍 그렇게해야지, 만약 내자식이 없다면 그경우엔 순수하게 데미지만 주는걸로하고

            //ㄴ정정 한번 이 오브젝트가 타입을 가지고 있게 해보자
            //그리고 그타입에 따라 other에게 switch case를 통해 종류에 따른 addComponent를 함
            //ㄴ이럴경우 소멸시 안사라질지도? clone이니깐 괜찮다고봄
            //ㄴ근데왜 이렇게? 컴포넌트 파일이 많아지게되니깐 좀맘에 안들어서

            DebuffEnable(other.gameObject);
            //ㄴ이걸 안할려고 합친것
            //ㄴ 머무르고 있는 적에게 디버프,데미지를적용
            if (debuffType.Length == 0)//디버프가 없다면 디버프처럼 초당데미지를 가하자사실상 저기 poison과 같을듯
                                       //포이즌을 빼고 이걸 넣게될수도있겠음
            {

            }
        }

    }
}
