using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//이건좀 어려울지도?
/*
 * 공격시 일정확률로 폭발이 일어나야함
 * 즉 공격이 닿았을때 뭔가를해서라도 첫재로 확률이 정상인가?와 데미지를 가한다 가 동시에 먹혀야함
 * 여기서 문제가, 액티비티 스위치가 패시브리스트를 통해서 들어가는데, 배우자마자 적용됨
 * 내가원하는식이면 얘가 공격당할때 확률놀음을 위해 호출해야할것
 * ㄴ 액티비티를 함수의 내용을 빼긴했음 이제 공격때마다 이 액티비티에 들어가서 확률놀음 하면 될지도
 */
 //액티비티의 호출자체를 플레이어쪽에서 간접? 적으로 호출할거같음 << 아마 무기쪽에서호출
 //그렇게되면 호출대상을 구별을 해야할것 <<일단은 패시브머니 자체는 액티비티를 추가안했으니 크게 신경안쓰도록
 //구분을 하려니 졸린건지 머리아프면서 안돌아감
public class Passive_Explosion : PassiveSkill {
    public float per;
    public int damage;
    private ParticleSystem myParticle;

    public override bool CheckPassivePer(Status requester, Status target)
    {
        Random.InitState((int)System.DateTime.Now.Ticks);//랜덤랜덤
        float flag = Random.Range(1f, 100f);
        if (per > flag && target != null)//만약 per이 flag보다 클경우 액션을취함 그러려면 몬스터를 챙겨와야함
        {
            PassiveActive(requester, target);//액티비티가 오버라이드인데 이걸 제외해야할지도모름
            return true;
        }
        return false;

        }
    public override void PassiveActive(Status caster, Status target)
    {
        myParticle = this.GetComponent<ParticleSystem>();

            //예상시나리오 캐스터의 무기 에서 게임오브젝트가 활성화중인지 보고 참이면 걔가 공격중인 적에게 damage를가함
            //활의경우는? 충돌하면 데미지가 나와야할텐데? << 스킬처럼 발사체에 묻어두고 보낼지도
            //그냥 충돌시에 플레이어컴포넌트를 불러서(혹은 패시브스킬리스트) 액티비티 전부돌릴듯
            //ㄴ무기도 엇비슷할것으로 추정
            ///// 그냥 패시브 호출시 캐스터와 타겟을 둘다 필요로 하도록 바꿈

            caster.MagicAttack(target, damage);//타겟은 무조건 있음 왜냐면 위에서 체클했음
            //            var p = weapon.enemy.gameObject.AddComponent<ParticleSystem>();
            //          p.Emit(myParticle.emission); 파티클 추가해려한 잔재

    }
    public override void bonusUp(double val1 = 0, double val2 = 0)
    {
        per += (float)val1;
        damage += (int)val2;
    }
    public override float[] GetAllValues()
    {
        float[] temp = new float[3] { per, (float)damage, 0 };
        return temp;
    }
}
