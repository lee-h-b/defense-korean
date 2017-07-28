using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSkill : SkillObject
{
    /*etc는 가져감 타겟에서안씀 size,addsize는 타겟도씀 <스플래쉬를 위해서
     damage내버려둠 addSize내버려둠 addEtc 내버려둠 caster등도 내버려둠
     guided가져감 + etc,sight 가져감*/
    //타겟이면? 

    /* etc 안가져감 lifetime안가져감 <애초에 범위용
     */
    public override void Init()
    {
            GetComponent<SphereCollider>().radius += addSize;
    }
    public override void Activity()
    {
        if (this.GetComponent<ParticleSystem>().isPlaying == false)//아마 타겟만쓸듯
            Destroy(gameObject);//얘가 파티클시간 다되면소멸
    }
}