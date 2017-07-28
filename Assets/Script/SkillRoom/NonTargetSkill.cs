using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonTargetSkill : SkillObject
{

    public bool guided = false;//이거 상황에따라 private가 맞을지도
    private GameObject sight;//시야가있게된다면 쓰게될거임

    public GameObject target;//범위는 모르겠지만 논타겟은가져감 << 혹시 마법사적이 추가될수도 있고 하니간
    public double speed;//속도에씀 <<이전 etc
    public double addSpeed;//이전 addEtc;\

    public override void Init()
    {
        base.Init();
        if (this.transform.Find("sight") != null)
            sight = this.transform.Find("sight").gameObject;
        transform.localScale += new Vector3(addSize, 0, addSize);
    }
    public override void Activity()
    {


            if (target == null)
            {
                //만약 타겟이없다면 직진하고 sight가 null이 아니라면 그걸활성화
                transform.Translate(Vector3.forward * (Time.deltaTime * (float)(etc + addEtc)));
            }
            else //있다면?
            {//해당방향으로 움직이면서 방향은 타겟위치를 바라보도록감
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * (float)(etc + addEtc));
                transform.LookAt(target.transform.position);
            }
        
    }


}
