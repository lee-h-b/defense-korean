using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//버그. 상태이상이 2개이상 중첩됐을때 문제가 생김 << 1하고 2했을때 1끝나고 2끝나면 2에서 돌려주는 값때문에 1이 영구지속된꼴
//ㄴ그럼 이미 상태이상이 돌고있으면 그걸받아서 하도록?
/*
 * 1. 상태이상이 이미 있으면 그거한테서 원래상태를 받아옴?
 * 2.끝낼때 다른상태이상이 존재할경우?
*/
//ㄴ좀원시적으로 해결ㅇ 해당디버프가 백업식으로 가지고있던걸돌려줌
//ㄴ이거대신 맨첨에 하고있던 디버프의 백업스탯을 가져오기?

/*하드하게 다음 경우를 가정하에만들도록함
 * 1.디버프는 순서대로만들어진다
 * 2.그순서는안바뀌고 queue처럼 하나지워지면 맨위에게 탑이된다
 * 3.위치는 안바뀐다
 */

public enum DebuffType { None, Poison, Slow, Bruise };
//원래 범위스킬에 있었음

public class BuffDebuff : MonoBehaviour {
    //상태이상쯤 flaot형으로 지속시간, 을가지겠음 (데미지는초당)
    //타입? 내가 어떠한 능력을 감소시킬것인가
    //ㄴ이능력은 ?방깎같은 옵션저하, 속도저하, 도트데미지,행동정지 크게 4종류

    //독,돌,바람으로함
    //바람은 속도감소 ,회피저하
    //독은 초당데미지, 크리율감소
    //돌은 방어,공격감소
    //3위력 내가 어느정도의 파워로가할것인가?
    
//    private Image durationImage;//이미지를 에디터에서 미리 초기화하려했으나 안됨 그러니 게임매니저에서 꺼내서씀
    private GameObject buffScreen;
    private Image image;
    public float duration;//지속시간
    
    public float startTime, repeatTime;//시작,반복시간
    public Status target;
    public StatusValue backup;//상대의 원래스탯,끝날때 반환함// 원래 하위에 뒀었는데 전부가 가지고 있기에 위로 옮김

    //마지막으로 확률 이확률을 오브젝트에게 던져줘서 랜덤시드값을하던
    //반대로 내가 랜덤시드값을받아서 걔한테 능력치 감소를가하게될거임
    //또추가로 이상태이상을 오브젝트로전달해서 사라지면 디버프가 소멸되게할거임
    //https://forum.unity3d.com/threads/rpg-buffs-and-debuffs.188882/
    //ㄴ이걸참고해서해봄

    public bool once;//백업과 비교해서 할수도 있긴할텐데
    //그러면 짜는 코드가 많아지게됨

    protected virtual void Active()
    {
        target = transform.parent.parent.GetComponent<Status>();//얘의 기본조건은 위에 있는얘가 스테이터스일것
        //ㄴ 스킬이 디버프 가하는방식이 자식을 만들어서가아니라 디버프 컴포넌트를 직접 넣는식

        if (repeatTime > 0)
        {            
            InvokeRepeating("Apply", 0, repeatTime);//해당시간에하고 그후 특정간격마다
        }
        else
            InvokeRepeating("Apply", 0, 0);

    }
    
    private void Update()
    {
        if (transform.parent.parent.tag == "Player" && image == null)//버프의 소유자가 플레이어인경우
        {
            buffScreen = GameObject.Find("Canvas").transform.Find("BuffDuration").gameObject;
            image = Instantiate(GameManager.inst.durationImage, buffScreen.transform);
            var skillName = this.name.Replace("(Clone)", "");

            image.GetComponent<BuffUI>().InitDuration(duration);
            image.GetComponent<Image>().sprite = GameManager.inst.Get_SImage(skillName);
        }


        //끝내기용 < 최적화에 문제있다고 생각함 < 국비니깐 이거말곤 안떠오름
        if (startTime + duration < Time.time)
        Invoke("End", 0);
        
    }
    public void Refresh()
    {
        Init(duration, repeatTime);
        if(image != null)
        image.GetComponent<BuffUI>().InitDuration(duration);

    }
    void Start()
    {
        Active();
        //백업에대한 조건을 걸게될꺼임, 아래는 그 조건에서 벗어났을경우가 됨
        //만약 트랜스폼의 부모의 0번째자식과 불일치면서 부모의 자식수가 2개이상
        if (this.transform != this.transform.parent.GetChild(0) && this.transform.parent.childCount >= 2)
        {
            //만약 이파일이 0번째가 아니라면 그 0번째의 백업을 가져오게됨
            backup = this.transform.parent.GetChild(0).GetComponent<BuffDebuff>().backup;
        }
        //만약 얘가 어디든 상관ㅇ없이 디버프가 끝나면 다시 백업으로 복구를할거임, 그렇다면 디버프를 한번만 걸던 once(?)를 다시 트루로 되돌려버리는 구문도 있게될것
        //ㄴ엄청 비효율적인거같은데 <그래도 이렇게 하는게 제일인거같음 아님말고
        else        backup = target.GetVirtualStatus();//특별히 백업으로 가져갈만한게 없을경우
        //가상스탯이 즘감되니 가상만챙김
    }
    public void Init(float _duration, float time)
    {
        startTime = Time.time;

        duration = _duration;
        repeatTime = time;

        //이걸 이미지 드로잉이란 새이름으로 다른곳으로 옮길수도
    }
    //필요한 자료형을 가져가서 쓰는식 < 너무하드코딩같아
    public virtual void Init(float duration, float time, int intval, float floatval)
    {
        Init(duration, time);
    }

    public virtual void Init(float duration, float time, int _atkval, int _defval)
    {
        Init(duration, time);
    }
    public virtual void Init(float duration, float time, float per)
    {
        Init(duration, time);
    }
   public virtual void Status()
    {
    }
/// <summary>
/// //////////////////
/// </summary>
    protected virtual void Apply()
    {
    }
    protected virtual void End()
    {
//        CancelInvoke();//현재인보크를 취소
        Destroy(gameObject);//기존엔 스크립트를 유닛자체에 넣었지만 이번엔 좀다르니깐 자기 오브젝트를 지우도록함
    }
}
public class Slow : BuffDebuff//테스트, 버프 디버프들이 한없이는아니여도 3~4개는될텐데 그것들을 전부 따로만들자니 너무많아짐 
    //ㄴ그렇다면 위에다가 이걸 합치면 어느정도 완화될거로 예상됨
    //일단이대로둬봤자안됨 따라서 여기서만들어도 분해해서 가져가도록
{
    public float value;//둔화퍼센트쯤

    //얘는 속도도 줄고 회피도 줄고
    public override void Init(float duration, float time, float per)
    {
        base.Init(duration, time);
        value = per;
    }
    public override void Status()
    {
            target.StatusBuff( 0, 0, -value, 0, 0, -value);
    }
    protected override void Apply()
    {
        if (once == false)
        {
            once = true;
            Status();
        }
    }
    protected override void End()
    {
         target.StatusBuff( 0 , 0, value, 0, 0, value);//나머지 전부 0이니 기존 백업대로 될거라고 생각함
        base.End();
    }

}


public class Poison : BuffDebuff
{
    public int value;
    public float valper;
    
    public override void Init(float duration, float time, int intval, float valueper)
    {
        base.Init(duration, time);
        value = intval;
        valper = valueper;

    }
    public override void Status()
    {
            target.StatusBuff( 0, 0, 0, -valper);//초기화하면서 값을 깎아내기
    }
    protected override void Apply()
    {
        target.TakeDamage(value);
        if (once == false)
        {
            once = true;
            Status();
        }

    }
    protected override void End()
    {
         target.StatusBuff(0 ,0, 0, valper);//나머지 전부 0이니 기존 백업대로 될거라고 생각함
        //        target.StatusBuff(target, 0, 0, 0, backup.criPer);
       // target.criPer = backup.criPer;

        base.End();
    }

}
public class Bruise : BuffDebuff
{
    public int value;//공격감소
    public int value2;//방어감소

    public override void Init(float duration, float time, int _atkval, int _defval)
    {
        base.Init(duration, time);
        value = _atkval;
        value2 = _defval;
    }
    public override void Status()
    {
            target.StatusBuff( -value, -value2);
    }
    protected override void Apply()
    {

        target.TakeDamage(value);
        if (once == false)
        {
            once = true;
            Status();
        }
    }
    protected override void End()
    {
              target.StatusBuff( value, value2);//나머지 전부 0이니 기존 백업대로 될거라고 생각함
       // target.StatusBuff(target, value, value2);

        base.End();
    }

}
